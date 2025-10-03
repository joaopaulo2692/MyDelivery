using Microsoft.EntityFrameworkCore;
using MyDelivery.Domain.Entities;
using MyDelivery.Infrastructure.Data;
using MyDelivery.Domain.Interfaces.Repository;
using Serilog;
using System;
using System.Threading.Tasks;

namespace MyDelivery.Domain.Interfaces.Repository
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly MyDbContext _context;

        public PedidoRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Pedido pedido)
        {
            try
            {
                await _context.Pedidos.AddAsync(pedido);
                await _context.SaveChangesAsync();
                Log.Information("Pedido adicionado com sucesso. Id={IdPedido}", pedido.IdPedido);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao adicionar pedido. Numero={NumeroPedido}", pedido.NumeroPedido);
                throw;
            }
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            try
            {
                _context.Pedidos.Update(pedido);
                await _context.SaveChangesAsync();
                Log.Information("Pedido atualizado com sucesso. Id={IdPedido}", pedido.IdPedido);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar pedido. Id={IdPedido}", pedido.IdPedido);
                throw;
            }
        }

        public async Task<Pedido?> ObterPorIdAsync(int idPedido)
        {
            try
            {
                var pedido = await _context.Pedidos
                    .Include(p => p.Ocorrencias)
                    .FirstOrDefaultAsync(p => p.IdPedido == idPedido);

                if (pedido != null)
                    Log.Information("Pedido obtido com sucesso. Id={IdPedido}", idPedido);
                else
                    Log.Warning("Pedido não encontrado. Id={IdPedido}", idPedido);

                return pedido;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao obter pedido. Id={IdPedido}", idPedido);
                throw;
            }
        }
    }
}
