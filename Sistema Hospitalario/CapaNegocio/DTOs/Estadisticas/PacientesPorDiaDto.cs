using System;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Estadisticas
{
    /// <summary>
    /// Representa el conteo de pacientes (activos y altas) para una fecha específica.
    /// </summary>
    public class PacientesPorDiaDto
    {
        /// <summary>
        /// Obtiene o establece la fecha del reporte.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Cantidad de pacientes con estado 'Activo'.
        /// </summary>
        public int CantActivos { get; set; }

        /// <summary>
        /// Cantidad de pacientes con estado 'Alta'.
        /// </summary>
        public int CantAltas { get; set; }
    }
}
