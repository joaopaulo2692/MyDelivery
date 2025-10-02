using System;
using FluentAssertions;
using Xunit;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;

namespace MyDelivery.UnitTests.Domain
{
    public class PedidoTests
    {
        [Fact]
        public void AdicionarOcorrencia_Deve_AumentarLista()
        {
            var pedido = new Pedido(1001, DateTime.Now);
            var oc = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now);

            pedido.AdicionarOcorrencia(oc);

            pedido.Ocorrencias.Should().ContainSingle();
            pedido.Ocorrencias.First().TipoOcorrencia.Should().Be(ETipoOcorrencia.EmRotaDeEntrega);
        }

        [Fact]
        public void RemoverOcorrencia_Deve_ReduzirLista()
        {
            var pedido = new Pedido( 1001, DateTime.Now);
            var oc = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now);
            pedido.AdicionarOcorrencia(oc);

            pedido.RemoverOcorrencia(oc.IdOcorrencia);

            pedido.Ocorrencias.Should().BeEmpty();
        }
    }
}
