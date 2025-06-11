-- Script completo para crear la base de datos
-- Primero creamos la tabla de usuarios
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) NOT NULL,
    Password NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    Role NVARCHAR(20) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT UQ_Users_Username UNIQUE (Username)
);

-- Crear índices para la tabla Users
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Username ON Users(Username);

-- Insertar usuario administrador por defecto
INSERT INTO Users (Name, Username, Password, Email, Role, IsActive)
VALUES (
    'Administrador',
    'admin',
    'admin123', -- En producción, esto debería ser un hash seguro
    'admin@ejemplo.com',
    'admin',
    1
);

-- Tabla de Roles
CREATE TABLE Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Roles_Name UNIQUE (Name)
);

-- Tabla de Permisos
CREATE TABLE Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(200),
    Module NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT UQ_Permissions_Name UNIQUE (Name)
);

-- Tabla de Asignación de Permisos a Roles
CREATE TABLE RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id),
    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
);

-- Insertar roles básicos
INSERT INTO Roles (Name, Description) VALUES
('admin', 'Administrador del sistema con acceso total'),
('manager', 'Gerente con acceso a gestión de usuarios y reportes'),
('user', 'Usuario estándar con acceso básico');

-- Insertar permisos básicos
INSERT INTO Permissions (Name, Description, Module) VALUES
('users.create', 'Crear usuarios', 'Users'),
('users.read', 'Ver usuarios', 'Users'),
('users.update', 'Actualizar usuarios', 'Users'),
('users.delete', 'Eliminar usuarios', 'Users'),
('reports.view', 'Ver reportes', 'Reports'),
('settings.manage', 'Gestionar configuración', 'Settings');

-- Asignar permisos al rol admin
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r
CROSS JOIN Permissions p
WHERE r.Name = 'admin';

-- Asignar permisos al rol manager
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r
CROSS JOIN Permissions p
WHERE r.Name = 'manager' AND p.Name IN ('users.read', 'reports.view');

-- Asignar permisos al rol user
INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT r.Id, p.Id
FROM Roles r
CROSS JOIN Permissions p
WHERE r.Name = 'user' AND p.Name = 'users.read';

-- Modificar la tabla Users para usar Roles
-- Primero, agregar la nueva columna RoleId
ALTER TABLE Users
ADD RoleId INT;

-- Actualizar los usuarios existentes con los nuevos RoleId
UPDATE Users
SET RoleId = (SELECT Id FROM Roles WHERE Name = 'admin')
WHERE Role = 'admin';

UPDATE Users
SET RoleId = (SELECT Id FROM Roles WHERE Name = 'user')
WHERE Role = 'user';

-- Agregar la restricción de clave foránea
ALTER TABLE Users
ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id);

-- Eliminar la columna Role antigua
ALTER TABLE Users
DROP COLUMN Role;

-- Tabla de Logs de Actividad
CREATE TABLE ActivityLogs (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    IpAddress NVARCHAR(50),
    UserAgent NVARCHAR(MAX),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla de Sesiones Activas
CREATE TABLE UserSessions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(MAX) NOT NULL,
    DeviceInfo NVARCHAR(MAX),
    IpAddress NVARCHAR(50),
    LastActivity DATETIME NOT NULL DEFAULT GETDATE(),
    ExpiresAt DATETIME NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla de Recuperación de Contraseña
CREATE TABLE PasswordResets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Token NVARCHAR(100) NOT NULL,
    ExpiresAt DATETIME NOT NULL,
    IsUsed BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Tabla de Perfiles de Usuario
CREATE TABLE UserProfiles (
    UserId INT PRIMARY KEY,
    ProfilePicture NVARCHAR(MAX),
    Address NVARCHAR(200),
    City NVARCHAR(100),
    State NVARCHAR(100),
    Country NVARCHAR(100),
    PostalCode NVARCHAR(20),
    BirthDate DATE,
    Gender NVARCHAR(20),
    Bio NVARCHAR(MAX),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Verificación final
SELECT 'Tablas creadas exitosamente' as Mensaje;
GO 