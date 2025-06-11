using System;

namespace AppAPIEmpacadora.Models.Entities
{
    public class UserProfile
    {
        public int UserId { get; set; }
        public string ProfilePicture { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Bio { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Propiedad de navegación
        public virtual User User { get; set; }
    }
} 