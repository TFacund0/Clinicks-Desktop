using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Medicos
{
    /// <summary>
    /// DTO simplificado utilizado para poblar selectores (ComboBox/Select) en la interfaz.
    /// </summary>
    public class MedicoSimpleDto
    {
        /// <summary>
        /// Identificador único del médico.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Cadena formateada que combina Nombre, Apellido y DNI/Matrícula para identificación visual.
        /// </summary>
        public string NombreCompletoYDNI { get; set; }
    }
}
