# Endpoint: Pedidos Disponibles por Tipo

## Descripción
Este endpoint permite obtener los pedidos de clientes que tienen disponibilidad para un tipo específico (ej: "Caja", "Kilo"), calculando la cantidad disponible restando las cajas ya asignadas a través de tarimas.

## Endpoint
```
GET /api/pedidos-clientes/disponibles/{tipo}
```

## Parámetros
- `tipo` (string, requerido): Tipo de producto para filtrar (ej: "Caja", "Kilo")

## Respuesta
Retorna una lista de `PedidoClientePorAsignarDTO` con la cantidad disponible calculada.

### Estructura de respuesta
```json
[
  {
    "id": 1,
    "tipo": "Caja",
    "cantidad": 45,
    "peso": 225.00,
    "pesoCajaCliente": 5.00,
         "producto": {
       "id": 1,
       "nombre": "Tomate Cherry",
       "codigo": "TOM001",
       "variedad": "Cherry"
     },
    "cliente": {
      "id": 1,
      "razonSocial": "Supermercado ABC"
    },
    "sucursal": {
      "id": 1,
      "nombre": "Sucursal Centro"
    }
  }
]
```

## Lógica de cálculo
1. **Obtiene pedidos** que tengan órdenes del tipo especificado
2. **Calcula cantidad asignada** sumando las cajas de todas las tarimas asociadas al pedido que coincidan con el tipo
3. **Calcula cantidad disponible**: `CantidadSolicitada - CantidadAsignada`
4. **Filtra** solo pedidos con `CantidadDisponible > 0`
5. **Retorna** la cantidad disponible en lugar de la solicitada

## Ejemplo de uso
```bash
curl -X GET "https://api.ejemplo.com/api/pedidos-clientes/disponibles/Caja" \
  -H "Authorization: Bearer {token}"
```

## Códigos de respuesta
- `200 OK`: Lista de pedidos disponibles
- `400 Bad Request`: Tipo no especificado o inválido
- `401 Unauthorized`: Token de autenticación requerido

## Notas importantes
- Solo retorna pedidos que tengan disponibilidad (cantidad disponible > 0)
- La cantidad en la respuesta es la **cantidad disponible**, no la solicitada original
- El cálculo considera todas las tarimas asignadas al pedido y sus clasificaciones por tipo
- Requiere autenticación JWT 