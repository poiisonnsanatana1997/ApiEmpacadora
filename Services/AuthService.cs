using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Repositories.Interfaces;
using BCrypt.Net;

namespace AppAPIEmpacadora.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            // Buscar el usuario por username
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // Verificar la contraseña
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            // Verificar si el usuario está activo
            if (!user.IsActive)
                throw new UnauthorizedAccessException("El usuario está inactivo");

            // Generar el token
            var token = GenerateJwtToken(user);

            // Mapear el usuario a DTO
            var userDto = new UserResponseDTO
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.Username,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleName = user.Role?.Name ?? "Sin rol",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return new AuthResponseDTO
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryInMinutes"])),
                User = userDto
            };
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                //new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "user"),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:ExpiryInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 