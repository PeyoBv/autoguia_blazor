# API de Diagnóstico Automotriz - AutoGuía

## Descripción General

La API de Diagnóstico proporciona endpoints REST para:
- Realizar diagnósticos de síntomas automotrices
- Consultar catálogos de sistemas y síntomas
- Acceder al historial de diagnósticos
- Registrar feedback de utilidad

**Base URL**: `https://localhost:5070/api` (Desarrollo)  
**Versión**: 1.0  
**Última actualización**: 23 de octubre de 2025

---

## 📋 Tabla de Contenidos

1. [Endpoints de Diagnóstico](#endpoints-de-diagnóstico)
2. [Endpoints de Sistemas](#endpoints-de-sistemas)
3. [Autenticación](#autenticación)
4. [Niveles de Urgencia](#niveles-de-urgencia)
5. [Códigos de Error](#códigos-de-error)
6. [Ejemplos de Uso](#ejemplos-de-uso)
7. [Mejores Prácticas](#mejores-prácticas)

---

## Endpoints de Diagnóstico

### 1. Diagnosticar Síntoma

**POST** `/api/diagnostico/diagnosticar`

Realiza un diagnóstico basado en la descripción del síntoma proporcionada por el usuario.

#### Headers
```http
Authorization: Bearer <token>
Content-Type: application/json
```

#### Request Body
```json
{
  "descripcionSintoma": "El motor hace ruidos extraños y pierde potencia"
}
```

| Campo | Tipo | Requerido | Validación | Descripción |
|-------|------|-----------|------------|-------------|
| `descripcionSintoma` | string | ✅ | Min: 10, Max: 1000 | Descripción detallada del problema |

#### Response (200 OK)
```json
{
  "nivelUrgencia": 3,
  "sintomaIdentificado": "Ruidos extraños en el motor",
  "causasPosibles": [
    {
      "id": 1,
      "descripcion": "Válvulas desgastadas",
      "descripcionDetallada": "Las válvulas del motor están gastadas por uso prolongado o falta de mantenimiento. Esto causa fricción excesiva y ruidos metálicos característicos.",
      "nivelProbabilidad": 4,
      "requiereServicioProfesional": true,
      "pasosVerificacion": [
        {
          "id": 1,
          "orden": 1,
          "descripcion": "Escuchar el motor en ralentí",
          "instruccionesDetalladas": "Arranca el motor y déjalo en ralentí (sin acelerar). Escucha atentamente cerca del compartimento del motor con el capó abierto.",
          "indicadoresExito": "Ruidos metálicos rítmicos sincronizados con las RPM"
        },
        {
          "id": 2,
          "orden": 2,
          "descripcion": "Verificar nivel de aceite",
          "instruccionesDetalladas": "Con el motor apagado y en superficie plana, extrae la varilla de aceite, límpiala, insértala nuevamente y verifica el nivel.",
          "indicadoresExito": "Nivel de aceite entre MIN y MAX en la varilla"
        }
      ],
      "recomendaciones": [
        {
          "id": 1,
          "descripcion": "Mantenimiento preventivo de válvulas",
          "detalle": "Ajustar válvulas cada 30,000 km o según especificaciones del fabricante. Usar aceite de calidad para reducir desgaste.",
          "frecuenciaKilometros": 30000,
          "frecuenciaMeses": 12
        }
      ]
    },
    {
      "id": 2,
      "descripcion": "Rodamientos de biela desgastados",
      "descripcionDetallada": "Desgaste de los rodamientos que conectan el cigüeñal con las bielas. Causa golpeteo grave.",
      "nivelProbabilidad": 3,
      "requiereServicioProfesional": true,
      "pasosVerificacion": [],
      "recomendaciones": []
    }
  ],
  "recomendacion": "Problema importante, DEBE llevar a servicio profesional.",
  "sugerirServicioProfesional": true,
  "sintomaRelacionadoId": 3
}
```

#### Respuesta - Campos

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `nivelUrgencia` | int | Nivel de urgencia de 1 a 4 |
| `sintomaIdentificado` | string | Descripción del síntoma identificado |
| `causasPosibles` | array | Lista de posibles causas ordenadas por probabilidad |
| `recomendacion` | string | Mensaje de recomendación según urgencia |
| `sugerirServicioProfesional` | bool | Indica si requiere atención profesional |
| `sintomaRelacionadoId` | int? | ID del síntoma en el catálogo (null si no se encontró) |

#### Códigos de Respuesta
- `200 OK` - Diagnóstico realizado exitosamente
- `400 Bad Request` - Validación fallida (descripción muy corta/larga)
- `401 Unauthorized` - Usuario no autenticado
- `500 Internal Server Error` - Error del servidor

---

### 2. Obtener Síntomas por Sistema

**GET** `/api/diagnostico/sintomas/{sistemaId}`

Obtiene la lista de síntomas asociados a un sistema automotriz específico.

#### Parameters
| Parámetro | Tipo | Ubicación | Requerido | Descripción |
|-----------|------|-----------|-----------|-------------|
| `sistemaId` | int | URL | ✅ | ID del sistema automotriz |

#### Response (200 OK)
```json
[
  {
    "id": 1,
    "descripcion": "El motor no enciende",
    "descripcionTecnica": "Falla de ignición o suministro de combustible que impide el arranque del motor",
    "nivelUrgencia": 4,
    "sistemaAutomotrizId": 1,
    "nombreSistema": "Sistema de Motor"
  },
  {
    "id": 2,
    "descripcion": "Pérdida de potencia al acelerar",
    "descripcionTecnica": "Respuesta deficiente del motor ante demanda de aceleración",
    "nivelUrgencia": 2,
    "sistemaAutomotrizId": 1,
    "nombreSistema": "Sistema de Motor"
  }
]
```

#### Códigos de Respuesta
- `200 OK` - Lista de síntomas obtenida
- `400 Bad Request` - ID inválido
- `404 Not Found` - Sistema no existe

---

### 3. Obtener Detalles de Causa

**GET** `/api/diagnostico/causa/{causaId}`

Obtiene los detalles completos de una causa posible, incluyendo pasos de verificación y recomendaciones preventivas.

#### Parameters
| Parámetro | Tipo | Ubicación | Requerido | Descripción |
|-----------|------|-----------|-----------|-------------|
| `causaId` | int | URL | ✅ | ID de la causa posible |

#### Response (200 OK)
```json
{
  "id": 1,
  "descripcion": "Batería descargada",
  "descripcionDetallada": "La batería no tiene suficiente carga para activar el motor de arranque. Común en climas fríos o baterías antiguas (más de 3-4 años).",
  "nivelProbabilidad": 4,
  "requiereServicioProfesional": false,
  "pasosVerificacion": [
    {
      "id": 1,
      "orden": 1,
      "descripcion": "Verificar luces del tablero",
      "instruccionesDetalladas": "Gira la llave a posición ON (sin arrancar). Observa si las luces del tablero encienden normalmente o se ven tenues.",
      "indicadoresExito": "Luces del tablero débiles o no encienden"
    },
    {
      "id": 2,
      "orden": 2,
      "descripcion": "Intentar arranque",
      "instruccionesDetalladas": "Gira la llave para arrancar el motor. Escucha el sonido del motor de arranque.",
      "indicadoresExito": "Motor de arranque gira lentamente o no gira"
    }
  ],
  "recomendaciones": [
    {
      "id": 1,
      "descripcion": "Reemplazar batería cada 3-4 años",
      "detalle": "Las baterías pierden capacidad con el tiempo. Reemplazar preventivamente evita quedar varado.",
      "frecuenciaKilometros": null,
      "frecuenciaMeses": 36
    },
    {
      "id": 2,
      "descripcion": "Revisar alternador anualmente",
      "detalle": "El alternador recarga la batería. Un alternador defectuoso descarga la batería rápidamente.",
      "frecuenciaKilometros": 20000,
      "frecuenciaMeses": 12
    }
  ]
}
```

#### Códigos de Respuesta
- `200 OK` - Causa encontrada
- `404 Not Found` - Causa no existe
- `400 Bad Request` - ID inválido

---

### 4. Obtener Historial de Consultas

**GET** `/api/diagnostico/historial`

Obtiene el historial de diagnósticos realizados por el usuario autenticado.

#### Headers
```http
Authorization: Bearer <token>
```

#### Response (200 OK)
```json
[
  {
    "id": 1,
    "sintomaDescrito": "El motor hace ruidos extraños al arrancar",
    "fechaConsulta": "2025-10-23T15:30:45Z",
    "respuestaAsistente": "Problema moderado, intente los pasos de verificación. Si persiste lleve a servicio.",
    "fueUtil": true,
    "sintomaRelacionadoId": 3,
    "nombreSintomaRelacionado": "Ruidos extraños en el motor"
  },
  {
    "id": 2,
    "sintomaDescrito": "Luces del tablero parpadeando",
    "fechaConsulta": "2025-10-22T10:15:30Z",
    "respuestaAsistente": "Problema importante, DEBE llevar a servicio profesional.",
    "fueUtil": null,
    "sintomaRelacionadoId": 8,
    "nombreSintomaRelacionado": "Fallas en el sistema eléctrico"
  }
]
```

#### Respuesta - Campos

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `id` | int | ID único de la consulta |
| `sintomaDescrito` | string | Descripción original del usuario |
| `fechaConsulta` | datetime | Fecha y hora en formato ISO 8601 (UTC) |
| `respuestaAsistente` | string | Recomendación generada por el sistema |
| `fueUtil` | bool? | Feedback del usuario (null = sin feedback) |
| `sintomaRelacionadoId` | int? | ID del síntoma identificado |
| `nombreSintomaRelacionado` | string? | Nombre del síntoma identificado |

#### Códigos de Respuesta
- `200 OK` - Historial obtenido exitosamente
- `401 Unauthorized` - Usuario no autenticado
- `500 Internal Server Error` - Error del servidor

---

### 5. Registrar Feedback

**POST** `/api/diagnostico/feedback/{consultaId}`

Permite al usuario registrar si el diagnóstico fue útil o no.

#### Parameters
| Parámetro | Tipo | Ubicación | Requerido | Descripción |
|-----------|------|-----------|-----------|-------------|
| `consultaId` | int | URL | ✅ | ID de la consulta a evaluar |

#### Headers
```http
Authorization: Bearer <token>
Content-Type: application/json
```

#### Request Body
```json
{
  "fueUtil": true
}
```

| Campo | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `fueUtil` | bool | ✅ | true = útil, false = no útil |

#### Response (200 OK)
```json
{
  "mensaje": "Feedback registrado exitosamente",
  "consultaId": 1,
  "fueUtil": true
}
```

#### Códigos de Respuesta
- `200 OK` - Feedback registrado exitosamente
- `400 Bad Request` - Validación fallida
- `401 Unauthorized` - Usuario no autenticado
- `404 Not Found` - Consulta no existe o no pertenece al usuario

---

## Endpoints de Sistemas

### 6. Obtener Todos los Sistemas

**GET** `/api/sistemas`

Obtiene el catálogo completo de sistemas automotrices con sus síntomas asociados.

#### Response (200 OK)
```json
[
  {
    "id": 1,
    "nombre": "Sistema de Motor",
    "descripcion": "Motor de combustión interna, componentes y funcionamiento. Incluye sistema de admisión, escape, lubricación y refrigeración.",
    "sintomas": [
      {
        "id": 1,
        "descripcion": "El motor no enciende",
        "descripcionTecnica": "Falla de ignición o suministro de combustible",
        "nivelUrgencia": 4,
        "sistemaAutomotrizId": 1,
        "nombreSistema": "Sistema de Motor"
      },
      {
        "id": 2,
        "descripcion": "Pérdida de potencia al acelerar",
        "descripcionTecnica": "Respuesta deficiente del motor ante demanda de aceleración",
        "nivelUrgencia": 2,
        "sistemaAutomotrizId": 1,
        "nombreSistema": "Sistema de Motor"
      }
    ]
  },
  {
    "id": 2,
    "nombre": "Sistema de Frenos",
    "descripcion": "Sistema hidráulico de frenado, discos, tambores y componentes auxiliares.",
    "sintomas": [
      {
        "id": 5,
        "descripcion": "Pedal de freno esponjoso",
        "descripcionTecnica": "Pérdida de presión en el sistema hidráulico",
        "nivelUrgencia": 3,
        "sistemaAutomotrizId": 2,
        "nombreSistema": "Sistema de Frenos"
      }
    ]
  }
]
```

#### Códigos de Respuesta
- `200 OK` - Catálogo obtenido exitosamente
- `500 Internal Server Error` - Error del servidor

---

### 7. Obtener Sistema por ID

**GET** `/api/sistemas/{id}`

Obtiene un sistema automotriz específico con todos sus síntomas.

#### Parameters
| Parámetro | Tipo | Ubicación | Requerido | Descripción |
|-----------|------|-----------|-----------|-------------|
| `id` | int | URL | ✅ | ID del sistema automotriz |

#### Response (200 OK)
```json
{
  "id": 1,
  "nombre": "Sistema de Motor",
  "descripcion": "Motor de combustión interna, componentes y funcionamiento. Incluye sistema de admisión, escape, lubricación y refrigeración.",
  "sintomas": [
    {
      "id": 1,
      "descripcion": "El motor no enciende",
      "descripcionTecnica": "Falla de ignición o suministro de combustible",
      "nivelUrgencia": 4,
      "sistemaAutomotrizId": 1,
      "nombreSistema": "Sistema de Motor"
    }
  ]
}
```

#### Códigos de Respuesta
- `200 OK` - Sistema encontrado
- `404 Not Found` - Sistema no existe
- `400 Bad Request` - ID inválido

---

### 8. Buscar Sistemas por Nombre

**GET** `/api/sistemas/buscar/{nombre}`

Realiza una búsqueda parcial (case-insensitive) de sistemas automotrices por nombre.

#### Parameters
| Parámetro | Tipo | Ubicación | Requerido | Validación | Descripción |
|-----------|------|-----------|-----------|------------|-------------|
| `nombre` | string | URL | ✅ | Min: 3 caracteres | Término de búsqueda parcial |

#### Ejemplos de URLs
- `/api/sistemas/buscar/motor` → Encuentra "Sistema de Motor"
- `/api/sistemas/buscar/fren` → Encuentra "Sistema de Frenos"
- `/api/sistemas/buscar/elec` → Encuentra "Sistema Eléctrico"

#### Response (200 OK)
```json
[
  {
    "id": 1,
    "nombre": "Sistema de Motor",
    "descripcion": "Motor de combustión interna, componentes y funcionamiento.",
    "sintomas": [...]
  }
]
```

#### Respuesta Vacía (200 OK)
```json
[]
```

#### Códigos de Respuesta
- `200 OK` - Búsqueda realizada (puede retornar array vacío)
- `400 Bad Request` - Nombre muy corto (menos de 3 caracteres)

---

## Autenticación

### JWT Bearer Token

Todos los endpoints marcados con 🔒 requieren autenticación mediante JWT.

#### Header de Autenticación
```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

### Endpoints con Autenticación Requerida

| Endpoint | Método | Autenticación |
|----------|--------|---------------|
| `/api/diagnostico/diagnosticar` | POST | 🔒 Requerida |
| `/api/diagnostico/historial` | GET | 🔒 Requerida |
| `/api/diagnostico/feedback/{consultaId}` | POST | 🔒 Requerida |
| `/api/sistemas` | GET | ❌ Pública |
| `/api/sistemas/{id}` | GET | ❌ Pública |
| `/api/sistemas/buscar/{nombre}` | GET | ❌ Pública |
| `/api/diagnostico/sintomas/{sistemaId}` | GET | ❌ Pública |
| `/api/diagnostico/causa/{causaId}` | GET | ❌ Pública |

### Obtener Token

Para obtener un token JWT, autentícate mediante el endpoint de login de AutoGuía:

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "usuario@ejemplo.com",
  "password": "tu_password"
}
```

**Respuesta**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-10-24T15:30:00Z"
}
```

---

## Niveles de Urgencia

El sistema clasifica los problemas en 4 niveles de urgencia:

| Nivel | Clasificación | Descripción | Acción Recomendada | Color UI |
|-------|---------------|-------------|-------------------|----------|
| **1** | 🟢 **Leve** | Problema menor que no afecta funcionamiento crítico | Monitorear y aplicar recomendaciones preventivas | Verde |
| **2** | 🟡 **Moderado** | Problema que debe atenderse pronto para evitar daños | Programar servicio en próximos 7-15 días | Amarillo |
| **3** | 🟠 **Alto** | Problema importante que requiere atención inmediata | Llevar a taller en próximos 1-3 días | Naranja |
| **4** | 🔴 **Crítico** | Problema peligroso que compromete seguridad | ⚠️ **DETENER VEHÍCULO INMEDIATAMENTE** | Rojo |

### Ejemplos por Nivel

#### Nivel 1 - Leve
- Ruidos leves en suspensión
- Desgaste normal de escobillas limpiaparabrisas
- Luz de revisión general encendida

#### Nivel 2 - Moderado
- Pérdida leve de potencia
- Vibraciones al frenar
- Consumo elevado de combustible

#### Nivel 3 - Alto
- Sobrecalentamiento del motor
- Ruidos fuertes en frenos
- Fugas de líquidos importantes

#### Nivel 4 - Crítico
- Motor no enciende
- Frenos no responden
- Luces de freno ABS y freno de mano encendidas simultáneamente

---

## Códigos de Error

### Códigos HTTP Estándar

| Código | Nombre | Descripción | Acción del Cliente |
|--------|--------|-------------|-------------------|
| **200** | OK | Solicitud exitosa | Procesar respuesta |
| **400** | Bad Request | Validación fallida en datos enviados | Revisar datos de entrada |
| **401** | Unauthorized | Token inválido, expirado o ausente | Re-autenticar usuario |
| **404** | Not Found | Recurso no existe | Verificar ID/URL |
| **500** | Internal Server Error | Error en el servidor | Reintentar más tarde |

### Estructura de Errores

Todos los errores siguen este formato JSON:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "descripcionSintoma": [
      "El campo descripcionSintoma debe tener entre 10 y 1000 caracteres."
    ]
  },
  "traceId": "00-3c8d9f2e4a5b6c7d8e9f0a1b2c3d4e5f-1234567890abcdef-00"
}
```

### Errores Comunes

#### 400 Bad Request - Validación

**Descripción muy corta**:
```json
{
  "errors": {
    "descripcionSintoma": [
      "El campo descripcionSintoma debe tener al menos 10 caracteres."
    ]
  }
}
```

**Descripción muy larga**:
```json
{
  "errors": {
    "descripcionSintoma": [
      "El campo descripcionSintoma no puede exceder 1000 caracteres."
    ]
  }
}
```

#### 401 Unauthorized

```json
{
  "title": "Unauthorized",
  "status": 401,
  "detail": "Token de autenticación inválido o expirado"
}
```

#### 404 Not Found

```json
{
  "title": "Not Found",
  "status": 404,
  "detail": "Sistema con ID 999 no encontrado"
}
```

---

## Ejemplos de Uso

### cURL

#### 1. Realizar Diagnóstico
```bash
curl -X POST https://localhost:5070/api/diagnostico/diagnosticar \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "descripcionSintoma": "El motor hace ruidos metálicos al arrancar en frío"
  }'
```

#### 2. Obtener Sistemas
```bash
curl -X GET https://localhost:5070/api/sistemas \
  -H "Content-Type: application/json"
```

#### 3. Buscar Sistema por Nombre
```bash
curl -X GET https://localhost:5070/api/sistemas/buscar/motor \
  -H "Content-Type: application/json"
```

#### 4. Obtener Síntomas de un Sistema
```bash
curl -X GET https://localhost:5070/api/diagnostico/sintomas/1 \
  -H "Content-Type: application/json"
```

#### 5. Obtener Detalles de Causa
```bash
curl -X GET https://localhost:5070/api/diagnostico/causa/1 \
  -H "Content-Type: application/json"
```

#### 6. Obtener Historial
```bash
curl -X GET https://localhost:5070/api/diagnostico/historial \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json"
```

#### 7. Registrar Feedback
```bash
curl -X POST https://localhost:5070/api/diagnostico/feedback/1 \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{"fueUtil": true}'
```

---

### JavaScript (Fetch API)

#### Realizar Diagnóstico
```javascript
async function diagnosticar(descripcion, token) {
  const response = await fetch('https://localhost:5070/api/diagnostico/diagnosticar', {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      descripcionSintoma: descripcion
    })
  });

  if (!response.ok) {
    throw new Error(`Error HTTP: ${response.status}`);
  }

  return await response.json();
}

// Uso
try {
  const resultado = await diagnosticar(
    'El motor pierde potencia al subir pendientes',
    'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'
  );
  console.log('Diagnóstico:', resultado);
} catch (error) {
  console.error('Error:', error);
}
```

#### Obtener Sistemas
```javascript
async function obtenerSistemas() {
  const response = await fetch('https://localhost:5070/api/sistemas');
  return await response.json();
}

// Uso
const sistemas = await obtenerSistemas();
sistemas.forEach(sistema => {
  console.log(`${sistema.nombre}: ${sistema.sintomas.length} síntomas`);
});
```

#### Registrar Feedback
```javascript
async function registrarFeedback(consultaId, fueUtil, token) {
  const response = await fetch(
    `https://localhost:5070/api/diagnostico/feedback/${consultaId}`,
    {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ fueUtil })
    }
  );

  return await response.json();
}

// Uso
await registrarFeedback(1, true, 'token_here');
```

---

### C# (HttpClient)

#### Realizar Diagnóstico
```csharp
public class DiagnosticoClient
{
    private readonly HttpClient _httpClient;
    private readonly string _token;

    public DiagnosticoClient(HttpClient httpClient, string token)
    {
        _httpClient = httpClient;
        _token = token;
    }

    public async Task<ResultadoDiagnosticoDto> DiagnosticarAsync(string descripcion)
    {
        var request = new { descripcionSintoma = descripcion };
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _token);

        var response = await _httpClient.PostAsJsonAsync(
            "/api/diagnostico/diagnosticar", 
            request
        );

        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<ResultadoDiagnosticoDto>();
    }
}

// Uso
var client = new DiagnosticoClient(httpClient, token);
var resultado = await client.DiagnosticarAsync("El motor vibra al arrancar");
Console.WriteLine($"Urgencia: {resultado.NivelUrgencia}");
Console.WriteLine($"Causas: {resultado.CausasPosibles.Count}");
```

#### Obtener Historial
```csharp
public async Task<List<ConsultaDiagnosticoDto>> ObtenerHistorialAsync()
{
    _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", _token);

    var response = await _httpClient.GetAsync("/api/diagnostico/historial");
    response.EnsureSuccessStatusCode();
    
    return await response.Content
        .ReadFromJsonAsync<List<ConsultaDiagnosticoDto>>();
}
```

---

### Python (Requests)

#### Realizar Diagnóstico
```python
import requests

def diagnosticar(descripcion, token):
    url = "https://localhost:5070/api/diagnostico/diagnosticar"
    headers = {
        "Authorization": f"Bearer {token}",
        "Content-Type": "application/json"
    }
    data = {
        "descripcionSintoma": descripcion
    }
    
    response = requests.post(url, json=data, headers=headers)
    response.raise_for_status()
    
    return response.json()

# Uso
resultado = diagnosticar(
    "El motor hace ruidos extraños",
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
)
print(f"Urgencia: {resultado['nivelUrgencia']}")
print(f"Recomendación: {resultado['recomendacion']}")
```

#### Obtener Sistemas
```python
def obtener_sistemas():
    url = "https://localhost:5070/api/sistemas"
    response = requests.get(url)
    response.raise_for_status()
    return response.json()

sistemas = obtener_sistemas()
for sistema in sistemas:
    print(f"{sistema['nombre']}: {len(sistema['sintomas'])} síntomas")
```

---

## Mejores Prácticas

### 1. Validación de Entrada

✅ **DO**
```javascript
// Validar antes de enviar
function validarDescripcion(desc) {
  if (!desc || desc.trim().length < 10) {
    throw new Error('Descripción muy corta');
  }
  if (desc.length > 1000) {
    throw new Error('Descripción muy larga');
  }
  return desc.trim();
}

const descripcion = validarDescripcion(userInput);
await diagnosticar(descripcion, token);
```

❌ **DON'T**
```javascript
// No validar y dejar que el servidor rechace
await diagnosticar(userInput, token); // Puede fallar con 400
```

---

### 2. Manejo de Errores

✅ **DO**
```javascript
try {
  const resultado = await diagnosticar(descripcion, token);
  mostrarResultado(resultado);
} catch (error) {
  if (error.status === 401) {
    // Token expirado, re-autenticar
    await renovarToken();
  } else if (error.status === 400) {
    // Error de validación
    mostrarErrores(error.errors);
  } else {
    // Error genérico
    mostrarMensajeError('Error al procesar diagnóstico');
  }
}
```

❌ **DON'T**
```javascript
const resultado = await diagnosticar(descripcion, token);
// Sin manejo de errores - crash si hay error
```

---

### 3. Rate Limiting

⚡ **Límites Recomendados**
- **Diagnósticos**: Máximo 10 por minuto por usuario
- **Consultas de catálogos**: Máximo 100 por minuto
- **Historial**: Máximo 20 por minuto

✅ **DO**
```javascript
// Implementar debouncing para búsquedas
const debouncedSearch = debounce(async (query) => {
  const sistemas = await buscarSistemas(query);
  mostrarResultados(sistemas);
}, 300);
```

❌ **DON'T**
```javascript
// Llamar en cada keystroke sin debounce
input.addEventListener('input', async (e) => {
  await buscarSistemas(e.target.value); // Muchas requests
});
```

---

### 4. Caching

Los catálogos de sistemas y síntomas cambian raramente. Implementar cache:

✅ **DO**
```javascript
// Cache con expiración
const cache = {
  sistemas: null,
  timestamp: null,
  TTL: 3600000 // 1 hora
};

async function obtenerSistemasConCache() {
  const now = Date.now();
  
  if (cache.sistemas && (now - cache.timestamp) < cache.TTL) {
    return cache.sistemas; // Retornar desde cache
  }
  
  // Fetch desde API
  cache.sistemas = await fetch('/api/sistemas').then(r => r.json());
  cache.timestamp = now;
  
  return cache.sistemas;
}
```

---

### 5. Logging y Telemetría

✅ **DO**
```javascript
// Registrar diagnósticos para análisis
async function diagnosticarConTelemetria(descripcion, token) {
  const inicio = Date.now();
  
  try {
    const resultado = await diagnosticar(descripcion, token);
    
    // Log exitoso
    logger.info('Diagnóstico exitoso', {
      duracion: Date.now() - inicio,
      nivelUrgencia: resultado.nivelUrgencia,
      causasEncontradas: resultado.causasPosibles.length
    });
    
    return resultado;
  } catch (error) {
    // Log de error
    logger.error('Error en diagnóstico', {
      duracion: Date.now() - inicio,
      error: error.message,
      statusCode: error.status
    });
    throw error;
  }
}
```

---

### 6. Timeout y Reintentos

✅ **DO**
```javascript
// Configurar timeout y reintentos
async function fetchConReintentos(url, options, maxReintentos = 3) {
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 10000); // 10s
  
  for (let i = 0; i < maxReintentos; i++) {
    try {
      const response = await fetch(url, {
        ...options,
        signal: controller.signal
      });
      clearTimeout(timeout);
      return response;
    } catch (error) {
      if (i === maxReintentos - 1) throw error;
      await sleep(1000 * Math.pow(2, i)); // Exponential backoff
    }
  }
}
```

---

### 7. Seguridad

✅ **DO**
- Almacenar tokens en `httpOnly cookies` o `sessionStorage`
- Nunca exponer tokens en URLs
- Implementar HTTPS en producción
- Validar datos antes de enviar
- Sanitizar descripciones de síntomas

❌ **DON'T**
- Almacenar tokens en `localStorage` (vulnerable a XSS)
- Enviar tokens en query parameters
- Confiar en datos del cliente sin validación

---

### 8. Paginación (Futuro)

Para historial extenso, implementar paginación:

```javascript
// Futuro endpoint con paginación
GET /api/diagnostico/historial?page=1&pageSize=10

// Respuesta
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalItems": 45,
  "totalPages": 5
}
```

---

## Versionamiento de API

### Versión Actual: v1.0

La API sigue versionamiento semántico:
- **MAJOR**: Cambios incompatibles (breaking changes)
- **MINOR**: Nueva funcionalidad compatible
- **PATCH**: Correcciones de bugs

### Deprecación de Features

Features deprecados se marcarán con `X-Deprecated` header:

```http
HTTP/1.1 200 OK
X-Deprecated: This endpoint will be removed in v2.0. Use /api/v2/diagnostico instead.
```

---

## Soporte y Contacto

- **Documentación**: https://docs.Rodavia.com
- **Issues**: https://github.com/Rodavia/issues
- **Email**: soporte@Rodavia.com

---

## Changelog

### v1.0 (2025-10-23)
- ✅ Lanzamiento inicial
- ✅ 8 endpoints de diagnóstico y sistemas
- ✅ Autenticación JWT
- ✅ Niveles de urgencia 1-4
- ✅ Feedback de usuarios

---

**Última actualización**: 23 de octubre de 2025  
**Versión de Documento**: 1.0  
**Autor**: Equipo AutoGuía
