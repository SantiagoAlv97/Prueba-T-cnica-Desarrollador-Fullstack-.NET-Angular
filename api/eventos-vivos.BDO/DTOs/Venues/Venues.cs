namespace eventos_vivos.BDO.DTOs.Venues
{
    public class CrearVenueRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
    }

    public class ActualizarVenueRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
    }

    public class VenueResponse
    {
        public long VenueId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string Ciudad { get; set; } = string.Empty;
    }
}
