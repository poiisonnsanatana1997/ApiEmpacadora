using AppAPIEmpacadora.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface ICajaClienteRepository
    {
        Task<IEnumerable<CajaCliente>> GetAllAsync();
        Task<CajaCliente> GetByIdAsync(int id);
        Task<CajaCliente> AddAsync(CajaCliente cajaCliente);
        Task<CajaCliente> UpdateAsync(CajaCliente cajaCliente);
        Task<bool> DeleteAsync(int id);
    }
} 