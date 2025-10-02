using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Serilog;
using Xunit;
using MyDelivery.Application.Service;
using MyDelivery.Application.Interfaces;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;

namespace MyDelivery.UnitTests.Services
{
    public class PedidoServiceTests
    {
        private readonly Mock<IPedidoRepository> _pedidoRepoMock;
        private readonly Mock<IOcorrenciaRepository> _ocRepoMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly PedidoService _pedidoService;

        public PedidoServiceTests()
        {
            _pedidoRepoMock = new Mock<IPedidoRepository>();
            _ocRepoMock = new Mock<IOcorrenciaRepository>();
            _loggerMock = new Mock<ILogger>();

            _pedidoService = new PedidoService(
                _pedidoRepoMock.Object,
                _ocRepoMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RegistrarOcorrencia_Deve_AdicionarOcorrenciaNoPedido()
        {
            var pedido = new Pedido(1234, DateTime.Now);

            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pedido);

            var ocorrencia = await _pedidoService.RegistrarOcorrencia(1, ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now);

            ocorrencia.Should().NotBeNull();
            pedido.Ocorrencias.Should().ContainSingle();
            _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);
            _ocRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Ocorrencia>()), Times.Once);
        }

        [Fact]
        public async Task RegistrarOcorrencia_Deve_LancarExcecao_SePedidoNaoEncontrado()
        {
            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pedido)null);

            Func<Task> act = async () => await _pedidoService.RegistrarOcorrencia(1, ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Pedido não encontrado.");
        }
            
        [Fact]
        public async Task ExcluirOcorrencia_Deve_RemoverOcorrencia()
        {
            var pedido = new Pedido(5678, DateTime.Now);
            var ocorrencia = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now);
            pedido.AdicionarOcorrencia(ocorrencia);

            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pedido);

            await _pedidoService.ExcluirOcorrencia(1, ocorrencia.IdOcorrencia);

            pedido.Ocorrencias.Should().BeEmpty();
            _ocRepoMock.Verify(r => r.RemoverAsync(It.IsAny<Ocorrencia>()), Times.Once);
            _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);
        }
    }
}
