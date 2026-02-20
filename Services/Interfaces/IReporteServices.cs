using AppAPIEmpacadora.Models.DTOs;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface IReporteServices
    {
        Task<IEnumerable<ReporteClasificacionesDTO>> ObtenerReporteClasificacionesAsync(int[] ids);
    }
}