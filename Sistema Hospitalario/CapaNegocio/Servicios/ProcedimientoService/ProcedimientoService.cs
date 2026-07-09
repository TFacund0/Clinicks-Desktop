using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.ProcedimientoService
{
    /// <summary>
    /// Servicio que gestiona el catálogo de procedimientos médicos del hospital.
    /// Permite la administración (creación, eliminación) y consulta de los diversos tipos de procedimientos.
    /// </summary>
    public class ProcedimientoService
    {
        private readonly IProcedimientoRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProcedimientoService"/> con el repositorio por defecto.
        /// </summary>
        public ProcedimientoService() : this(new ProcedimientoRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProcedimientoService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de procedimientos.</param>
        public ProcedimientoService(IProcedimientoRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el listado de todos los procedimientos registrados para su visualización.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarProcedimientoDto"/>.</returns>
        public List<MostrarProcedimientoDto> ObtenerProcedimientos()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Registra un nuevo tipo de procedimiento médico.
        /// </summary>
        /// <param name="nombre">Nombre descriptivo del procedimiento.</param>
        /// <exception cref="ArgumentException">Se lanza si el nombre es nulo o está vacío.</exception>
        public void AgregarProcedimiento(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.");

            _repo.Insertar(nombre);
        }

        /// <summary>
        /// Elimina un procedimiento médico del catálogo buscando por su nombre.
        /// </summary>
        /// <param name="nombre">Nombre del procedimiento a eliminar.</param>
        public void EliminarProcedimiento(string nombre)
        {
            _repo.Eliminar(nombre);
        }

        /// <summary>
        /// Obtiene una lista simplificada de todos los procedimientos registrados.
        /// </summary>
        /// <returns>Lista de <see cref="ProcedimientoDto"/>.</returns>
        public List<ProcedimientoDto> ListarProcedimientos() { 
           return _repo.ListarProcedimientos();
        }
    }
}
