using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Especialidades;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de gestionar el catálogo de especialidades médicas mediante Entity Framework.
    /// </summary>
    public class EspecialidadRepository : IEspecialidadRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EspecialidadRepository"/>.
        /// </summary>
        public EspecialidadRepository()
        {
            
        }

        /// <inheritdoc />
        public List<EspecialidadDto> GetAll()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.especialidad
                         .Select(e => new EspecialidadDto
                         {
                             Id = e.id_especialidad,
                             Nombre = e.nombre,
                         })
                         .ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(string nombre)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                db.especialidad.Add(new especialidad { nombre = nombre });
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Eliminar(string nombre)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var esp = db.especialidad.FirstOrDefault(e => e.nombre.ToLower() == nombre.ToLower());
                if (esp != null)
                
                try
                {
                    db.especialidad.Remove(esp);
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                    {
                        throw new Exception("No se puede eliminar la especialidad porque tiene medicos asociados. Primero elimine los medicos que ejercen esta especialidad.");
                    }
                    else
                    {
                        throw; // vuelve a lanzar si no es ese el error
                    }
                }
            }
        }

        /// <summary>
        /// Verifica si una especialidad con el mismo nombre ya existe en la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre de la especialidad a verificar.</param>
        /// <returns>True si el nombre ya está registrado.</returns>
        public bool ExisteNombre(string nombre)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.especialidad.Any(e => e.nombre == nombre);
            }
        }
    }
}
