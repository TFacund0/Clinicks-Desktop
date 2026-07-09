using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using Sistema_Hospitalario.CapaPresentacion.Medico.Pacientes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Medico
{
    /// <summary>
    /// Formulario principal para el perfil del Médico. 
    /// Proporciona acceso a la gestión de pacientes, turnos y realización de consultas.
    /// </summary>
    public partial class MenuMedicos : Form
    {
        /// <summary>
        /// Inicializa una nueva instancia del formulario <see cref="MenuMedicos"/>.
        /// Configura el perfil del médico basado en la sesión activa.
        /// </summary>
        public MenuMedicos()
        {
            InitializeComponent();

            ConfigurarPerfilMedico();
        }

        /// <summary>
        /// Recupera los datos del médico desde el servicio utilizando la información de la sesión
        /// y actualiza las etiquetas de la interfaz (Nombre y Especialidad).
        /// </summary>
        public void ConfigurarPerfilMedico()
        {
            int _idMedicoLogueado;
            var _medicoService = new MedicoService();

            // Obtenemos el ID del médico de la sesión
            if (!SesionUsuario.IdMedicoAsociado.HasValue)
            {
                MessageBox.Show("Error fatal: No se pudo identificar al médico. Cierre sesión y vuelva a intentarlo.", "Error de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _idMedicoLogueado = SesionUsuario.IdMedicoAsociado.Value;

            // Obtenemos los datos del médico
            var medico = _medicoService.ObtenerMedicoPorId(_idMedicoLogueado);
            if (medico != null)
            {
                lblMedico.Text = $"Dr. {medico.Nombre} {medico.Apellido}";
                lblEspecialidad.Text = $"Especialidad: {medico.Especialidad}";
            }
            else
            {
                MessageBox.Show("Error al cargar el perfil del médico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        /// <summary>
        /// Carga dinámicamente un Control de Usuario (UserControl) dentro del panel principal del menú.
        /// </summary>
        /// <param name="uc">El control de usuario que se desea mostrar.</param>
        public void AbrirUserControl(UserControl uc)
        {
            panelMenu.Controls.Clear();   // Limpia el panel
            uc.Dock = DockStyle.Fill;           // Que ocupe todo el espacio disponible
            panelMenu.Controls.Add(uc);   // Lo agrega al panel
            uc.BringToFront();                  // Lo trae al frente
        }

        private void Boton_Click(object sender, EventArgs e)
        {
            // sender es el botón que se clickeó
            Button btnClic = (Button)sender;

            // Recorre todos los botones dentro del mismo contenedor
            foreach (Control ctrl in panelMenu.Controls)
            {
                if (ctrl is Button b)
                    b.BackColor =  Color.FromArgb(49, 163, 166);
            }

            // Marca el botón presionado
            btnClic.BackColor = Color.LightSeaGreen;
        }
        

        private void Btn_pacientes_Click(object sender, EventArgs e)
        {
            
            Boton_Click(sender, e);
            AbrirUserControl(new UC_PacientesM()); // Poner el UserControl de Pacientes
        }

        private void Btn_turnos_Click(object sender, EventArgs e)
        {
            
            Boton_Click(sender, e);
            AbrirUserControl(new panel1()); // Poner el UserControl de Turnos

        }

        private void Btn_consulta_Click(object sender, EventArgs e)
        {
            Boton_Click(sender, e);
            AbrirUserControl(new ConsultaMedica()); // Poner el UserControl de Consultas
        }
       
        private void btn_salir_Click(object sender, EventArgs e)
        {
            // Confirmación para salir

            DialogResult dr = MessageBox.Show("¿Seguro que desea salir?",
                                      "Confirmación",
                                      MessageBoxButtons.YesNo
                                    );

            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
