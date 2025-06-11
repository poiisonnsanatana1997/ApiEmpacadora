using System.ComponentModel.DataAnnotations;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; }
    }

    public class AuthResponseDTO
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserResponseDTO User { get; set; }
    }
} 