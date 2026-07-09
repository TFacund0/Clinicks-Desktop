namespace Sistema_Hospitalario.CapaNegocio.DTOs.Estadisticas
{
    /// <summary>
    /// Distribución de turnos según su estado actual para reportes estadísticos.
    /// </summary>
    public class TurnosEstadosDistribucionDto
    {
        /// <summary>
        /// Cantidad de turnos en estado 'Pendiente'.
        /// </summary>
        public int Pendientes { get; set; }

        /// <summary>
        /// Cantidad de turnos en estado 'Atendido'.
        /// </summary>
        public int Atendidos { get; set; }

        /// <summary>
        /// Cantidad de turnos en estado 'Cancelado'.
        /// </summary>
        public int Cancelados { get; set; }
    }
}
