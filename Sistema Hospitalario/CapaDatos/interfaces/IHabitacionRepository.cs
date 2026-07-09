using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaNegocio.DTOs.Habitaciones;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión de la infraestructura de habitaciones.
    /// </summary>
    public interface IHabitacionRepository
    {
        /// <summary>
        /// Obtiene el listado completo de habitaciones con sus detalles de piso y tipo.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarHabitacionDto"/>.</returns>
        List<MostrarHabitacionDto> GetAll();

        /// <summary>
        /// Crea una nueva habitación en el sistema.
        /// </summary>
        /// <param name="NroPiso">Número de piso donde se ubica.</param>
        /// <param name="tipoHabitacion">ID del tipo de habitación (ej. General, Terapia).</param>
        void Insertar(int NroPiso, int tipoHabitacion);

        /// <summary>
        /// Elimina una habitación específica identificada por su piso y número.
        /// </summary>
        /// <param name="nroPiso">Número de piso.</param>
        /// <param name="NroHabitacion">Número de la habitación.</param>
        void Eliminar(int nroPiso, int NroHabitacion);

        /// <summary>
        /// Obtiene el catálogo de tipos de habitación disponibles.
        /// </summary>
        /// <returns>Lista de <see cref="TiposHabitacionDto"/>.</returns>
        List<TiposHabitacionDto> ListarTiposHabitacion();
    }
}
