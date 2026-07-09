using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios
{
    /// <summary>
    /// DTO que encapsula el resultado de un intento de inicio de sesión y la información de la sesión resultante.
    /// </summary>
    public class UsuarioLoginResultadoDto
    {
        /// <summary>Indica si las credenciales fueron válidas y el acceso fue concedido.</summary>
        public bool LoginExitoso { get; set; }

        /// <summary>ID del usuario autenticado.</summary>
        public int IdUsuario { get; set; }

        /// <summary>Nombre de cuenta (Login).</summary>
        public string Username { get; set; }

        /// <summary>Nombre del rol del usuario (ej. 'Administrador').</summary>
        public string NombreRol { get; set; }

        /// <summary>ID del médico asociado (si aplica).</summary>
        public int? IdMedicoAsociado { get; set; }

        /// <summary>Mensaje de error a mostrar cuando <see cref="LoginExitoso"/> es falso.</summary>
        public string MensajeError { get; set; }
    }
}
