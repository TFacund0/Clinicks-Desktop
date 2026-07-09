using Sistema_Hospitalario.CapaDatos;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Historiales;
using Sistema_Hospitalario.CapaNegocio.DTOs.Consultas;
using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de la persistencia de médicos, consultas e historial clínico consolidado.
    /// Utiliza Entity Framework para interactuar con las tablas de Médicos, Consultas e Internaciones.
    /// </summary>
    public class MedicoRepository : IMedicoRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="MedicoRepository"/>.
        /// </summary>
        public MedicoRepository()
        {
        }

        /// <summary>
        /// Recupera un objeto de entidad médico completo por su identificador único.
        /// </summary>
        /// <param name="idMedico">ID del médico a buscar.</param>
        /// <returns>Objeto <see cref="medico"/> o null si no se encuentra.</returns>
        public medico ObtenerMedicoPorId(int idMedico)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.medico.FirstOrDefault(m => m.id_medico == idMedico);
            }
        }

        /// <inheritdoc />
        public (bool Ok, int IdGenerado, string Error) Insertar(string nombre, string apellido, string dni, string direccion, string matricula, string correo, int idEspecialidad)
        {
            try
            {
                using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
                {
                    int dniint = Convert.ToInt32(dni);
                    bool existeDni = db.medico.Any(unMedico => unMedico.DNI == dniint);
                    if (existeDni)
                        return (false, 0, "Ya existe un medico registrado con ese DNI.");

                    var nuevoMedico = new medico
                    {
                        nombre = nombre,
                        apellido = apellido,
                        DNI = Convert.ToInt32(dni),
                        direccion = direccion,
                        matricula = matricula,
                        correo_electronico = correo,
                        id_especialidad = idEspecialidad
                    };

                    db.medico.Add(nuevoMedico);
                    db.SaveChanges();

                    return (true, nuevoMedico.id_medico, null);
                }
            }
            catch (Exception ex)
            {
                // Creamos un mensaje de error más detallado
                string errorMessage = ex.Message;

                // Verificamos si hay una excepción interna (el mensaje de la base de datos)
                if (ex.InnerException != null)
                {
                    errorMessage += " --> Inner Exception: " + ex.InnerException.Message;
                }

                // Devolvemos el mensaje completo
                return (false, 0, $"Error al guardar el medico: {errorMessage}");
            }
        }

        /// <inheritdoc />
        public void Eliminar(int idMedico)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var medico = db.medico.FirstOrDefault(m => m.id_medico == idMedico);
                if (medico != null)
                {
                    try
                    {
                        db.medico.Remove(medico);
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                            ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                        {
                            throw new Exception("No se puede eliminar el médico porque está asociado a otros registros.");
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public List<MostrarMedicoDto> ObtenerMedicos()
        {

            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var lista = db.medico
                    .Select(m => new MostrarMedicoDto
                    {
                        IdMedico = m.id_medico,
                        Nombre = m.nombre,
                        Apellido = m.apellido,
                        DNI = m.DNI.ToString(),
                        Direccion = m.direccion,
                        Matricula = m.matricula,
                        Correo = m.correo_electronico,
                        Especialidad = (m.especialidad != null ? m.especialidad.nombre : "ninguna")
                    })
                    .ToList();

                return lista;
            }
        }

        /// <inheritdoc />
        public List<MedicoDto> ListarMedicos()
        {
            using (var context = new Sistema_HospitalarioEntities_Conexion())
            {
                var medicos = context.medico.ToList();
                var medicoDtos = medicos.Select(m => new MedicoDto
                {
                    Id = m.id_medico,
                    Matricula = m.matricula,
                    Nombre = m.nombre,
                    Apellido = m.apellido,
                    Especialidad = m.especialidad.nombre,
                    Direccion = m.direccion,
                    Email = m.correo_electronico
                }).ToList();
                return medicoDtos;
            }
        }
        /// <inheritdoc />
        public List<PacienteListadoMedicoDto> ObtenerTodosParaMedico(DateTime? fechaTurno)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var query = db.paciente.AsQueryable();

                if (fechaTurno.HasValue)
                {
                    query = query.Where(p => p.turno.Any(t =>
                        DbFunctions.TruncateTime(t.fecha_turno) == fechaTurno.Value.Date
                    ));
                }
                var lista = query
                    .Select(p => new PacienteListadoMedicoDto
                    {
                        IdPaciente = p.id_paciente,
                        FechaNacim = p.fecha_nacimiento,
                        Nombre = p.nombre,
                        Apellido = p.apellido,
                        Direccion = p.direccion,
                        Dni = p.dni.ToString(), // Convertimos el int a string
                        Observacion = p.observaciones,

                       
                        Telefono = p.telefono.FirstOrDefault().numero_telefono ?? "N/A",
                        Estado = p.estado_paciente.nombre,

                        
                        Habitacion = p.internacion.Any(i => i.fecha_fin == null)
                                     ? p.internacion.OrderByDescending(i => i.fecha_inicio)
                                                  .FirstOrDefault(i => i.fecha_fin == null)
                                                  .nro_habitacion.ToString()
                                     : "Ambulatorio"
                    })
                    .ToList();

                return lista;
            }
        }
        /// <inheritdoc />
        public int ContarTotalPacientes()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                // Simplemente cuenta todas las filas en la tabla paciente
                return db.paciente.Count();
            }
        }
        /// <inheritdoc />
        public int? ObtenerIdPacientePorDni(int dni)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.paciente
                         .Where(p => p.dni == dni)
                         .Select(p => (int?)p.id_paciente)
                         .FirstOrDefault();
            }
        }

        /// <inheritdoc />
        public void InsertarConsulta(ConsultaAltaDto consulta, int idMedico, int idPaciente)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var entidad = new Consulta
                {
                    motivo = consulta.Motivo,
                    diagnostico = consulta.Diagnostico,
                    tratamiento = consulta.Tratamiento,
                    fecha_consulta = consulta.Fecha,
                    id_medico = idMedico,
                    id_paciente = idPaciente
                };

                try
                {
                    db.Consulta.Add(entidad);
                    db.SaveChanges();
                }
                // Captura el error específico de validación de EF (como el de la contraseña)
                catch (DbEntityValidationException ex)
                {
                    var sb = new StringBuilder();
                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendLine($"- {error.PropertyName}: {error.ErrorMessage}");
                        }
                    }
                    throw new Exception("Error de validación: \n" + sb.ToString());
                }
            }
        }
        /// <inheritdoc />
        public List<HistorialItemDto> ObtenerHistorialConsultas(int idPaciente)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.Consulta
                    .Where(c => c.id_paciente == idPaciente)
                    .Select(c => new HistorialItemDto
                    {
                        Fecha = c.fecha_consulta,
                        Tipo = "Consulta",
                        Motivo = c.motivo,
                        Diagnostico = c.diagnostico,
                        Tratamiento = c.tratamiento,
                        NombreMedico = c.medico.nombre + " " + c.medico.apellido,
                        DniMedico = c.medico.DNI.ToString(),
                        IdMedico = c.medico.id_medico
                    })
                    .ToList();
            }
        }
        /// <inheritdoc />
        public List<HistorialItemDto> ObtenerHistorialInternaciones(int idPaciente)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.internacion
                    .Where(i => i.id_paciente == idPaciente)
                    .Select(i => new HistorialItemDto
                    {
                        Fecha = i.fecha_inicio,
                        FechaFin = i.fecha_fin,
                        Tipo = "Procedimiento/Internación",
                        Motivo = i.motivo, 
                        Diagnostico = i.procedimiento.nombre,
                        NombreMedico = i.medico.nombre + " " + i.medico.apellido,
                        DniMedico = i.medico.DNI.ToString(),
                        IdMedico = i.medico.id_medico
                    })
                    .ToList();
            }
        }

        /// <inheritdoc />
        public List<HistorialItemDto> ObtenerHistorialTurnos(int idPaciente)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.turno
                    .Where(t => t.id_paciente == idPaciente)
                    .OrderByDescending(t => t.fecha_turno) // Ordenamos
                    .Select(t => new HistorialItemDto
                    {
                        Fecha = t.fecha_turno,
                        Tipo = "Turno (" + t.estado_turno.nombre + ")", 
                        Motivo = t.motivo,
                        Diagnostico = (t.procedimiento != null ? t.procedimiento.nombre : "N/A"), // Mostramos el procedimiento si lo tiene
                        Tratamiento = "N/A", 
                        NombreMedico = t.medico.nombre + " " + t.medico.apellido,
                        DniMedico = t.medico.DNI.ToString(),
                        IdMedico = t.medico.id_medico
                    })
                    .ToList();
            }
        }
    }
}
