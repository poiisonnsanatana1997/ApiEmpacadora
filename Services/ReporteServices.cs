using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class ReporteServices : IReporteServices
    {
        private readonly IOrdenEntradaRepository _ordenEntradaRepository;
        public ReporteServices(IOrdenEntradaRepository ordenEntradaRepository)
        {
            _ordenEntradaRepository = ordenEntradaRepository;
        }
        public async Task<IEnumerable<ReporteClasificacionesDTO>> ObtenerReporteClasificacionesAsync(int[] ids)
        {
            try{
                var reportes = new List<ReporteClasificacionesDTO>();
                foreach(var id in ids){
                    var pedido = await _ordenEntradaRepository.ObtenerPedidoCompletoPorIdAsync(id);
                    if(pedido == null) continue; // Si no se encuentra el pedido, se omite y se continúa con el siguiente ID

                    var resporte = new ReporteClasificacionesDTO{
                        Id = pedido.Id,
                        Codigo = pedido.Codigo,
                        Proveedor = pedido.Proveedor,
                        TotalMermas = pedido.Clasificaciones.FirstOrDefault()?.Mermas?.Sum(m => m.Peso) ?? 0,
                        TotalRetornos = pedido.Clasificaciones.FirstOrDefault()?.RetornosDetalle?.Sum(r => r.Peso) ?? 0,
                        FechaRecepcion = pedido.FechaRecepcion?.ToString("dd/MM/yyyy HH:mm:ss") ?? string.Empty,
                        PesoNetoRecibido = pedido.Clasificaciones.FirstOrDefault()?.PesoTotal ?? 0
                    };

                    // Generamos la informacion de los tipos con la información de clasificación con sus atributos XL, L, M y S
                    var informacionTipos = new InformacionTipoDTO[4];
                    informacionTipos[0] = new InformacionTipoDTO{
                        Tipo = "XL",
                        Precio = pedido.Clasificaciones.FirstOrDefault()?.XL ?? 0,
                        Peso = pedido.Clasificaciones.FirstOrDefault()?.TarimasClasificaciones.Where(t => t.Tipo == "XL").Sum(t => t.Peso) ?? 0
                    };
                    informacionTipos[1] = new InformacionTipoDTO{
                        Tipo = "L",
                        Precio = pedido.Clasificaciones.FirstOrDefault()?.L ?? 0,
                        Peso = pedido.Clasificaciones.FirstOrDefault()?.TarimasClasificaciones.Where(t => t.Tipo == "L").Sum(t => t.Peso) ?? 0
                    };
                    informacionTipos[2] = new InformacionTipoDTO{
                        Tipo = "M",
                        Precio = pedido.Clasificaciones.FirstOrDefault()?.M ?? 0,
                        Peso = pedido.Clasificaciones.FirstOrDefault()?.TarimasClasificaciones.Where(t => t.Tipo == "M").Sum(t => t.Peso) ?? 0
                    };
                    informacionTipos[3] = new InformacionTipoDTO{
                        Tipo = "S",
                        Precio = pedido.Clasificaciones.FirstOrDefault()?.S ?? 0,
                        Peso = pedido.Clasificaciones.FirstOrDefault()?.TarimasClasificaciones.Where(t => t.Tipo == "S").Sum(t => t.Peso) ?? 0
                    };

                    resporte.InformacionTipos = informacionTipos;
                    reportes.Add(resporte);
                }
                return reportes;
            }catch(Exception ex){
                throw new Exception(ex.Message);
            }
        }
    }
}