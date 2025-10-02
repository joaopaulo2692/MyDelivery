using Microsoft.AspNetCore.Mvc;
using MyDelivery.Application.DTOs;
using MyDelivery.Application.Service;

namespace MyDelivery.API.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidosController : ControllerBase
    {
        private readonly PedidoService _service;
        public PedidosController(PedidoService service) => _service = service;

        [HttpPost("{pedidoId}/ocorrencias")]
        public async Task<IActionResult> PostOcorrencia(int pedidoId, [FromBody] RegistrarOcorrenciaDto dto)
        {
            var oc = await _service.RegistrarOcorrencia(pedidoId, dto.TipoOcorrencia, dto.HoraOcorrencia);
            return CreatedAtAction(nameof(GetOcorrencia), new { pedidoId = pedidoId, id = oc.IdOcorrencia }, oc);
        }

        [HttpDelete("{pedidoId}/ocorrencias/{id}")]
        public async Task<IActionResult> DeleteOcorrencia(int pedidoId, int id)
        {
            await _service.ExcluirOcorrencia(pedidoId, id);
            return NoContent();
        }

        [HttpGet("{pedidoId}/ocorrencias/{id}")]
        public IActionResult GetOcorrencia(int pedidoId, int id) => Ok(); // implementar
    }


}
