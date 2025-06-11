using System;

namespace AppAPIEmpacadora.Models.DTOs
{
    public class UserCreateDTO
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool isActive { get; set; }
        public int RoleId { get; set; }
    }

    public class UserUpdateDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 