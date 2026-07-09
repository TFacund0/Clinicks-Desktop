using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaNegocio.DTOs.CamaDTO;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la gestión de camas dentro de las habitaciones.
    /// </summary>
    public interface ICamaRepository
    {
        /// <summary>
        /// Obtiene el listado de todas las camas registradas con su estado de disponibilidad actual.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarCamaDTO"/>.</returns>
        List<MostrarCamaDTO> GetAll();

        /// <summary>
        /// Registra una nueva cama en una habitación específica.
        /// </summary>
        /// <param name="NroHabitacion">Número de habitación donde se agregará la cama.</param>
        void Insertar(int NroHabitacion);

        /// <summary>
        /// Elimina una cama específica identificada por su habitación y número de cama.
        /// </summary>
        /// <param name="NroHabitacion">Número de habitación.</param>
        /// <param name="NroCama">Número de cama.</param>
        void Eliminar(int NroHabitacion, int NroCama);

        /// <summary>
        /// Actualiza el estado de disponibilidad de una cama.
        /// </summary>
        /// <param name="NroHabitacion">Número de habitación.</param>
        /// <param name="NroCama">Número de cama.</param>
        /// <param name="NuevoEstadoId">ID del nuevo estado (ej. Disponible, Ocupada, Mantenimiento).</param>
        void CambiarEstado(int NroHabitacion, int NroCama, int NuevoEstadoId);

        /// <summary>
        /// Obtiene el catálogo de estados posibles de una cama.
        /// </summary>
        /// <returns>Lista de entidades <see cref="estado_cama"/>.</returns>
        List<estado_cama> GetEstadosCama();
    }
}
