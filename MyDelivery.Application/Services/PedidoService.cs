using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Application.Services
{
    using Microsoft.Extensions.Logging;
    using MyDelivery.Application.Interfaces;
    using MyDelivery.Domain.Entities;
    using MyDelivery.Domain.Enums;
    using MyDelivery.Domain.Interfaces.Repository;
    using MyDelivery.Domain.Interfaces.Service;

    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepo;
        private readonly IOcorrenciaRepository _ocRepo;
        private readonly ILogger<PedidoService> _logger;

        public PedidoService(IPedidoRepository pedidoRepo, IOcorrenciaRepository ocRepo, ILogger<PedidoService> logger)
        {
            _pedidoRepo = pedidoRepo;
            _ocRepo = ocRepo;
            _logger = logger;
        }

        public async Task<Pedido> CriarPedidoAsync(int numeroPedido, DateTime horaPedido)
        {
            try
            {
                var pedido = new Pedido(numeroPedido, horaPedido);
                await _pedidoRepo.AdicionarAsync(pedido);
                _logger.LogInformation("Pedido criado. Id={Id} Numero={Numero}", pedido.IdPedido, numeroPedido);
                return pedido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido. Numero={Numero}", numeroPedido);
                throw;
            }
        }

        public async Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora)
        {
            try
            {
                var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
                    ?? throw new KeyNotFoundException("Pedido não encontrado.");

                var ocorrencia = Ocorrencia.Criar(tipo, hora, pedidoId);
                pedido.AdicionarOcorrencia(ocorrencia);
                await _pedidoRepo.AtualizarAsync(pedido);

                _logger.LogInformation("Ocorrência registrada. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
                return ocorrencia;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar ocorrência. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
                throw;
            }
        }

        public async Task ExcluirOcorrencia(int pedidoId, int idOcorrencia)
        {
            try
            {
                var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
                    ?? throw new KeyNotFoundException("Pedido não encontrado.");

                var oc = pedido.Ocorrencias.FirstOrDefault(o => o.IdOcorrencia == idOcorrencia)
                    ?? throw new KeyNotFoundException("Ocorrência não encontrada no pedido.");

                pedido.RemoverOcorrencia(idOcorrencia);
                await _pedidoRepo.AtualizarAsync(pedido);
                await _ocRepo.RemoverAsync(oc);

                _logger.LogInformation("Ocorrência excluída. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir ocorrência. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
                throw;
            }
        }

        public async Task<Ocorrencia> ObterOcorrenciaPorIdAsync(int pedidoId, int idOcorrencia)
        {
            try
            {
                var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
                    ?? throw new KeyNotFoundException("Pedido não encontrado.");

                var ocorrencia = pedido.Ocorrencias.FirstOrDefault(o => o.IdOcorrencia == idOcorrencia)
                    ?? throw new KeyNotFoundException("Ocorrência não encontrada no pedido.");

                _logger.LogInformation("Ocorrência obtida. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
                return ocorrencia;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter ocorrência. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
                throw;
            }
        }

        public async Task<Pedido?> ObterPedidoPorIdAsync(int pedidoId)
        {
            try
            {
                return await _pedidoRepo.ObterPorIdAsync(pedidoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido. PedidoId={PedidoId}", pedidoId);
                throw;
            }
        }
    }

}
