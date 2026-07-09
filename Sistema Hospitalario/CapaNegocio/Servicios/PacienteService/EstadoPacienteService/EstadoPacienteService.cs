using System;
using System.Collections.Generic;
using System.Linq;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.PacienteDTO;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService
{
    /// <summary>
    /// Servicio que gestiona el catálogo de estados clínicos en los que puede encontrarse un paciente.
    /// Proporciona acceso a los estados definidos (ej. Activo, Internado, Alta).
    /// </summary>
    public class EstadoPacienteService
    {
        private readonly IPacienteRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EstadoPacienteService"/> con el repositorio por defecto.
        /// </summary>
        public EstadoPacienteService() : this(new PacienteRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EstadoPacienteService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de pacientes.</param>
        public EstadoPacienteService(IPacienteRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el listado completo de estados de paciente, ordenados alfabéticamente por nombre.
        /// </summary>
        /// <returns>Lista de <see cref="EstadoPacienteDto"/>.</returns>
        public List<EstadoPacienteDto> ListarEstados()
        {
            var listaEstados = _repo.GetEstados();
            
            return listaEstados.OrderBy(e => e.Nombre).ToList();
        }
    }
}
