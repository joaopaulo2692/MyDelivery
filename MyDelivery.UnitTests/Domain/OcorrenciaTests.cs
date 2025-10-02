using System;
using FluentAssertions;
using Xunit;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;

namespace MyDelivery.UnitTests.Domain
{
    public class OcorrenciaTests
    {
        [Fact]
        public void Criar_Deve_RetornarOcorrenciaValida()
        {
            var hora = DateTime.Now;
            var oc = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, hora);

            oc.Should().NotBeNull();
            oc.TipoOcorrencia.Should().Be(ETipoOcorrencia.EmRotaDeEntrega);
            oc.HoraOcorrencia.Should().Be(hora);
            oc.IndFinalizadora.Should().BeFalse();
        }
    }
}
