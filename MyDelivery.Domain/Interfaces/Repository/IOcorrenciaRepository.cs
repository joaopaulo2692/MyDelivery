using MyDelivery.Domain.Entities;
using System.Threading.Tasks;

namespace MyDelivery.Application.Interfaces
{
    public interface IOcorrenciaRepository
    {
        Task<Pedido?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Ocorrencia ocorrencia);
        Task RemoverAsync(Ocorrencia ocorrencia);
    }
}
