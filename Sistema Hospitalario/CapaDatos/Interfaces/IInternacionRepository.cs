using Sistema_Hospitalario.CapaNegocio.DTOs.HistorialDTO;
using Sistema_Hospitalario.CapaNegocio.DTOs.InternacionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión de internaciones de pacientes.
    /// </summary>
    public interface IInternacionRepository
    {
        /// <summary>
        /// Obtiene el listado de todas las internaciones registradas.
        /// </summary>
        /// <returns>Lista de <see cref="InternacionDto"/>.</returns>
        List<InternacionDto> GetAll();

        /// <summary>
        /// Registra el inicio de una nueva internación para un paciente.
        /// </summary>
        /// <param name="internacion">DTO con los datos de la internación.</param>
        void Insertar(InternacionDto internacion);

        /// <summary>
        /// Elimina el registro de una internación por su ID.
        /// </summary>
        /// <param name="id_internacion">ID de la internación.</param>
        void Eliminar(int id_internacion);

        /// <summary>
        /// Actualiza los datos de una internación existente (ej. fecha de fin, motivo).
        /// </summary>
        /// <param name="id_internacion">ID de la internación a modificar.</param>
        /// <param name="internacion">DTO con los datos actualizados.</param>
        void Actualizar(int id_internacion, InternacionDto internacion);

        /// <summary>
        /// Finaliza una internación registrando el egreso del paciente y liberando la cama asignada.
        /// </summary>
        /// <param name="dto">DTO con los datos del egreso.</param>
        void FinalizarInternacion(FinalizarInternacionDto dto);
    }
}
