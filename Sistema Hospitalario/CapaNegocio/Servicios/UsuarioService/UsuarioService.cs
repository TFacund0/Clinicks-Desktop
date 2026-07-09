using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.DTOs.Comunes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;
using Sistema_Hospitalario.CapaNegocio.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.UsuarioService
{
    /// <summary>
    /// Servicio que gestiona la seguridad y administración de usuarios del sistema.
    /// Incluye funcionalidades para autenticación, gestión de perfiles, roles y hashing de contraseñas.
    /// </summary>
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UsuarioService"/> con el repositorio por defecto.
        /// </summary>
        public UsuarioService() : this(new UsuarioRepository())
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UsuarioService"/> con un repositorio inyectado (útil para pruebas).
        /// </summary>
        /// <param name="repo">Instancia del repositorio de usuarios.</param>
        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        /// <summary>
        /// Obtiene y filtra el listado de usuarios según un campo y valor específicos.
        /// </summary>
        /// <param name="campo">Campo por el cual filtrar u ordenar (NombreUsuario, Nombre, Apellido, correo, Rol, Estado).</param>
        /// <param name="valor">Valor de búsqueda para el filtro.</param>
        /// <returns>Lista de <see cref="MostrarUsuariosDto"/> filtrada y ordenada.</returns>
        public List<MostrarUsuariosDto> ObtenerUsuarios(string campo = null, string valor = null)
        {
            var listaMaestra = _repo.ObtenerUsuarios();
            List<MostrarUsuariosDto> resultado;

            // Filtrado (si hay valor)
            if (!string.IsNullOrEmpty(valor))
            {
                string valorLower = valor.ToLower();
                switch (campo)
                {
                    case "NombreUsuario":
                        resultado = listaMaestra.Where(u => u.NombreUsuario.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "Nombre":
                        resultado = listaMaestra.Where(u => u.Nombre.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "Apellido":
                        resultado = listaMaestra.Where(u => u.Apellido.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "correo":
                        resultado = listaMaestra.Where(u => u.Correo.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "Rol":
                        resultado = listaMaestra.Where(u => u.Rol.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    case "Estado":
                        resultado = listaMaestra.Where(u => u.Estado.ToLower().StartsWith(valorLower)).ToList();
                        break;
                    default:
                        resultado = listaMaestra.Where(u => u.IdUsuario.ToString() == valor).ToList();
                        break;
                }
            }
            else
            {
                resultado = listaMaestra;
            }

            // Ordenamiento (si no hay valor)
            if (string.IsNullOrEmpty(valor))
            {
                switch (campo)
                {
                    case "NombreUsuario":
                        resultado = resultado.OrderBy(u => u.NombreUsuario).ToList();
                        break;
                    case "Nombre":
                        resultado = resultado.OrderBy(u => u.Nombre).ToList();
                        break;
                    case "Apellido":
                        resultado = resultado.OrderBy(u => u.Apellido).ToList();
                        break;
                    case "correo":
                        resultado = resultado.OrderBy(u => u.Correo).ToList();
                        break;
                    case "Rol":
                        resultado = resultado.OrderBy(u => u.Rol).ToList();
                        break;
                    case "Estado":
                        resultado = resultado.OrderBy(u => u.Estado).ToList();
                        break;
                    default:
                        resultado = resultado.OrderBy(u => u.IdUsuario).ToList();
                        break;
                }
            }
            return resultado;
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema, realizando el hashing de la contraseña y validando unicidad.
        /// </summary>
        /// <param name="dto">DTO con los datos del nuevo usuario.</param>
        /// <returns>Tupla con el estado de éxito, el ID generado y un mensaje de error si aplica.</returns>
        public (bool Ok, int IdGenerado, string Error) AgregarUsuario(UsuarioAltaDto dto)
        {
            if (_repo.ExisteUsername(dto.NombreUsuario))
            {
                return (false, 0, $"El nombre de usuario '{dto.NombreUsuario}' ya está en uso.");
            }

            string hashedPassword = PasswordHasher.Hash(dto.Password);

            return _repo.Insertar(
                dto.Nombre,
                dto.Apellido,
                dto.NombreUsuario,
                dto.IdEstado,
                dto.IdRol,
                hashedPassword,
                dto.Correo,
                dto.IdMedico

            );
        }

        /// <summary>
        /// Elimina un usuario del sistema por su ID, con restricciones de seguridad para cuentas críticas.
        /// </summary>
        /// <param name="idUsuario">ID del usuario a eliminar.</param>
        /// <exception cref="InvalidOperationException">Se lanza si se intenta eliminar al Administrador principal (ID 1).</exception>
        public void EliminarUsuario(int idUsuario)
        {
            if (idUsuario == 1)
            {
                throw new InvalidOperationException("No se puede eliminar al usuario Administrador principal.");
            }

            _repo.Eliminar(idUsuario);
        }

        /// <summary>
        /// Obtiene el catálogo de roles disponibles para asignar a usuarios.
        /// </summary>
        /// <returns>Lista de <see cref="CatalogoItemDto"/> con los roles.</returns>
        public List<CatalogoItemDto> ListarRoles()
        {
            return _repo.ObtenerRoles();
        }

        /// <summary>
        /// Obtiene el catálogo de estados posibles de una cuenta de usuario.
        /// </summary>
        /// <returns>Lista de <see cref="CatalogoItemDto"/> con los estados.</returns>
        public List<CatalogoItemDto> ListarEstadosUsuario()
        {
            return _repo.ObtenerEstadosUsuario();
        }

        /// <summary>
        /// Genera un informe estadístico de la cantidad de usuarios agrupados por su rol.
        /// </summary>
        /// <returns>Diccionario donde la clave es el nombre del rol y el valor es la cantidad de usuarios.</returns>
        public Dictionary<string, int> ObtenerConteoUsuariosPorRol()
        {
            var todosLosUsuarios = _repo.ObtenerUsuarios();

            var conteoPorRol = todosLosUsuarios
                .GroupBy(u => u.Rol)
                .Select(grupo => new
                {
                    NombreRol = grupo.Key,
                    Cantidad = grupo.Count()
                })
                .ToDictionary(item => item.NombreRol, item => item.Cantidad);
            return conteoPorRol;
        }

        /// <summary>
        /// Valida las credenciales de acceso de un usuario.
        /// </summary>
        /// <param name="usuario">Nombre de usuario (username).</param>
        /// <param name="contraseña">Contraseña en texto plano para validar.</param>
        /// <returns>Objeto <see cref="UsuarioLoginResultadoDto"/> con el resultado de la autenticación y datos de sesión.</returns>
        internal UsuarioLoginResultadoDto ValidarCredenciales(string usuario, string contraseña)
        {
            var datosUsuario = _repo.ObtenerUsuarioParaLogin(usuario);

            if (datosUsuario != null && PasswordHasher.Verificar(contraseña, datosUsuario.PasswordHashAlmacenado))
            {
                // Migración transparente: si el hash almacenado es legacy (SHA-256 sin salt),
                // se regenera con PBKDF2 aprovechando que tenemos la contraseña en claro.
                if (PasswordHasher.EsHashLegacy(datosUsuario.PasswordHashAlmacenado))
                {
                    _repo.ActualizarPasswordHash(datosUsuario.IdUsuario, PasswordHasher.Hash(contraseña));
                }

                return new UsuarioLoginResultadoDto
                {
                    LoginExitoso = true,
                    IdUsuario = datosUsuario.IdUsuario,
                    Username = datosUsuario.Username,
                    NombreRol = datosUsuario.NombreRol,
                    IdMedicoAsociado = datosUsuario.IdMedicoAsociado // Puede ser null
                };
            }
            else
            {
                // Falla (usuario no encontrado o contraseña incorrecta)
                return new UsuarioLoginResultadoDto { LoginExitoso = false };

            }
        }
    }
}
