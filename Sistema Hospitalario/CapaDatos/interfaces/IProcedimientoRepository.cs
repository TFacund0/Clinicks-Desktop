using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión del catálogo de procedimientos médicos.
    /// </summary>
    public interface IProcedimientoRepository
    {
        /// <summary>
        /// Obtiene el listado completo de procedimientos médicos disponibles en el sistema.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarProcedimientoDto"/>.</returns>
        List<MostrarProcedimientoDto> GetAll();

        /// <summary>
        /// Registra un nuevo tipo de procedimiento médico.
        /// </summary>
        /// <param name="nombre">Nombre descriptivo del procedimiento.</param>
        void Insertar(string nombre);

        /// <summary>
        /// Elimina un procedimiento médico del sistema buscando por su nombre.
        /// </summary>
        /// <param name="nombre">Nombre del procedimiento a eliminar.</param>
        void Eliminar(string nombre);

        /// <summary>
        /// Obtiene el listado de procedimientos en formato simple (ID y nombre).
        /// </summary>
        /// <returns>Lista de <see cref="ProcedimientoDto"/>.</returns>
        List<ProcedimientoDto> ListarProcedimientos();
    }
}
