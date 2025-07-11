using AppAPIEmpacadora.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AppAPIEmpacadora.Repositories.Interfaces
{
    public interface IClasificacionRepository
    {
        Task<IEnumerable<Clasificacion>> GetAllAsync();
        Task<Clasificacion> GetByIdAsync(int id);
        Task<Clasificacion> AddAsync(Clasificacion clasificacion);
        Task<Clasificacion> UpdateAsync(Clasificacion clasificacion);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Clasificacion>> GetByDateAndProductAsync(DateTime date, int idProducto);
        Task<IEnumerable<TarimaClasificacion>> GetTarimasClasificacionByTipoAsync(int idClasificacion, string tipo);
        Task<bool> UpdateTarimasClasificacionAsync(IEnumerable<TarimaClasificacion> tarimasClasificacion);
    }
} 