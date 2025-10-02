using MyDelivery.Domain.Entities;
using System.Threading.Tasks;

namespace MyDelivery.Application.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsync(int id);
        Task AtualizarAsync(Pedido pedido);
    }

    public interface IOcorrenciaRepository
    {
        Task AdicionarAsync(Ocorrencia ocorrencia);
        Task RemoverAsync(Ocorrencia ocorrencia);
    }
}
