using Sistema_Hospitalario.CapaNegocio.DTOs.Historiales;
using Sistema_Hospitalario.CapaNegocio.DTOs.Consultas;
using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión de médicos, consultas e historial clínico.
    /// </summary>
    public interface IMedicoRepository
    {
        /// <summary>
        /// Inserta un nuevo registro de médico en el sistema.
        /// </summary>
        /// <param name="nombre">Nombre del médico.</param>
        /// <param name="apellido">Apellido del médico.</param>
        /// <param name="dni">DNI del médico.</param>
        /// <param name="direccion">Dirección de residencia.</param>
        /// <param name="matricula">Matrícula profesional.</param>
        /// <param name="correo">Correo electrónico.</param>
        /// <param name="idEspecialidad">ID de la especialidad asociada.</param>
        /// <returns>Tupla con éxito, ID generado y error si aplica.</returns>
        (bool Ok, int IdGenerado, string Error) Insertar(string nombre, string apellido, string dni, string direccion, string matricula, string correo, int idEspecialidad);
        /// <summary>
        /// Elimina un médico del sistema por su ID.
        /// </summary>
        /// <param name="idMedico">ID del médico.</param>
        void Eliminar(int idMedico);
        /// <summary>
        /// Obtiene la lista de médicos formateada para visualización en grillas.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarMedicoDto"/>.</returns>
        List<MostrarMedicoDto> ObtenerMedicos();
        /// <summary>
        /// Obtiene un listado básico de médicos.
        /// </summary>
        /// <returns>Lista de <see cref="MedicoDto"/>.</returns>
        List<MedicoDto> ListarMedicos();
        /// <summary>
        /// Registra una nueva consulta médica en la base de datos.
        /// </summary>
        /// <param name="consulta">DTO con los datos de la consulta.</param>
        /// <param name="idMedico">ID del médico que realiza la consulta.</param>
        /// <param name="idPaciente">ID del paciente atendido.</param>
        void InsertarConsulta(ConsultaAltaDto consulta, int idMedico, int idPaciente);
        /// <summary>
        /// Busca el ID de un paciente a partir de su número de DNI.
        /// </summary>
        /// <param name="dni">DNI del paciente.</param>
        /// <returns>El ID del paciente, o <c>null</c> si no existe.</returns>
        int? ObtenerIdPacientePorDni(int dni);
        /// <summary>
        /// Obtiene la lista de pacientes asociados a los procesos de un médico, filtrable por fecha de turno.
        /// </summary>
        /// <param name="fechaTurno">Fecha opcional para filtrar los pacientes atendidos.</param>
        /// <returns>Lista de <see cref="PacienteListadoMedicoDto"/>.</returns>
        List<PacienteListadoMedicoDto> ObtenerTodosParaMedico(DateTime? fechaTurno);
        /// <summary>
        /// Obtiene la cantidad total de pacientes únicos registrados en el sistema.
        /// </summary>
        /// <returns>Conteo total de pacientes.</returns>
        int ContarTotalPacientes();
        /// <summary>
        /// Recupera el historial de consultas registradas para un paciente.
        /// </summary>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <returns>Lista de ítems de historial de tipo consulta.</returns>
        List<HistorialItemDto> ObtenerHistorialConsultas(int idPaciente);

        /// <summary>
        /// Recupera el historial de internaciones registradas para un paciente.
        /// </summary>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <returns>Lista de ítems de historial de tipo internación.</returns>
        List<HistorialItemDto> ObtenerHistorialInternaciones(int idPaciente);
        /// <summary>
        /// Recupera el historial de turnos programados para un paciente.
        /// </summary>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <returns>Lista de ítems de historial de tipo turno.</returns>
        List<HistorialItemDto> ObtenerHistorialTurnos(int idPaciente);
    }
}
