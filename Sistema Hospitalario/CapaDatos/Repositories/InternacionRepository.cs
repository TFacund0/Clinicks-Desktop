using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Historiales;
using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de gestionar la persistencia de las internaciones y el estado 
    /// de los recursos asociados (pacientes y camas) mediante Entity Framework.
    /// </summary>
    public class InternacionRepository : IInternacionRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InternacionRepository"/>.
        /// </summary>
        public InternacionRepository()
        {
        }

        /// <inheritdoc />
        public List<InternacionDto> GetAll()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                return db.internacion
                    .Select(i => new InternacionDto
                    {
                        Id_internacion = i.id_internacion,
                        Id_paciente = i.id_paciente,
                        Id_medico = i.id_medico,
                        NombreCompletoMedico = i.medico.nombre + " " + i.medico.apellido,
                        Id_procedimiento = i.id_procedimiento,
                        procedimiento = i.procedimiento.nombre,
                        Internado = i.paciente.nombre + " " + i.paciente.apellido,
                        Fecha_ingreso = i.fecha_inicio,
                        Fecha_egreso = i.fecha_fin,
                        Diagnostico = i.motivo,
                        Nro_habitacion = i.nro_habitacion,
                        Id_cama = i.id_cama,
                        Nro_piso = i.cama.habitacion.nro_piso
                    }).ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(InternacionDto internacion)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                // 1) Buscar y validar paciente
                var paciente = db.paciente.Find(internacion.Id_paciente);
                if (paciente == null)
                {
                    throw new Exception($"No se encontró el paciente con ID {internacion.Id_paciente}");
                }

                // 2) Buscar el estado "Internado" en la tabla estado_paciente
                var estadoInternado = db.estado_paciente
                    .FirstOrDefault(e => e.nombre == "Internado");

                if (estadoInternado == null)
                {
                    throw new Exception("No existe el estado de paciente 'Internado' en la tabla estado_paciente.");
                }

                // Asignar el id encontrado
                paciente.id_estado_paciente = estadoInternado.id_estado_paciente;
                db.Entry(paciente).State = EntityState.Modified;

                // 3) Buscar y validar cama (clave compuesta: id_cama + nro_habitacion)
                var cama = db.cama.Find(internacion.Id_cama, internacion.Nro_habitacion);
                if (cama == null)
                {
                    throw new Exception(
                        $"No se encontró la cama con Id_cama = {internacion.Id_cama} y Nro_habitacion = {internacion.Nro_habitacion}");
                }

                // 4) Buscar el estado "Ocupada" en la tabla estado_cama
                var estadoOcupada = db.estado_cama
                    .FirstOrDefault(e => e.disponibilidad == "Ocupada");

                if (estadoOcupada == null)
                {
                    throw new Exception("No existe el estado de cama 'Ocupada' en la tabla estado_cama.");
                }

                // Asignar el id encontrado
                cama.id_estado_cama = estadoOcupada.id_estado_cama;
                db.Entry(cama).State = EntityState.Modified;

                // 5) Crear la internación como ya lo hacías
                var nuevaInternacion = new internacion
                {
                    id_paciente = internacion.Id_paciente,
                    id_medico = internacion.Id_medico,
                    id_procedimiento = internacion.Id_procedimiento,
                    fecha_inicio = internacion.Fecha_ingreso,
                    fecha_fin = internacion.Fecha_egreso,
                    motivo = internacion.Diagnostico,
                    nro_habitacion = internacion.Nro_habitacion,
                    id_cama = internacion.Id_cama
                };

                db.internacion.Add(nuevaInternacion);

                // 6) Guardar todos los cambios juntos
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Eliminar(int id_internacion)
            {
                using (var db = new Sistema_HospitalarioEntities_Conexion())
                {
                    var internacion = db.internacion.Find(id_internacion);
                    if (internacion != null)
                    {
                        db.internacion.Remove(internacion);
                        db.SaveChanges();
                    }
                }
            }

        /// <inheritdoc />
        public void Actualizar(int id_internacion, InternacionDto internacion)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var internacionExistente = db.internacion.Find(id_internacion);
                if (internacionExistente != null)
                {
                    internacionExistente.id_paciente = internacion.Id_paciente;
                    internacionExistente.id_medico = internacion.Id_medico;
                    internacionExistente.id_procedimiento = internacion.Id_procedimiento;
                    internacionExistente.fecha_inicio = internacion.Fecha_ingreso;
                    internacionExistente.fecha_fin = internacion.Fecha_egreso;
                    internacionExistente.motivo = internacion.Diagnostico;
                    internacionExistente.nro_habitacion = internacion.Nro_habitacion;
                    internacionExistente.id_cama = internacion.Id_cama;
                    db.Entry(internacionExistente).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Registra el egreso de un paciente, liberando la cama y actualizando el estado del paciente a 'Alta'.
        /// </summary>
        /// <param name="dto">DTO con los datos necesarios para finalizar la internación.</param>
        /// <exception cref="InvalidOperationException">Se lanza si no se encuentra la internación especificada.</exception>
        public void FinalizarInternacion(FinalizarInternacionDto dto)
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var entidad = db.internacion.SingleOrDefault(i => i.id_internacion == dto.IdInternacion);

                if (entidad == null)
                    throw new InvalidOperationException("No se encontró la internación.");

                // Campos de internación
                entidad.fecha_fin = dto.FechaEgreso;
                entidad.motivo = dto.DiagnosticoEgreso;

                // Cambiar estado de cama
                var cama = db.cama.SingleOrDefault(c => c.id_cama == dto.IdCama);
                if (cama != null)
                {
                    cama.id_estado_cama = 1;
                }
                var paciente = db.paciente.SingleOrDefault(p => p.id_paciente == entidad.id_paciente);
                if (paciente != null)
                {
                    // cambiar estado de paciente a "alta" segun el campo de texto, no el id
                    var estadoAlta = db.estado_paciente.SingleOrDefault(e => e.nombre.ToLower() == "alta");
                    if (estadoAlta != null)
                    {
                        paciente.id_estado_paciente = estadoAlta.id_estado_paciente;
                    }

                }

                db.SaveChanges();
            }
        }
    }
}
