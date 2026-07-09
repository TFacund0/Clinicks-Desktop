using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.CamaDTO;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de gestionar la persistencia y estados de las camas mediante Entity Framework.
    /// Maneja la asignación de camas a habitaciones y el control de disponibilidad.
    /// </summary>
    public class CamaRepository : ICamaRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="CamaRepository"/>.
        /// </summary>
        public CamaRepository()
        {
        }

        /// <inheritdoc />
        public List<MostrarCamaDTO> GetAll()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.cama
                            .Select(e => new MostrarCamaDTO
                            {
                                NroHabitacion = e.nro_habitacion,
                                Estado = e.estado_cama.disponibilidad,
                                NroCama = e.id_cama,
                                IdEstadoCama = e.id_estado_cama
                            })
                            .ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(int nroHabitacion)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                int idEstadoPorDefecto = db.estado_cama
                                           .Where(e => e.disponibilidad == "Disponible")
                                           .Select(e => e.id_estado_cama)
                                           .FirstOrDefault();

                if (idEstadoPorDefecto == 0)
                {
                    // Lanzamos un error claro si no existe el estado "Disponible"
                    throw new Exception("Error de configuración: No se encontró el estado 'Disponible' en la base de datos.");
                }

                // 3. Creamos la cama con el ID de estado correcto
                var nuevaCama = new cama
                {
                    nro_habitacion = nroHabitacion,
                    id_estado_cama = idEstadoPorDefecto
                };

                db.cama.Add(nuevaCama);
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Eliminar(int nroHabitacion, int nroCama)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var cama = db.cama.FirstOrDefault(e => e.nro_habitacion == nroHabitacion && e.id_cama == nroCama);
                if (cama == null)
                {
                    // La capa de datos no interactúa con la UI: notifica el problema mediante una excepción.
                    throw new InvalidOperationException($"No se encontró la cama {nroCama} en la habitación {nroHabitacion}.");
                }

                db.cama.Remove(cama);
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void CambiarEstado(int nroHabitacion, int nroCama, int nuevoEstadoId)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var cama = db.cama.FirstOrDefault(e => e.nro_habitacion == nroHabitacion && e.id_cama == nroCama);
                if (cama != null)
                {
                    cama.id_estado_cama = nuevoEstadoId;
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Obtiene el catálogo completo de estados de cama (Disponible, Ocupada, etc.) desde la base de datos.
        /// </summary>
        /// <returns>Lista de entidades <see cref="estado_cama"/>.</returns>
        public List<estado_cama> GetEstadosCama()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.estado_cama.ToList();
            }
        }
    }
}
