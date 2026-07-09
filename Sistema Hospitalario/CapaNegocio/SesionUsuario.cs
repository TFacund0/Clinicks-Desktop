using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;

namespace Sistema_Hospitalario.CapaNegocio // O el namespace que uses
{
    /// <summary>
    /// Gestiona la sesión global del usuario autenticado en la aplicación.
    /// Proporciona acceso centralizado a la información del usuario y su estado de autenticación.
    /// </summary>
    public static class SesionUsuario
    {
        /// <summary>
        /// Obtiene el identificador único del usuario en la base de datos.
        /// </summary>
        public static int IdUsuario { get; private set; }

        /// <summary>
        /// Obtiene el nombre de usuario (login) utilizado para la autenticación.
        /// </summary>
        public static string Username { get; private set; }

        /// <summary>
        /// Obtiene el nombre del rol asignado al usuario (ej. "Admin", "Medico", "Recepcionista").
        /// </summary>
        public static string NombreRol { get; private set; }

        /// <summary>
        /// Obtiene el identificador del médico asociado si el usuario tiene perfil médico.
        /// Es <c>null</c> si el usuario no es un médico.
        /// </summary>
        public static int? IdMedicoAsociado { get; private set; }

        /// <summary>
        /// Obtiene un valor que indica si hay un usuario autenticado actualmente.
        /// </summary>
        public static bool IsLoggedIn { get; private set; } = false;

        /// <summary>
        /// Registra el inicio de sesión exitoso, almacenando los datos del usuario en la sesión global.
        /// </summary>
        /// <param name="datosUsuario">DTO con la información del resultado del login.</param>
        public static void Login(UsuarioLoginResultadoDto datosUsuario)
        {
            if (datosUsuario != null && datosUsuario.LoginExitoso)
            {
                IdUsuario = datosUsuario.IdUsuario;
                Username = datosUsuario.Username;
                NombreRol = datosUsuario.NombreRol;
                IdMedicoAsociado = datosUsuario.IdMedicoAsociado;
                IsLoggedIn = true;
            }
        }

        /// <summary>
        /// Finaliza la sesión actual, limpiando todos los datos del usuario y marcando el estado como no autenticado.
        /// </summary>
        public static void Logout()
        {
            IdUsuario = 0;
            Username = null;
            NombreRol = null;
            IdMedicoAsociado = null;
            IsLoggedIn = false;
        }
    }
}
