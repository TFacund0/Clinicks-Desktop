using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Home
{
    /// <summary>
    /// DTO que representa una entrada de actividad reciente en el tablero principal.
    /// Consolida información de pacientes, turnos e internaciones.
    /// </summary>
    public class HomeDto
    {
        /// <summary>
        /// Obtiene o establece el nombre del paciente involucrado en la actividad.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido del paciente involucrado en la actividad.
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Describe el tipo de acción realizada (ej. "Turno registrado", "Internación finalizada").
        /// </summary>
        public string Accion { get; set; }

        /// <summary>
        /// Fecha y hora en la que se registró la actividad.
        /// </summary>
        public DateTime Horario { get; set; }

        /// <summary>
        /// Categoría o tipo de procedimiento asociado a la actividad (opcional).
        /// </summary>
        public string Tipo { get; set; }
    }
}
