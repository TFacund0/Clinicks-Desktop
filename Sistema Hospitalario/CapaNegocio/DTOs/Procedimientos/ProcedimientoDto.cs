using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos
{
    /// <summary>
    /// Representa un procedimiento médico disponible en el catálogo del hospital.
    /// </summary>
    public class ProcedimientoDto
    {
        /// <summary>Identificador único del procedimiento.</summary>
        public int Id { get; set; }

        /// <summary>Nombre o descripción del procedimiento.</summary>
        public string Name { get; set; }
    }
}
