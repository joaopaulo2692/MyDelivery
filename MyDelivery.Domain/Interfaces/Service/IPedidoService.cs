using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;

namespace MyDelivery.Application.Interfaces
{
    public interface IPedidoService
    {
        Task<Ocorrencia> RegistrarOcorrencia(int pedidoId, ETipoOcorrencia tipo, DateTime hora);
        Task ExcluirOcorrencia(int pedidoId, int idOcorrencia);
    }
}
