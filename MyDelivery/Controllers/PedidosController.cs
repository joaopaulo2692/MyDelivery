using Microsoft.AspNetCore.Mvc;
using MyDelivery.Application.DTOs;
using MyDelivery.Application.DTOs.Ocorrencia;
using MyDelivery.Application.Interfaces;
using MyDelivery.Domain.DTOs.Pedido;
using MyDelivery.Domain.Entities;
using MyDelivery.Domain.Interfaces.Service;
using System.Security.Claims;

namespace MyDelivery.API.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _service;
        private readonly ILogger<PedidosController> _logger;

        public PedidosController(IPedidoService service, ILogger<PedidosController> logger)
        {
            _service = service;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }
            return idClaim.Value;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] CriarPedidoDto dto)
        {
            try
            {
                var userId = GetCurrentUserId(); // valida se usuário está autenticado
                var pedido = await _service.CriarPedidoAsync(dto.NumeroPedido, dto.HoraPedido);

                return CreatedAtAction(nameof(GetPedido),
                    new { pedidoId = pedido.IdPedido },
                    new ApiResponse<Pedido>
                    {
                        Success = true,
                        Data = pedido,
                        Message = "Pedido criado com sucesso"
                    });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuário não autenticado");
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Erro de negócio ao criar pedido");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao criar pedido. Numero={Numero}", dto.NumeroPedido);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno no servidor",
                    Data = ex.Message
                });
            }
        }

        [HttpGet("{pedidoId}")]
        public async Task<IActionResult> GetPedido(int pedidoId)
        {
            try
            {
                var userId = GetCurrentUserId(); // valida se usuário está autenticado
                var pedido = await _service.ObterPedidoPorIdAsync(pedidoId);

                if (pedido == null)
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Pedido não encontrado"
                    });

                return Ok(new ApiResponse<Pedido>
                {
                    Success = true,
                    Data = pedido,
                    Message = "Pedido obtido com sucesso"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Usuário não autenticado");
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao obter pedido. Id={Id}", pedidoId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erro interno no servidor",
                    Data = ex.Message
                });
            }
        }
    }
}
