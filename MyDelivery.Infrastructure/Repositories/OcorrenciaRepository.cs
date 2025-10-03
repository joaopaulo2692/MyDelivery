using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyDelivery.Application.Interfaces;
using MyDelivery.Domain.Entities;
using MyDelivery.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace MyDelivery.Infrastructure.Repositories
{
    public class OcorrenciaRepository : IOcorrenciaRepository
    {
        private readonly MyDbContext _context;
        private readonly ILogger<OcorrenciaRepository> _logger;

        public OcorrenciaRepository(MyDbContext context, ILogger<OcorrenciaRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Pedido
        public async Task<Pedido?> ObterPorIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando pedido {PedidoId} no banco de dados", id);

                return await _context.Pedidos
                    .Include(p => p.Ocorrencias)
                    .FirstOrDefaultAsync(p => p.IdPedido == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido {PedidoId}", id);
                throw;
            }
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            try
            {
                _logger.LogInformation("Atualizando pedido {PedidoId}", pedido.IdPedido);

                _context.Pedidos.Update(pedido);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Pedido {PedidoId} atualizado com sucesso", pedido.IdPedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido {PedidoId}", pedido.IdPedido);
                throw;
            }
        }

        // Ocorrência
        public async Task AdicionarAsync(Ocorrencia ocorrencia)
        {
            try
            {
                _logger.LogInformation("Adicionando ocorrência {OcorrenciaId} para o pedido {PedidoId}",
                    ocorrencia.IdOcorrencia, ocorrencia.PedidoId);

                _context.Ocorrencias.Add(ocorrencia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Ocorrência {OcorrenciaId} adicionada com sucesso", ocorrencia.IdOcorrencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar ocorrência {OcorrenciaId}", ocorrencia.IdOcorrencia);
                throw;
            }
        }

        public async Task RemoverAsync(Ocorrencia ocorrencia)
        {
            try
            {
                _logger.LogInformation("Removendo ocorrência {OcorrenciaId} do pedido {PedidoId}",
                    ocorrencia.IdOcorrencia, ocorrencia.PedidoId);

                _context.Ocorrencias.Remove(ocorrencia);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Ocorrência {OcorrenciaId} removida com sucesso", ocorrencia.IdOcorrencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover ocorrência {OcorrenciaId}", ocorrencia.IdOcorrencia);
                throw;
            }
        }
    }
}
