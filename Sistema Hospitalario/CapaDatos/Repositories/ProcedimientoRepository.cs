using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de gestionar el catálogo de procedimientos médicos mediante Entity Framework.
    /// </summary>
    public class ProcedimientoRepository : IProcedimientoRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ProcedimientoRepository"/>.
        /// </summary>
        public ProcedimientoRepository()
        {
        }

        /// <inheritdoc />
        public List<MostrarProcedimientoDto> GetAll()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.procedimiento
                         .Select(e => new MostrarProcedimientoDto
                         {
                             Nombre = e.nombre
                         })
                         .ToList();
            }
        }

        /// <inheritdoc />
        public void Insertar(string nombre)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                db.procedimiento.Add(new procedimiento { nombre = nombre });
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Eliminar(string nombre)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var esp = db.procedimiento.FirstOrDefault(e => e.nombre.ToLower() == nombre.ToLower());
                if (esp != null)
                {
                    db.procedimiento.Remove(esp);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Verifica si un procedimiento con el mismo nombre ya existe en el sistema.
        /// </summary>
        /// <param name="nombre">Nombre a verificar.</param>
        /// <returns>True si el nombre ya está registrado.</returns>
        public bool ExisteNombre(string nombre)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.procedimiento.Any(e => e.nombre == nombre);
            }
        }

        /// <summary>
        /// Obtiene el listado de procedimientos en formato simplificado para selectores o combos.
        /// </summary>
        /// <returns>Lista de <see cref="ProcedimientoDto"/>.</returns>
        public List<ProcedimientoDto> ListarProcedimientos()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.procedimiento
                         .Select(p => new ProcedimientoDto
                         {
                             Id = p.id_procedimiento,
                             Name = p.nombre
                         })
                         .ToList();
            }
        }
    }
}
