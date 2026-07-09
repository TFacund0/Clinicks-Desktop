using System;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Estadisticas
{
    /// <summary>
    /// Estructura para el gráfico de volumen de turnos por día.
    /// </summary>
    public class TurnosPorDiaDto
    {
        /// <summary>
        /// Fecha del día consultado.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Conteo total de turnos (independientemente del estado) para ese día.
        /// </summary>
        public int Cantidad { get; set; }
    }
}
