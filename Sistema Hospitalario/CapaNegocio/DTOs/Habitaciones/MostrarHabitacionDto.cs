using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Habitaciones
{
    /// <summary>
    /// DTO utilizado para mostrar detalles de habitaciones en listados de la interfaz.
    /// </summary>
    public class MostrarHabitacionDto
    {
        /// <summary>
        /// Obtiene o establece el número de piso.
        /// </summary>
        public int NroPiso { get; set; }

        /// <summary>
        /// Obtiene o establece el número identificador de la habitación.
        /// </summary>
        public int NroHabitacion { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del tipo de habitación.
        /// </summary>
        public string TipoHabitacion { get; set; }

        /// <summary>
        /// Obtiene o establece el ID del tipo de habitación.
        /// </summary>
        public int IdTipoHabitacion { get; set; }
    }
}
