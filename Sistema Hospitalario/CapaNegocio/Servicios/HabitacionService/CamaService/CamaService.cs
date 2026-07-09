using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.CamaDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService.CamaService
{
    /// <summary>
    /// Servicio que gestiona las camas dentro de las habitaciones del hospital.
    /// Permite administrar la disponibilidad, estados y asignación de camas a habitaciones.
    /// </summary>
    public class CamaService
    {
        private readonly ICamaRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CamaService"/> con el repositorio por defecto.
        /// </summary>
        public CamaService() : this(new CamaRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CamaService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de camas.</param>
        public CamaService(ICamaRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el listado completo de todas las camas registradas en el sistema.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarCamaDTO"/>.</returns>
        public List<MostrarCamaDTO> ObtenerCamas()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Agrega una nueva cama a una habitación específica.
        /// </summary>
        /// <param name="nroHabitacion">Número de habitación donde se agregará la cama.</param>
        /// <exception cref="ArgumentException">Se lanza si el número de habitación es negativo.</exception>
        public void AgregarCama(int nroHabitacion)
        {
            if (nroHabitacion < 0)
                throw new ArgumentException("el numero de habitacion debe ser positivo");

            _repo.Insertar(nroHabitacion);
        }

        /// <summary>
        /// Elimina una cama específica de una habitación.
        /// </summary>
        /// <param name="nroHabitacion">Número de habitación.</param>
        /// <param name="nroCama">Número de cama a eliminar.</param>
        public void EliminarCama(int nroHabitacion, int nroCama)
        {
            _repo.Eliminar(nroHabitacion, nroCama);
        }

        /// <summary>
        /// Cambia el estado de disponibilidad de una cama específica.
        /// </summary>
        /// <param name="nroHabitacion">Número de habitación.</param>
        /// <param name="idCama">Identificador único de la cama.</param>
        /// <param name="nuevoEstadoId">ID del nuevo estado (ej. Disponible, Ocupada, Mantenimiento).</param>
        public void CambiarEstado(int nroHabitacion, int idCama, int nuevoEstadoId)
        {   
            _repo.CambiarEstado(nroHabitacion, idCama, nuevoEstadoId);
        }

        /// <summary>
        /// Obtiene la cantidad total de camas registradas en el sistema.
        /// </summary>
        /// <returns>Total de camas.</returns>
        public int TotalCamas()
        {
            var totalCamas = _repo.GetAll().Count;
            return totalCamas;
        }

        /// <summary>
        /// Cuenta la cantidad de camas que se encuentran en un estado de disponibilidad específico.
        /// </summary>
        /// <param name="p_nombre_estado">Nombre del estado a filtrar (ej. "disponible").</param>
        /// <returns>Número de camas en dicho estado.</returns>
        public int TotalCamasXEstado(string p_nombre_estado)
        {
            var totalCamasXEstado = _repo.GetAll().Count(c => c.Estado.ToLower() == p_nombre_estado.ToLower());
            return totalCamasXEstado;
        }

        /// <summary>
        /// Obtiene la lista de camas pertenecientes a una habitación específica.
        /// </summary>
        /// <param name="p_nroHabitacion">Número de habitación (como cadena).</param>
        /// <returns>Lista de <see cref="CamaDto"/> de la habitación indicada.</returns>
        public List<CamaDto> ListarCamasXHabitacion(string p_nroHabitacion)
        {
            var camasList = _repo.GetAll();
            
            return camasList
                .Where(c => c.NroHabitacion.ToString() == p_nroHabitacion)
                .Select(c => new CamaDto
                {
                    NroCama = c.NroCama,
                    NroHabitacion = c.NroHabitacion,
                    IdEstadoCama = c.IdEstadoCama,
                    EstadoCama = c.Estado
                }).ToList();
        }

        /// <summary>
        /// Obtiene el catálogo de todos los estados de cama posibles (disponibilidad).
        /// </summary>
        /// <returns>Lista de <see cref="CamaDto"/> representando los estados disponibles.</returns>
        public List<CamaDto> ListarEstadosCama()
        {
            var camasList = _repo.GetEstadosCama();
            return camasList
                .Select(e => new CamaDto
                {
                    IdEstadoCama = e.id_estado_cama,
                    EstadoCama = e.disponibilidad
                }).ToList();
        }
    }
}
