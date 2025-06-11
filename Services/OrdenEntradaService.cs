using AppAPIEmpacadora.Models.DTOs;
using AppAPIEmpacadora.Repositories.Interfaces;

namespace AppAPIEmpacadora.Services
{
    public class OrdenEntradaService : IOrdenEntradaService
    {
        private readonly IOrdenEntradaRepository _ordenEntradaRepository;
        private readonly IProductoRepository _productoRepository;
        private readonly IProveedorRepository _proveedorRepository;

        public OrdenEntradaService(
            IOrdenEntradaRepository ordenEntradaRepository,
            IProductoRepository productoRepository,
            IProveedorRepository proveedorRepository)
        {
            _ordenEntradaRepository = ordenEntradaRepository;
            _productoRepository = productoRepository;
            _proveedorRepository = proveedorRepository;
        }

        public async Task<IEnumerable<OrdenEntradaDTO>> ObtenerOrdenesEntradaAsync()
        {
            return await _ordenEntradaRepository.ObtenerOrdenesEntradaAsync();
        }

        public async Task<OrdenEntradaDTO> ObtenerOrdenEntradaPorCodigoAsync(string codigo)
        {
            return await _ordenEntradaRepository.ObtenerOrdenEntradaPorCodigoAsync(codigo);
        }

        public async Task<OrdenEntradaDTO> CrearOrdenEntradaAsync(CrearOrdenEntradaDTO dto, string usuarioRegistro)
        {
            var producto = await _productoRepository.ObtenerPorIdAsync(dto.ProductoId);
            if (producto == null)
                throw new ArgumentException("El producto especificado no existe");

            var proveedor = await _proveedorRepository.ObtenerPorIdAsync(dto.ProveedorId);
            if (proveedor == null)
                throw new ArgumentException("El proveedor especificado no existe");

            var ultimaOrden = await _ordenEntradaRepository.ObtenerUltimaOrdenEntradaAsync();
            var numeroSiguiente = 1;
            if (ultimaOrden != null)
            {
                var ultimoNumero = int.Parse(ultimaOrden.Codigo.Split('-')[1]);
                numeroSiguiente = ultimoNumero + 1;
            }

            var codigo = $"OE-{numeroSiguiente:D6}";

            var ordenEntrada = new OrdenEntradaDTO
            {
                Codigo = codigo,
                Proveedor = proveedor,
                Producto = producto,
                FechaEstimada = dto.FechaEstimada,
                FechaRegistro = DateTime.UtcNow,
                Estado = dto.Estado,
                Observaciones = dto.Observaciones
            };

            await _ordenEntradaRepository.CrearOrdenEntradaAsync(ordenEntrada, usuarioRegistro);
            return ordenEntrada;
        }

        public async Task<OrdenEntradaDTO> ActualizarOrdenEntradaAsync(string codigo, CrearOrdenEntradaDTO dto, string usuarioModificacion)
        {
            var orden = await _ordenEntradaRepository.ObtenerOrdenEntradaPorCodigoAsync(codigo);
            if (orden == null)
                return null;

            var producto = await _productoRepository.ObtenerPorIdAsync(dto.ProductoId);
            if (producto == null)
                throw new ArgumentException("El producto especificado no existe");

            var proveedor = await _proveedorRepository.ObtenerPorIdAsync(dto.ProveedorId);
            if (proveedor == null)
                throw new ArgumentException("El proveedor especificado no existe");

            orden.Proveedor = proveedor;
            orden.Producto = producto;
            orden.FechaEstimada = dto.FechaEstimada;
            orden.Estado = dto.Estado;
            orden.Observaciones = dto.Observaciones;

            var resultado = await _ordenEntradaRepository.ActualizarOrdenEntradaAsync(orden);
            return resultado ? orden : null;
        }

        public async Task<bool> EliminarOrdenEntradaAsync(string codigo)
        {
            return await _ordenEntradaRepository.EliminarOrdenEntradaAsync(codigo);
        }

        public async Task<DetalleOrdenEntradaDTO> ObtenerDetalleOrdenEntradaAsync(string codigo)
        {
            var orden = await _ordenEntradaRepository.ObtenerOrdenEntradaPorCodigoAsync(codigo);
            if (orden == null)
                return null;

            var tarimas = await _ordenEntradaRepository.ObtenerTarimasPorOrdenAsync(codigo);

            return new DetalleOrdenEntradaDTO
            {
                OrdenEntrada = orden,
                Tarimas = tarimas.Select(t => new TarimaDTO
                {
                    Numero = t.Numero,
                    PesoBruto = t.PesoBruto,
                    PesoTara = t.PesoTara,
                    PesoTarima = t.PesoTarima,
                    PesoPatin = t.PesoPatin,
                    PesoNeto = t.PesoNeto,
                    CantidadCajas = t.CantidadCajas,
                    PesoPorCaja = t.PesoPorCaja,
                    Observaciones = t.Observaciones
                }).ToList()
            };
        }

        public async Task<bool> ActualizarPesajeTarimaAsync(string codigo, string numeroTarima, ActualizarPesajeTarimaDTO dto)
        {
            var tarima = await _ordenEntradaRepository.ObtenerTarimaPorNumeroAsync(codigo, numeroTarima);
            if (tarima == null)
                return false;

            tarima.PesoBruto = dto.PesoBruto;
            tarima.PesoTara = dto.PesoTara;
            tarima.PesoTarima = dto.PesoTarima;
            tarima.PesoPatin = dto.PesoPatin;
            tarima.PesoNeto = dto.PesoNeto;
            tarima.CantidadCajas = dto.CantidadCajas;
            tarima.PesoPorCaja = dto.PesoPorCaja;
            tarima.Observaciones = dto.Observaciones;

            return await _ordenEntradaRepository.ActualizarTarimaAsync(tarima);
        }

        public async Task<bool> EliminarTarimaAsync(string codigo, string numeroTarima)
        {
            var tarima = await _ordenEntradaRepository.ObtenerTarimaPorNumeroAsync(codigo, numeroTarima);
            if (tarima == null)
                return false;

            return await _ordenEntradaRepository.EliminarTarimaAsync(tarima);
        }

        public async Task<decimal> ObtenerPesoTotalRecibidoHoyAsync()
        {
            return await _ordenEntradaRepository.ObtenerPesoTotalRecibidoHoyAsync();
        }

        public async Task<int> ObtenerCantidadPendientesHoyAsync()
        {
            return await _ordenEntradaRepository.ObtenerCantidadPendientesHoyAsync();
        }

        public async Task<TarimaDTO> CrearTarimaAsync(string codigoOrden, TarimaDTO tarima)
        {
            // Validaciones adicionales si es necesario
            return await _ordenEntradaRepository.CrearTarimaAsync(codigoOrden, tarima);
        }
    }
} 