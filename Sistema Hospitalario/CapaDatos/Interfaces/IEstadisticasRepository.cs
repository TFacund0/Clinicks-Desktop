using System;
using System.Collections.Generic;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las consultas de agregación y conteo utilizadas por el módulo de estadísticas gerenciales.
    /// </summary>
    public interface IEstadisticasRepository
    {
        /// <summary>
        /// Cuenta los pacientes que se encuentran en un estado determinado a una fecha dada.
        /// </summary>
        /// <param name="estado">Nombre del estado del paciente.</param>
        /// <param name="fecha">Fecha de referencia.</param>
        /// <returns>Cantidad de pacientes.</returns>
        int ContarPacientesPorEstadoYFecha(string estado, DateTime fecha);

        /// <summary>
        /// Cuenta las camas según su disponibilidad actual.
        /// </summary>
        /// <param name="disponibilidad">Nombre del estado de la cama (por ejemplo, "Disponible").</param>
        /// <returns>Cantidad de camas.</returns>
        int ContarCamasPorDisponibilidad(string disponibilidad);

        /// <summary>
        /// Obtiene la cantidad de turnos agrupados por día dentro de un rango de fechas.
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del rango.</param>
        /// <param name="fechaFin">Fecha final del rango.</param>
        /// <returns>Diccionario fecha → cantidad de turnos.</returns>
        Dictionary<DateTime, int> ObtenerConteoTurnosPorDia(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene la distribución de turnos según su estado dentro de un rango de fechas.
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del rango.</param>
        /// <param name="fechaFin">Fecha final del rango.</param>
        /// <returns>Diccionario estado → cantidad de turnos.</returns>
        Dictionary<string, int> ObtenerDistribucionEstadosTurnos(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene la cantidad de pacientes registrados agrupados por día dentro de un rango de fechas.
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del rango.</param>
        /// <param name="fechaFin">Fecha final del rango.</param>
        /// <returns>Diccionario fecha → cantidad de pacientes registrados.</returns>
        Dictionary<DateTime, int> ObtenerConteoPacientesRegistradosPorDia(DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene la distribución actual de pacientes según su estado.
        /// </summary>
        /// <returns>Diccionario estado → cantidad de pacientes.</returns>
        Dictionary<string, int> ObtenerDistribucionPacientesPorEstado();
    }
}
