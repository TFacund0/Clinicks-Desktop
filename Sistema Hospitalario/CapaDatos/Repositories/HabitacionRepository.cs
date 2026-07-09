using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Habitaciones;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de la gestión de la infraestructura de habitaciones mediante Entity Framework.
    /// Permite la administración de pisos, habitaciones y sus tipos asociados.
    /// </summary>
    public class HabitacionRepository : IHabitacionRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HabitacionRepository"/>.
        /// </summary>
        public HabitacionRepository()
        {
        }

        /// <inheritdoc />
        public List<MostrarHabitacionDto> GetAll()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.habitacion
                         .Select(e => new MostrarHabitacionDto
                         {
                             NroPiso = e.nro_piso,
                             NroHabitacion = e.nro_habitacion,
                             TipoHabitacion = e.tipo_habitacion.nombre,
                             IdTipoHabitacion = e.id_tipo_habitacion

                         })
                         .ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(int nroPiso, int tipoHabitacion)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                db.habitacion.Add(new habitacion { nro_piso = nroPiso, id_tipo_habitacion = tipoHabitacion });
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Eliminar(int nroPiso, int nroHabitacion)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var esp = db.habitacion.FirstOrDefault(e => e.nro_habitacion == nroHabitacion && e.nro_piso == nroPiso);
                if (esp != null)
                {
                    try
                    {
                        db.habitacion.Remove(esp);
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                            ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                        {
                            throw new Exception("No se puede eliminar la habitación porque tiene camas asociadas. Primero elimine las camas de esta habitación.");
                        }
                        else
                        {
                            throw; // vuelve a lanzar si no es ese el error
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el catálogo de tipos de habitaciones registrados en el sistema.
        /// </summary>
        /// <returns>Lista de <see cref="TiposHabitacionDto"/>.</returns>
        public List<TiposHabitacionDto> ListarTiposHabitacion()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.tipo_habitacion
                            .Select(e => new TiposHabitacionDto
                            {
                                IdTipoHabitacion = e.id_tipo_habitacion,
                                Nombre = e.nombre

                            })
                            .ToList();
            }
        }
    }
}
