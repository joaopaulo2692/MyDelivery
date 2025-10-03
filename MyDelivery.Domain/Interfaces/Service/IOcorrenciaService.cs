using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;
using System.Threading.Tasks;

namespace MyDelivery.Application.Interfaces
{
    public interface IOcorrenciaService
    {
        Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora);
        Task ExcluirOcorrencia(int pedidoId, int idOcorrencia);
        Task<Ocorrencia> ObterOcorrenciaPorIdAsync(int pedidoId, int idOcorrencia);
    }
}
