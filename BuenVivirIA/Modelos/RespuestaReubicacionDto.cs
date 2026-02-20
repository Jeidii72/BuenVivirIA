namespace BuenVivirIA.Api.Modelos
{
    // Este modelo representa la respuesta final del análisis
    public class RespuestaReubicacionDto
    {
        public string Pais { get; set; }              // País analizado
        public int PuntajeViabilidad { get; set; }   // Score entre 0 y 100
        public string NivelViabilidad { get; set; }  // Baja, Media, Alta
        public string CiudadRecomendada { get; set; } // Ciudad sugerida
        public string Ventajas { get; set; }         // Pros
        public string Desventajas { get; set; }      // Contras
        public string Insight { get; set; }          // Conclusión estratégica
    }
}
