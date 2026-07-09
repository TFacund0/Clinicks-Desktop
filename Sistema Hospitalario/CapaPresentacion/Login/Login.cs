using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;
using Sistema_Hospitalario.CapaNegocio.Servicios.UsuarioService;
using Sistema_Hospitalario.CapaPresentacion.Administrador;
using Sistema_Hospitalario.CapaPresentacion.Administrativo;
using Sistema_Hospitalario.CapaPresentacion.Gerente;
using Sistema_Hospitalario.CapaPresentacion.Medico;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsInicio_de_sesion
{
    /// <summary>
    /// Formulario de acceso al sistema. Gestiona la autenticación de usuarios y la redirección según el rol.
    /// </summary>
    public partial class Login : Form
    {
        /// <summary>
        /// Servicio para manejar la lógica de negocio de usuarios.
        /// </summary>
        UsuarioService _usuarioService = new UsuarioService();

        /// <summary>
        /// Inicializa una nueva instancia del formulario <see cref="Login"/>.
        /// </summary>
        public Login()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Maneja el evento de clic en el botón ingresar.
        /// Realiza la validación de credenciales y redirige al menú correspondiente según el rol del usuario.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void BotonIngresar_Click_1(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Text.Trim();

            try
            {
                // ================== 1) Usuarios reales en BD ==================
                UsuarioLoginResultadoDto resultadoLogin = _usuarioService.ValidarCredenciales(usuario, contraseña);

                // Si el servicio devuelve null o LoginExitoso == false, credenciales inválidas
                if (resultadoLogin == null || !resultadoLogin.LoginExitoso)
                {
                    MessageBox.Show(
                        resultadoLogin?.MensajeError ?? "Usuario o contraseña incorrectos.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                // Si llegamos acá, el login fue exitoso
                SesionUsuario.Login(resultadoLogin);

                this.Hide();

                switch (resultadoLogin.NombreRol.ToLower())
                {
                    case "administrativo":
                        new MenuAdministrativo().ShowDialog();
                        break;

                    case "medico":
                        new MenuMedicos().ShowDialog();
                        break;

                    case "administrador":
                    case "moderador":
                        new MenuModer().ShowDialog();
                        break;

                    case "gerente":
                        new MenuGerente().ShowDialog();
                        break;

                    default:
                        MessageBox.Show("El rol del usuario no tiene una pantalla asignada.",
                                        "Rol no configurado",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Warning);
                        break;
                }

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al inicializar: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

    }
}
