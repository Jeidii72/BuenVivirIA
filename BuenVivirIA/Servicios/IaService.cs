using System.Text;
using System.Text.Json;
using BuenVivirIA.Api.Modelos;

namespace BuenVivirIA.Api.Servicios
{
    public class IaService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // Constructor donde inyectamos HttpClient y configuración
        public IaService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // Método principal que ejecuta el análisis con IA
        public async Task<RespuestaReubicacionDto> AnalizarAsync(SolicitudReubicacionDto solicitud)
        {
            // 🔐 Obtener API Key desde appsettings.json
            var apiKey = _configuration["OpenRouter:ApiKey"];

            // Limpiar headers anteriores
            _httpClient.DefaultRequestHeaders.Clear();

            // 🔑 Headers requeridos por OpenRouter
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "http://localhost");
            _httpClient.DefaultRequestHeaders.Add("X-Title", "BuenVivirIA");

            // 🧠 PROMPT optimizado y estricto
            var prompt = $@"
            Responde ÚNICAMENTE en JSON válido.
            No agregues texto fuera del JSON.
            No uses markdown.
            No uses ```.

            El JSON debe tener EXACTAMENTE estas propiedades:

            {{
              ""Pais"": ""{solicitud.Pais}"",
              ""PuntajeViabilidad"": número entero obligatorio entre 0 y 100 (NO usar escala 1-10, NO usar decimales),
              ""NivelViabilidad"": ""Baja | Media | Alta"",
              ""CiudadRecomendada"": ""string"",
              ""Ventajas"": ""máximo 400 caracteres"",
              ""Desventajas"": ""máximo 400 caracteres"",
              ""Insight"": ""máximo 300 caracteres""
            }}

            Reglas obligatorias:
            - 0–39 = Baja
            - 40–69 = Media
            - 70–100 = Alta
            - El puntaje debe ser coherente con el nivel.

            Analiza la viabilidad de vivir en {solicitud.Pais}
            con presupuesto {solicitud.NivelPresupuesto},
            trabajo {solicitud.TipoTrabajo}
            y prioridad {solicitud.Prioridad}.
            ";

            // 📦 Cuerpo de la solicitud para OpenRouter
            var cuerpoSolicitud = new
            {
                model = "mistralai/mistral-7b-instruct",
                messages = new[]
                {
                    new {
                        role = "system",
                        content = "Eres un experto en calidad de vida y reubicación internacional. Devuelve solo JSON válido."
                    },
                    new {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.3,
                max_tokens = 800
            };

            var contenido = new StringContent(
                JsonSerializer.Serialize(cuerpoSolicitud),
                Encoding.UTF8,
                "application/json");

            // 🚀 Enviar petición POST
            var respuesta = await _httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                contenido);

            if (!respuesta.IsSuccessStatusCode)
                throw new Exception($"Error en OpenRouter: {respuesta.StatusCode}");

            var textoRespuesta = await respuesta.Content.ReadAsStringAsync();

            // 📄 Parsear respuesta general
            using var doc = JsonDocument.Parse(textoRespuesta);

            var mensaje = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            // 🧹 Limpiar posibles bloques markdown
            var limpio = mensaje
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // 🔥 Extraer únicamente el JSON válido
            var inicio = limpio.IndexOf("{");
            var fin = limpio.LastIndexOf("}");

            if (inicio == -1 || fin == -1)
                throw new Exception("No se encontró un JSON válido en la respuesta de la IA.");

            limpio = limpio.Substring(inicio, fin - inicio + 1);

            using var jsonDoc = JsonDocument.Parse(limpio);
            var root = jsonDoc.RootElement;

            // ===============================
            // 🔢 Conversión robusta del puntaje
            // ===============================
            int puntaje = 0;

            if (root.TryGetProperty("PuntajeViabilidad", out var puntajeElement))
            {
                if (puntajeElement.ValueKind == JsonValueKind.Number)
                {
                    if (puntajeElement.TryGetInt32(out int valorInt))
                        puntaje = valorInt;
                    else if (puntajeElement.TryGetDouble(out double valorDouble))
                        puntaje = (int)Math.Round(valorDouble);
                }
                else if (puntajeElement.ValueKind == JsonValueKind.String)
                {
                    var texto = puntajeElement.GetString();

                    if (int.TryParse(texto, out int valorTexto))
                        puntaje = valorTexto;
                    else if (double.TryParse(texto, out double valorDoubleTexto))
                        puntaje = (int)Math.Round(valorDoubleTexto);
                }
            }

            // 🔥 Normalizar si la IA usó escala 1–10
            if (puntaje <= 10)
            {
                puntaje = puntaje * 10;
            }

            // ===============================
            // 📊 Corrección automática del NivelViabilidad
            // ===============================
            string nivelCalculado;

            if (puntaje <= 39)
                nivelCalculado = "Baja";
            else if (puntaje <= 69)
                nivelCalculado = "Media";
            else
                nivelCalculado = "Alta";

            // ===============================
            // 🏗 Construcción final del DTO
            // ===============================
            var resultado = new RespuestaReubicacionDto
            {
                Pais = root.GetProperty("Pais").GetString(),
                PuntajeViabilidad = puntaje,
                NivelViabilidad = nivelCalculado, // usamos el nivel corregido
                CiudadRecomendada = root.GetProperty("CiudadRecomendada").GetString(),
                Ventajas = root.GetProperty("Ventajas").GetString(),
                Desventajas = root.GetProperty("Desventajas").GetString(),
                Insight = root.GetProperty("Insight").GetString()
            };

            return resultado;
        }
    }
}