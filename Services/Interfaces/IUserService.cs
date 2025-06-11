using System.Collections.Generic;
using System.Threading.Tasks;
using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDTO> GetByIdAsync(int id);
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();
        Task<UserResponseDTO> CreateAsync(UserCreateDTO userDto);
        Task<UserResponseDTO> UpdateAsync(int id, UserUpdateDTO userDto);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
} 