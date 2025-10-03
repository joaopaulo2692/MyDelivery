using Microsoft.AspNetCore.Mvc;
using MyDelivery.Application.DTOs.Ocorrencia;
using MyDelivery.Application.Interfaces;
using System.Security.Claims;

namespace MyDelivery.API.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class OcorrenciasController : ControllerBase
    {
        private readonly IOcorrenciaService _service;
        private readonly ILogger<OcorrenciasController> _logger;

        public OcorrenciasController(IOcorrenciaService service, ILogger<OcorrenciasController> logger)
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

        [HttpPost("{pedidoId}/ocorrencias")]
        public async Task<IActionResult> PostOcorrencia(int pedidoId, [FromBody] RegistrarOcorrenciaDto dto)
        {
            try
            {
                var userId = GetCurrentUserId(); // verifica autenticação

                var ocorrencia = await _service.RegistrarOcorrencia(pedidoId, dto.TipoOcorrencia, dto.HoraOcorrencia);
                return CreatedAtAction(nameof(GetOcorrencia),
                    new { pedidoId = pedidoId, id = ocorrencia.IdOcorrencia }, ocorrencia);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Tentativa de acesso não autenticado.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Pedido ou ocorrência não encontrada. PedidoId={PedidoId}", pedidoId);
                return NotFound(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Erro de negócio ao registrar ocorrência. PedidoId={PedidoId}", pedidoId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao registrar ocorrência. PedidoId={PedidoId}", pedidoId);
                return StatusCode(500, new { message = "Erro interno no servidor", details = ex.Message });
            }
        }

        [HttpDelete("{pedidoId}/ocorrencias/{id}")]
        public async Task<IActionResult> DeleteOcorrencia(int pedidoId, int id)
        {
            try
            {
                var userId = GetCurrentUserId(); // verifica autenticação

                await _service.ExcluirOcorrencia(pedidoId, id);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Tentativa de acesso não autenticado.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Pedido ou ocorrência não encontrada para exclusão. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, id);
                return NotFound(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Erro de negócio ao excluir ocorrência. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao excluir ocorrência. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, id);
                return StatusCode(500, new { message = "Erro interno no servidor", details = ex.Message });
            }
        }

        [HttpGet("{pedidoId}/ocorrencias/{id}")]
        public async Task<IActionResult> GetOcorrencia(int pedidoId, int id)
        {
            try
            {
                var userId = GetCurrentUserId(); // verifica autenticação

                var ocorrencia = await _service.ObterOcorrenciaPorIdAsync(pedidoId, id);
                return Ok(ocorrencia);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Tentativa de acesso não autenticado.");
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Pedido ou ocorrência não encontrada. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao obter ocorrência. PedidoId={PedidoId} OcorrenciaId={OcId}", pedidoId, id);
                return StatusCode(500, new { message = "Erro interno no servidor", details = ex.Message });
            }
        }
    }
}
