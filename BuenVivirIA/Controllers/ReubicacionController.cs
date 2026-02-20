using Microsoft.AspNetCore.Mvc;
using BuenVivirIA.Api.Modelos;
using BuenVivirIA.Api.Servicios;

namespace BuenVivirIA.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReubicacionController : ControllerBase
    {
        private readonly ReubicacionService _service;

        public ReubicacionController(ReubicacionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Analizar(SolicitudReubicacionDto solicitud)
        {
            var resultado = await _service.AnalizarAsync(solicitud);
            return Ok(resultado);
        }
    }
}