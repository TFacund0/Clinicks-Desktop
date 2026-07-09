using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Habitaciones
{
    /// <summary>
    /// Representa los datos básicos de una habitación en la infraestructura hospitalaria.
    /// </summary>
    public class HabitacionDto
    {
        /// <summary>
        /// Obtiene o establece el número identificador de la habitación.
        /// </summary>
        public int Nro_habitacion { get; set; }

        /// <summary>
        /// Obtiene o establece el número de piso donde se encuentra la habitación.
        /// </summary>
        public int Nro_piso { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción del tipo de habitación (ej. 'Común', 'Terapia').
        /// </summary>
        public string Tipo_habitacion { get; set; }
    }
}
