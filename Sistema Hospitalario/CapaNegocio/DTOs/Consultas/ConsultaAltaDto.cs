using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Consultas
{
    /// <summary>
    /// DTO diseñado para capturar la información necesaria al registrar una nueva consulta médica.
    /// </summary>
    public class ConsultaAltaDto
    {
        /// <summary>
        /// Obtiene o establece el DNI del paciente que asiste a la consulta.
        /// </summary>
        public string DniPaciente { get; set; }

        /// <summary>
        /// Obtiene o establece el número de afiliado, utilizado para validaciones de cobertura o identidad.
        /// </summary>
        public string NroAfiliado { get; set; }

        /// <summary>
        /// Obtiene o establece el motivo principal de la visita médica.
        /// </summary>
        public string Motivo { get; set; }

        /// <summary>
        /// Obtiene o establece el diagnóstico presuntivo o confirmado emitido por el profesional.
        /// </summary>
        public string Diagnostico { get; set; }

        /// <summary>
        /// Obtiene o establece las indicaciones de tratamiento o medicación.
        /// </summary>
        public string Tratamiento { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora en la que se realiza la consulta.
        /// </summary>
        public DateTime Fecha { get; set; }
    }
}
