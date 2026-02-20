namespace BuenVivirIA.Api.Modelos
{
    // Este modelo representa la información que envía el usuario
    public class SolicitudReubicacionDto
    {
        public string Pais { get; set; }              // País que desea analizar
        public string NivelPresupuesto { get; set; }  // Bajo, Medio o Alto
        public string TipoTrabajo { get; set; }       // Remoto o Presencial
        public string Prioridad { get; set; }         // Seguridad, Costo, Clima, etc.
    }
}