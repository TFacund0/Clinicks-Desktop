using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaNegocio.DTOs.Comunes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;
using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaDatos.Interfaces;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de la gestión de usuarios, autenticación y seguridad en la base de datos.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UsuarioRepository"/>.
        /// </summary>
        public UsuarioRepository()
        {
        }

        /// <inheritdoc />
        public (bool Ok, int IdGenerado, string Error) Insertar(string Nombre, string Apellido, string NombreUsuario, int Estado, int Rol, string Password, string correo, int? IdMedico)
        {
            try
            {
                using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
                {
                    bool existeUsuario = db.usuario.Any(unUsuario => unUsuario.username == NombreUsuario);
                    if (existeUsuario)
                        return (false, 0, "Ya existe un usuario registrado con ese nombre de usuario.");

                    var usuarioCreado = new usuario
                    {
                        nombre = Nombre,
                        apellido = Apellido,
                        username = NombreUsuario,
                        id_estado_usuario = Estado,
                        id_rol = Rol,
                        password = Password,
                        email = correo,
                        id_medico = IdMedico
                    };

                    db.usuario.Add(usuarioCreado);
                    db.SaveChanges();

                    return (true, usuarioCreado.id_usuario, null);
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
                return (false, 0, $"Error al guardar el usuario: {errorMessage}");
            }
        }

        /// <inheritdoc />
        public void Eliminar(int IdUsuario)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var usuario = db.usuario.FirstOrDefault(m => m.id_usuario == IdUsuario);
                if (usuario != null)
                {
                    try
                    {
                        db.usuario.Remove(usuario);
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        if (ex.InnerException == null || ex.InnerException.InnerException == null)
                        {
                            throw;
                        }
                        else
                        {
                            throw new Exception("No se puede eliminar el usuario .");
                        }
                    }
                }
            }
        }

        /// <inheritdoc />
        public List<MostrarUsuariosDto> ObtenerUsuarios()
        {
            using (var db = new Sistema_HospitalarioEntities_Conexion())
            {
                var lista = db.usuario
                    .Select(m => new MostrarUsuariosDto
                    {
                        IdUsuario = m.id_usuario,
                        Nombre = m.nombre,
                        Apellido = m.apellido,
                        NombreUsuario = m.username,
                        Estado = m.estado_usuario.nombre,
                        Rol = m.rol.nombre,
                        Correo = m.email
                    })
                    .ToList();

                return lista;
            }

        }

        /// <inheritdoc />
        public bool ExisteUsername(string username)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.usuario.Any(u => u.username == username);
            }
        }

        /// <inheritdoc />
        public DatosLoginUsuarioDto ObtenerUsuarioParaLogin(string username)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var usuario = db.usuario
                    .Where(u => u.username == username)
                    .Select(u => new DatosLoginUsuarioDto
                    {
                        IdUsuario = u.id_usuario,
                        Username = u.username,
                        PasswordHashAlmacenado = u.password, // El hash guardado
                                                             // Manejo seguro por si el rol es nulo
                        NombreRol = (u.rol != null ? u.rol.nombre : "Sin Rol"),
                        IdMedicoAsociado = u.id_medico
                    })
                    .FirstOrDefault();

                return usuario;
            }
        }

        /// <inheritdoc />
        public void ActualizarPasswordHash(int idUsuario, string nuevoHash)
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var usuario = db.usuario.FirstOrDefault(u => u.id_usuario == idUsuario);
                if (usuario != null)
                {
                    usuario.password = nuevoHash;
                    db.SaveChanges();
                }
            }
        }

        /// <inheritdoc />
        public List<CatalogoItemDto> ObtenerRoles()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.rol
                         .Select(r => new CatalogoItemDto { Id = r.id_rol, Nombre = r.nombre })
                         .ToList();
            }
        }

        /// <inheritdoc />
        public List<CatalogoItemDto> ObtenerEstadosUsuario()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                return db.estado_usuario
                         .Select(e => new CatalogoItemDto { Id = e.id_estado_usuario, Nombre = e.nombre })
                         .ToList();
            }
        }
    }
}
