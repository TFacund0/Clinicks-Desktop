using Sistema_Hospitalario.CapaDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaNegocio.DTOs.HabitacionDTO;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService
{
    /// <summary>
    /// Servicio que gestiona la infraestructura de habitaciones del hospital.
    /// Permite administrar la creación, eliminación y consulta de habitaciones por piso y tipo.
    /// </summary>
    public class HabitacionService
    {
        private readonly IHabitacionRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HabitacionService"/> con el repositorio por defecto.
        /// </summary>
        public HabitacionService() : this(new HabitacionRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HabitacionService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de habitaciones.</param>
        public HabitacionService(IHabitacionRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el listado completo de todas las habitaciones con su información detallada.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarHabitacionDTO"/>.</returns>
        public List<MostrarHabitacionDTO> ObtenerHabitaciones()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Registra una nueva habitación en el sistema.
        /// </summary>
        /// <param name="nroPiso">Número de piso donde se ubica la habitación.</param>
        /// <param name="tipoHabitacion">ID del tipo de habitación (ej. individual, compartida).</param>
        /// <exception cref="ArgumentException">Se lanza si el número de piso es negativo.</exception>
        public void AgregarHabitacion(int nroPiso, int tipoHabitacion)
        {
            if (nroPiso < 0)
                throw new ArgumentException("El piso debe ser mayor o igual a 0.");

            _repo.Insertar(nroPiso, tipoHabitacion);
        }

        /// <summary>
        /// Elimina una habitación específica identificada por su ubicación.
        /// </summary>
        /// <param name="nroPiso">Número de piso.</param>
        /// <param name="nroHabitacion">Número de habitación.</param>
        public void EliminarHabitacion(int nroPiso, int nroHabitacion)
        {
            _repo.Eliminar(nroPiso, nroHabitacion);
        }

        /// <summary>
        /// Obtiene el catálogo de tipos de habitaciones disponibles.
        /// </summary>
        /// <returns>Lista de <see cref="TiposHabitacionDTO"/>.</returns>
        public List<TiposHabitacionDTO> ListarTiposHabitacion()
        {
            return _repo.ListarTiposHabitacion();
        }

        /// <summary>
        /// Obtiene la cantidad total de habitaciones registradas en el hospital.
        /// </summary>
        /// <returns>Total de habitaciones.</returns>
        public int TotalHabitaciones()
        {
            var totalHabitaciones = _repo.GetAll().Count;
            return totalHabitaciones;
        }

        /// <summary>
        /// Obtiene la lista de habitaciones filtradas por un piso específico, proporcionado como texto.
        /// </summary>
        /// <param name="pisoTexto">Cadena de texto que representa el número de piso.</param>
        /// <returns>Lista de <see cref="HabitacionDto"/> del piso indicado, o lista vacía si el formato es inválido.</returns>
        public List<HabitacionDto> ListarHabitacionesXPiso(string pisoTexto)
        {
            var habitaciones = new List<HabitacionDto>();

            // Validar que el texto sea numérico y mayor a 0
            if (string.IsNullOrWhiteSpace(pisoTexto) || !int.TryParse(pisoTexto, out var piso) || piso <= 0)
            {
                return habitaciones; // devuelve lista vacía
            }

            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                habitaciones = db.habitacion
                    .Where(h => h.nro_piso == piso)
                    .OrderBy(h => h.nro_habitacion)
                    .Select(h => new HabitacionDto
                    {
                        Nro_habitacion = h.nro_habitacion,
                        Nro_piso = h.nro_piso,
                        Tipo_habitacion = h.tipo_habitacion.nombre
                    })
                    .ToList();
            }

            return habitaciones;
        }
    }
}