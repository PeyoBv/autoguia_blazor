using AutoGuia.Core.DTOs;
using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Services.Payments;
using AutoGuia.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AutoGuia.Web.Services.Payments;

/// <summary>
/// Implementación del gateway de pagos con Transbank Webpay OneClick
/// </summary>
public class TransbankGateway : ITransbankGateway
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<TransbankGateway> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    // Configuración de Transbank
    private readonly string _commerceCode;
    private readonly string _apiKey;
    private readonly string _environment;
    private readonly string _baseUrl;
    private readonly bool _isSandbox;

    // URLs de Transbank
    private const string SANDBOX_URL = "https://webpay3gint.transbank.cl";
    private const string PRODUCTION_URL = "https://webpay3g.transbank.cl";

    public bool IsSandbox => _isSandbox;

    public TransbankGateway(
        ApplicationDbContext context,
        ILogger<TransbankGateway> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient();

        // Cargar configuración
        _commerceCode = _configuration["Transbank:CommerceCode"] ?? "597055555584";
        _apiKey = _configuration["Transbank:ApiKey"] ?? "579B532A7440BB0C9079DED94D31EA1615BACEB56610332264630D42D0A36B1C";
        _environment = _configuration["Transbank:Environment"] ?? "Sandbox";
        _isSandbox = _environment.Equals("Sandbox", StringComparison.OrdinalIgnoreCase);
        _baseUrl = _isSandbox ? SANDBOX_URL : PRODUCTION_URL;

        // Configurar HttpClient
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.DefaultRequestHeaders.Add("Tbk-Api-Key-Id", _commerceCode);
        _httpClient.DefaultRequestHeaders.Add("Tbk-Api-Key-Secret", _apiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public string GetEnvironmentInfo()
    {
        return $"Environment: {_environment}, BaseURL: {_baseUrl}, CommerceCode: {_commerceCode}";
    }

    // ==================== INSCRIPCIÓN DE TARJETA ====================

    public async Task<IniciarInscripcionResponseDto> IniciarInscripcionAsync(
        IniciarInscripcionRequestDto request,
        string usuarioId)
    {
        try
        {
            _logger.LogInformation("Iniciando inscripción de tarjeta para usuario {UsuarioId}", usuarioId);

            // Crear orden de compra única
            var buyOrder = $"INS-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..8]}";

            // Crear registro de transacción
            var transaction = new TransbankTransaction
            {
                UsuarioId = usuarioId,
                Type = TransbankTransactionType.Inscription,
                Status = TransbankTransactionStatus.Pending,
                Amount = 0, // Inscripción no tiene monto
                BuyOrder = buyOrder,
                ReturnUrl = request.ReturnUrl,
                Environment = _environment,
                TransactionToken = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TransbankTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Preparar request para Transbank OneClick
            var transbankRequest = new
            {
                username = request.Username,
                email = request.Email,
                response_url = request.ReturnUrl
            };

            var jsonContent = JsonSerializer.Serialize(transbankRequest);
            transaction.RequestPayload = jsonContent;

            _logger.LogInformation("Enviando request a Transbank: {Request}", jsonContent);

            // Llamar a API de Transbank
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/rswebpaytransaction/api/oneclick/v1.0/inscriptions", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            transaction.ResponsePayload = responseBody;

            _logger.LogInformation("Response de Transbank: {Response}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error al iniciar inscripción: {StatusCode} - {Response}",
                    response.StatusCode, responseBody);

                transaction.Status = TransbankTransactionStatus.Error;
                transaction.ErrorMessage = $"Error HTTP {response.StatusCode}: {responseBody}";
                await _context.SaveChangesAsync();

                return new IniciarInscripcionResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Error al comunicarse con Transbank: {response.StatusCode}"
                };
            }

            // Parsear respuesta de Transbank
            var transbankResponse = JsonSerializer.Deserialize<TransbankInscriptionResponse>(responseBody);

            if (transbankResponse == null || string.IsNullOrEmpty(transbankResponse.Token))
            {
                transaction.Status = TransbankTransactionStatus.Error;
                transaction.ErrorMessage = "Respuesta inválida de Transbank";
                await _context.SaveChangesAsync();

                return new IniciarInscripcionResponseDto
                {
                    Success = false,
                    ErrorMessage = "Respuesta inválida de Transbank"
                };
            }

            // Actualizar transacción con token
            transaction.TransactionToken = transbankResponse.Token;
            transaction.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Registrar log
            await RegistrarLogAsync(transaction.Id, usuarioId, PaymentLogLevel.Info,
                "INSCRIPTION_STARTED", "Inscripción de tarjeta iniciada exitosamente");

            return new IniciarInscripcionResponseDto
            {
                Success = true,
                Token = transbankResponse.Token,
                UrlWebpay = transbankResponse.UrlWebpay,
                TransactionId = transaction.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar inscripción para usuario {UsuarioId}", usuarioId);

            await RegistrarLogAsync(null, usuarioId, PaymentLogLevel.Error,
                "INSCRIPTION_ERROR", $"Error al iniciar inscripción: {ex.Message}", ex.StackTrace);

            return new IniciarInscripcionResponseDto
            {
                Success = false,
                ErrorMessage = $"Error interno: {ex.Message}"
            };
        }
    }

    public async Task<ConfirmarInscripcionResponseDto> ConfirmarInscripcionAsync(string token)
    {
        try
        {
            _logger.LogInformation("Confirmando inscripción con token {Token}", token);

            // Limpiar el ChangeTracker para evitar conflictos con entidades rastreadas
            _context.ChangeTracker.Clear();

            // Buscar la transacción por el token
            var transaction = await _context.TransbankTransactions
                .FirstOrDefaultAsync(t => t.TransactionToken == token);

            if (transaction == null)
            {
                _logger.LogWarning("Transacción no encontrada para token {Token}", token);
                return new ConfirmarInscripcionResponseDto
                {
                    Success = false,
                    ErrorMessage = "Transacción no encontrada"
                };
            }

            // Llamar a API de Transbank para confirmar
            var response = await _httpClient.PutAsync(
                $"/rswebpaytransaction/api/oneclick/v1.0/inscriptions/{token}",
                null);

            var responseBody = await response.Content.ReadAsStringAsync();
            transaction.ResponsePayload = responseBody;

            _logger.LogInformation("Response de confirmación: {Response}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                transaction.Status = TransbankTransactionStatus.Rejected;
                transaction.ErrorMessage = $"Error HTTP {response.StatusCode}";
                await _context.SaveChangesAsync();

                return new ConfirmarInscripcionResponseDto
                {
                    Success = false,
                    ErrorMessage = $"Error al confirmar inscripción: {response.StatusCode}"
                };
            }

            // Parsear respuesta
            var confirmResponse = JsonSerializer.Deserialize<TransbankConfirmResponse>(responseBody);

            if (confirmResponse == null)
            {
                transaction.Status = TransbankTransactionStatus.Error;
                await _context.SaveChangesAsync();

                return new ConfirmarInscripcionResponseDto
                {
                    Success = false,
                    ErrorMessage = "Respuesta inválida de Transbank"
                };
            }

            // Verificar código de respuesta (0 = aprobado)
            if (confirmResponse.ResponseCode != 0)
            {
                transaction.Status = TransbankTransactionStatus.Rejected;
                transaction.ResponseCode = confirmResponse.ResponseCode.ToString();
                transaction.ResponseMessage = "Transacción rechazada por el banco";
                await _context.SaveChangesAsync();

                return new ConfirmarInscripcionResponseDto
                {
                    Success = false,
                    ErrorMessage = "Inscripción rechazada por el banco",
                    ResponseCode = confirmResponse.ResponseCode.ToString()
                };
            }

            // Verificar si ya existe un PaymentMethod con este TbkToken
            var existingPaymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.TbkToken == confirmResponse.TbkUser);

            PaymentMethod paymentMethod;

            if (existingPaymentMethod != null)
            {
                // Ya existe, actualizar los datos
                _logger.LogInformation("PaymentMethod ya existe con token {Token}, actualizando datos", confirmResponse.TbkUser);
                
                paymentMethod = existingPaymentMethod;
                paymentMethod.Last4Digits = confirmResponse.CardDetail?.CardNumber ?? paymentMethod.Last4Digits;
                paymentMethod.CardType = confirmResponse.CardDetail?.CardType ?? paymentMethod.CardType;
                paymentMethod.IsActive = true;
                paymentMethod.LastValidationDate = DateTime.UtcNow;
                paymentMethod.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // No existe, crear nuevo
                paymentMethod = new PaymentMethod
                {
                    UsuarioId = transaction.UsuarioId,
                    TbkToken = confirmResponse.TbkUser,
                    Last4Digits = confirmResponse.CardDetail?.CardNumber ?? "****",
                    CardType = confirmResponse.CardDetail?.CardType,
                    CardholderName = transaction.UsuarioId,
                    IsDefault = !await _context.PaymentMethods.AnyAsync(pm => pm.UsuarioId == transaction.UsuarioId),
                    IsActive = true,
                    InscriptionDate = DateTime.UtcNow,
                    LastValidationDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PaymentMethods.Add(paymentMethod);
            }

            // Guardar primero el PaymentMethod para obtener el ID generado
            // Detach la transacción temporalmente para evitar que se incluya en este SaveChanges
            _context.Entry(transaction).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            await _context.SaveChangesAsync();

            // Reattach la transacción y actualizar con el ID real
            _context.TransbankTransactions.Attach(transaction);
            transaction.Status = TransbankTransactionStatus.Approved;
            transaction.AuthorizationCode = confirmResponse.AuthorizationCode;
            transaction.ResponseCode = "0";
            transaction.TransactionDate = DateTime.UtcNow;
            transaction.PaymentMethodId = paymentMethod.Id; // Ahora tiene el ID generado
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Registrar log
            await RegistrarLogAsync(transaction.Id, transaction.UsuarioId, PaymentLogLevel.Info,
                "INSCRIPTION_CONFIRMED", $"Tarjeta inscrita exitosamente. Token: {paymentMethod.TbkToken}");

            return new ConfirmarInscripcionResponseDto
            {
                Success = true,
                TbkToken = paymentMethod.TbkToken,
                Last4Digits = paymentMethod.Last4Digits,
                CardType = paymentMethod.CardType,
                PaymentMethodId = paymentMethod.Id,
                AuthorizationCode = confirmResponse.AuthorizationCode,
                ResponseCode = "0"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al confirmar inscripción con token {Token}", token);

            await RegistrarLogAsync(null, null, PaymentLogLevel.Error,
                "INSCRIPTION_CONFIRM_ERROR", $"Error al confirmar inscripción: {ex.Message}", ex.StackTrace);

            return new ConfirmarInscripcionResponseDto
            {
                Success = false,
                ErrorMessage = $"Error interno: {ex.Message}"
            };
        }
    }

    public async Task<bool> EliminarMedioPagoAsync(int paymentMethodId, string usuarioId)
    {
        try
        {
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId && pm.UsuarioId == usuarioId);

            if (paymentMethod == null)
            {
                _logger.LogWarning("Medio de pago {PaymentMethodId} no encontrado para usuario {UsuarioId}",
                    paymentMethodId, usuarioId);
                return false;
            }

            // Eliminar token en Transbank
            var response = await _httpClient.DeleteAsync(
                $"/rswebpaytransaction/api/oneclick/v1.0/inscriptions/{paymentMethod.TbkToken}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Error al eliminar token en Transbank: {StatusCode}", response.StatusCode);
                // Continuar de todas formas para desactivar localmente
            }

            // Desactivar localmente (no eliminar físicamente)
            paymentMethod.IsActive = false;
            paymentMethod.InactiveReason = "Eliminado por el usuario";
            paymentMethod.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await RegistrarLogAsync(null, usuarioId, PaymentLogLevel.Info,
                "PAYMENT_METHOD_DELETED", $"Medio de pago {paymentMethodId} eliminado");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar medio de pago {PaymentMethodId}", paymentMethodId);
            return false;
        }
    }

    // ==================== COBROS ====================

    public async Task<CobrarConTokenResponseDto> CobrarConTokenAsync(CobrarConTokenRequestDto request)
    {
        try
        {
            _logger.LogInformation("Iniciando cobro con token para usuario {UsuarioId}, monto {Monto}",
                request.UsuarioId, request.Monto);

            // Validar medio de pago
            var paymentMethod = await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.Id == request.PaymentMethodId && pm.IsActive);

            if (paymentMethod == null || paymentMethod.UsuarioId != request.UsuarioId)
            {
                return new CobrarConTokenResponseDto
                {
                    Success = false,
                    ErrorMessage = "Medio de pago no válido o inactivo"
                };
            }

            // Verificar si ya existe una transacción con esta orden de compra (idempotencia)
            var existingTransaction = await _context.TransbankTransactions
                .FirstOrDefaultAsync(t => t.BuyOrder == request.BuyOrder);

            if (existingTransaction != null)
            {
                _logger.LogWarning("Orden de compra duplicada: {BuyOrder}", request.BuyOrder);

                if (existingTransaction.Status == TransbankTransactionStatus.Approved)
                {
                    return new CobrarConTokenResponseDto
                    {
                        Success = true,
                        TransactionId = existingTransaction.Id,
                        AuthorizationCode = existingTransaction.AuthorizationCode,
                        Amount = existingTransaction.Amount,
                        BuyOrder = existingTransaction.BuyOrder,
                        ResponseMessage = "Transacción ya procesada previamente"
                    };
                }
            }

            // Crear registro de transacción
            var transaction = new TransbankTransaction
            {
                UsuarioId = request.UsuarioId,
                PaymentMethodId = request.PaymentMethodId,
                SuscripcionId = request.SuscripcionId,
                Type = TransbankTransactionType.RecurringCharge,
                Status = TransbankTransactionStatus.Pending,
                Amount = request.Monto,
                BuyOrder = request.BuyOrder,
                Installments = request.Cuotas,
                Environment = _environment,
                TransactionToken = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TransbankTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Preparar request para Transbank
            var transbankRequest = new
            {
                username = paymentMethod.TbkToken,
                tbk_user = paymentMethod.TbkToken,
                buy_order = request.BuyOrder,
                amount = (int)request.Monto, // Transbank espera monto en centavos
                installments_number = request.Cuotas
            };

            var jsonContent = JsonSerializer.Serialize(transbankRequest);
            transaction.RequestPayload = jsonContent;

            _logger.LogInformation("Enviando cobro a Transbank: {Request}", jsonContent);

            // Llamar a API de Transbank para cobrar
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(
                "/rswebpaytransaction/api/oneclick/v1.0/transactions",
                content);

            var responseBody = await response.Content.ReadAsStringAsync();
            transaction.ResponsePayload = responseBody;

            _logger.LogInformation("Response de cobro: {Response}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                transaction.Status = TransbankTransactionStatus.Error;
                transaction.ErrorMessage = $"Error HTTP {response.StatusCode}";
                paymentMethod.FailedAttempts++;
                paymentMethod.LastFailedAttempt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new CobrarConTokenResponseDto
                {
                    Success = false,
                    TransactionId = transaction.Id,
                    ErrorMessage = $"Error al procesar cobro: {response.StatusCode}"
                };
            }

            // Parsear respuesta
            var chargeResponse = JsonSerializer.Deserialize<TransbankChargeResponse>(responseBody);

            if (chargeResponse == null)
            {
                transaction.Status = TransbankTransactionStatus.Error;
                await _context.SaveChangesAsync();

                return new CobrarConTokenResponseDto
                {
                    Success = false,
                    TransactionId = transaction.Id,
                    ErrorMessage = "Respuesta inválida de Transbank"
                };
            }

            // Verificar código de respuesta
            if (chargeResponse.ResponseCode != 0)
            {
                transaction.Status = TransbankTransactionStatus.Rejected;
                transaction.ResponseCode = chargeResponse.ResponseCode.ToString();
                transaction.ResponseMessage = "Transacción rechazada";
                paymentMethod.FailedAttempts++;
                paymentMethod.LastFailedAttempt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                await RegistrarLogAsync(transaction.Id, request.UsuarioId, PaymentLogLevel.Warning,
                    "CHARGE_REJECTED", $"Cobro rechazado. Código: {chargeResponse.ResponseCode}");

                return new CobrarConTokenResponseDto
                {
                    Success = false,
                    TransactionId = transaction.Id,
                    ResponseCode = chargeResponse.ResponseCode.ToString(),
                    ErrorMessage = "Transacción rechazada por el banco"
                };
            }

            // Cobro exitoso
            transaction.Status = TransbankTransactionStatus.Approved;
            transaction.AuthorizationCode = chargeResponse.AuthorizationCode;
            transaction.ResponseCode = "0";
            transaction.TransactionDate = chargeResponse.TransactionDate;
            transaction.UpdatedAt = DateTime.UtcNow;

            // Resetear contador de fallos del medio de pago
            paymentMethod.FailedAttempts = 0;
            paymentMethod.LastValidationDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await RegistrarLogAsync(transaction.Id, request.UsuarioId, PaymentLogLevel.Info,
                "CHARGE_APPROVED", $"Cobro aprobado. Monto: ${request.Monto}, Auth: {chargeResponse.AuthorizationCode}");

            return new CobrarConTokenResponseDto
            {
                Success = true,
                TransactionId = transaction.Id,
                AuthorizationCode = chargeResponse.AuthorizationCode,
                Amount = request.Monto,
                BuyOrder = request.BuyOrder,
                TransactionDate = chargeResponse.TransactionDate,
                ResponseCode = "0",
                ResponseMessage = "Transacción aprobada",
                Last4Digits = paymentMethod.Last4Digits
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar cobro con token");

            await RegistrarLogAsync(null, request.UsuarioId, PaymentLogLevel.Error,
                "CHARGE_ERROR", $"Error al procesar cobro: {ex.Message}", ex.StackTrace);

            return new CobrarConTokenResponseDto
            {
                Success = false,
                ErrorMessage = $"Error interno: {ex.Message}"
            };
        }
    }

    public async Task<CobrarConTokenResponseDto> CobrarSuscripcionAsync(int suscripcionId, decimal monto)
    {
        try
        {
            // Obtener suscripción
            var suscripcion = await _context.Suscripciones
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.Id == suscripcionId);

            if (suscripcion == null)
            {
                return new CobrarConTokenResponseDto
                {
                    Success = false,
                    ErrorMessage = "Suscripción no encontrada"
                };
            }

            // Obtener medio de pago predeterminado del usuario
            var paymentMethod = await _context.PaymentMethods
                .Where(pm => pm.UsuarioId == suscripcion.UsuarioId && pm.IsActive)
                .OrderByDescending(pm => pm.IsDefault)
                .ThenByDescending(pm => pm.InscriptionDate)
                .FirstOrDefaultAsync();

            if (paymentMethod == null)
            {
                return new CobrarConTokenResponseDto
                {
                    Success = false,
                    ErrorMessage = "No se encontró un medio de pago válido"
                };
            }

            // Generar orden de compra única
            var buyOrder = $"SUB-{suscripcionId}-{DateTime.UtcNow:yyyyMMddHHmmss}";

            // Realizar cobro
            var request = new CobrarConTokenRequestDto
            {
                UsuarioId = suscripcion.UsuarioId,
                PaymentMethodId = paymentMethod.Id,
                Monto = monto,
                SuscripcionId = suscripcionId,
                BuyOrder = buyOrder,
                Cuotas = 1
            };

            return await CobrarConTokenAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cobrar suscripción {SuscripcionId}", suscripcionId);

            return new CobrarConTokenResponseDto
            {
                Success = false,
                ErrorMessage = $"Error interno: {ex.Message}"
            };
        }
    }

    // Continuará en la siguiente parte...
    
    // ==================== WEBHOOK ====================
    
    public async Task<WebhookResponseDto> ProcesarWebhookAsync(TransbankWebhookDto webhook)
    {
        try
        {
            _logger.LogInformation("Procesando webhook de Transbank. Token: {Token}, BuyOrder: {BuyOrder}",
                webhook.Token, webhook.BuyOrder);

            // Buscar transacción
            var transaction = await _context.TransbankTransactions
                .FirstOrDefaultAsync(t => t.TransactionToken == webhook.Token || t.BuyOrder == webhook.BuyOrder);

            if (transaction == null)
            {
                _logger.LogWarning("Transacción no encontrada para webhook. Token: {Token}", webhook.Token);
                return new WebhookResponseDto
                {
                    Success = false,
                    Message = "Transacción no encontrada"
                };
            }

            // Actualizar transacción con datos del webhook
            transaction.AuthorizationCode = webhook.AuthorizationCode;
            transaction.ResponseCode = webhook.ResponseCode;
            transaction.TransactionDate = webhook.TransactionDate ?? DateTime.UtcNow;
            transaction.WebhookProcessed = true;
            transaction.WebhookProcessedAt = DateTime.UtcNow;
            transaction.UpdatedAt = DateTime.UtcNow;

            // Actualizar estado según respuesta
            if (webhook.Status == "AUTHORIZED" || webhook.ResponseCode == "0")
            {
                transaction.Status = TransbankTransactionStatus.Approved;
            }
            else
            {
                transaction.Status = TransbankTransactionStatus.Rejected;
            }

            await _context.SaveChangesAsync();

            await RegistrarLogAsync(transaction.Id, transaction.UsuarioId, PaymentLogLevel.Info,
                "WEBHOOK_PROCESSED", $"Webhook procesado. Estado: {webhook.Status}");

            return new WebhookResponseDto
            {
                Success = true,
                Message = "Webhook procesado correctamente",
                TransactionId = transaction.Id
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar webhook");

            await RegistrarLogAsync(null, null, PaymentLogLevel.Error,
                "WEBHOOK_ERROR", $"Error al procesar webhook: {ex.Message}", ex.StackTrace);

            return new WebhookResponseDto
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }

    // ==================== CONSULTAS ====================

    public async Task<TransbankTransactionDto> ObtenerEstadoTransaccionAsync(string token)
    {
        var transaction = await _context.TransbankTransactions
            .FirstOrDefaultAsync(t => t.TransactionToken == token);

        if (transaction == null)
        {
            throw new Exception("Transacción no encontrada");
        }

        return MapToDto(transaction);
    }

    public async Task<List<PaymentMethodDto>> ObtenerMediosPagoAsync(string usuarioId)
    {
        return await _context.PaymentMethods
            .Where(pm => pm.UsuarioId == usuarioId && pm.IsActive)
            .OrderByDescending(pm => pm.IsDefault)
            .ThenByDescending(pm => pm.InscriptionDate)
            .Select(pm => new PaymentMethodDto
            {
                Id = pm.Id,
                Last4Digits = pm.Last4Digits,
                CardType = pm.CardType,
                ExpirationDate = pm.ExpirationDate,
                CardholderName = pm.CardholderName,
                IsDefault = pm.IsDefault,
                IsActive = pm.IsActive,
                InscriptionDate = pm.InscriptionDate,
                FailedAttempts = pm.FailedAttempts,
                CardMask = pm.CardMask,
                Description = pm.Description
            })
            .ToListAsync();
    }

    public async Task<PaymentMethodDto?> ObtenerMedioPagoPredeterminadoAsync(string usuarioId)
    {
        var paymentMethod = await _context.PaymentMethods
            .Where(pm => pm.UsuarioId == usuarioId && pm.IsActive && pm.IsDefault)
            .FirstOrDefaultAsync();

        if (paymentMethod == null)
        {
            // Si no hay predeterminado, devolver el más reciente
            paymentMethod = await _context.PaymentMethods
                .Where(pm => pm.UsuarioId == usuarioId && pm.IsActive)
                .OrderByDescending(pm => pm.InscriptionDate)
                .FirstOrDefaultAsync();
        }

        return paymentMethod == null ? null : new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            Last4Digits = paymentMethod.Last4Digits,
            CardType = paymentMethod.CardType,
            ExpirationDate = paymentMethod.ExpirationDate,
            CardholderName = paymentMethod.CardholderName,
            IsDefault = paymentMethod.IsDefault,
            IsActive = paymentMethod.IsActive,
            InscriptionDate = paymentMethod.InscriptionDate,
            FailedAttempts = paymentMethod.FailedAttempts,
            CardMask = paymentMethod.CardMask,
            Description = paymentMethod.Description
        };
    }

    public async Task<bool> EstablecerMedioPagoPredeterminadoAsync(int paymentMethodId, string usuarioId)
    {
        try
        {
            // Quitar predeterminado de todos
            var allMethods = await _context.PaymentMethods
                .Where(pm => pm.UsuarioId == usuarioId)
                .ToListAsync();

            foreach (var method in allMethods)
            {
                method.IsDefault = method.Id == paymentMethodId;
                method.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al establecer medio de pago predeterminado");
            return false;
        }
    }

    public async Task<List<TransbankTransactionDto>> ObtenerHistorialTransaccionesAsync(string usuarioId, int limit = 50)
    {
        return await _context.TransbankTransactions
            .Where(t => t.UsuarioId == usuarioId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<TransbankTransaction?> ObtenerDetalleTransaccionAsync(int transactionId)
    {
        return await _context.TransbankTransactions
            .Include(t => t.PaymentMethod)
            .Include(t => t.Suscripcion)
            .FirstOrDefaultAsync(t => t.Id == transactionId);
    }

    public async Task<bool> ValidarMedioPagoAsync(int paymentMethodId)
    {
        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId);

        return paymentMethod != null && paymentMethod.IsActive && !paymentMethod.RequiresValidation;
    }

    // ==================== MÉTODOS AUXILIARES ====================

    private async Task RegistrarLogAsync(
        int? transactionId,
        string? usuarioId,
        PaymentLogLevel level,
        string eventName,
        string message,
        string? stackTrace = null)
    {
        try
        {
            var log = new PaymentLog
            {
                TransactionId = transactionId,
                UsuarioId = usuarioId,
                Level = level,
                Event = eventName,
                Message = message,
                StackTrace = stackTrace,
                Source = "TransbankGateway",
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentLogs.Add(log);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar log de pago");
        }
    }

    private TransbankTransactionDto MapToDto(TransbankTransaction transaction)
    {
        return new TransbankTransactionDto
        {
            Id = transaction.Id,
            Type = transaction.Type.ToString(),
            Status = transaction.Status.ToString(),
            Amount = transaction.Amount,
            BuyOrder = transaction.BuyOrder,
            AuthorizationCode = transaction.AuthorizationCode,
            ResponseMessage = transaction.ResponseMessage,
            TransactionDate = transaction.TransactionDate,
            CreatedAt = transaction.CreatedAt,
            TypeDescription = transaction.TypeDescription,
            StatusDescription = transaction.StatusDescription
        };
    }

    // ==================== MODELOS DE TRANSBANK API ====================

    private class TransbankInscriptionResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("url_webpay")]
        public string UrlWebpay { get; set; } = string.Empty;
    }

    private class TransbankConfirmResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("tbk_user")]
        public string TbkUser { get; set; } = string.Empty;

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; } = string.Empty;

        [JsonPropertyName("card_detail")]
        public CardDetail? CardDetail { get; set; }
    }

    private class CardDetail
    {
        [JsonPropertyName("card_number")]
        public string CardNumber { get; set; } = string.Empty;

        [JsonPropertyName("card_type")]
        public string? CardType { get; set; }
    }

    private class TransbankChargeResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("authorization_code")]
        public string AuthorizationCode { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("buy_order")]
        public string BuyOrder { get; set; } = string.Empty;

        [JsonPropertyName("transaction_date")]
        public DateTime TransactionDate { get; set; }
    }
}
