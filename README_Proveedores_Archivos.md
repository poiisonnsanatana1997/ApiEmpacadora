# Manejo de Archivos de Situación Fiscal en Proveedores

## Descripción
Se ha implementado la funcionalidad para manejar archivos de situación fiscal en el módulo de proveedores. Los archivos se almacenan en el directorio `wwwroot/uploads/situaciones-fiscales/` con nombres únicos generados automáticamente.

## Cambios Implementados

### 1. DTOs Modificados
- `CreateProveedorDTO`: El campo `SituacionFiscal` ahora es de tipo `IFormFile?` en lugar de `string`
- `UpdateProveedorDTO`: El campo `SituacionFiscal` ahora es de tipo `IFormFile?` en lugar de `string`

### 2. Servicio Actualizado
- `ProveedorService`: Se agregó manejo de archivos con las siguientes funcionalidades:
  - Guardado de archivos con nombres únicos
  - Eliminación de archivos antiguos al actualizar
  - Limpieza de archivos al eliminar proveedores
  - **Generación de URLs completas** para archivos en `GetProveedorByIdAsync`

### 3. Controlador Actualizado
- `ProveedoresController`: Se agregó `[FromForm]` a los métodos POST y PUT para manejar archivos

## Funcionalidades

### Crear Proveedor (POST)
- **Endpoint**: `POST /api/proveedores`
- **Parámetros**: `CreateProveedorDTO` con `SituacionFiscal` como `IFormFile`
- **Funcionalidad**: Guarda el archivo y almacena la ruta en la base de datos

### Actualizar Proveedor (PUT)
- **Endpoint**: `PUT /api/proveedores/{id}`
- **Parámetros**: `UpdateProveedorDTO` con `SituacionFiscal` como `IFormFile`
- **Funcionalidad**: Reemplaza el archivo anterior y actualiza la ruta

### Obtener Proveedor Completo (GET)
- **Endpoint**: `GET /api/proveedores/completo/{id}`
- **Respuesta**: `ProveedorDTO` con `SituacionFiscal` como **URL completa** para mostrar en el frontend
- **Ejemplo de URL generada**: `https://tu-dominio.com/uploads/situaciones-fiscales/{guid}_documento.pdf`

### Eliminar Proveedor (DELETE)
- **Endpoint**: `DELETE /api/proveedores/{id}`
- **Funcionalidad**: Elimina el proveedor y su archivo asociado

## Ejemplo de Uso

### Frontend - Mostrar archivo
```javascript
// Al obtener un proveedor completo
const proveedor = await fetch('/api/proveedores/completo/1');
const data = await proveedor.json();

// La URL del archivo ya viene completa
if (data.situacionFiscal) {
    // Mostrar el archivo en un iframe o enlace
    document.getElementById('archivo').src = data.situacionFiscal;
}
```

### Frontend - Subir archivo
```javascript
const formData = new FormData();
formData.append('nombre', 'Proveedor Ejemplo');
formData.append('rfc', 'ABC123456789');
formData.append('situacionFiscal', archivoSeleccionado);

await fetch('/api/proveedores', {
    method: 'POST',
    body: formData
});
```

## Notas Importantes

1. **Seguridad**: Los archivos son públicos y accesibles directamente desde la URL
2. **Nombres únicos**: Se generan con GUID para evitar conflictos
3. **Limpieza automática**: Los archivos se eliminan automáticamente al actualizar o eliminar proveedores
4. **URLs completas**: El método `GetProveedorByIdAsync` devuelve URLs completas listas para usar en el frontend

## Uso de la API

### Crear Proveedor con Archivo
```http
POST /api/proveedores
Content-Type: multipart/form-data
Authorization: Bearer {token}

Form Data:
- Nombre: "Proveedor Ejemplo"
- RFC: "XAXX010101000"
- Telefono: "555-1234"
- Correo: "proveedor@ejemplo.com"
- DireccionFiscal: "Calle Principal 123"
- SituacionFiscal: [archivo PDF/Word/Imagen]
- FechaRegistro: "2025-06-25T10:00:00Z"
```

### Actualizar Proveedor con Nuevo Archivo
```http
PUT /api/proveedores/{id}
Content-Type: multipart/form-data
Authorization: Bearer {token}

Form Data:
- Nombre: "Proveedor Actualizado"
- SituacionFiscal: [nuevo archivo]
```

## Tipos de Archivo Soportados
- PDF (.pdf)
- Imágenes (.jpg, .jpeg, .png)
- Documentos Word (.doc, .docx)
- Otros tipos como `application/octet-stream`

## Estructura de Archivos
```
wwwroot/
└── uploads/
    └── situaciones-fiscales/
        ├── {guid}_documento1.pdf
        ├── {guid}_documento2.docx
        └── {guid}_imagen.jpg
```

## Consideraciones de Seguridad
- Los archivos se almacenan con nombres únicos (GUID) para evitar conflictos
- Los archivos antiguos se eliminan automáticamente al actualizar
- Solo usuarios autenticados pueden crear/actualizar proveedores

## Ejemplo de Uso con JavaScript/Fetch
```javascript
// Crear proveedor con archivo
const formData = new FormData();
formData.append('Nombre', 'Proveedor Ejemplo');
formData.append('RFC', 'XAXX010101000');
formData.append('SituacionFiscal', fileInput.files[0]);
formData.append('FechaRegistro', new Date().toISOString());

const response = await fetch('/api/proveedores', {
    method: 'POST',
    headers: {
        'Authorization': 'Bearer ' + token
    },
    body: formData
});
```

## Migración de Datos Existentes
Si tienes proveedores existentes con `SituacionFiscal` como string, estos seguirán funcionando. El campo se mantiene como string en la base de datos, pero ahora puede contener la ruta del archivo o el valor original.

## Notas Técnicas
- Los archivos se guardan en el sistema de archivos del servidor
- Se utiliza `IWebHostEnvironment` para obtener la ruta del directorio web
- Los nombres de archivo incluyen un GUID para garantizar unicidad
- Se maneja la limpieza de archivos en operaciones de actualización y eliminación 