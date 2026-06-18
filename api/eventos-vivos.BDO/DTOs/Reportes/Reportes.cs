namespace eventos_vivos.BDO.DTOs.Reportes
{
    public class ReporteOcupacionResponse
    {
        public long EventoId { get; set; }
        public string EventoTitulo { get; set; } = string.Empty;
        public int CapacidadMaxima { get; set; }
        public int EntradasVendidas { get; set; }
        public int EntradasDisponibles { get; set; }
        public decimal PorcentajeOcupacion { get; set; }
        public decimal TotalIngresos { get; set; }
        public string EstadoEvento { get; set; } = string.Empty;
    }
}
