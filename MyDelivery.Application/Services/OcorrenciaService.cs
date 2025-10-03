using Microsoft.Extensions.Logging;
using MyDelivery.Application.Interfaces;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;
using MyDelivery.Domain.Interfaces.Repository;

public class OcorrenciaService : IOcorrenciaService
{
    private readonly IPedidoRepository _pedidoRepo;
    private readonly IOcorrenciaRepository _ocRepo;
    private readonly ILogger<OcorrenciaService> _logger;

    public OcorrenciaService(IPedidoRepository pedidoRepo, IOcorrenciaRepository ocRepo, ILogger<OcorrenciaService> logger)
    {
        _pedidoRepo = pedidoRepo;
        _ocRepo = ocRepo;
        _logger = logger;
    }
    public async Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora)
    {
        try
        {
            // validação de enum
            if (!Enum.IsDefined(typeof(ETipoOcorrencia), tipo))
                throw new ArgumentException("Tipo de ocorrência inválido.", nameof(tipo));

            var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
                ?? throw new KeyNotFoundException("Pedido não encontrado.");

         
            switch (tipo)
            {
                case ETipoOcorrencia.EntregueComSucesso:
                    if (pedido.IndEntregue)
                        throw new InvalidOperationException("O pedido já foi entregue.");
                    break;

                case ETipoOcorrencia.ClienteAusente:
               
                    _logger.LogWarning("Registrando ausência do cliente. PedidoId={PedidoId}", pedidoId);
                    break;

                case ETipoOcorrencia.AvariaNoProduto:
                    // talvez precise marcar para reentrega, acionar suporte etc.
                    break;

                case ETipoOcorrencia.EmRotaDeEntrega:
                    if (pedido.EstaConcluido())
                        throw new InvalidOperationException("Pedido já concluído; não pode voltar para rota.");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(tipo), tipo, "Tipo de ocorrência não suportado.");
            }

            var ocorrencia = Ocorrencia.Criar(tipo, hora, pedido.IdPedido);

            pedido.AdicionarOcorrencia(ocorrencia);

            await _pedidoRepo.AtualizarAsync(pedido);
            //await _ocRepo.AdicionarAsync(ocorrencia);

            _logger.LogInformation("Ocorrência registrada. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
            return ocorrencia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar ocorrência. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
            throw;
        }
    }

    //public async Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora)
    //{
    //    try
    //    {
    //        var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId)
    //            ?? throw new KeyNotFoundException("Pedido não encontrado.");

    //        var ocorrencia = Ocorrencia.Criar(tipo, hora);

    //        pedido.AdicionarOcorrencia(ocorrencia);

    //        await _pedidoRepo.AtualizarAsync(pedido);
    //        await _ocRepo.AdicionarAsync(ocorrencia);

    //        _logger.LogInformation("Ocorrência registrada. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
    //        return ocorrencia;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex, "Erro ao registrar ocorrência. PedidoId={PedidoId} Tipo={Tipo}", pedidoId, tipo);
    //        throw;
    //    }
    //}

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
            //await _ocRepo.RemoverAsync(oc);

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

            var ocorrencia = pedido.Ocorrencias.FirstOrDefault(o => o.IdOcorrencia == idOcorrencia);
            if (ocorrencia == null)
                throw new KeyNotFoundException("Ocorrência não encontrada no pedido.");

            _logger.LogInformation("Ocorrência obtida. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
            return ocorrencia;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter ocorrência. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, idOcorrencia);
            throw;
        }
    }
}
