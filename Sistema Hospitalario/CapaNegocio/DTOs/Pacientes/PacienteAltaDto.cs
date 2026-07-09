using Sistema_Hospitalario.CapaDatos;
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
    /// DTO que encapsula la información requerida para dar de alta a un nuevo paciente en el sistema.
    /// </summary>
    public class PacienteAltaDto
    {
        /// <summary>Nombre del paciente.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido del paciente.</summary>
        public string Apellido { get; set; }

        /// <summary>Documento Nacional de Identidad.</summary>
        public int Dni { get; set; }

        /// <summary>Fecha de nacimiento (opcional).</summary>
        public DateTime? FechaNacimiento { get; set; }

        /// <summary>Notas adicionales o antecedentes relevantes.</summary>
        public string Observaciones { get; set; }

        /// <summary>Domicilio particular.</summary>
        public string Direccion { get; set; }

        /// <summary>Dirección de correo electrónico.</summary>
        public string Email { get; set; }

        /// <summary>Número de teléfono de contacto.</summary>
        public string Telefono { get; set; }

        /// <summary>Estado inicial del paciente (ej. 'Activo').</summary>
        public string EstadoInicial { get; set; }
    }
}

