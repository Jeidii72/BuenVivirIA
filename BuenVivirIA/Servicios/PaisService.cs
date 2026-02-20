using System.Text.Json;
using BuenVivirIA.Api.Modelos;

namespace BuenVivirIA.Api.Servicios
{
    public class PaisService
    {
        private readonly HttpClient _httpClient;

        // Inyectamos HttpClient para poder consumir APIs externas
        public PaisService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PaisDto> ObtenerPaisAsync(string pais)
        {
            // Limpiamos el nombre del país
            var paisLimpio = pais.Trim();
            var paisCodificado = Uri.EscapeDataString(paisLimpio);

            // Construimos URL de REST Countries
            var url = $"https://restcountries.com/v3.1/name/{paisCodificado}?fullText=true";

            var respuesta = await _httpClient.GetAsync(url);

            if (!respuesta.IsSuccessStatusCode)
                throw new Exception($"País no encontrado: {pais}");

            var contenido = await respuesta.Content.ReadAsStringAsync();
            var documento = JsonDocument.Parse(contenido);

            var raiz = documento.RootElement[0];

            // Mapeamos datos importantes
            return new PaisDto
            {
                Nombre = raiz.GetProperty("name").GetProperty("common").GetString(),
                Capital = raiz.GetProperty("capital")[0].GetString(),
                Region = raiz.GetProperty("region").GetString(),
                Poblacion = raiz.GetProperty("population").GetInt64()
            };
        }
    }
}