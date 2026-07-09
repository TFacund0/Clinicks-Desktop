using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios
{
    /// <summary>
    /// DTO interno utilizado para transportar la información de autenticación recuperada de la base de datos.
    /// Facilita la validación de credenciales y la carga de roles.
    /// </summary>
    public class DatosLoginUsuarioDto
    {
        /// <summary>ID único del usuario.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Nombre de usuario (Login).</summary>
        public string Username { get; set; }

        /// <summary>Hash de la contraseña almacenado en el sistema.</summary>
        public string PasswordHashAlmacenado { get; set; }

        /// <summary>Nombre del rol asignado (ej. 'Administrador', 'Médico').</summary>
        public string NombreRol { get; set; }

        /// <summary>ID del médico asociado (si el usuario es un profesional de la salud).</summary>
        public int? IdMedicoAsociado { get; set; }
    }
}
