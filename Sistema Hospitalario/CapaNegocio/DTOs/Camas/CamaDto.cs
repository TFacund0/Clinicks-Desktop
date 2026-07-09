using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Camas
{
    /// <summary>
    /// Representa los datos básicos de una cama en el sistema.
    /// </summary>
    public class CamaDto
    {
        /// <summary>
        /// Obtiene o establece el número identificador de la cama.
        /// </summary>
        public int NroCama { get; set; }

        /// <summary>
        /// Obtiene o establece el número de habitación a la que pertenece la cama.
        /// </summary>
        public int NroHabitacion { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del estado de la cama.
        /// </summary>
        public int IdEstadoCama { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción textual del estado (ej. Disponible, Ocupada).
        /// </summary>
        public string EstadoCama { get; set; }
    }
}
