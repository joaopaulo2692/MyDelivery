using Microsoft.EntityFrameworkCore;
using MyDelivery.Domain.Entities;
using MyDelivery.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            await _context.Pedidos.AddAsync(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }

        public async Task<Pedido?> ObterPorIdAsync(int idPedido)
        {
            return await _context.Pedidos
                .Include(p => p.Ocorrencias) // carrega as ocorrências relacionadas
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);
        }
    }

}
