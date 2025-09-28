using AppAPIEmpacadora.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Services.Interfaces
{
    public interface ITarimaService
    {
        Task<IEnumerable<TarimaDTO>> GetTarimasAsync();
        Task<TarimaDTO> GetTarimaByIdAsync(int id);
        Task<CreateTarimaResponseDTO> CreateTarimaAsync(CreateTarimaDTO createTarimaDto, string usuario);
        Task<TarimaDTO> UpdateTarimaAsync(int id, UpdateTarimaDTO updateTarimaDto, string usuario);
        Task<bool> DeleteTarimaAsync(int id);
        Task<bool> EliminarTarimaClasificacionAsync(int idTarima, int idClasificacion);
        Task<IEnumerable<TarimaParcialCompletaDTO>> GetTarimasParcialesAsync();
        Task<IEnumerable<TarimaParcialCompletaDTO>> GetTarimasParcialesYCompletasAsync();
        Task<TarimaDTO> UpdateTarimaParcialAsync(TarimaUpdateParcialDTO dto, string usuario);
        Task<List<PedidoClienteResponseDTO>> BuscarPedidosCompatiblesAsync(List<TarimaAsignacionRequestDTO> tarimasAsignacion);
    }
} 