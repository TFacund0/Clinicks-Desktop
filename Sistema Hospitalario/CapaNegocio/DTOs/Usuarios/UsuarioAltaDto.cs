namespace Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios
{
    /// <summary>
    /// DTO que encapsula los datos necesarios para registrar un nuevo usuario en el sistema.
    /// </summary>
    public class UsuarioAltaDto
    {
        /// <summary>Nombre del nuevo usuario.</summary>
        public string Nombre { get; set; }

        /// <summary>Apellido del nuevo usuario.</summary>
        public string Apellido { get; set; }

        /// <summary>Nombre de cuenta (Login).</summary>
        public string NombreUsuario { get; set; }

        /// <summary>ID del estado inicial (ej. 1 para Activo).</summary>
        public int IdEstado { get; set; }

        /// <summary>ID del rol a asignar (ej. 1 para Administrador, 2 para Médico).</summary>
        public int IdRol { get; set; }

        /// <summary>Contraseña en texto plano (será hasheada antes de persistir).</summary>
        public string Password { get; set; }

        /// <summary>Correo electrónico del usuario.</summary>
        public string Correo { get; set; }

        /// <summary>ID del médico asociado (si el rol es Médico).</summary>
        public int? IdMedico { get; set; }
    }
}
