using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Especialidades
{
    /// <summary>
    /// DTO que representa una especialidad médica dentro del sistema.
    /// </summary>
    public class EspecialidadDto
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la especialidad.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre descriptivo de la especialidad (ej. Pediatría, Cardiología).
        /// </summary>
        public string Nombre { get; set; }
    }
}
