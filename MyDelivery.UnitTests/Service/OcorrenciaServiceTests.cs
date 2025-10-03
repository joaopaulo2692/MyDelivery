using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyDelivery.Application.Interfaces;
using MyDelivery.Application.Services;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;
using MyDelivery.Domain.Interfaces.Repository;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MyDelivery.UnitTests.Services
{
    public class OcorrenciaServiceTests
    {
        private readonly Mock<IPedidoRepository> _pedidoRepoMock;
        private readonly Mock<IOcorrenciaRepository> _ocRepoMock;
        private readonly Mock<ILogger<OcorrenciaService>> _loggerMock;
        private readonly OcorrenciaService _ocorrenciaService;

        public OcorrenciaServiceTests()
        {
            _pedidoRepoMock = new Mock<IPedidoRepository>();
            _ocRepoMock = new Mock<IOcorrenciaRepository>();
            _loggerMock = new Mock<ILogger<OcorrenciaService>>();

            _ocorrenciaService = new OcorrenciaService(
                _pedidoRepoMock.Object,
                _ocRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RegistrarOcorrencia_Deve_AdicionarOcorrenciaNoPedido()
        {
            // Arrange
            var pedido = new Pedido(1234, DateTime.Now);
            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pedido);

            // Act
            var ocorrencia = await _ocorrenciaService.RegistrarOcorrencia(
                1, ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now
            );

            // Assert
            ocorrencia.Should().NotBeNull();
            pedido.Ocorrencias.Should().ContainSingle();
            _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);

            // Não deve chamar _ocRepo.AdicionarAsync, pois agora só usamos AtualizarAsync
            _ocRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Ocorrencia>()), Times.Never);
        }

        [Fact]
        public async Task RegistrarOcorrencia_Deve_LancarExcecao_SePedidoNaoEncontrado()
        {
            // Arrange
            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pedido)null);

            // Act
            Func<Task> act = async () => await _ocorrenciaService.RegistrarOcorrencia(
                1, ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now
            );

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                     .WithMessage("Pedido não encontrado.");
        }

        [Fact]
        public async Task ExcluirOcorrencia_Deve_RemoverOcorrencia()
        {
            // Arrange
            var pedido = new Pedido(5678, DateTime.Now);
            var ocorrencia = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now, pedido.NumeroPedido);
            pedido.AdicionarOcorrencia(ocorrencia);

            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pedido);

            // Act
            await _ocorrenciaService.ExcluirOcorrencia(1, ocorrencia.IdOcorrencia);

            // Assert
            pedido.Ocorrencias.Should().BeEmpty();
            _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);

            // Agora a exclusão ainda chama _ocRepo.RemoverAsync
            _ocRepoMock.Verify(r => r.RemoverAsync(It.IsAny<Ocorrencia>()), Times.Once);
        }

        [Fact]
        public void AdicionarOcorrencia_NaoPermiteMesmoTipoEm10Minutos()
        {
            var pedido = new Pedido(1001, DateTime.Now);
            var agora = DateTime.Now;

            var oc1 = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, agora, pedido.IdPedido);
            pedido.AdicionarOcorrencia(oc1);

            var oc2 = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, agora.AddMinutes(5), pedido.IdPedido);

            Action act = () => pedido.AdicionarOcorrencia(oc2);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Não é possível cadastrar duas ocorrências do mesmo tipo em menos de 10 minutos.");
        }

        [Theory]
        [InlineData(ETipoOcorrencia.EntregueComSucesso, true)]
        [InlineData(ETipoOcorrencia.ClienteAusente, false)]
        public void AdicionarOcorrencia_SegundaOcorrencia_MarcaFinalizadoraETipoEntrega(ETipoOcorrencia tipo, bool esperadoIndEntregue)
        {
            var pedido = new Pedido(1001, DateTime.Now);

            // primeira ocorrência
            var oc1 = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now, pedido.IdPedido);
            pedido.AdicionarOcorrencia(oc1);

            // segunda ocorrência
            var oc2 = Ocorrencia.Criar(tipo, DateTime.Now.AddMinutes(15), pedido.IdPedido);
            pedido.AdicionarOcorrencia(oc2);

            // a segunda deve ser finalizadora
            oc2.IndFinalizadora.Should().BeTrue();

            // pedido deve refletir o IndEntregue corretamente
            pedido.IndEntregue.Should().Be(esperadoIndEntregue);
        }
        [Fact]
        public void RemoverOcorrencia_PedidoConcluido_DeveLancarExcecao()
        {
            var pedido = new Pedido(1001, DateTime.Now);

            // adiciona primeira
            var oc1 = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now, pedido.IdPedido);
            pedido.AdicionarOcorrencia(oc1);

            // adiciona segunda que finaliza
            var oc2 = Ocorrencia.Criar(ETipoOcorrencia.EntregueComSucesso, DateTime.Now.AddMinutes(15), pedido.IdPedido);
            pedido.AdicionarOcorrencia(oc2);

            Action act = () => pedido.RemoverOcorrencia(oc1.IdOcorrencia);
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Pedido já concluído; exclusão de ocorrência não permitida.");
        }


    }
}
