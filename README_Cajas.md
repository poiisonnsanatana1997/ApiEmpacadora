# Entidad Cajas

## Descripción
La entidad `Caja` representa las cajas utilizadas en el sistema de empacadora. Cada caja tiene un tipo, cantidad, peso y puede estar asociada a una clasificación específica.

## Estructura de la Base de Datos

### Tabla: Cajas
- **Id** (int, PK, IDENTITY): Identificador único de la caja
- **Tipo** (nvarchar(20), NOT NULL): Tipo de caja
- **Cantidad** (decimal(18,2), NULL): Cantidad de la caja
- **Peso** (decimal(18,2), NULL): Peso de la caja
- **FechaRegistro** (datetime2, NOT NULL): Fecha de registro de la caja
- **UsuarioRegistro** (nvarchar(50), NOT NULL): Usuario que registró la caja
- **IdClasificacion** (int, NULL, FK): Referencia a la clasificación asociada

### Relaciones
- **Clasificacion**: Relación opcional con la tabla Clasificaciones (FK: IdClasificacion)

## Endpoints de la API

### GET /api/Cajas
Obtiene todas las cajas registradas.
- **Respuesta**: Lista de `CajaSummaryDTO`
- **Autenticación**: No requerida

### GET /api/Cajas/{id}
Obtiene una caja específica por su ID.
- **Parámetros**: `id` (int) - ID de la caja
- **Respuesta**: `CajaDTO` o 404 si no existe
- **Autenticación**: No requerida

### POST /api/Cajas
Crea una nueva caja.
- **Body**: `CreateCajaDTO`
- **Respuesta**: `CajaDTO` con la caja creada
- **Autenticación**: Requerida (JWT)

### PUT /api/Cajas/{id}
Actualiza una caja existente.
- **Parámetros**: `id` (int) - ID de la caja
- **Body**: `UpdateCajaDTO`
- **Respuesta**: 204 (No Content) o 404 si no existe
- **Autenticación**: Requerida (JWT)

### DELETE /api/Cajas/{id}
Elimina una caja.
- **Parámetros**: `id` (int) - ID de la caja
- **Respuesta**: 204 (No Content) o 404 si no existe
- **Autenticación**: Requerida (JWT)

### POST /api/Cajas/ajustar-cantidad
Ajusta la cantidad de una caja existente o crea una nueva si no existe.
- **Body**: `AjustarCantidadCajaDTO`
- **Respuesta**: `CajaDTO` con la caja actualizada
- **Autenticación**: Requerida (JWT)

## DTOs

### CajaDTO
```csharp
{
    "id": 1,
    "tipo": "Estándar",
    "cantidad": 100.50,
    "peso": 25.75,
    "fechaRegistro": "2024-07-24T21:59:12",
    "usuarioRegistro": "admin",
    "idClasificacion": 1
}
```

### CajaSummaryDTO
```csharp
{
    "id": 1,
    "tipo": "Estándar",
    "cantidad": 100.50,
    "peso": 25.75,
    "fechaRegistro": "2024-07-24T21:59:12"
}
```

### CreateCajaDTO
```csharp
{
    "tipo": "Estándar",           // Requerido, máximo 20 caracteres
    "cantidad": 100.50,          // Opcional
    "peso": 25.75,               // Opcional
    "idClasificacion": 1         // Opcional
}
```

### UpdateCajaDTO
```csharp
{
    "tipo": "Estándar",           // Opcional, máximo 20 caracteres
    "cantidad": 100.50,          // Opcional
    "peso": 25.75,               // Opcional
    "idClasificacion": 1         // Opcional
}
```

### AjustarCantidadCajaDTO
```csharp
{
    "tipo": "Estándar",           // Requerido, máximo 20 caracteres
    "cantidadAjuste": 50.25,     // Requerido (positivo para sumar, negativo para restar)
    "pesoAjuste": 12.5,          // Opcional
    "idClasificacion": 1         // Opcional
}
```

## Ejemplos de Uso

### Crear una nueva caja
```http
POST /api/Cajas
Authorization: Bearer {token}
Content-Type: application/json

{
    "tipo": "Estándar",
    "cantidad": 100.50,
    "peso": 25.75,
    "idClasificacion": 1
}
```

### Obtener todas las cajas
```http
GET /api/Cajas
```

### Actualizar una caja
```http
PUT /api/Cajas/1
Authorization: Bearer {token}
Content-Type: application/json

{
    "tipo": "Especial",
    "peso": 30.00
}
```

### Ajustar cantidad de caja (sumar)
```http
POST /api/Cajas/ajustar-cantidad
Authorization: Bearer {token}
Content-Type: application/json

{
    "tipo": "Estándar",
    "cantidadAjuste": 50.25,
    "pesoAjuste": 12.5,
    "idClasificacion": 1
}
```

### Ajustar cantidad de caja (restar)
```http
POST /api/Cajas/ajustar-cantidad
Authorization: Bearer {token}
Content-Type: application/json

{
    "tipo": "Estándar",
    "cantidadAjuste": -25.0,
    "idClasificacion": 1
}
```

## Arquitectura

La implementación sigue el patrón de arquitectura establecido en el proyecto:

- **Entidad**: `Models/Entities/Caja.cs`
- **DTOs**: `Models/DTOs/CajaDTOs.cs`
- **Repositorio**: `Infrastructure/Repositories/CajaRepository.cs`
- **Interfaz del Repositorio**: `Repositories/Interfaces/ICajaRepository.cs`
- **Servicio**: `Services/CajaService.cs`
- **Interfaz del Servicio**: `Services/Interfaces/ICajaService.cs`
- **Controlador**: `Controllers/CajasController.cs`

## Migración

La migración `20250724215912_AddCajaEntity` se aplicó exitosamente a la base de datos, creando la tabla `Cajas` con todas las columnas y relaciones necesarias. 