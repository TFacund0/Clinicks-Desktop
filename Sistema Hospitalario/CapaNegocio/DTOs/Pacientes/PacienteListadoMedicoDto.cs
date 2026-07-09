using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes
{
    /// <summary>
    /// DTO optimizado para las vistas médicas donde se requiere una visión rápida del estado del paciente.
    /// </summary>
    public class PacienteListadoMedicoDto
    {
        /// <summary>ID del paciente.</summary>
        public int IdPaciente { get; set; } 

        /// <summary>Fecha de nacimiento.</summary>
        public DateTime FechaNacim { get; set; }

        /// <summary>Nombre del paciente.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido del paciente.</summary>
        public string Apellido { get; set; }

        /// <summary>Dirección de domicilio.</summary>
        public string Direccion { get; set; }

        /// <summary>Documento Nacional de Identidad.</summary>
        public string Dni { get; set; }

        /// <summary>Ubicación física actual (ej. 'Hab. 101' o 'Ambulatorio').</summary>
        public string Habitacion { get; set; }

        /// <summary>Teléfono de contacto.</summary>
        public string Telefono { get; set; }

        /// <summary>Resumen de observaciones clínicas.</summary>
        public string Observacion { get; set; }

        /// <summary>Estado clínico actual (ej. 'Internado', 'En Espera').</summary>
        public string Estado { get; set; }
    }
}
