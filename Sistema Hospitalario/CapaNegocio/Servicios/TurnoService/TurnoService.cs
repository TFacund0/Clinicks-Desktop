using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.TurnoDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;   

namespace Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio para los turnos médicos.
    /// Permite la programación, actualización de estados, consultas de agenda y reportes de turnos.
    /// </summary>
    public class TurnoService
    {
        private readonly ITurnoRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TurnoService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de turnos.</param>
        public TurnoService(ITurnoRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TurnoService"/> usando el repositorio por defecto.
        /// </summary>
        public TurnoService() : this(new TurnoRepository())
        {
        }

        /// <summary>
        /// Obtiene la lista de turnos programados para un médico en una fecha específica, optimizada para la visualización en la agenda.
        /// </summary>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha de la agenda.</param>
        /// <returns>Lista de <see cref="TurnoAgendaDto"/>.</returns>
        public List<TurnoAgendaDto> ObtenerTurnosParaAgenda(int idMedico, DateTime fecha)
        {
            return _repo.ObtenerTurnosParaAgenda(idMedico, fecha);
        }

        /// <summary>
        /// Obtiene los contadores (totales, pendientes, etc.) para la agenda de un médico en una fecha dada.
        /// </summary>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha de consulta.</param>
        /// <returns>Objeto <see cref="AgendaContadoresDto"/> con las estadísticas del día.</returns>
        public AgendaContadoresDto ObtenerContadoresAgenda(int idMedico, DateTime fecha)
        {
            return _repo.ObtenerContadoresAgenda(idMedico, fecha);
        }

        /// <summary>
        /// Actualiza el estado de un turno específico.
        /// </summary>
        /// <param name="idTurno">Identificador único del turno.</param>
        /// <param name="idNuevoEstado">ID del nuevo estado a asignar.</param>
        /// <returns><c>true</c> si la actualización fue exitosa; de lo contrario, <c>false</c>.</returns>
        public bool ActualizarEstadoTurno(int idTurno, int idNuevoEstado)
        {
            return _repo.ActualizarEstadoTurno(idTurno, idNuevoEstado);
        }
        /// <summary>
        /// Obtiene un listado completo de todos los turnos registrados.
        /// </summary>
        /// <returns>Lista de <see cref="ListadoTurno"/>.</returns>
        public List<ListadoTurno> ListarTurnos ()
        {
            return _repo.ListadoTurnos();
        }

        /// <summary>
        /// Cuenta la cantidad de turnos en un estado particular. 
        /// Si el estado es "Pendiente", solo cuenta los correspondientes al día de hoy.
        /// </summary>
        /// <param name="estado">Nombre del estado a filtrar.</param>
        /// <returns>Cantidad de turnos que cumplen el criterio.</returns>
        public int CantidadTurnosPorEstado(string estado)
        {
            var turnos = _repo.GetAll().Where(t => t.Estado.ToLower() == estado.ToLower());

            if (estado.ToLower() == "pendiente")
            {
                var hoy = DateTime.Today;
                turnos = turnos.Where(t => t.FechaTurno.Date == hoy);
            }

            return turnos.Count();
        }

        /// <summary>
        /// Cuenta todos los turnos con estado "Pendiente" sin filtrar por fecha.
        /// </summary>
        /// <returns>Cantidad total de turnos pendientes.</returns>
        public int CantidadTurnosPendientes()
        {
            var turnosPendientes = _repo.GetAll()
                                        .Where(t => t.Estado.ToLower() == "pendiente");
            return turnosPendientes.Count();
        }

        /// <summary>
        /// Registra un nuevo turno en el sistema.
        /// </summary>
        /// <param name="turnoDto">DTO con los datos del turno a crear.</param>
        public void RegistrarTurno(TurnoDto turnoDto)
        {
            _repo.Insertar(turnoDto);
        }

        /// <summary>
        /// Actualiza la información de un turno existente.
        /// </summary>
        /// <param name="id_turno">ID del turno a modificar.</param>
        /// <param name="turnoDto">DTO con los nuevos datos.</param>
        public void ActualizarTurno(int id_turno, TurnoDto turnoDto)
        {
            _repo.Actualizar(id_turno, turnoDto);
        }

        /// <summary>
        /// Elimina un turno del sistema por su identificador.
        /// </summary>
        /// <param name="id_turno">ID del turno a eliminar.</param>
        public void EliminarTurno(int id_turno)
        {
            _repo.Eliminar(id_turno);
        }

        /// <summary>
        /// Obtiene la información detallada de un turno específico.
        /// </summary>
        /// <param name="p_id_turno">ID del turno.</param>
        /// <returns>Objeto <see cref="TurnoDto"/> con los detalles.</returns>
        public TurnoDto ObtenerDetalle(int p_id_turno)
        {
            return _repo.ObtenerDetalle(p_id_turno);
        }

        /// <summary>
        /// Obtiene el catálogo de estados de turnos disponibles.
        /// </summary>
        /// <returns>Lista de <see cref="ListadoEstadoTurno"/>.</returns>
        public List<ListadoEstadoTurno> ListadoEstadosTurnos()
        {
            return _repo.ListarEstadosTurno();
        }
        
        /// <summary>
        /// Verifica si ya existe un turno programado para el mismo paciente con el mismo médico en la misma fecha.
        /// </summary>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha a verificar.</param>
        /// <returns><c>true</c> si ya existe un turno; de lo contrario, <c>false</c>.</returns>
        public bool ExisteTurnoMismoDiaMismoMedicoPaciente(int idPaciente, int idMedico, DateTime fecha)
        {
            return _repo.ExisteTurnoMismoDiaMismoMedicoPaciente(idPaciente, idMedico, fecha);
        }

        /// <summary>
        /// Verifica si existe un turno duplicado en el mismo día/médico/paciente, excluyendo un turno específico (útil para validación en edición).
        /// </summary>
        /// <param name="idTurnoExcluir">ID del turno que se está editando y debe ser ignorado en la búsqueda.</param>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha a verificar.</param>
        /// <returns><c>true</c> si existe un duplicado; de lo contrario, <c>false</c>.</returns>
        public bool ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(int idTurnoExcluir, int idPaciente, int idMedico, DateTime fecha)
        {
            return _repo.ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(
                idTurnoExcluir, idPaciente, idMedico, fecha);
        }
    }
}
