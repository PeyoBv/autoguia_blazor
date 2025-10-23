using AutoGuia.Core.Entities;
using AutoGuia.Infrastructure.Data;

namespace AutoGuia.Infrastructure.Data.Seeders;

/// <summary>
/// Seeder para poblar la base de datos con datos iniciales del módulo de diagnóstico
/// Incluye sistemas automotrices, síntomas, causas, pasos de verificación y recomendaciones
/// </summary>
public static class DiagnosticoSeeder
{
    /// <summary>
    /// Siembra datos iniciales del módulo de diagnóstico
    /// Evita duplicados verificando si ya existen sistemas automotrices
    /// </summary>
    public static void SeedDiagnosticoData(AutoGuiaDbContext context)
    {
        // Evitar duplicados
        if (context.SistemasAutomotrices.Any())
            return;

        // 1. Crear sistemas automotrices
        var sistemas = new List<SistemaAutomotriz>
        {
            new SistemaAutomotriz
            {
                Nombre = "Sistema de Motor",
                Descripcion = "Motor de combustión interna, componentes y funcionamiento",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new SistemaAutomotriz
            {
                Nombre = "Sistema de Transmisión",
                Descripcion = "Transmisión manual o automática, embrague y diferencial",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new SistemaAutomotriz
            {
                Nombre = "Sistema de Frenos",
                Descripcion = "Frenos de disco, pastillas, cilindros y líquido de frenos",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new SistemaAutomotriz
            {
                Nombre = "Sistema de Suspensión",
                Descripcion = "Amortiguadores, muelles y brazos de suspensión",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new SistemaAutomotriz
            {
                Nombre = "Sistema Eléctrico",
                Descripcion = "Batería, alternador, starter y sistema de carga",
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.SistemasAutomotrices.AddRange(sistemas);
        context.SaveChanges();

        var sistemasDict = context.SistemasAutomotrices.ToDictionary(s => s.Nombre);

        // 2. Crear síntomas por sistema
        var sintomas = new List<Sintoma>
        {
            // Motor
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Motor"].Id,
                Descripcion = "El motor no enciende",
                DescripcionTecnica = "Falla de ignición o combustible",
                NivelUrgencia = 4,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Motor"].Id,
                Descripcion = "Humo blanco del escape",
                DescripcionTecnica = "Posible ingreso de agua o refrigerante al motor",
                NivelUrgencia = 3,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Motor"].Id,
                Descripcion = "Ruidos extraños en el motor",
                DescripcionTecnica = "Golpeteo o chirridos indicando desgaste",
                NivelUrgencia = 2,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            // Transmisión
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Transmisión"].Id,
                Descripcion = "Cambios de marcha bruscos",
                DescripcionTecnica = "Falla en embrague o fluido de transmisión",
                NivelUrgencia = 2,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Transmisión"].Id,
                Descripcion = "Olor a quemado desde la transmisión",
                DescripcionTecnica = "Sobrecalentamiento del fluido de transmisión",
                NivelUrgencia = 3,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            // Frenos
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Frenos"].Id,
                Descripcion = "Pedal de freno suave o hundido",
                DescripcionTecnica = "Posible fuga de fluido o aire en el sistema",
                NivelUrgencia = 4,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Frenos"].Id,
                Descripcion = "Chirridos al frenar",
                DescripcionTecnica = "Pastillas de freno desgastadas o sucias",
                NivelUrgencia = 1,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            // Suspensión
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema de Suspensión"].Id,
                Descripcion = "Vehículo rebota excesivamente",
                DescripcionTecnica = "Amortiguadores desgastados o muelles rotos",
                NivelUrgencia = 2,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            },
            // Sistema Eléctrico
            new Sintoma
            {
                SistemaAutomotrizId = sistemasDict["Sistema Eléctrico"].Id,
                Descripcion = "Luz de batería encendida",
                DescripcionTecnica = "Posible falla del alternador o batería",
                NivelUrgencia = 3,
                EsActivo = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.Sintomas.AddRange(sintomas);
        context.SaveChanges();

        var sintomasDict = context.Sintomas.ToDictionary(s => s.Descripcion);

        // 3. Crear causas posibles
        var causasPosibles = new List<CausaPosible>
        {
            // Causa: Motor no enciende
            new CausaPosible
            {
                SintomaId = sintomasDict["El motor no enciende"].Id,
                Descripcion = "Batería descargada",
                DescripcionDetallada = "La batería no tiene suficiente carga para activar el motor de arranque",
                NivelProbabilidad = 4,
                RequiereServicioProfesional = false,
                FechaCreacion = DateTime.UtcNow
            },
            new CausaPosible
            {
                SintomaId = sintomasDict["El motor no enciende"].Id,
                Descripcion = "Problemas de combustible",
                DescripcionDetallada = "Depósito vacío, filtro tapado o falla de bomba de combustible",
                NivelProbabilidad = 3,
                RequiereServicioProfesional = true,
                FechaCreacion = DateTime.UtcNow
            },
            new CausaPosible
            {
                SintomaId = sintomasDict["El motor no enciende"].Id,
                Descripcion = "Falla en el sistema de ignición",
                DescripcionDetallada = "Bujías defectuosas, cable roto o módulo de ignición dañado",
                NivelProbabilidad = 2,
                RequiereServicioProfesional = true,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.CausasPosibles.AddRange(causasPosibles);
        context.SaveChanges();

        var causasDict = context.CausasPosibles.ToDictionary(c => c.Descripcion);

        // 4. Crear pasos de verificación
        var pasosVerificacion = new List<PasoVerificacion>
        {
            // Pasos para "Batería descargada"
            new PasoVerificacion
            {
                CausaPosibleId = causasDict["Batería descargada"].Id,
                Orden = 1,
                Descripcion = "Revisar conexiones de la batería",
                InstruccionesDetalladas = "Abre el capó y verifica que los cables positivo (rojo) y negativo (negro) estén bien conectados a los polos de la batería",
                IndicadoresExito = "Conexiones firmes sin corrosión",
                FechaCreacion = DateTime.UtcNow
            },
            new PasoVerificacion
            {
                CausaPosibleId = causasDict["Batería descargada"].Id,
                Orden = 2,
                Descripcion = "Intentar arrancar el vehículo",
                InstruccionesDetalladas = "Gira la llave de contacto a la posición de arranque (START). Si no arranca, la batería está descargada",
                IndicadoresExito = "Motor arranca normalmente",
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.PasosVerificacion.AddRange(pasosVerificacion);
        context.SaveChanges();

        // 5. Crear recomendaciones preventivas
        var recomendacionesPreventivas = new List<RecomendacionPreventiva>
        {
            new RecomendacionPreventiva
            {
                CausaPosibleId = causasDict["Batería descargada"].Id,
                Descripcion = "Revisar batería regularmente",
                Detalle = "Inspecciona el estado de la batería cada 3 meses y limpia la corrosión de los terminales",
                FrecuenciaKilometros = 10000,
                FrecuenciaMeses = 3,
                FechaCreacion = DateTime.UtcNow
            },
            new RecomendacionPreventiva
            {
                CausaPosibleId = causasDict["Batería descargada"].Id,
                Descripcion = "Cambiar batería cada 3-5 años",
                Detalle = "Las baterías típicamente duran entre 3 y 5 años. Reemplazarla preventivamente evita quedarte varado",
                FrecuenciaKilometros = 0,
                FrecuenciaMeses = 36,
                FechaCreacion = DateTime.UtcNow
            }
        };

        context.RecomendacionesPreventivas.AddRange(recomendacionesPreventivas);
        context.SaveChanges();
    }
}
