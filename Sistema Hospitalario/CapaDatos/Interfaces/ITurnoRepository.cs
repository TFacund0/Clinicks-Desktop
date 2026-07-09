using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaNegocio.DTOs.TurnoDTO;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión de turnos médicos y agenda.
    /// </summary>
    public interface ITurnoRepository
    {
        /// <summary>
        /// Recupera todos los turnos registrados en el sistema.
        /// </summary>
        /// <returns>Lista de <see cref="TurnoDto"/>.</returns>
        List<TurnoDto> GetAll();
        /// <summary>
        /// Registra un nuevo turno en la base de datos.
        /// </summary>
        /// <param name="turno">DTO con la información del turno a crear.</param>
        void Insertar(TurnoDto turno);
        /// <summary>
        /// Actualiza la información de un turno existente.
        /// </summary>
        /// <param name="id_turno">Identificador único del turno.</param>
        /// <param name="turno">DTO con los nuevos datos del turno.</param>
        void Actualizar(int id_turno, TurnoDto turno);
        /// <summary>
        /// Elimina un registro de turno del sistema.
        /// </summary>
        /// <param name="id_turno">Identificador único del turno.</param>
        void Eliminar(int id_turno);

        /// <summary>
        /// Obtiene los turnos programados para un médico en una fecha específica, optimizado para la vista de agenda.
        /// </summary>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha de la agenda.</param>
        /// <returns>Lista de <see cref="TurnoAgendaDto"/>.</returns>
        List<TurnoAgendaDto> ObtenerTurnosParaAgenda(int idMedico, DateTime fecha);
        /// <summary>
        /// Obtiene el conteo consolidado de estados de turnos para un médico en un día específico.
        /// </summary>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha de la agenda.</param>
        /// <returns>Objeto <see cref="AgendaContadoresDto"/> con los totales.</returns>
        AgendaContadoresDto ObtenerContadoresAgenda(int idMedico, DateTime fecha);
        /// <summary>
        /// Cambia el estado de un turno (ej. de Pendiente a Atendido).
        /// </summary>
        /// <param name="idTurno">ID del turno.</param>
        /// <param name="idNuevoEstado">ID del nuevo estado.</param>
        /// <returns>True si la actualización fue exitosa.</returns>
        bool ActualizarEstadoTurno(int idTurno, int idNuevoEstado);
        /// <summary>
        /// Verifica si ya existe un turno programado para el mismo paciente, médico y día.
        /// </summary>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha a verificar.</param>
        /// <returns>True si existe una colisión de horario.</returns>
        bool ExisteTurnoMismoDiaMismoMedicoPaciente(int idPaciente, int idMedico, DateTime fecha);
        /// <summary>
        /// Verifica colisión de turnos excluyendo un ID específico (útil para validación en ediciones).
        /// </summary>
        /// <param name="idTurnoExcluir">ID del turno que se está editando.</param>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <param name="idMedico">ID del médico.</param>
        /// <param name="fecha">Fecha a verificar.</param>
        /// <returns>True si existe una colisión.</returns>
        bool ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(int idTurnoExcluir, int idPaciente, int idMedico, DateTime fecha);

        /// <summary>
        /// Obtiene el listado general de turnos para su visualización en grillas.
        /// </summary>
        /// <returns>Lista de <see cref="ListadoTurno"/>.</returns>
        List<ListadoTurno> ListadoTurnos();

        /// <summary>
        /// Obtiene el detalle completo de un turno específico.
        /// </summary>
        /// <param name="id_turno">Identificador único del turno.</param>
        /// <returns>Objeto <see cref="TurnoDto"/> con el detalle.</returns>
        TurnoDto ObtenerDetalle(int id_turno);

        /// <summary>
        /// Obtiene el catálogo de estados posibles de un turno.
        /// </summary>
        /// <returns>Lista de <see cref="ListadoEstadoTurno"/>.</returns>
        List<ListadoEstadoTurno> ListarEstadosTurno();
    }
}
