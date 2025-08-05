# Endpoint de Progreso de Pedido Cliente

## Descripción
Este endpoint permite obtener el progreso detallado de un pedido cliente específico, incluyendo sus órdenes asociadas y las tarimas relacionadas.

## Endpoint
```
GET /api/pedidocliente/{id}/progreso
```

## Parámetros
- `id` (int, requerido): ID del pedido cliente

## Respuesta
Retorna un objeto `PedidoClienteProgresoDTO` con la siguiente estructura:

```json
{
  "id": 1,
  "estatus": "En Proceso",
  "porcentajeSurtido": 75.5,
  "observaciones": "Pedido urgente",
  "ordenes": [
    {
      "id": 1,
      "tipo": "Caja",
      "cantidad": 100,
      "peso": 25.5,
      "idProducto": 1,
      "idPedidoCliente": 1,
      "estatus": "Activo",
      "fechaRegistro": "2024-01-15T10:30:00",
      "usuarioRegistro": "usuario1",
      "producto": {
        "id": 1,
        "nombre": "Tomate",
        "codigo": "TOM001",
        "variedad": "Cherry"
      },
      "pedidoCliente": {
        "id": 1,
        "observaciones": "Pedido urgente",
        "estatus": "En Proceso"
      }
    }
  ],
           "tarimas": [
      {
        "id": 1,
        "codigo": "TAR001",
        "estatus": "Completa",
        "observaciones": "Tarima con 50 cajas",
        "upc": "123456789",
        "peso": 1250.5,
                 "tarimasClasificaciones": [
           {
             "tipo": "Caja",
             "peso": 25.5,
             "cantidad": 50
           }
         ]
      }
    ]
}
```

## Códigos de Respuesta
- `200 OK`: Pedido encontrado y progreso retornado exitosamente
- `404 Not Found`: Pedido cliente no encontrado

## Autenticación
Requiere autenticación mediante JWT token en el header:
```
Authorization: Bearer {token}
```

## Ejemplo de Uso

### Request
```bash
curl -X GET "https://api.empacadora.com/api/pedidocliente/1/progreso" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

### Response
```json
{
  "id": 1,
  "estatus": "En Proceso",
  "porcentajeSurtido": 75.5,
  "observaciones": "Pedido urgente",
  "ordenes": [...],
  "tarimas": [...]
}
```

## Implementación Técnica

### Arquitectura
- **Controller**: `PedidoClienteController.GetProgreso(int id)`
- **Service**: `PedidoClienteService.ObtenerProgresoAsync(int id)`
- **Repository**: `PedidoClienteRepository.ObtenerPorIdAsync(int id)`
- **DTO**: `PedidoClienteProgresoDTO`

### Dependencias
- `IPedidoClienteService`
- `IOrdenPedidoClienteService`
- `ITarimaService`

### Relaciones Incluidas
- PedidoCliente → Sucursal
- PedidoCliente → Cliente
- PedidoCliente → PedidoTarimas → Tarima → TarimasClasificaciones
- OrdenesPedidoCliente → Producto

## Notas de Implementación
- El endpoint incluye las relaciones necesarias para obtener información completa del pedido
- Las tarimas se obtienen a través de la relación `PedidoTarima`
- El porcentaje de surtido se calcula desde la entidad `PedidoCliente`
- Se incluyen validaciones para pedidos no encontrados 