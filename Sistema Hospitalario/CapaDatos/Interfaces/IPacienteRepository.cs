using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la entidad Paciente.
    /// </summary>
    public interface IPacienteRepository
    {
        /// <summary>
        /// Recupera la lista completa de pacientes desde la base de datos.
        /// </summary>
        /// <returns>Lista de <see cref="PacienteDto"/>.</returns>
        List<PacienteDto> GetAll();

        /// <summary>
        /// Obtiene el catálogo de estados posibles para los pacientes (Activo, Internado, etc.).
        /// </summary>
        /// <returns>Lista de <see cref="EstadoPacienteDto"/>.</returns>
        List<EstadoPacienteDto> GetEstados();

        /// <summary>
        /// Inserta un nuevo registro de paciente en la base de datos.
        /// </summary>
        /// <param name="paciente">DTO con los datos del paciente a insertar.</param>
        void Insertar(PacienteDto paciente);

        /// <summary>
        /// Elimina un paciente de la base de datos por su ID.
        /// </summary>
        /// <param name="id_paciente">ID del paciente a eliminar.</param>
        void Eliminar(int id_paciente);

        /// <summary>
        /// Actualiza la información de un paciente existente.
        /// </summary>
        /// <param name="id_paciente">ID del paciente a modificar.</param>
        /// <param name="paciente">DTO con los nuevos datos actualizados.</param>
        void Actualizar(int id_paciente, PacienteDto paciente);
    }
}
