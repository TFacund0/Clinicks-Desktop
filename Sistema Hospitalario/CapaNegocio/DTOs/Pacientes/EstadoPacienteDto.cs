using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes
{
    /// <summary>
    /// Representa un estado clínico o administrativo de un paciente (ej. 'Activo', 'Internado', 'Alta').
    /// </summary>
    public class EstadoPacienteDto
    {
        /// <summary>Identificador único del estado.</summary>
        public int Id { get; set; }        

        /// <summary>Nombre descriptivo del estado.</summary>
        public string Nombre { get; set; } 
    }
}
