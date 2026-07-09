using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Historiales
{
    /// <summary>
    /// Representa una entrada individual en el historial clínico consolidado de un paciente.
    /// Puede ser una consulta, un turno o una internación/procedimiento.
    /// </summary>
    public class HistorialItemDto
    {
        /// <summary>
        /// Fecha de inicio o realización del evento médico.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Fecha de finalización (opcional, aplicable principalmente a internaciones).
        /// </summary>
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Categoría del evento (ej. "Consulta", "Turno", "Procedimiento/Internación").
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Razón o síntoma inicial que motivó la atención.
        /// </summary>
        public string Motivo { get; set; }

        /// <summary>
        /// Conclusión médica o nombre del procedimiento realizado.
        /// </summary>
        public string Diagnostico { get; set; }

        /// <summary>
        /// Prescripción médica o indicaciones post-atención.
        /// </summary>
        public string Tratamiento { get; set; }

        /// <summary>
        /// Nombre y apellido del médico que atendió al paciente.
        /// </summary>
        public string NombreMedico { get; set; }

        /// <summary>
        /// Documento Nacional de Identidad del profesional interviniente.
        /// </summary>
        public string DniMedico { get; set; }

        /// <summary>
        /// Identificador único del médico (utilizado para filtrado por profesional).
        /// </summary>
        public int IdMedico { get; set; }
    }
}
