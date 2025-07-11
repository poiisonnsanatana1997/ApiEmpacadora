# Configuración de CORS - API Empacadora

## Problema
El error de CORS ocurre cuando el frontend (que corre en `http://localhost:3000`) intenta hacer peticiones al backend (que corre en `http://localhost/AppAPIEmpacadora`) y el servidor no tiene configurado CORS correctamente.

## Solución Implementada

### 1. Configuración en Program.cs
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

### 2. Middleware de CORS
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

### 3. Manejo de Peticiones OPTIONS
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

### 4. Configuración en web.config
Se agregaron headers de CORS en el archivo web.config para IIS:

```xml
<httpProtocol>
  <customHeaders>
    <add name="Access-Control-Allow-Origin" value="*" />
    <add name="Access-Control-Allow-Headers" value="Content-Type, Authorization, X-Requested-With" />
    <add name="Access-Control-Allow-Methods" value="GET, POST, PUT, DELETE, OPTIONS" />
    <add name="Access-Control-Allow-Credentials" value="true" />
  </customHeaders>
</httpProtocol>
```

### 5. Atributo EnableCors en Controladores
Los controladores que necesitan CORS tienen el atributo `[EnableCors("Desarrollo")]`:

```csharp
[Route("api/[controller]")]
[ApiController]
[EnableCors("Desarrollo")]
public class ClasificacionesController : ControllerBase
```

## Orden de Middleware Importante
El orden de los middlewares es crucial:

1. `app.UseHttpsRedirection()`
2. `app.UseStaticFiles()`
3. Middleware personalizado para OPTIONS
4. `app.UseCors()` - **DEBE ir antes de UseAuthentication y UseAuthorization**
5. `app.UseAuthentication()`
6. `app.UseAuthorization()`

## Verificación
Para verificar que CORS está funcionando:

1. Abre las herramientas de desarrollador del navegador (F12)
2. Ve a la pestaña Network
3. Haz una petición desde tu frontend
4. Verifica que la respuesta incluya los headers de CORS:
   - `Access-Control-Allow-Origin`
   - `Access-Control-Allow-Headers`
   - `Access-Control-Allow-Methods`
   - `Access-Control-Allow-Credentials`

## Troubleshooting

### Error: "No 'Access-Control-Allow-Origin' header is present"
- Verifica que el middleware de CORS esté antes de UseAuthentication
- Asegúrate de que el origen del frontend esté incluido en la política de CORS
- Revisa que no haya configuraciones conflictivas en IIS

### Error: "Request header field Authorization is not allowed"
- Verifica que `Authorization` esté incluido en `Access-Control-Allow-Headers`
- Asegúrate de que el middleware de OPTIONS esté configurado correctamente

### Error: "Method OPTIONS is not allowed"
- Verifica que OPTIONS esté incluido en los verbos permitidos en web.config
- Asegúrate de que el middleware personalizado para OPTIONS esté funcionando

## Notas Importantes
- En producción, considera usar orígenes específicos en lugar de `*`
- El atributo `AllowCredentials` requiere que `Access-Control-Allow-Origin` no sea `*`
- Para desarrollo local, asegúrate de que el entorno esté configurado como "Development" 