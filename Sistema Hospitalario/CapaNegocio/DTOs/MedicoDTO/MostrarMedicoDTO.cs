using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.MedicoDTO
{
    /// <summary>
    /// DTO para la visualización de datos de médicos en grillas de administración.
    /// </summary>
    public class MostrarMedicoDTO
    {
        /// <summary>ID del médico.</summary>
        public int IdMedico { get; set; }

        /// <summary>Nombre del médico.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido del médico.</summary>
        public string Apellido { get; set; }

        /// <summary>Documento Nacional de Identidad.</summary>
        public string DNI { get; set; }

        /// <summary>Dirección de contacto.</summary>
        public string Direccion { get; set; }

        /// <summary>Número de matrícula habilitante.</summary>
        public string Matricula { get; set; }

        /// <summary>Correo electrónico de contacto.</summary>
        public string Correo { get; set; }

        /// <summary>Nombre de la especialidad.</summary>
        public string Especialidad { get; set; }
    }
}
