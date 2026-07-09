using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de la gestión de turnos médicos y agenda utilizando Entity Framework.
    /// Maneja la persistencia de los estados de turnos y colisiones de horarios.
    /// </summary>
    public class TurnoRepository : ITurnoRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="TurnoRepository"/>.
        /// </summary>
        public TurnoRepository()
        {
        }

        /// <inheritdoc />
        public List<TurnoAgendaDto> ObtenerTurnosParaAgenda(int idMedico, DateTime fecha)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                // 1. Traemos datos "crudos" de la BD
                var turnosCrudos = db.turno
                    .Where(t => t.id_medico == idMedico &&
                                DbFunctions.TruncateTime(t.fecha_turno) == fecha.Date)
                    .OrderBy(t => t.fecha_turno) // Ordenamos por la fecha completa
                    .Select(t => new
                    {
                        IdTurno = t.id_turno,
                        FechaHoraTurno = t.fecha_turno,
                        PacienteNombre = t.paciente.nombre,
                        PacienteApellido = t.paciente.apellido,
                        Estado = t.estado_turno.nombre
                    })
                    .ToList(); // El SQL se ejecuta aquí

                // 2. Convertimos a DTO en C# para formatear la hora
                var resultadoFinal = turnosCrudos
                    .Select(t => new TurnoAgendaDto
                    {
                        IdTurno = t.IdTurno,
                        Hora = t.FechaHoraTurno.ToString("HH:mm"), // Formato 24hs (ej: 09:05)
                        Paciente = t.PacienteNombre + " " + t.PacienteApellido,
                        Estado = t.Estado
                    })
                    .ToList();

                return resultadoFinal;
            }
        }

        /// <inheritdoc />
        public AgendaContadoresDto ObtenerContadoresAgenda(int idMedico, DateTime fecha)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var contadores = db.turno
                    .Where(t => t.id_medico == idMedico &&
                                DbFunctions.TruncateTime(t.fecha_turno) == fecha.Date)
                    .GroupBy(t => t.estado_turno.nombre)
                    .Select(g => new { Estado = g.Key, Cantidad = g.Count() })
                    .ToDictionary(k => k.Estado, v => v.Cantidad);

                // ¡IMPORTANTE! Reemplazá "Pendiente", "Completado", etc.,
                // con los nombres EXACTOS de tu tabla 'estado_turno'.
                return new AgendaContadoresDto
                {
                    Pendientes = contadores.ContainsKey("Pendiente") ? contadores["Pendiente"] : 0,
                    Completadas = contadores.ContainsKey("Completado") ? contadores["Completado"] : 0,
                    Canceladas = (contadores.ContainsKey("Cancelado") ? contadores["Cancelado"] : 0) +
                                 (contadores.ContainsKey("Reprogramado") ? contadores["Reprogramado"] : 0)
                };
            }
        }

        /// <inheritdoc />
        public bool ActualizarEstadoTurno(int idTurno, int idNuevoEstado)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var turno = db.turno.Find(idTurno);
                if (turno == null) return false;

                turno.id_estado_turno = idNuevoEstado;
                db.SaveChanges();
                return true;
            }
        }
        /// <inheritdoc />
        public List<TurnoDto> GetAll()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var turnos = from t in db.turno
                             join p in db.paciente on t.id_paciente equals p.id_paciente
                             join m in db.medico on t.id_medico equals m.id_medico
                             join pr in db.procedimiento on t.id_procedimiento equals pr.id_procedimiento
                             join e in db.estado_turno on t.id_estado_turno equals e.id_estado_turno
                             select new TurnoDto
                             {
                                 Id_turno = t.id_turno,
                                 Id_paciente = p.id_paciente,
                                 Id_medico = m.id_medico,
                                 Id_procedimiento = pr.id_procedimiento,
                                 Paciente = p.nombre + " " + p.apellido,
                                 Medico = m.nombre + " " + m.apellido,
                                 Procedimiento = pr.nombre,
                                 FechaTurno = t.fecha_turno,
                                 Correo = p.correo_electronico,
                                 DNI = p.dni.ToString(),
                                 Telefono = t.telefono,
                                 Estado = e.nombre,
                                 Observaciones = t.motivo
                             };
                return turnos.ToList();
            }
        }

        /// <summary>
        /// Obtiene el detalle completo de un turno específico por su ID.
        /// </summary>
        /// <param name="id_turno">ID del turno.</param>
        /// <returns>DTO con el detalle del turno.</returns>
        public TurnoDto ObtenerDetalle(int id_turno)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var t = db.turno
                    .AsNoTracking()
                    .Where(x => x.id_turno == id_turno)
                    .Select(x => new TurnoDto
                    {
                        Id_turno = x.id_turno,
                        Id_paciente = x.id_paciente,
                        Paciente = x.paciente.nombre + " " + x.paciente.apellido,
                        Id_medico = x.id_medico,
                        Medico = x.medico.apellido + " " + x.medico.nombre,
                        Id_procedimiento = x.id_procedimiento,
                        Procedimiento = x.procedimiento.nombre,
                        FechaRegistro = x.fecha_registracion,
                        FechaTurno = x.fecha_turno,
                        Observaciones = x.motivo,
                        Correo = x.correo_electronico,  
                        Telefono = x.telefono,          
                        Estado = x.estado_turno.nombre
                    })
                    .FirstOrDefault();

                return t;
            }
        }

        /// <summary>
        /// Obtiene un listado simplificado de turnos para su visualización masiva.
        /// </summary>
        /// <returns>Lista de <see cref="ListadoTurno"/>.</returns>
        public List<ListadoTurno> ListadoTurnos()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var turnos = from t in db.turno
                             join p in db.paciente on t.id_paciente equals p.id_paciente
                             join m in db.medico on t.id_medico equals m.id_medico
                             join pr in db.procedimiento on t.id_procedimiento equals pr.id_procedimiento
                             join e in db.estado_turno on t.id_estado_turno equals e.id_estado_turno
                             select new ListadoTurno
                             {
                                 Id_turno = t.id_turno,
                                 Paciente = p.nombre + " " + p.apellido,
                                 Medico = m.nombre + " " + m.apellido,
                                 Id_medico = m.id_medico,
                                 Procedimiento = pr.nombre,
                                 FechaTurno = t.fecha_turno,
                                 Fecha_Del_Turno = t.fecha_turno,
                                 Estado = e.nombre
                             };
                return turnos.ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(TurnoDto p_turno)
            {
                using (var db = new Sistema_HospitalarioEntities_Conexion())
                {
                    // ================== 1) Estado del turno: Pendiente ==================
                    var estadoPendiente = db.estado_turno
                        .AsNoTracking()
                        .FirstOrDefault(e => e.nombre.ToLower() == "pendiente");

                    if (estadoPendiente == null)
                        throw new Exception("No se encontró el estado 'Pendiente' en la tabla estado_turno.");

                    // ================== 2) Obtener paciente ==================
                    var paciente = db.paciente.SingleOrDefault(p => p.id_paciente == p_turno.Id_paciente);
                    if (paciente == null)
                        throw new Exception($"No se encontró el paciente con ID {p_turno.Id_paciente}.");

                    // ================== 3) Cambiar estado del paciente si está "Dado de alta" ==================
                    var estadoAlta = db.estado_paciente
                        .AsNoTracking()
                        .FirstOrDefault(e => e.nombre.ToLower() == "alta");
                    var estadoActivo = db.estado_paciente
                        .AsNoTracking()
                        .FirstOrDefault(e => e.nombre.ToLower() == "activo");

                    if (estadoAlta == null || estadoActivo == null)
                        throw new Exception("No se encontraron los estados 'alta' y/o 'activo' en la tabla estado_paciente.");

                    // Si el paciente está "dado de alta", lo pasamos a "Activo"
                    if (paciente.id_estado_paciente == estadoAlta.id_estado_paciente)
                    {
                        paciente.id_estado_paciente = estadoActivo.id_estado_paciente;
                        db.Entry(paciente).State = EntityState.Modified;
                    }

                    // ================== 4) Crear turno ==================
                    var nuevoTurno = new turno
                    {
                        id_paciente = p_turno.Id_paciente,
                        id_medico = p_turno.Id_medico,
                        id_procedimiento = p_turno.Id_procedimiento,
                        fecha_turno = p_turno.FechaTurno,
                        correo_electronico = p_turno.Correo,
                        fecha_registracion = DateTime.Now,
                        telefono = p_turno.Telefono,
                        motivo = p_turno.Observaciones,
                        id_estado_turno = estadoPendiente.id_estado_turno
                    };

                    db.turno.Add(nuevoTurno);

                    // ================== 5) Guardar TODO junto ==================
                    db.SaveChanges();
                }
            }


        /// <inheritdoc />
        public void Actualizar(int id_turno, TurnoDto turnoDto)
            {
                using (var db = new Sistema_HospitalarioEntities_Conexion())
                {
                    var turnoExistente = db.turno.Find(id_turno)
                                         ?? throw new Exception("Turno no encontrado.");

                    turnoExistente.id_paciente = turnoDto.Id_paciente;
                    turnoExistente.id_medico = turnoDto.Id_medico;
                    turnoExistente.id_procedimiento = turnoDto.Id_procedimiento;
                    turnoExistente.fecha_turno = turnoDto.FechaTurno;
                    turnoExistente.motivo = turnoDto.Observaciones;

                    // Actualizar estado SOLO si viene texto en el DTO
                    if (!string.IsNullOrWhiteSpace(turnoDto.Estado))
                    {
                        // Buscamos el ID del estado por nombre (case-insensitive)
                        var nuevoEstadoId = db.estado_turno
                            .Where(e => e.nombre.Equals(
                                turnoDto.Estado,
                                StringComparison.OrdinalIgnoreCase))
                            .Select(e => e.id_estado_turno)   // ajustá el nombre si la PK se llama distinto
                            .FirstOrDefault();                // si no encuentra, devuelve 0

                        // Si encontramos un estado válido, lo asignamos
                        if (nuevoEstadoId != 0)
                        {
                            turnoExistente.id_estado_turno = nuevoEstadoId; // FK en la tabla turno
                        }
                        // Si no lo encuentra, conserva el estado actual
                    }

                    // Correo: si viene vacío, lo guardamos como null
                    turnoExistente.correo_electronico =
                        string.IsNullOrWhiteSpace(turnoDto.Correo)
                            ? null
                            : turnoDto.Correo.Trim();

                    // Teléfono: igual idea
                    turnoExistente.telefono =
                        string.IsNullOrWhiteSpace(turnoDto.Telefono)
                            ? null
                            : turnoDto.Telefono.Trim();

                    db.SaveChanges();
                }
            }

        /// <inheritdoc />
        public void Eliminar(int id_turno)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var turnoExistente = db.turno.Find(id_turno) ?? throw new Exception("Turno no encontrado.");
                db.turno.Remove(turnoExistente);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Obtiene el catálogo de estados de turnos disponibles.
        /// </summary>
        /// <returns>Lista de <see cref="ListadoEstadoTurno"/>.</returns>
        public List<ListadoEstadoTurno> ListarEstadosTurno()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var estados = from e in db.estado_turno
                              select new ListadoEstadoTurno
                              {
                                  Id_estado = e.id_estado_turno.ToString(),
                                  Estado = e.nombre
                              };
                return estados.ToList();
            }
        }

        /// <inheritdoc />
        public bool ExisteTurnoMismoDiaMismoMedicoPaciente(int idPaciente, int idMedico, DateTime fecha)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var fechaSoloDia = fecha.Date;

                return db.turno.Any(t =>
                    t.id_paciente == idPaciente &&
                    t.id_medico == idMedico &&
                    DbFunctions.TruncateTime(t.fecha_turno) == fechaSoloDia
                );
            }
        }

        /// <inheritdoc />
        public bool ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(int idTurnoExcluir, int idPaciente, int idMedico, DateTime fecha)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var fechaSoloDia = fecha.Date;

                return db.turno.Any(t =>
                    t.id_turno != idTurnoExcluir &&              // <<< excluimos el actual
                    t.id_paciente == idPaciente &&
                    t.id_medico == idMedico &&
                    DbFunctions.TruncateTime(t.fecha_turno) == fechaSoloDia
                );
            }
        }
    }
}
