using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Camas
{
    /// <summary>
    /// DTO utilizado para la visualización de camas en grillas o listados de la interfaz de usuario.
    /// </summary>
    public class MostrarCamaDto
    {
        /// <summary>
        /// Obtiene o establece el número identificador de la cama.
        /// </summary>
        public int NroCama { get; set; }

        /// <summary>
        /// Obtiene o establece el número de la habitación asociada.
        /// </summary>
        public int NroHabitacion { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del estado actual de la cama.
        /// </summary>
        public int IdEstadoCama { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción de disponibilidad (ej. 'Libre', 'Ocupada').
        /// </summary>
        public string Estado { get; set; }
    }
}
