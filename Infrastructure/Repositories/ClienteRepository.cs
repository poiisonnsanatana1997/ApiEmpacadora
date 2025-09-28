using AppAPIEmpacadora.Infrastructure.Data;
using AppAPIEmpacadora.Models.Entities;
using AppAPIEmpacadora.Models.Exceptions;
using AppAPIEmpacadora.Repositories.Interfaces;
using AppAPIEmpacadora.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppAPIEmpacadora.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;

        public ClienteRepository(ApplicationDbContext context, ILoggingService loggingService)
        {
            _context = context;
            _loggingService = loggingService;
        }

        public async Task<IEnumerable<Cliente>> GetAllAsync()
        {
            try
            {
                _loggingService.LogDatabaseOperation("SELECT", "Cliente");
                
                var clientes = await _context.Clientes
                                             .Include(c => c.Sucursales)
                                             .Include(c => c.CajasCliente)
                                             .ToListAsync();
                
                _loggingService.LogInformation("Se obtuvieron {Count} clientes exitosamente", clientes.Count);
                return clientes;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al obtener todos los clientes", ex);
                throw new DatabaseException("Error al obtener la lista de clientes", ex);
            }
        }

        public async Task<Cliente> GetByIdAsync(int id)
        {
            try
            {
                _loggingService.LogDatabaseOperation("SELECT", "Cliente", id);
                
                var cliente = await _context.Clientes
                                             .Include(c => c.Sucursales)
                                             .Include(c => c.CajasCliente)
                                             .FirstOrDefaultAsync(c => c.Id == id);
                
                if (cliente == null)
                {
                    _loggingService.LogWarning("Cliente con ID {Id} no encontrado", id);
                    throw new EntityNotFoundException("Cliente", id);
                }
                
                _loggingService.LogInformation("Cliente con ID {Id} obtenido exitosamente", id);
                return cliente;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al obtener cliente con ID {Id}", ex, id);
                throw new DatabaseException($"Error al obtener cliente con ID {id}", ex);
            }
        }

        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            try
            {
                _loggingService.LogDatabaseOperation("INSERT", "Cliente");
                
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                
                _loggingService.LogInformation("Cliente creado exitosamente con ID {Id}", cliente.Id);
                return cliente;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al crear cliente", ex);
                throw new DatabaseException("Error al crear el cliente", ex);
            }
        }

        public async Task<Cliente> UpdateAsync(Cliente cliente)
        {
            try
            {
                _loggingService.LogDatabaseOperation("UPDATE", "Cliente", cliente.Id);
                
                _context.Entry(cliente).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                
                _loggingService.LogInformation("Cliente con ID {Id} actualizado exitosamente", cliente.Id);
                return cliente;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al actualizar cliente con ID {Id}", ex, cliente.Id);
                throw new DatabaseException($"Error al actualizar cliente con ID {cliente.Id}", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                _loggingService.LogDatabaseOperation("DELETE", "Cliente", id);
                
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null)
                {
                    _loggingService.LogWarning("Cliente con ID {Id} no encontrado para eliminar", id);
                    throw new EntityNotFoundException("Cliente", id);
                }

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                
                _loggingService.LogInformation("Cliente con ID {Id} eliminado exitosamente", id);
                return true;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _loggingService.LogError("Error al eliminar cliente con ID {Id}", ex, id);
                throw new DatabaseException($"Error al eliminar cliente con ID {id}", ex);
            }
        }
    }
} 