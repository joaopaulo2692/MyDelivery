using MyDelivery.Application.Interfaces;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;
using Serilog;

namespace MyDelivery.Application.Service
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepo;
        private readonly IOcorrenciaRepository _ocRepo;
        private readonly ILogger _logger; 

        public PedidoService(IPedidoRepository pedidoRepo, IOcorrenciaRepository ocRepo, ILogger logger)
        {
            _pedidoRepo = pedidoRepo;
            _ocRepo = ocRepo;
            _logger = logger;
        }

        //public async Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora)
        //{
        //    var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
        //        ?? throw new KeyNotFoundException("Pedido não encontrado.");

        //    var ocorrencia = Ocorrencia.Criar(tipo, hora);
        //    pedido.AdicionarOcorrencia(ocorrencia);

        //    await _pedidoRepo.AtualizarAsync(pedido);
        //    await _ocRepo.AdicionarAsync(ocorrencia);

        //    _logger.Information("Ocorrência registrada. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
        //    return ocorrencia;
        //}

        public async Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora)
        {
            var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

            var ocorrencia = Ocorrencia.Criar(tipo, hora);
            pedido.AdicionarOcorrencia(ocorrencia);         
            await _pedidoRepo.AtualizarAsync(pedido);
            _logger.Information("Ocorrência registrada. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
            return ocorrencia;
        }


        public async Task ExcluirOcorrencia(int pedidoId, int idOcorrencia)
        {
            var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

            var oc = pedido.Ocorrencias.FirstOrDefault(o => o.IdOcorrencia == idOcorrencia)
                ?? throw new KeyNotFoundException("Ocorrência não encontrada no pedido.");

            pedido.RemoverOcorrencia(idOcorrencia);

            await _pedidoRepo.AtualizarAsync(pedido);
            await _ocRepo.RemoverAsync(oc);

            _logger.Information("Ocorrência excluída. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
        }
    }
}
