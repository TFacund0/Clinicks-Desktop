using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de las operaciones CRUD de pacientes utilizando Entity Framework.
    /// </summary>
    public class PacienteRepository : IPacienteRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="PacienteRepository"/>.
        /// </summary>
        public PacienteRepository()
        {
        }

        /// <inheritdoc />
        public List<PacienteDto> GetAll()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var estado_activo = "Activo";
                var estado_internado = "Internado";
                var estado_alta = "Alta";

                return db.paciente
                    .Where(p => p.estado_paciente.nombre.ToLower() == estado_activo.ToLower() || p.estado_paciente.nombre.ToLower() == estado_internado.ToLower() || p.estado_paciente.nombre.ToLower() == estado_alta.ToLower())
                    .Select(p => new PacienteDto
                    {
                        Id = p.id_paciente,
                        Nombre = p.nombre,
                        Apellido = p.apellido,
                        Fecha_nacimiento = p.fecha_nacimiento,
                        Observaciones = p.observaciones,
                        Dni = p.dni,
                        Direccion = p.direccion,
                        Email = p.correo_electronico,
                        Estado_paciente = p.estado_paciente.nombre,
                        Telefono = p.telefono.FirstOrDefault().numero_telefono
                    })
                    .ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(PacienteDto paciente)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var estado = db.estado_paciente.FirstOrDefault(e => e.nombre == paciente.Estado_paciente);
                if (estado == null)
                    throw new Exception($"No se encontró el estado de paciente '{paciente.Estado_paciente}'");

                var nuevoPaciente = new paciente
                {
                    nombre = paciente.Nombre,
                    apellido = paciente.Apellido,
                    fecha_nacimiento = paciente.Fecha_nacimiento,
                    fecha_registracion = DateTime.Now,
                    observaciones = paciente.Observaciones,
                    dni = paciente.Dni,
                    direccion = paciente.Direccion,
                    correo_electronico = paciente.Email,
                    id_estado_paciente = estado.id_estado_paciente
                };
                
                if (!string.IsNullOrWhiteSpace(paciente.Telefono))
                {
                    nuevoPaciente.telefono.Add(new telefono
                    {
                        numero_telefono = paciente.Telefono.Trim()
                    });
                }

                db.paciente.Add(nuevoPaciente);
                db.SaveChanges();   
            }   
        }

        /// <inheritdoc />
        public void Eliminar(int id_paciente)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var paciente = db.paciente.FirstOrDefault(p => p.id_paciente == id_paciente);
                if (paciente != null)
                {
                    db.paciente.Remove(paciente);
                    db.SaveChanges();
                }
            }
        }

        /// <inheritdoc />
        public void Actualizar(int id_paciente, PacienteDto pacienteActualizado)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                // Cargamos el paciente con sus teléfonos
                var paciente = db.paciente
                    .Include("telefono")
                    .FirstOrDefault(p => p.id_paciente == id_paciente);

                if (paciente == null)
                    throw new Exception($"No se encontró el paciente con ID {id_paciente}.");

                // Buscamos el estado según el nombre del DTO
                var estado = db.estado_paciente
                    .FirstOrDefault(e => e.nombre == pacienteActualizado.Estado_paciente);

                if (estado == null)
                    throw new Exception($"No se encontró el estado '{pacienteActualizado.Estado_paciente}'.");

                // Actualizamos los datos básicos
                paciente.nombre = pacienteActualizado.Nombre;
                paciente.apellido = pacienteActualizado.Apellido;
                paciente.fecha_nacimiento = pacienteActualizado.Fecha_nacimiento;
                paciente.observaciones = pacienteActualizado.Observaciones;
                paciente.dni = pacienteActualizado.Dni;
                paciente.direccion = pacienteActualizado.Direccion;
                paciente.correo_electronico = pacienteActualizado.Email;
                paciente.id_estado_paciente = estado.id_estado_paciente;

                // Ahora actualizamos el teléfono
                if (!string.IsNullOrWhiteSpace(pacienteActualizado.Telefono))
                {
                    var primerTelefono = paciente.telefono.FirstOrDefault();

                    if (primerTelefono != null)
                    {
                        // Si ya existe un teléfono, lo sobreescribimos
                        primerTelefono.numero_telefono = pacienteActualizado.Telefono.Trim();
                    }
                    else
                    {
                        // Si no hay ninguno, lo agregamos
                        paciente.telefono.Add(new telefono
                        {
                            numero_telefono = pacienteActualizado.Telefono.Trim()
                        });
                    }
                }

                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public List<EstadoPacienteDto> GetEstados()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                return db.estado_paciente
                    .Select(e => new EstadoPacienteDto
                    {
                        Id = e.id_estado_paciente,
                        Nombre = e.nombre
                    })
                    .ToList();
            }
        }
    }
}
