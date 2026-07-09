namespace Sistema_Hospitalario.CapaNegocio.DTOs.EstadisticasDTO
{
    /// <summary>
    /// Distribución global de pacientes según su estado clínico o administrativo actual.
    /// </summary>
    public class PacientesEstadosDistribucionDto
    {
        /// <summary>
        /// Total de pacientes en estado 'Activo'.
        /// </summary>
        public int Activos { get; set; }

        /// <summary>
        /// Total de pacientes actualmente 'Internados'.
        /// </summary>
        public int Internados { get; set; }

        /// <summary>
        /// Total de pacientes que han recibido el 'Alta'.
        /// </summary>
        public int Altas { get; set; }
    }
}
