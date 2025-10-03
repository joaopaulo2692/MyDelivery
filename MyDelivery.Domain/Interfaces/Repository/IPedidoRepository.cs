using MyDelivery.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDelivery.Domain.Interfaces.Repository
{
    public interface IPedidoRepository
    {
        Task<Pedido?> ObterPorIdAsync(int idPedido);
        Task AdicionarAsync(Pedido pedido);
        Task AtualizarAsync(Pedido pedido);
    }
}
