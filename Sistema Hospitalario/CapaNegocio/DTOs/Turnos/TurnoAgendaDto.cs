using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sistema_Hospitalario.CapaNegocio.DTOs.Turnos
{
    /// <summary>
    /// Representa un turno individual dentro de la agenda diaria de un médico.
    /// </summary>
    public class TurnoAgendaDto
    {
        /// <summary>Identificador único del turno.</summary>
        public int IdTurno { get; set; }

        /// <summary>Hora del turno formateada como cadena (ej. "09:30").</summary>
        public string Hora { get; set; }

        /// <summary>Nombre completo del paciente.</summary>
        public string Paciente { get; set; }

        /// <summary>Estado actual del turno (ej. 'Pendiente', 'Atendido').</summary>
        public string Estado { get; set; }
    }

    /// <summary>
    /// DTO que consolida los totales por estado para la vista de agenda.
    /// </summary>
    public class AgendaContadoresDto
    {
        /// <summary>Cantidad de turnos pendientes.</summary>
        public int Pendientes { get; set; }

        /// <summary>Cantidad de turnos completados/atendidos.</summary>
        public int Completadas { get; set; }

        /// <summary>Cantidad de turnos cancelados.</summary>
        public int Canceladas { get; set; }
    }

}
