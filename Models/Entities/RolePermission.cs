using System;

namespace AppAPIEmpacadora.Models.Entities
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propiedades de navegación
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
} 