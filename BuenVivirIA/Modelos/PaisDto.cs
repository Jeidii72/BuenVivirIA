namespace BuenVivirIA.Api.Modelos
{
    // Modelo interno para mapear la API externa REST Countries
    public class PaisDto
    {
        public string Nombre { get; set; }
        public string Capital { get; set; }
        public string Region { get; set; }
        public long Poblacion { get; set; }
    }
}
