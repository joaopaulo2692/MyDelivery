using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MyDelivery.Application.Interfaces;
using MyDelivery.Application.Services;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Enums;
using MyDelivery.Domain.Interfaces.Repository;
using System;
using System.Linq;
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
            var pedido = new Pedido(1234, DateTime.Now);

            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pedido);

            var ocorrencia = await _ocorrenciaService.RegistrarOcorrencia(
                1, ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now
            );

            ocorrencia.Should().NotBeNull();
            pedido.Ocorrencias.Should().ContainSingle();
            _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);
            _ocRepoMock.Verify(r => r.AdicionarAsync(It.IsAny<Ocorrencia>()), Times.Once);
        }

        [Fact]
        public async Task RegistrarOcorrencia_Deve_LancarExcecao_SePedidoNaoEncontrado()
        {
            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(It.IsAny<int>())).ReturnsAsync((Pedido)null);

            Func<Task> act = async () => await _ocorrenciaService.RegistrarOcorrencia(
                1, ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now
            );

            await act.Should().ThrowAsync<KeyNotFoundException>()
                     .WithMessage("Pedido não encontrado.");
        }

        [Fact]
        public async Task ExcluirOcorrencia_Deve_RemoverOcorrencia()
        {
            var pedido = new Pedido(5678, DateTime.Now);
            var ocorrencia = Ocorrencia.Criar(ETipoOcorrencia.EmRotaDeEntrega, DateTime.Now, pedido.IdPedido);
            pedido.AdicionarOcorrencia(ocorrencia);

            _pedidoRepoMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(pedido);

            await _ocorrenciaService.ExcluirOcorrencia(1, ocorrencia.IdOcorrencia);

            pedido.Ocorrencias.Should().BeEmpty();
            _ocRepoMock.Verify(r => r.RemoverAsync(It.IsAny<Ocorrencia>()), Times.Once);
            _pedidoRepoMock.Verify(r => r.AtualizarAsync(pedido), Times.Once);
        }
    }
}
