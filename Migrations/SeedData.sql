-- Insertar roles básicos
INSERT INTO Roles (Name, Description, IsActive, CreatedAt, UpdatedAt)
VALUES 
('admin', 'Administrador del sistema con acceso total', 1, GETDATE(), GETDATE()),
('manager', 'Gerente con acceso a gestión de usuarios y reportes', 1, GETDATE(), GETDATE()),
('user', 'Usuario estándar con acceso básico', 1, GETDATE(), GETDATE());

-- Insertar permisos básicos
INSERT INTO Permissions (Name, Description, Module, CreatedAt)
VALUES 
('users.create', 'Crear usuarios', 'Users', GETDATE()),
('users.read', 'Ver usuarios', 'Users', GETDATE()),
('users.update', 'Actualizar usuarios', 'Users', GETDATE()),
('users.delete', 'Eliminar usuarios', 'Users', GETDATE()),
('reports.view', 'Ver reportes', 'Reports', GETDATE()),
('settings.manage', 'Gestionar configuración', 'Settings', GETDATE());

-- Asignar permisos al rol admin
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, GETDATE()
FROM Roles r
CROSS JOIN Permissions p
WHERE r.Name = 'admin';

-- Asignar permisos al rol manager
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, GETDATE()
FROM Roles r
CROSS JOIN Permissions p
WHERE r.Name = 'manager' AND p.Name IN ('users.read', 'reports.view');

-- Asignar permisos al rol user
INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
SELECT r.Id, p.Id, GETDATE()
FROM Roles r
CROSS JOIN Permissions p
WHERE r.Name = 'user' AND p.Name = 'users.read';

-- Insertar usuarios de prueba con contraseñas hasheadas
-- Nota: Los hashes fueron generados con BCrypt con work factor 12
INSERT INTO Users (Name, Username, Password, Email, PhoneNumber, RoleId, IsActive, CreatedAt, UpdatedAt)
VALUES 
('Administrador', 'admin', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewKy7P2p5K2qZ0XK', 'admin@empacadora.com', '1234567890', (SELECT Id FROM Roles WHERE Name = 'admin'), 1, GETDATE(), GETDATE()),
('Gerente', 'manager', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewKy7P2p5K2qZ0XK', 'manager@empacadora.com', '0987654321', (SELECT Id FROM Roles WHERE Name = 'manager'), 1, GETDATE(), GETDATE()),
('Usuario', 'user', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewKy7P2p5K2qZ0XK', 'user@empacadora.com', '1122334455', (SELECT Id FROM Roles WHERE Name = 'user'), 1, GETDATE(), GETDATE());

-- Insertar perfiles de usuario
INSERT INTO UserProfiles (UserId, Address, City, State, Country, PostalCode, BirthDate, Gender, Bio, UpdatedAt)
VALUES 
((SELECT Id FROM Users WHERE Username = 'admin'), 'Calle Principal 123', 'Ciudad', 'Estado', 'País', '12345', '1990-01-01', 'Masculino', 'Administrador del sistema', GETDATE()),
((SELECT Id FROM Users WHERE Username = 'manager'), 'Avenida Central 456', 'Ciudad', 'Estado', 'País', '54321', '1992-05-15', 'Femenino', 'Gerente de operaciones', GETDATE()),
((SELECT Id FROM Users WHERE Username = 'user'), 'Calle Secundaria 789', 'Ciudad', 'Estado', 'País', '67890', '1995-10-20', 'Masculino', 'Usuario estándar', GETDATE()); 