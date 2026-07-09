using Sistema_Hospitalario.CapaDatos;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.ConsultaDTO;
using Sistema_Hospitalario.CapaNegocio.DTOs.HistorialDTO;
using Sistema_Hospitalario.CapaNegocio.DTOs.MedicoDTO;
using Sistema_Hospitalario.CapaNegocio.DTOs.moderDTO;
using Sistema_Hospitalario.CapaNegocio.DTOs.PacienteDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService
{
    /// <summary>
    /// Servicio que gestiona la lógica de negocio para los médicos.
    /// Proporciona funcionalidades para la gestión de médicos, consultas, historial de pacientes y listados.
    /// </summary>
    public class MedicoService
    {
        private readonly IMedicoRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MedicoService"/> con el repositorio por defecto.
        /// </summary>
        public MedicoService() : this(new MedicoRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MedicoService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de médicos.</param>
        public MedicoService(IMedicoRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene el número total de pacientes registrados en el sistema.
        /// </summary>
        /// <returns>Cantidad total de pacientes.</returns>
        public int ObtenerConteoTotalPacientes()
        {
            return _repo.ContarTotalPacientes();
        }

        /// <summary>
        /// Obtiene una lista de pacientes filtrada por nombre, apellido, DNI y opcionalmente por fecha de turno.
        /// </summary>
        /// <param name="nombre">Filtro por nombre de paciente.</param>
        /// <param name="apellido">Filtro por apellido de paciente.</param>
        /// <param name="dni">Filtro por DNI (comienza con).</param>
        /// <param name="fechaTurno">Filtro opcional por fecha de turno.</param>
        /// <returns>Lista de <see cref="PacienteListadoMedicoDto"/> ordenada por apellido y nombre.</returns>
        public List<PacienteListadoMedicoDto> ObtenerPacientes(string nombre, string apellido, string dni, DateTime? fechaTurno)
        {
            // 1. Obtenemos la lista "maestra" completa
            var listaMaestra = _repo.ObtenerTodosParaMedico(fechaTurno);

            // 2. Aplicamos filtros en memoria (LINQ)

            if (!string.IsNullOrEmpty(nombre))
            {
                listaMaestra = listaMaestra.Where(p => p.Nombre.ToLower().Contains(nombre.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(apellido))
            {
                listaMaestra = listaMaestra.Where(p => p.Apellido.ToLower().Contains(apellido.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(dni))
            {
                listaMaestra = listaMaestra.Where(p => p.Dni.StartsWith(dni)).ToList();
            }

            // (Aquí iría el filtro por Fecha Ultimo Turno si lo implementamos)

            return listaMaestra.OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToList();
        }

        /// <summary>
        /// Registra una nueva consulta médica para un paciente.
        /// </summary>
        /// <param name="dto">DTO con los datos de la consulta.</param>
        /// <param name="idMedicoLogueado">ID del médico que realiza la consulta.</param>
        /// <returns>Una tupla con el estado de éxito y un mensaje de error si falla.</returns>
        public (bool Ok, string Error) RegistrarConsulta(ConsultaAltaDTO dto, int idMedicoLogueado)
        {
            try
            {
                // --- VERIFICACIONES ---
                if (string.IsNullOrWhiteSpace(dto.DniPaciente))
                    return (false, "El DNI del paciente es obligatorio.");

                if (string.IsNullOrWhiteSpace(dto.Motivo))
                    return (false, "El Motivo de la consulta es obligatorio.");

                // Convertimos el DNI de string a int para buscar
                if (!int.TryParse(dto.DniPaciente, out int dniPacienteNum))
                    return (false, "El formato del DNI es incorrecto (debe ser numérico).");

                // Buscamos al paciente en la BD
                paciente pacienteEncontrado;
                using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
                {
                    // Usamos 'FirstOrDefault' que es seguro
                    pacienteEncontrado = db.paciente.FirstOrDefault(p => p.dni == dniPacienteNum);
                }

                // Verificación 1: ¿Existe el paciente?
                if (pacienteEncontrado == null)
                {
                    return (false, $"No se encontró ningún paciente con el DNI {dto.DniPaciente}.");
                }

                // --- CREACIÓN DEL OBJETO ---
                var nuevaConsulta = new Sistema_Hospitalario.CapaDatos.Consulta
                {
                    motivo = dto.Motivo,
                    diagnostico = dto.Diagnostico,
                    tratamiento = dto.Tratamiento,
                    fecha_consulta = dto.Fecha,

                    // Asignamos las llaves
                    id_medico = idMedicoLogueado,
                    id_paciente = pacienteEncontrado.id_paciente
                };

                // --- GUARDADO ---
                _repo.InsertarConsulta(nuevaConsulta);

                return (true, null); // ¡Éxito!
            }
            catch (Exception ex)
            {
                return (false, "Error inesperado: " + ex.Message);
            }
        }
        /// <summary>
        /// Obtiene y filtra la lista de médicos según un campo y valor específicos.
        /// </summary>
        /// <param name="campo">Nombre del campo por el cual filtrar (Nombre, Apellido, DNI, etc.).</param>
        /// <param name="valor">Valor a buscar en el campo especificado.</param>
        /// <returns>Lista de médicos filtrada y ordenada según el campo especificado.</returns>
        public List<MostrarMedicoDTO> ObtenerMedicos(string campo = null, string valor = null)
        {
            var listaCompleta = _repo.ObtenerMedicos();

            List<MostrarMedicoDTO> resultado;

            // FILTRAMOS SI HAY VALOR
            if (!string.IsNullOrEmpty(valor))
            {
                string valorLower = valor.ToLower();
                switch (campo)
                {
                    case "Nombre":
                        resultado = listaCompleta.Where(m => m.Nombre.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "Apellido":
                        resultado = listaCompleta.Where(m => m.Apellido.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "DNI":
                        resultado = listaCompleta.Where(m => m.DNI.StartsWith(valor)).ToList();
                        break;
                    case "Direccion":
                        resultado = listaCompleta.Where(m => m.Direccion.StartsWith(valor)).ToList();
                        break;
                    case "Matricula":
                        resultado = listaCompleta.Where(m => m.Matricula.StartsWith(valor)).ToList();
                        break;
                    case "Correo":
                        resultado = listaCompleta.Where(m => m.Correo.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "Especialidad":
                        resultado = listaCompleta.Where(m => m.Especialidad.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    default: //IdMedico
                        resultado = listaCompleta.Where(m => m.IdMedico.ToString() == valor).ToList();
                        break;
                }
            }
            else
            {

                resultado = listaCompleta;
            }

            // ordenamiento
            if (string.IsNullOrEmpty(valor))
            {
                switch (campo)
                {
                    case "Nombre":
                        resultado = resultado.OrderBy(m => m.Nombre).ToList();
                        break;
                    case "Apellido":
                        resultado = resultado.OrderBy(m => m.Apellido).ToList();
                        break;
                    case "DNI":
                        resultado = resultado.OrderBy(m => m.DNI).ToList();
                        break;
                    case "Direccion":
                        resultado = resultado.OrderBy(m => m.Direccion).ToList();
                        break;
                    case "Matricula":
                        resultado = resultado.OrderBy(m => m.Matricula).ToList();
                        break;
                    case "Correo":
                        resultado = resultado.OrderBy(m => m.Correo).ToList();
                        break;
                    case "Especialidad":
                        resultado = resultado.OrderBy(m => m.Especialidad).ToList();
                        break;
                    default: // Orden por defecto (IdMedico)
                        resultado = resultado.OrderBy(m => m.IdMedico).ToList();
                        break;
                }
            }

            return resultado;
        }

        /// <summary>
        /// Agrega un nuevo médico al sistema.
        /// </summary>
        /// <param name="nombre">Nombre del médico.</param>
        /// <param name="apellido">Apellido del médico.</param>
        /// <param name="dni">DNI del médico.</param>
        /// <param name="direccion">Dirección de residencia.</param>
        /// <param name="matricula">Número de matrícula profesional.</param>
        /// <param name="correo">Correo electrónico.</param>
        /// <param name="idEspecialidad">Identificador de la especialidad médica.</param>
        /// <exception cref="System.Exception">Se lanza si el repositorio reporta un error en la inserción.</exception>
        public void AgregarMedico(string nombre, string apellido, string dni, string direccion, string matricula, string correo, int idEspecialidad)
        {
            var (Ok, _, Error) = _repo.Insertar(nombre, apellido, dni, direccion, matricula, correo, idEspecialidad);
            if (!Ok)
            {
                throw new System.Exception(Error);
            }
        }

        /// <summary>
        /// Elimina un médico del sistema por su ID.
        /// </summary>
        /// <param name="idMedico">Identificador único del médico.</param>
        public void EliminarMedico(int idMedico)
        {
            _repo.Eliminar(idMedico);
        }
        /// <summary>
        /// Obtiene un listado básico de todos los médicos.
        /// </summary>
        /// <returns>Lista de <see cref="MedicoDto"/>.</returns>
        public List<MedicoDto> ListarMedicos()
        {
            return _repo.ListarMedicos();
        }

        /// <summary>
        /// Obtiene una lista simplificada de médicos optimizada para mostrar en controles de selección (ComboBox).
        /// </summary>
        /// <returns>Lista de <see cref="MedicoSimpleDTO"/> con nombre formateado y DNI.</returns>
        public List<MedicoSimpleDTO> ObtenerMedicosParaComboBox()
        {
            var todosLosMedicos = _repo.ObtenerMedicos();

            var resultado = todosLosMedicos
                            .OrderBy(m => m.Apellido).ThenBy(m => m.Nombre).ThenBy(m => m.DNI)
                            .Select(m => new MedicoSimpleDTO
                            {
                                Id = m.IdMedico,
                                NombreCompletoYDNI = $"{m.Apellido}, {m.Nombre} ({m.DNI}) esp:{m.Especialidad}"
                            })
                            .ToList();
            return resultado;
        }

        /// <summary>
        /// Obtiene el historial médico consolidado de un paciente, incluyendo consultas, internaciones y turnos.
        /// </summary>
        /// <param name="idPaciente">ID del paciente.</param>
        /// <param name="IdMedico">ID del médico solicitante (usado para control de acceso si aplica).</param>
        /// <returns>Lista de <see cref="HistorialItemDto"/> ordenada cronológicamente en forma descendente.</returns>
        public List<HistorialItemDto> ObtenerHistorial(int idPaciente, int IdMedico)
        {
            // 1. Traemos las dos listas
            var listaConsultas = _repo.ObtenerHistorialConsultas(idPaciente);
            var listaInternaciones = _repo.ObtenerHistorialInternaciones(idPaciente);
            var listaTurnos = _repo.ObtenerHistorialTurnos(idPaciente);

            // 2. Las juntamos
            var listaCompleta = listaConsultas.Concat(listaInternaciones).Concat(listaTurnos).ToList();
     
            

            // 4. Ordenamos y devolvemos la lista de datos
            return listaCompleta.OrderByDescending(h => h.Fecha).ToList();
        }

        /// <summary>
        /// Busca la información detallada de un médico por su identificador único.
        /// </summary>
        /// <param name="idMedico">ID del médico a buscar.</param>
        /// <returns>Objeto <see cref="MedicoDto"/> o <c>null</c> si no se encuentra.</returns>
        public MedicoDto ObtenerMedicoPorId(int idMedico)
        {
            var medicos = _repo.ListarMedicos();
            return medicos.FirstOrDefault(m => m.Id == idMedico);
        }
    }
}