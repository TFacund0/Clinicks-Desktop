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
    /// DTO principal que representa la entidad Paciente con datos calculados y colecciones.
    /// </summary>
    public class PacienteDto
    {
        /// <summary>Identificador único del paciente.</summary>
        public int Id { get; set; }

        /// <summary>Documento Nacional de Identidad.</summary>
        public int Dni { get; set; }

        /// <summary>Propiedad calculada que devuelve el nombre en formato 'Apellido, Nombre'.</summary>
        public string Paciente 
        { 
            get
            {
                return $"{Apellido}, {Nombre}";
            }
        }

        /// <summary>Nombre del paciente.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido del paciente.</summary>
        public string Apellido { get; set; }

        /// <summary>Fecha de nacimiento.</summary>
        public DateTime Fecha_nacimiento {get; set; }

        /// <summary>Observaciones o notas clínicas generales.</summary>
        public string Observaciones { get; set; }

        /// <summary>Dirección de domicilio.</summary>
        public string Direccion {  get; set; }

        /// <summary>Dirección de correo electrónico.</summary>
        public string Email { get; set; }

        /// <summary>Descripción del estado actual del paciente.</summary>
        public string Estado_paciente { get; set; }

        /// <summary>ID del estado del paciente.</summary>
        public int Id_estado_paciente { get; set; }

        /// <summary>Número de teléfono principal.</summary>
        public string Telefono { get; set; }
        
        /// <summary>Colección de todos los teléfonos asociados al paciente.</summary>
        public virtual ICollection<telefono> Telefonos { get; set; }

        /// <summary>Propiedad calculada que devuelve la edad actual del paciente.</summary>
        public int Edad 
        { 
            get
            {
                var today = DateTime.Today;
                var age = today.Year - Fecha_nacimiento.Year;
                if (Fecha_nacimiento.Date > today.AddYears(-age)) age--;
                return age;
            }
        }
    }

    /// <summary>
    /// Estructura ligera para su uso en controles de selección (ComboBox).
    /// </summary>
    public class PacienteItem
    {
        /// <summary>ID del paciente.</summary>
        public int Id { get; set; }

        /// <summary>Texto descriptivo a mostrar (ej. 'Apellido, Nombre (DNI)').</summary>
        public string Texto { get; set; }
    }

    /// <summary>
    /// DTO optimizado para visualización en grillas generales de listado.
    /// </summary>
    public class PacienteListadoDto
    {
        /// <summary>ID del paciente.</summary>
        public int Id { get; set; }

        /// <summary>Nombre completo formateado.</summary>
        public string Paciente { get; set; }     

        /// <summary>Documento Nacional de Identidad.</summary>
        public int DNI { get; set; }

        /// <summary>Edad actual.</summary>
        public int Edad { get; set; }

        /// <summary>Estado administrativo/clínico.</summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// DTO que contiene la información completa para la vista de detalle de un paciente.
    /// </summary>
    public class PacienteDetalleDto
    {
        /// <summary>ID del paciente.</summary>
        public int Id { get; set; }

        /// <summary>Nombre.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido.</summary>
        public string Apellido { get; set; }

        /// <summary>Documento Nacional de Identidad.</summary>
        public int DNI { get; set; }

        /// <summary>Fecha de nacimiento.</summary>
        public DateTime FechaNacimiento { get; set; }

        /// <summary>Teléfono de contacto.</summary>
        public string Telefono { get; set; }

        /// <summary>Dirección de domicilio.</summary>
        public string Direccion { get; set; }

        /// <summary>Estado del paciente.</summary>
        public string Estado { get; set; }       

        /// <summary>Correo electrónico.</summary>
        public string Email { get; set; }

        /// <summary>Notas u observaciones.</summary>
        public string Observaciones { get; set; }
    }
}

