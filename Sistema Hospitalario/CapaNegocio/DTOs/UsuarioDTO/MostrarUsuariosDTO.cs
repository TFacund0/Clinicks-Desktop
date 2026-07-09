namespace Sistema_Hospitalario.CapaNegocio.DTOs.UsuarioDTO
{
    /// <summary>
    /// DTO diseñado para listar los usuarios en las pantallas de administración de seguridad.
    /// </summary>
    public class MostrarUsuariosDTO
    {
        /// <summary>ID único del usuario.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Nombre real de la persona.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido real de la persona.</summary>
        public string Apellido { get; set; }

        /// <summary>Nombre de usuario (Login).</summary>
        public string NombreUsuario { get; set; }

        /// <summary>Estado de la cuenta (ej. 'Activo', 'Bloqueado').</summary>
        public string Estado { get; set; }

        /// <summary>Nombre del rol asignado.</summary>
        public string Rol { get; set; }

        /// <summary>Correo electrónico asociado.</summary>
        public string Correo { get; set; }
    }
}
