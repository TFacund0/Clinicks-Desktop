namespace Sistema_Hospitalario.CapaNegocio.DTOs.Estadisticas
{
    /// <summary>
    /// Representa la disponibilidad actual de camas en el hospital.
    /// </summary>
    public class CamasDistribucionDto
    {
        /// <summary>
        /// Número de camas marcadas como 'Ocupada'.
        /// </summary>
        public int Ocupadas { get; set; }

        /// <summary>
        /// Número de camas marcadas como 'Disponible'.
        /// </summary>
        public int Disponibles { get; set; }
    }
}
