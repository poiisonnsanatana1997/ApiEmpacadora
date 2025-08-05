# Endpoint: Crear Pedido Cliente con Órdenes

## Descripción
Nuevo endpoint que permite crear un pedido de cliente junto con múltiples órdenes asociadas en una sola operación transaccional.

## Endpoint
```
POST /api/PedidoCliente/con-ordenes
```

## Autenticación
Requiere token JWT válido en el header Authorization.

## Estructura de Request

### CreatePedidoClienteConOrdenesDTO
```json
{
    "observaciones": "string (requerido)",
    "estatus": "string (requerido)",
    "fechaEmbarque": "DateTime? (opcional)",
    "idSucursal": "int (requerido)",
    "idCliente": "int (requerido)",
    "fechaRegistro": "DateTime (requerido)",
    "activo": "bool (opcional, default: true)",
    "ordenes": [
        {
            "tipo": "string (requerido, max 20 caracteres)",
            "cantidad": "decimal? (opcional)",
            "peso": "decimal? (opcional)",
            "idProducto": "int? (opcional)"
        }
    ]
}
```

## Estructura de Response

### PedidoClienteConOrdenesResponseDTO
```json
{
    "id": "int",
    "observaciones": "string",
    "estatus": "string",
    "fechaEmbarque": "DateTime?",
    "fechaModificacion": "DateTime?",
    "fechaRegistro": "DateTime",
    "usuarioRegistro": "string",
    "activo": "bool",
    "sucursal": "string",
    "cliente": "string",
    "ordenes": [
        {
            "id": "int",
            "tipo": "string",
            "cantidad": "decimal?",
            "peso": "decimal?",
            "fechaRegistro": "DateTime",
            "usuarioRegistro": "string",
            "producto": {
                "id": "int",
                "nombre": "string",
                "codigo": "string"
            }
        }
    ]
}
```

## Ejemplo de Uso

### Request
```http
POST /api/PedidoCliente/con-ordenes
Authorization: Bearer {token}
Content-Type: application/json

{
    "observaciones": "Pedido de tomates para exportación",
    "estatus": "Pendiente",
    "fechaEmbarque": "2024-12-25T10:00:00",
    "idSucursal": 1,
    "idCliente": 1,
    "fechaRegistro": "2024-12-20T08:00:00",
    "activo": true,
    "ordenes": [
        {
            "tipo": "XL",
            "cantidad": 100,
            "peso": 50.5,
            "idProducto": 1
        },
        {
            "tipo": "L",
            "cantidad": 200,
            "peso": 75.2,
            "idProducto": 2
        }
    ]
}
```

### Response (201 Created)
```json
{
    "id": 1,
    "observaciones": "Pedido de tomates para exportación",
    "estatus": "Pendiente",
    "fechaEmbarque": "2024-12-25T10:00:00",
    "fechaModificacion": null,
    "fechaRegistro": "2024-12-20T08:00:00",
    "usuarioRegistro": "admin",
    "activo": true,
    "sucursal": "Sucursal Principal",
    "cliente": "Cliente Exportador S.A.",
    "ordenes": [
        {
            "id": 1,
            "tipo": "XL",
            "cantidad": 100,
            "peso": 50.5,
            "fechaRegistro": "2024-12-20T08:00:00",
            "usuarioRegistro": "admin",
            "producto": {
                "id": 1,
                "nombre": "Tomate Cherry",
                "codigo": "TOM-001"
            }
        },
        {
            "id": 2,
            "tipo": "L",
            "cantidad": 200,
            "peso": 75.2,
            "fechaRegistro": "2024-12-20T08:00:00",
            "usuarioRegistro": "admin",
            "producto": {
                "id": 2,
                "nombre": "Tomate Roma",
                "codigo": "TOM-002"
            }
        }
    ]
}
```

## Códigos de Respuesta

- **201 Created**: Pedido y órdenes creados exitosamente
- **400 Bad Request**: Datos de entrada inválidos
- **401 Unauthorized**: Token JWT inválido o faltante
- **500 Internal Server Error**: Error interno del servidor

## Validaciones

### Pedido Cliente
- `observaciones`: Requerido
- `estatus`: Requerido
- `idSucursal`: Requerido, debe existir en la base de datos
- `idCliente`: Requerido, debe existir en la base de datos
- `fechaRegistro`: Requerido

### Órdenes
- `tipo`: Requerido, máximo 20 caracteres
- `cantidad`: Opcional, decimal positivo
- `peso`: Opcional, decimal positivo
- `idProducto`: Opcional, debe existir en la base de datos si se proporciona

## Características

1. **Transaccional**: Si falla la creación de alguna orden, se revierte toda la operación
2. **Flexible**: Permite crear pedidos con 0, 1 o múltiples órdenes
3. **Consistente**: Todas las órdenes se asocian automáticamente al pedido creado
4. **Auditable**: Registra el usuario que realiza la operación

## Diferencias con el Endpoint Original

| Característica | POST /api/PedidoCliente | POST /api/PedidoCliente/con-ordenes |
|----------------|-------------------------|-------------------------------------|
| Crea pedido | ✅ | ✅ |
| Crea órdenes | ❌ | ✅ |
| Transaccional | ❌ | ✅ |
| Respuesta incluye órdenes | ❌ | ✅ |
| Complejidad | Simple | Compleja |

## Notas de Implementación

- El endpoint utiliza el patrón de arquitectura existente del proyecto
- Mantiene la separación de responsabilidades entre servicios
- Reutiliza la lógica de validación existente
- Sigue las convenciones de nomenclatura del proyecto 