using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones
{
    /// <summary>
    /// Representa los datos completos de una internación, incluyendo referencias a médicos, pacientes y recursos físicos.
    /// </summary>
    public class InternacionDto
    {
        /// <summary>Identificador único de la internación.</summary>
        public int Id_internacion { get; set; }

        /// <summary>ID del paciente internado.</summary>
        public int Id_paciente { get; set; }

        /// <summary>Nombre completo del paciente.</summary>
        public string Internado { get; set; } 

        /// <summary>ID del médico responsable de la internación.</summary>
        public int Id_medico{ get; set; }

        /// <summary>Nombre completo del médico responsable.</summary>
        public string NombreCompletoMedico { get; set; }

        /// <summary>ID del procedimiento principal asociado.</summary>
        public int  Id_procedimiento { get; set; }

        /// <summary>Nombre del procedimiento realizado.</summary>
        public string procedimiento { get; set; }

        /// <summary>Fecha y hora de inicio de la internación.</summary>
        public DateTime Fecha_ingreso { get; set; }

        /// <summary>Fecha y hora de egreso (nulo si el paciente sigue internado).</summary>
        public DateTime? Fecha_egreso { get; set; }

        /// <summary>Motivo o diagnóstico de ingreso.</summary>
        public string Diagnostico { get; set; }

        /// <summary>Número de habitación asignada.</summary>
        public int Nro_habitacion { get; set; }

        /// <summary>Número de cama asignada.</summary>
        public int Id_cama { get; set; }

        /// <summary>Piso donde se encuentra la habitación.</summary>
        public int Nro_piso { get; set; }
    }

    /// <summary>
    /// DTO optimizado para el listado masivo de pacientes actualmente internados.
    /// </summary>
    public class ListadoInternacionDto
    {
        /// <summary>Número de habitación.</summary>
        public int Nro_habitacion { get; set; } 

        /// <summary>Número de piso.</summary>
        public int Nro_piso { get; set; }

        /// <summary>Nombre completo del paciente internado.</summary>
        public string Internado { get; set; }

        /// <summary>Fecha de ingreso al hospital.</summary>
        public DateTime Fecha_ingreso { get; set; }

        /// <summary>Número de cama.</summary>
        public int Cama { get; set; }

        /// <summary>Tipo de habitación (ej. Terapia, Común).</summary>
        public string Tipo_habitacion { get; set; }
    }

    /// <summary>
    /// DTO que encapsula los datos requeridos para procesar el alta médica de un paciente.
    /// </summary>
    public class FinalizarInternacionDto
    {
        /// <summary>ID de la internación a cerrar.</summary>
        public int IdInternacion { get; set; }

        /// <summary>Número de habitación que se libera.</summary>
        public int NroHabitacion { get; set; }

        /// <summary>Número de cama que se libera.</summary>
        public int IdCama { get; set; }

        /// <summary>Fecha original de ingreso.</summary>
        public DateTime FechaIngreso { get; set; }

        /// <summary>Fecha efectiva de egreso.</summary>
        public DateTime FechaEgreso { get; set; }

        /// <summary>Diagnóstico inicial registrado.</summary>
        public string DiagnosticoIngreso { get; set; }

        /// <summary>Diagnóstico final o resumen de egreso.</summary>
        public string DiagnosticoEgreso { get; set; }

        /// <summary>ID del médico que firma el alta.</summary>
        public int IdMedicoEgreso { get; set; }
    }

}
