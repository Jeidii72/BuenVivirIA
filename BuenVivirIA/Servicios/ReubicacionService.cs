using BuenVivirIA.Api.Modelos;

namespace BuenVivirIA.Api.Servicios
{
    public class ReubicacionService
    {
        private readonly PaisService _paisService;
        private readonly IaService _iaService;

        public ReubicacionService(PaisService paisService, IaService iaService)
        {
            _paisService = paisService;
            _iaService = iaService;
        }

        public async Task<RespuestaReubicacionDto> AnalizarAsync(SolicitudReubicacionDto solicitud)
        {
            var datosPais = await _paisService.ObtenerPaisAsync(solicitud.Pais);

            var resultado = await _iaService.AnalizarAsync(solicitud);

            resultado.Pais = datosPais.Nombre;

            return resultado;
        }
    }
}