using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes
{
    /// <summary>
    /// DTO que contiene los campos editables de un paciente existente.
    /// </summary>
    public class PacienteEditarDto
    {
        /// <summary>Identificador único del paciente.</summary>
        public int Id { get; set; }             

        /// <summary>Nuevo número de teléfono de contacto.</summary>
        public string Telefono { get; set; }

        /// <summary>Nueva dirección de domicilio.</summary>
        public string Direccion { get; set; }

        /// <summary>Nombre del nuevo estado a asignar.</summary>
        public string Estado { get; set; }      

        /// <summary>Nueva dirección de correo electrónico.</summary>
        public string Email { get; set; }

        /// <summary>Observaciones actualizadas.</summary>
        public string Observaciones { get; set; }
    }
}

