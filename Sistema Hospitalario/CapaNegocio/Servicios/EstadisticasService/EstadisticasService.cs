using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.EstadisticasDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.EstadisticasService
{
    /// <summary>
    /// Servicio encargado de generar datos estadísticos y reportes consolidados para el dashboard del sistema.
    /// Proporciona información sobre evolución de pacientes, disponibilidad de camas y estados de turnos.
    /// </summary>
    public class EstadisticasService
    {
        private readonly IEstadisticasRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EstadisticasService"/> con el repositorio por defecto.
        /// </summary>
        public EstadisticasService() : this(new EstadisticasRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EstadisticasService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de estadísticas.</param>
        public EstadisticasService(IEstadisticasRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el conteo de pacientes activos y altas por día durante los últimos 7 días.
        /// </summary>
        /// <returns>Lista de <see cref="PacientesPorDiaDto"/> con la evolución semanal.</returns>
        public List<PacientesPorDiaDto> ObtenerPacientesSemana()
        {
            var lista = new List<PacientesPorDiaDto>();

            DateTime hoy = DateTime.Today;
            DateTime inicio = hoy.AddDays(-6);

            for (DateTime dia = inicio; dia <= hoy; dia = dia.AddDays(1))
            {
                int cantActivos = _repo.ContarPacientesPorEstadoYFecha("activo", dia.Date);
                int cantAltas = _repo.ContarPacientesPorEstadoYFecha("alta", dia.Date);

                lista.Add(new PacientesPorDiaDto
                {
                    Fecha = dia.Date,
                    CantActivos = cantActivos,
                    CantAltas = cantAltas
                });
            }

            return lista;
        }

        /// <summary>
        /// Obtiene la distribución actual de camas entre ocupadas y disponibles.
        /// </summary>
        /// <returns>Objeto <see cref="CamasDistribucionDto"/> con los totales por estado.</returns>
        public CamasDistribucionDto ObtenerDistribucionCamas()
        {
            int ocupadas = _repo.ContarCamasPorDisponibilidad("ocupada");
            int disponibles = _repo.ContarCamasPorDisponibilidad("disponible");

            return new CamasDistribucionDto
            {
                Ocupadas = ocupadas,
                Disponibles = disponibles
            };
        }

        /// <summary>
        /// Obtiene la cantidad de turnos programados diariamente durante la última semana.
        /// </summary>
        /// <returns>Lista de <see cref="TurnosPorDiaDto"/> con la serie temporal de turnos.</returns>
        public List<TurnosPorDiaDto> ObtenerTurnosPorDiaUltimaSemana()
        {
            DateTime hoy = DateTime.Today;
            DateTime inicio = hoy.AddDays(-6);

            // El repo devuelve un diccionarioFecha->Cantidad
            var conteo = _repo.ObtenerConteoTurnosPorDia(inicio, hoy);

            var lista = new List<TurnosPorDiaDto>();

            // Armamos la semana completa, incluyendo días sin turnos (cantidad 0)
            for (DateTime d = inicio; d <= hoy; d = d.AddDays(1))
            {
                int cantidad = conteo.ContainsKey(d.Date) ? conteo[d.Date] : 0;

                lista.Add(new TurnosPorDiaDto
                {
                    Fecha = d.Date,
                    Cantidad = cantidad
                });
            }

            return lista;
        }

        /// <summary>
        /// Obtiene la distribución de los estados de los turnos (Pendientes, Atendidos, Cancelados) registrados en los últimos 7 días.
        /// </summary>
        /// <returns>Objeto <see cref="TurnosEstadosDistribucionDto"/> con la distribución de estados.</returns>
        public TurnosEstadosDistribucionDto ObtenerDistribucionEstadosTurnosUltimaSemana()
        {
            DateTime hoy = DateTime.Today;
            DateTime inicio = hoy.AddDays(-6);

            var dic = _repo.ObtenerDistribucionEstadosTurnos(inicio, hoy);

            // normalizamos claves a lower
            int pendientes = dic.ContainsKey("pendiente") ? dic["pendiente"] : 0;
            int atendidos = dic.ContainsKey("atendido") ? dic["atendido"] : 0;
            int cancelados = dic.ContainsKey("cancelado") ? dic["cancelado"] : 0;

            return new TurnosEstadosDistribucionDto
            {
                Pendientes = pendientes,
                Atendidos = atendidos,
                Cancelados = cancelados
            };
        }

        /// <summary>
        /// Obtiene la cantidad de nuevos pacientes registrados en el sistema por cada día de la última semana.
        /// </summary>
        /// <returns>Lista de <see cref="PacientesRegistradosPorDiaDto"/>.</returns>
        public List<PacientesRegistradosPorDiaDto> ObtenerPacientesRegistradosPorDiaUltimaSemana()
        {
            DateTime hoy = DateTime.Today;
            DateTime inicio = hoy.AddDays(-6);

            // El repo devuelve un diccionario Fecha -> Cantidad
            var conteo = _repo.ObtenerConteoPacientesRegistradosPorDia(inicio, hoy);

            var lista = new List<PacientesRegistradosPorDiaDto>();

            // Rellenamos la semana completa (aunque algún día tenga 0)
            for (DateTime d = inicio; d <= hoy; d = d.AddDays(1))
            {
                int cantidad = conteo.ContainsKey(d.Date) ? conteo[d.Date] : 0;

                lista.Add(new PacientesRegistradosPorDiaDto
                {
                    Fecha = d.Date,
                    Cantidad = cantidad
                });
            }

            return lista;
        }

        /// <summary>
        /// Obtiene la distribución actual de pacientes según su estado clínico (Activo, Internado, Alta).
        /// </summary>
        /// <returns>Objeto <see cref="PacientesEstadosDistribucionDto"/> con los totales de distribución.</returns>
        public PacientesEstadosDistribucionDto ObtenerDistribucionPacientesPorEstado()
        {
            // El repo devuelve un diccionario estado -> cantidad
            var dic = _repo.ObtenerDistribucionPacientesPorEstado();

            int activos = dic.ContainsKey("activo") ? dic["activo"] : 0;
            int internados = dic.ContainsKey("internado") ? dic["internado"] : 0;
            int altas = dic.ContainsKey("alta") ? dic["alta"] : 0;

            return new PacientesEstadosDistribucionDto
            {
                Activos = activos,
                Internados = internados,
                Altas = altas
            };
        }
    }
}

