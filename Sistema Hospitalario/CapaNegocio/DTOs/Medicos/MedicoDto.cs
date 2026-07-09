using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Medicos
{
    /// <summary>
    /// Representa los datos detallados de un profesional médico.
    /// </summary>
    public class MedicoDto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del médico.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el número de matrícula profesional.
        /// </summary>
        public string Matricula { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del médico.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el apellido del médico.
        /// </summary>
        public string Apellido { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la especialidad principal.
        /// </summary>
        public string Especialidad { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección del consultorio o domicilio profesional.
        /// </summary>
        public string Direccion { get; set; }

        /// <summary>
        /// Obtiene o establece la dirección de correo electrónico institucional.
        /// </summary>
        public string Email { get; set; }
    }
}
