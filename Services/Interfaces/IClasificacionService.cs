using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IClasificacionService
    {
        Task<IEnumerable<ClasificacionDTO>> GetClasificacionesAsync();
        Task<ClasificacionDTO> GetClasificacionByIdAsync(int id);
        Task<ClasificacionDTO> CreateClasificacionAsync(CreateClasificacionDTO createClasificacionDto, string usuario);
        Task<ClasificacionDTO> UpdateClasificacionAsync(int id, UpdateClasificacionDTO updateClasificacionDto);
        Task<bool> DeleteClasificacionAsync(int id);
        Task<AjustePesoClasificacionResponseDTO> AjustarPesoClasificacionAsync(int idClasificacion, AjustePesoClasificacionDTO ajusteDto, string usuario);
    }
} 