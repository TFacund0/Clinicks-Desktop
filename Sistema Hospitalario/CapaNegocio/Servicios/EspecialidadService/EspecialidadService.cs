using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.Especialidades;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.EspecialidadService
{
    /// <summary>
    /// Servicio que gestiona el catálogo de especialidades médicas disponibles en el hospital.
    /// Proporciona métodos para listar, agregar y eliminar especialidades.
    /// </summary>
    public class EspecialidadService
    {
        private readonly IEspecialidadRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EspecialidadService"/> con el repositorio por defecto.
        /// </summary>
        public EspecialidadService() : this(new EspecialidadRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EspecialidadService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de especialidades.</param>
        public EspecialidadService(IEspecialidadRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el listado completo de todas las especialidades médicas registradas.
        /// </summary>
        /// <returns>Lista de <see cref="EspecialidadDto"/>.</returns>
        public List<EspecialidadDto> ObtenerEspecialidades()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Registra una nueva especialidad médica en el catálogo.
        /// </summary>
        /// <param name="nombre">Nombre de la especialidad (ej. Cardiología, Pediatría).</param>
        /// <exception cref="ArgumentException">Se lanza si el nombre es nulo o está vacío.</exception>
        public void AgregarEspecialidad(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            _repo.Insertar(nombre);
        }

        /// <summary>
        /// Elimina una especialidad médica del sistema buscando por su nombre exacto.
        /// </summary>
        /// <param name="nombre">Nombre de la especialidad a eliminar.</param>
        public void EliminarEspecialidad(string nombre)
        {
            _repo.Eliminar(nombre);
        }
    }
}
