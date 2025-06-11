# API Empacadora

API REST para el sistema de empacadora con autenticación JWT.

## Estructura del Proyecto

```
AppAPIEmpacadora/
├── Controllers/           # Controladores de la API
├── Models/               # Modelos y DTOs
│   ├── DTOs/            # Data Transfer Objects
│   └── Entities/        # Entidades de dominio
├── Services/            # Servicios de aplicación
│   ├── Interfaces/      # Interfaces de servicios
│   └── Implementations/ # Implementaciones de servicios
├── Repositories/        # Repositorios de datos
│   ├── Interfaces/      # Interfaces de repositorios
│   └── Implementations/ # Implementaciones de repositorios
├── Infrastructure/      # Configuración e implementaciones de infraestructura
│   ├── Data/           # Configuración de base de datos
│   ├── Security/       # Configuración de seguridad
│   └── Services/       # Servicios de infraestructura
├── Common/             # Utilidades y helpers comunes
│   ├── Exceptions/     # Excepciones personalizadas
│   ├── Extensions/     # Extensiones de métodos
│   └── Helpers/        # Clases helper
└── Migrations/         # Scripts de migración de base de datos
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