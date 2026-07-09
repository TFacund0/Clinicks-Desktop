using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Turnos
{
    /// <summary>
    /// DTO principal que representa la información completa de un turno médico.
    /// </summary>
    public class TurnoDto
    {
        /// <summary>ID único del turno.</summary>
        public int Id_turno { get; set; }

        /// <summary>Nombre completo del paciente.</summary>
        public string Paciente { get; set; }

        /// <summary>ID del paciente.</summary>
        public int Id_paciente { get; set; }

        /// <summary>Nombre completo del médico.</summary>
        public string Medico { get; set; }

        /// <summary>ID del médico asignado.</summary>
        public int Id_medico { get; set; }

        /// <summary>Nombre del procedimiento a realizar.</summary>
        public string Procedimiento { get; set; }

        /// <summary>ID del procedimiento.</summary>
        public int Id_procedimiento { get; set; }

        /// <summary>Correo electrónico de contacto del paciente.</summary>
        public string Correo { get; set; }

        /// <summary>DNI del paciente.</summary>
        public string DNI { get; set; }

        /// <summary>Teléfono de contacto del paciente.</summary>
        public string Telefono { get; set; }

        /// <summary>Fecha y hora programada para el turno.</summary>
        public DateTime FechaTurno { get; set; }

        /// <summary>Fecha y hora de creación del registro del turno.</summary>
        public DateTime FechaRegistro { get; set; }

        /// <summary>Notas adicionales o preparativos para el turno.</summary>
        public string Observaciones { get; set; }

        /// <summary>Estado actual del turno (ej. 'Confirmado', 'Cancelado').</summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// DTO optimizado para visualización en grillas de listado de turnos generales.
    /// </summary>
    public class ListadoTurno
    {
        /// <summary>ID del turno.</summary>
        public int Id_turno { get; set; }

        /// <summary>Nombre del paciente.</summary>
        public string Paciente { get; set; }

        /// <summary>Nombre del médico.</summary>
        public string Medico { get; set; }

        /// <summary>ID del médico.</summary>
        public int Id_medico { get; set; }

        /// <summary>Nombre del procedimiento.</summary>
        public string Procedimiento { get; set; }

        /// <summary>Fecha programada del turno.</summary>
        public DateTime FechaTurno { get; set; }

        /// <summary>Fecha del turno (propiedad redundante para compatibilidad).</summary>
        public DateTime Fecha_Del_Turno { get; set; }

        /// <summary>Descripción del estado.</summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// Estructura simple para el listado de estados de turnos en selectores.
    /// </summary>
    public class ListadoEstadoTurno
    {
        /// <summary>ID del estado (como cadena).</summary>
        public string Id_estado { get; set; }

        /// <summary>Descripción del estado.</summary>
        public string Estado { get; set; }
    }
}
