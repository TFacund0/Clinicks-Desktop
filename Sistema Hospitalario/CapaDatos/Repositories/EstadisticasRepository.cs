using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Sistema_Hospitalario.CapaDatos.Interfaces;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio especializado en la extracción de métricas y datos agregados para reportes y tableros.
    /// Proporciona conteos y distribuciones de pacientes, camas y turnos.
    /// </summary>
    public class EstadisticasRepository : IEstadisticasRepository
    {
        /// <summary>
        /// Cuenta la cantidad de pacientes que se registraron en una fecha específica y tienen un estado determinado.
        /// </summary>
        /// <param name="estado">Nombre del estado del paciente.</param>
        /// <param name="fecha">Fecha de registro a filtrar.</param>
        /// <returns>Cantidad de pacientes encontrados.</returns>
        public int ContarPacientesPorEstadoYFecha(string estado, DateTime fecha)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                return db.paciente
                    .Count(p =>
                        DbFunctions.TruncateTime(p.fecha_registracion) == fecha &&
                        p.estado_paciente.nombre.ToLower() == estado.ToLower());
            }
        }

        /// <summary>
        /// Cuenta las camas según su estado de disponibilidad actual.
        /// </summary>
        /// <param name="disponibilidad">Nombre de la disponibilidad (ej. 'Disponible', 'Ocupada').</param>
        /// <returns>Cantidad de camas en ese estado.</returns>
        public int ContarCamasPorDisponibilidad(string disponibilidad)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                return db.cama
                    .Count(c => c.estado_cama.disponibilidad.ToLower() == disponibilidad.ToLower());
            }
        }

        /// <summary>
        /// Obtiene un histórico diario del volumen de turnos registrados en un periodo de tiempo.
        /// </summary>
        /// <param name="fechaInicio">Fecha desde.</param>
        /// <param name="fechaFin">Fecha hasta.</param>
        /// <returns>Diccionario donde la clave es la fecha y el valor es el conteo de turnos.</returns>
        public Dictionary<DateTime, int> ObtenerConteoTurnosPorDia(DateTime fechaInicio, DateTime fechaFin)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                // truncamos fecha para comparar solo día
                var query = db.turno
                    .Where(t =>
                        DbFunctions.TruncateTime(t.fecha_registracion) >= fechaInicio.Date &&
                        DbFunctions.TruncateTime(t.fecha_registracion) <= fechaFin.Date)
                    .GroupBy(t => DbFunctions.TruncateTime(t.fecha_registracion))
                    .Select(g => new
                    {
                        Fecha = g.Key.Value,
                        Cantidad = g.Count()
                    })
                    .ToList();

                // pasamos a diccionario Fecha -> Cantidad
                var dic = new Dictionary<DateTime, int>();

                foreach (var item in query)
                {
                    dic[item.Fecha.Date] = item.Cantidad;
                }

                return dic;
            }
        }

        /// <summary>
        /// Obtiene la distribución porcentual o absoluta de turnos por su estado en un rango de fechas.
        /// </summary>
        /// <param name="fechaInicio">Fecha desde.</param>
        /// <param name="fechaFin">Fecha hasta.</param>
        /// <returns>Diccionario con el nombre del estado y su cantidad.</returns>
        public Dictionary<string, int> ObtenerDistribucionEstadosTurnos(DateTime fechaInicio, DateTime fechaFin)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var query = db.turno
                    .Where(t =>
                        DbFunctions.TruncateTime(t.fecha_registracion) >= fechaInicio.Date &&
                        DbFunctions.TruncateTime(t.fecha_registracion) <= fechaFin.Date)
                    .GroupBy(t => t.estado_turno.nombre)
                    .Select(g => new
                    {
                        Estado = g.Key,
                        Cantidad = g.Count()
                    })
                    .ToList();

                var dic = new Dictionary<string, int>();

                foreach (var item in query)
                {
                    if (item.Estado == null) continue;

                    string clave = item.Estado.ToLower();
                    dic[clave] = item.Cantidad;
                }

                return dic;
            }
        }

        /// <summary>
        /// Obtiene el flujo diario de nuevos pacientes registrados en el sistema.
        /// </summary>
        /// <param name="fechaInicio">Fecha desde.</param>
        /// <param name="fechaFin">Fecha hasta.</param>
        /// <returns>Diccionario cronológico de registros de pacientes.</returns>
        public Dictionary<DateTime, int> ObtenerConteoPacientesRegistradosPorDia(DateTime fechaInicio, DateTime fechaFin)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var query = db.paciente
                    .Where(p =>
                        DbFunctions.TruncateTime(p.fecha_registracion) >= fechaInicio.Date &&
                        DbFunctions.TruncateTime(p.fecha_registracion) <= fechaFin.Date)
                    .GroupBy(p => DbFunctions.TruncateTime(p.fecha_registracion))
                    .Select(g => new
                    {
                        Fecha = g.Key.Value,
                        Cantidad = g.Count()
                    })
                    .ToList();

                var dic = new Dictionary<DateTime, int>();

                foreach (var item in query)
                {
                    dic[item.Fecha.Date] = item.Cantidad;
                }

                return dic;
            }
        }

        /// <summary>
        /// Obtiene la distribución actual de todos los pacientes en el sistema según su estado clínico/administrativo.
        /// </summary>
        /// <returns>Diccionario con los totales por estado (ej. Activo, Internado, Alta).</returns>
        public Dictionary<string, int> ObtenerDistribucionPacientesPorEstado()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var query = db.paciente
                    .GroupBy(p => p.estado_paciente.nombre)
                    .Select(g => new
                    {
                        Estado = g.Key,
                        Cantidad = g.Count()
                    })
                    .ToList();

                var dic = new Dictionary<string, int>();

                foreach (var item in query)
                {
                    if (item.Estado == null)
                        continue;

                    string clave = item.Estado.ToLower(); // "activo", "internado", "alta"
                    dic[clave] = item.Cantidad;
                }

                return dic;
            }
        }
    }
}

