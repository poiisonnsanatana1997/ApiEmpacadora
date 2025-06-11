using System;
using System.Collections.Generic;

namespace AppAPIEmpacadora.Models.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public DateTime CreatedAt { get; set; }

        // Propiedades de navegación
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
} 