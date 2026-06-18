export interface Usuario {
  usuarioId?: number | string | null;
  nombre?: string | null;
  email?: string | null;
  rol?: string | null;
  fotoUrl?: string | null;
  picture?: string | null;
  activo?: boolean | null;
  fechaCreacion?: string | null;
  fechaUltimoAcceso?: string | null;
}
