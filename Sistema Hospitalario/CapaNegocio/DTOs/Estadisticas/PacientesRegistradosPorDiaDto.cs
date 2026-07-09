using System;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Estadisticas
{
    /// <summary>
    /// Estructura para visualizar el crecimiento de la base de pacientes registrados.
    /// </summary>
    public class PacientesRegistradosPorDiaDto
    {
        /// <summary>
        /// Fecha de registro.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Cantidad de pacientes nuevos registrados en esta fecha.
        /// </summary>
        public int Cantidad { get; set; }
    }
}
