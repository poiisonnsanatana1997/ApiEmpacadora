using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ICajaService
    {
        Task<IEnumerable<CajaSummaryDTO>> GetCajasAsync();
        Task<CajaDTO> GetCajaByIdAsync(int id);
        Task<CajaDTO> CreateCajaAsync(CreateCajaDTO createCajaDto, string usuario);
        Task<CajaDTO> UpdateCajaAsync(int id, UpdateCajaDTO updateCajaDto);
        Task<bool> DeleteCajaAsync(int id);
        Task<CajaDTO> AjustarCantidadCajaAsync(AjustarCantidadCajaDTO ajusteDto, string usuario);
        Task<IEnumerable<CajaDTO>> GetCajasByClasificacionAsync(int idClasificacion);
    }
} 