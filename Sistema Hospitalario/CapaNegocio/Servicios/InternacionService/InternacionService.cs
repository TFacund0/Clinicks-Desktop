using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.InternacionService
{
    /// <summary>
    /// Servicio que gestiona el ciclo de vida de las internaciones de pacientes.
    /// Incluye el registro de ingresos, seguimiento por procedimiento y finalización de internaciones (egresos).
    /// </summary>
    public class InternacionService
    {
        private readonly IInternacionRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InternacionService"/> con el repositorio por defecto.
        /// </summary>
        public InternacionService() : this(new InternacionRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InternacionService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de internaciones.</param>
        public InternacionService(IInternacionRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el listado completo de todas las internaciones registradas.
        /// </summary>
        /// <returns>Lista de <see cref="InternacionDto"/>.</returns>
        public List<InternacionDto> ListadoInternacionDtos()
        {
            var listadoInternaciones = _repo.GetAll();
            return listadoInternaciones;
        }

        /// <summary>
        /// Registra un nuevo ingreso de internación en el sistema.
        /// </summary>
        /// <param name="dto">DTO con los datos de la internación a crear.</param>
        public void AltaInternacion(InternacionDto dto)
        {
            _repo.Insertar(dto);
        }

        /// <summary>
        /// Cuenta cuántas internaciones están asociadas a un procedimiento médico específico.
        /// </summary>
        /// <param name="id_procedimiento">ID del procedimiento a consultar.</param>
        /// <returns>Número total de internaciones encontradas.</returns>
        public int TotalInternacionesXProcedimiento(int id_procedimiento)
        {
            var totalInternaciones = _repo.GetAll()
                .Count(i => i.Id_procedimiento == id_procedimiento);
            return totalInternaciones;
        }

        /// <summary>
        /// Finaliza un proceso de internación registrando la fecha y diagnóstico de egreso.
        /// Realiza validaciones de coherencia de fechas y datos obligatorios.
        /// </summary>
        /// <param name="dto">DTO con los datos de finalización.</param>
        /// <exception cref="InvalidOperationException">
        /// Se lanza si la fecha de egreso es anterior a la de ingreso o si falta el diagnóstico.
        /// </exception>
        public void FinalizarInternacion(FinalizarInternacionDto dto)
        {
            // 🔹 Validaciones de negocio
            if (dto.FechaEgreso < dto.FechaIngreso)
                throw new InvalidOperationException("La fecha de egreso no puede ser anterior a la fecha de ingreso.");

            if (string.IsNullOrWhiteSpace(dto.DiagnosticoEgreso))
                throw new InvalidOperationException("Debe ingresar un diagnóstico de egreso.");

            // 🔹 Delegamos al repositorio la actualización en BD
            _repo.FinalizarInternacion(dto);
        }
    }
}
