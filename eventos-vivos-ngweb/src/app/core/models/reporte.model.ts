export interface ReporteOcupacion {
  eventoId?: number | string | null;
  eventoTitulo?: string | null;
  entradasVendidas?: number | string | null;
  entradasDisponibles?: number | string | null;
  porcentajeOcupacion?: number | string | null;
  totalIngresos?: number | string | null;
  estadoEvento?: string | null;
}
