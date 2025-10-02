using MyDelivery.Domain.Entities;
using MyDelivery.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using MyDelivery.Infrastructure.Data;

namespace MyDelivery.Infrastructure.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly MyDbContext _context;

        public PedidoRepository(MyDbContext context) => _context = context;

        public async Task<Pedido?> ObterPorIdAsync(int id) =>
            await _context.Pedidos
                .Include(p => p.Ocorrencias)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

        public async Task AtualizarAsync(Pedido pedido)
        {
            _context.Pedidos.Update(pedido);
            await _context.SaveChangesAsync();
        }
    }

    public class OcorrenciaRepository : IOcorrenciaRepository
    {
        private readonly MyDbContext _context;

        public OcorrenciaRepository(MyDbContext context) => _context = context;

        public async Task AdicionarAsync(Ocorrencia ocorrencia)
        {
            _context.Ocorrencias.Add(ocorrencia);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(Ocorrencia ocorrencia)
        {
            _context.Ocorrencias.Remove(ocorrencia);
            await _context.SaveChangesAsync();
        }
    }
}
