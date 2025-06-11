using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using BCrypt.Net;

namespace AppAPIEmpacadora.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return MapToUserResponseDTO(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToUserResponseDTO);
        }

        public async Task<UserResponseDTO> CreateAsync(UserCreateDTO userDto)
        {
            // Validar que el email no exista
            if (await _userRepository.ExistsByEmailAsync(userDto.Email))
                throw new InvalidOperationException("El email ya está registrado");

            // Validar que el username no exista
            if (await _userRepository.ExistsByUsernameAsync(userDto.Username))
                throw new InvalidOperationException("El nombre de usuario ya está registrado");

            // Crear el usuario
            var user = new User
            {
                Name = userDto.Name,
                Username = userDto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                RoleId = userDto.RoleId,
                IsActive = userDto.isActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Guardar el usuario
            user = await _userRepository.AddAsync(user);

            return MapToUserResponseDTO(user);
        }

        public async Task<UserResponseDTO> UpdateAsync(int id, UserUpdateDTO userDto)
        {
            // Obtener el usuario existente
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            // Actualizar los campos permitidos
            user.Name = userDto.Name;
            user.PhoneNumber = userDto.PhoneNumber;
            user.IsActive = userDto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // Guardar los cambios
            await _userRepository.UpdateAsync(user);

            return MapToUserResponseDTO(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            await _userRepository.DeleteAsync(user);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _userRepository.ExistsAsync(id);
        }

        private static UserResponseDTO MapToUserResponseDTO(User user)
        {
            return new UserResponseDTO
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
        }
    }
} 