# API Empacadora

API REST para el sistema de empacadora con autenticación JWT.

## Estructura del Proyecto

```
AppAPIEmpacadora/
├── Controllers/           # Controladores de la API
├── Models/               # Modelos y DTOs
│   ├── DTOs/            # Data Transfer Objects
│   ├── Entities/        # Entidades de dominio
│   └── Exceptions/      # Excepciones personalizadas
├── Services/            # Servicios de aplicación
│   ├── Interfaces/      # Interfaces de servicios
│   └── Implementations/ # Implementaciones de servicios
├── Repositories/        # Repositorios de datos
│   ├── Interfaces/      # Interfaces de repositorios
│   └── Implementations/ # Implementaciones de repositorios
├── Infrastructure/      # Configuración e implementaciones de infraestructura
│   ├── Data/           # Configuración de base de datos
│   └── Repositories/   # Implementaciones de repositorios
├── Middleware/         # Middleware personalizado
├── Migrations/         # Scripts de migración de base de datos
└── wwwroot/uploads/    # Archivos subidos por usuarios
```

## Arquitectura

El proyecto sigue una arquitectura en capas con Clean Architecture:

1. **Capa de Presentación (Controllers)**
   - Maneja las peticiones HTTP
   - Valida los datos de entrada
   - Devuelve respuestas HTTP apropiadas
   - No contiene lógica de negocio

2. **Capa de Aplicación (Services)**
   - Implementa la lógica de negocio
   - Coordina las operaciones entre diferentes componentes
   - Maneja la transformación de datos entre DTOs y entidades

3. **Capa de Dominio (Models/Entities)**
   - Define las entidades del negocio
   - Contiene las reglas de negocio
   - Define las interfaces de repositorio

4. **Capa de Infraestructura (Infrastructure)**
   - Implementa las interfaces de repositorio
   - Maneja la persistencia de datos
   - Proporciona servicios técnicos

## Principios de Diseño

- **SOLID**: Seguimos los principios SOLID para un código mantenible y escalable
- **DRY**: No repetir código
- **Dependency Injection**: Para un acoplamiento bajo entre componentes
- **Repository Pattern**: Para abstraer el acceso a datos
- **Unit of Work**: Para manejar transacciones
- **CQRS**: Separación de comandos y consultas cuando sea necesario

## Tecnologías Utilizadas

- .NET 8
- Entity Framework Core
- SQL Server
- JWT para autenticación
- Swagger para documentación
- AutoMapper para mapeo de objetos
- FluentValidation para validaciones
- Serilog para logging

## Configuración de CORS

### Problema
El error de CORS ocurre cuando el frontend (que corre en `http://localhost:3000`) intenta hacer peticiones al backend (que corre en `http://localhost/AppAPIEmpacadora`) y el servidor no tiene configurado CORS correctamente.

### Solución Implementada

#### 1. Configuración en Program.cs
Se han configurado dos políticas de CORS:

```csharp
// Política general que permite cualquier origen
options.AddPolicy("PermitirTodo", policy =>
{
    policy.SetIsOriginAllowed(origin => true)
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
});

// Política específica para desarrollo
options.AddPolicy("Desarrollo", policy =>
{
    policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()
          .AllowCredentials();
});
```

#### 2. Middleware de CORS
El middleware se aplica de forma condicional según el entorno:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseCors("Desarrollo");
}
else
{
    app.UseCors("PermitirTodo");
}
```

#### 3. Manejo de Peticiones OPTIONS
Se agregó un middleware personalizado para manejar las peticiones preflight OPTIONS:

```csharp
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", context.Request.Headers["Origin"]);
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});
```

## Sistema de Manejo de Excepciones y Logging

### Excepciones Personalizadas
Se crearon excepciones específicas para diferentes tipos de errores:

- **EntityNotFoundException**: Para entidades no encontradas
- **ValidationException**: Para errores de validación
- **DatabaseException**: Para errores de base de datos
- **FileOperationException**: Para errores de operaciones de archivo
- **BusinessRuleException**: Para violaciones de reglas de negocio

### Servicio de Logging
Se implementó un servicio de logging estructurado (`ILoggingService`) que incluye:

- Logging de información, advertencias, errores y críticos
- Logging específico para operaciones de base de datos
- Logging de operaciones de archivo
- Logging de acciones de usuario
- Logging de violaciones de reglas de negocio

### Middleware de Manejo de Excepciones
Se creó un middleware global que:

- Captura todas las excepciones no manejadas
- Mapea excepciones específicas a códigos de estado HTTP apropiados
- Registra las excepciones en el sistema de logging
- Devuelve respuestas JSON estructuradas con información del error

## Endpoints Principales

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

### Cajas
- `GET /api/Cajas` - Obtener todas las cajas
- `GET /api/Cajas/{id}` - Obtener caja por ID
- `POST /api/Cajas` - Crear nueva caja
- `PUT /api/Cajas/{id}` - Actualizar caja
- `DELETE /api/Cajas/{id}` - Eliminar caja
- `POST /api/Cajas/ajustar-cantidad` - Ajustar cantidad de caja

### Pedidos Cliente
- `GET /api/pedidos-cliente` - Obtener todos los pedidos
- `GET /api/pedidos-cliente/{id}` - Obtener pedido por ID
- `POST /api/pedidos-cliente` - Crear nuevo pedido
- `POST /api/pedidos-cliente/con-ordenes` - Crear pedido con órdenes
- `GET /api/pedidos-cliente/{id}/progreso` - Obtener progreso del pedido
- `GET /api/pedidos-clientes/disponibles/{tipo}` - Obtener pedidos disponibles por tipo
- `POST /api/pedidos-cliente/{pedidoId}/asignar-tarimas` - Asignar tarimas a pedido
- `DELETE /api/pedidos-cliente/desasignar-tarimas` - Desasignar tarimas de pedidos

### Tarimas
- `GET /api/tarimas` - Obtener todas las tarimas
- `GET /api/tarimas/{id}` - Obtener tarima por ID
- `POST /api/tarimas` - Crear nueva tarima
- `PUT /api/tarimas/{id}` - Actualizar tarima
- `DELETE /api/tarimas/{id}` - Eliminar tarima

### Proveedores
- `GET /api/proveedores` - Obtener todos los proveedores
- `GET /api/proveedores/{id}` - Obtener proveedor por ID
- `GET /api/proveedores/completo/{id}` - Obtener proveedor completo con archivos
- `POST /api/proveedores` - Crear nuevo proveedor (con archivos)
- `PUT /api/proveedores/{id}` - Actualizar proveedor (con archivos)
- `DELETE /api/proveedores/{id}` - Eliminar proveedor

### Clientes
- `GET /api/clientes` - Obtener todos los clientes
- `GET /api/clientes/{id}` - Obtener cliente por ID
- `POST /api/clientes` - Crear nuevo cliente
- `PUT /api/clientes/{id}` - Actualizar cliente
- `DELETE /api/clientes/{id}` - Eliminar cliente

## Funcionalidades Especiales

### Asignación Múltiple de Tarimas
Permite asignar múltiples tarimas a un pedido de cliente y calcular automáticamente el porcentaje de surtido del pedido.

**Endpoint**: `POST /api/pedidos-cliente/{pedidoId}/asignar-tarimas`

**Funcionalidades**:
- Validaciones de existencia de pedido y tarimas
- Asignación de tarimas con transacciones
- Cálculo automático de porcentaje de surtido
- Actualización automática del estatus del pedido
- Auditoría de operaciones

### Desasignación Masiva de Tarimas
Permite desasignar múltiples tarimas de pedidos de cliente de manera masiva.

**Endpoint**: `DELETE /api/pedidos-cliente/desasignar-tarimas`

**Funcionalidades**:
- Eliminación de relaciones PedidoTarima
- Re cálculo de porcentaje de surtido
- Actualización automática de estatus
- Respuesta con pedidos afectados

### Manejo de Archivos
Sistema para manejar archivos de situación fiscal en proveedores.

**Características**:
- Almacenamiento en `wwwroot/uploads/situaciones-fiscales/`
- Nombres únicos con GUID
- URLs completas para frontend
- Limpieza automática de archivos antiguos
- Soporte para PDF, imágenes y documentos Word

### Pedidos con Órdenes
Endpoint para crear pedidos junto con múltiples órdenes asociadas en una sola operación transaccional.

**Endpoint**: `POST /api/pedidos-cliente/con-ordenes`

**Características**:
- Operación transaccional
- Creación de pedido y órdenes en una sola operación
- Respuesta incluye órdenes creadas
- Validaciones completas

### Progreso de Pedidos
Endpoint para obtener el progreso detallado de un pedido cliente.

**Endpoint**: `GET /api/pedidos-cliente/{id}/progreso`

**Información incluida**:
- Detalles del pedido
- Órdenes asociadas
- Tarimas relacionadas
- Porcentaje de surtido
- Información de productos

### Pedidos Disponibles
Endpoint para obtener pedidos que tienen disponibilidad para un tipo específico.

**Endpoint**: `GET /api/pedidos-clientes/disponibles/{tipo}`

**Lógica**:
- Calcula cantidad disponible restando cajas ya asignadas
- Filtra solo pedidos con disponibilidad > 0
- Retorna cantidad disponible en lugar de solicitada

## DTOs Principales

### CajaDTO
```json
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

### PedidoClienteDTO
```json
{
    "id": 1,
    "observaciones": "Pedido urgente",
    "estatus": "Surtiendo",
    "fechaEmbarque": "2024-01-15T00:00:00",
    "fechaModificacion": "2024-01-10T15:30:00",
    "fechaRegistro": "2024-01-10T10:30:00",
    "usuarioRegistro": "admin",
    "activo": true,
    "sucursal": "Sucursal Centro",
    "cliente": "Cliente ABC",
    "porcentajeSurtido": 75.50
}
```

### ProveedorDTO
```json
{
    "id": 1,
    "nombre": "Proveedor Ejemplo",
    "rfc": "XAXX010101000",
    "telefono": "555-1234",
    "correo": "proveedor@ejemplo.com",
    "direccionFiscal": "Calle Principal 123",
    "situacionFiscal": "https://tu-dominio.com/uploads/situaciones-fiscales/guid_documento.pdf",
    "fechaRegistro": "2024-01-10T10:30:00",
    "usuarioRegistro": "admin",
    "activo": true
}
```

## Configuración de Logging

### Instalación de Dependencias
Para que el logging a archivos funcione correctamente, instala el paquete NuGet:

```bash
dotnet add package Serilog.Extensions.Logging.File
```

### Configuración en appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": "Information",
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ]
  }
}
```

## Orden de Middleware Importante
El orden de los middlewares es crucial:

1. `app.UseHttpsRedirection()`
2. `app.UseStaticFiles()`
3. Middleware personalizado para OPTIONS
4. `app.UseCors()` - **DEBE ir antes de UseAuthentication y UseAuthorization**
5. `app.UseAuthentication()`
6. `app.UseAuthorization()`
7. `app.UseMiddleware<ExceptionHandlingMiddleware>()`

## Ejemplos de Uso

### Autenticación
```bash
curl -X POST "https://api.ejemplo.com/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "password"}'
```

### Crear Pedido con Órdenes
```bash
curl -X POST "https://api.ejemplo.com/api/pedidos-cliente/con-ordenes" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "observaciones": "Pedido de tomates para exportación",
    "estatus": "Pendiente",
    "fechaEmbarque": "2024-12-25T10:00:00",
    "idSucursal": 1,
    "idCliente": 1,
    "fechaRegistro": "2024-12-20T08:00:00",
    "ordenes": [
      {
        "tipo": "XL",
        "cantidad": 100,
        "peso": 50.5,
        "idProducto": 1
      }
    ]
  }'
```

### Asignar Tarimas a Pedido
```bash
curl -X POST "https://api.ejemplo.com/api/pedidos-cliente/1/asignar-tarimas" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '[1, 2, 3]'
```

### Subir Archivo de Proveedor
```bash
curl -X POST "https://api.ejemplo.com/api/proveedores" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "nombre=Proveedor Ejemplo" \
  -F "rfc=XAXX010101000" \
  -F "situacionFiscal=@documento.pdf" \
  -F "fechaRegistro=2024-01-10T10:30:00"
```

## Notas Importantes

### Seguridad
- Los archivos son públicos y accesibles directamente desde la URL
- Solo usuarios autenticados pueden realizar operaciones de escritura
- Se utilizan tokens JWT para autenticación

### Transacciones
- Las operaciones complejas se ejecutan dentro de transacciones
- Se garantiza la integridad de los datos
- Rollback automático en caso de fallo

### Auditoría
- Se registra el usuario que realiza cada operación
- Se mantiene fecha de registro y modificación
- Logs detallados para debugging y auditoría

### Consideraciones de Rendimiento
- Los servicios Singleton se crean una sola vez y se reutilizan
- Los repositorios y servicios de negocio son Scoped
- Se utiliza Entity Framework Core para optimización de consultas

## Troubleshooting

### Error de CORS
- Verifica que el middleware de CORS esté antes de UseAuthentication
- Asegúrate de que el origen del frontend esté incluido en la política
- Revisa configuraciones conflictivas en IIS

### Error de Archivos
- Verifica permisos de escritura en el directorio wwwroot/uploads
- Asegúrate de que el directorio existe
- Revisa el tamaño máximo de archivos permitido

### Error de Base de Datos
- Verifica la cadena de conexión en appsettings.json
- Asegúrate de que las migraciones estén aplicadas
- Revisa los logs para errores específicos