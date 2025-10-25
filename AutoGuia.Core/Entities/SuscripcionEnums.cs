namespace AutoGuia.Core.Entities;

/// <summary>
/// Tipo de duración de un plan de suscripción
/// </summary>
public enum TipoDuracion
{
    /// <summary>
    /// Suscripción mensual (30 días)
    /// </summary>
    Mensual = 1,
    
    /// <summary>
    /// Suscripción anual (365 días)
    /// </summary>
    Anual = 2
}

/// <summary>
/// Estado actual de una suscripción
/// </summary>
public enum EstadoSuscripcion
{
    /// <summary>
    /// Suscripción activa y vigente
    /// </summary>
    Activa = 1,
    
    /// <summary>
    /// Suscripción cancelada por el usuario
    /// </summary>
    Cancelada = 2,
    
    /// <summary>
    /// Suscripción vencida (expiró el período de pago)
    /// </summary>
    Vencida = 3,
    
    /// <summary>
    /// Suscripción en período de prueba
    /// </summary>
    Prueba = 4,
    
    /// <summary>
    /// Suscripción suspendida (por falta de pago)
    /// </summary>
    Suspendida = 5
}
