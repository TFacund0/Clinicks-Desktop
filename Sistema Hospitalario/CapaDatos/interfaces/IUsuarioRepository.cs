using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la seguridad y gestión de usuarios del sistema.
    /// </summary>
    public interface IUsuarioRepository
    {
        /// <summary>
        /// Inserta un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="Nombre">Nombre real del usuario.</param>
        /// <param name="Apellido">Apellido real del usuario.</param>
        /// <param name="NombreUsuario">Nombre de acceso (username).</param>
        /// <param name="Estado">ID del estado de cuenta.</param>
        /// <param name="Rol">ID del rol asignado.</param>
        /// <param name="Password">Hash de la contraseña.</param>
        /// <param name="correo">Correo electrónico.</param>
        /// <param name="IdMedico">ID del médico asociado (opcional).</param>
        /// <returns>Tupla con el resultado de la operación.</returns>
        (bool Ok, int IdGenerado, string Error) Insertar(string Nombre, string Apellido, string NombreUsuario, int Estado, int Rol, string Password, string correo, int? IdMedico);

        /// <summary>
        /// Elimina un usuario de la base de datos.
        /// </summary>
        /// <param name="IdUsuario">ID del usuario a eliminar.</param>
        void Eliminar(int IdUsuario);

        /// <summary>
        /// Obtiene el listado de usuarios registrados con sus nombres de rol y estado.
        /// </summary>
        /// <returns>Lista de <see cref="MostrarUsuariosDto"/>.</returns>
        List<MostrarUsuariosDto> ObtenerUsuarios();

        /// <summary>
        /// Verifica si un nombre de usuario ya está registrado en el sistema.
        /// </summary>
        /// <param name="username">Nombre de usuario a consultar.</param>
        /// <returns>True si el nombre de usuario ya existe.</returns>
        bool ExisteUsername(string username);

        /// <summary>
        /// Recupera la información necesaria para el proceso de validación de credenciales.
        /// </summary>
        /// <param name="username">Nombre de usuario.</param>
        /// <returns>Objeto <see cref="DatosLoginUsuarioDto"/> con el hash almacenado y datos de perfil.</returns>
        DatosLoginUsuarioDto ObtenerUsuarioParaLogin(string username);

        /// <summary>
        /// Actualiza el hash de contraseña almacenado de un usuario.
        /// Se utiliza para migrar hashes legacy a un formato más seguro.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <param name="nuevoHash">Nuevo hash de contraseña a almacenar.</param>
        void ActualizarPasswordHash(int idUsuario, string nuevoHash);
    }
}
