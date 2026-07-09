using System;
using System.Windows.Forms;

using Sistema_Hospitalario.CapaPresentacion.Administrativo.Hospitalización;
using Sistema_Hospitalario.CapaPresentacion.Administrativo.Pacientes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using System.Linq.Expressions;
using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo
{
    /// <summary>
    /// Formulario principal para el personal Administrativo.
    /// Gestiona la navegación centralizada entre Pacientes, Turnos y Hospitalizaciones.
    /// </summary>
    public partial class MenuAdministrativo : Form
    {
        /// <summary>
        /// Inicializa una nueva instancia del formulario <see cref="MenuAdministrativo"/>.
        /// </summary>
        public MenuAdministrativo()
        {
            InitializeComponent();
            this.Text = "Sistema Hospitalario";
        }

        // ======================= NAVEGACIÓN CENTRAL =======================
        
        /// <summary>
        /// Libera los controles previos y carga un nuevo <see cref="UserControl"/> en el panel contenedor principal.
        /// </summary>
        /// <param name="uc">El control de usuario que se desea mostrar.</param>
        private void AbrirUserControl(UserControl uc)
        {
            try
            {
                foreach (Control c in panelContenedor.Controls) c.Dispose();
                panelContenedor.Controls.Clear();

                uc.Dock = DockStyle.Fill;
                panelContenedor.Controls.Add(uc);
                uc.BringToFront();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar: {ex.Message}",
                   "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ======================= HOME =======================
        private void btnHome_Click(object sender, EventArgs e)
        {
            AbrirUserControl(new UC_HomeGerente());
        }

        // ======================= PACIENTES =======================
        private void btnPacientes_Click(object sender, EventArgs e)
        {
            AbrirUserControl(CrearUCPaciente());
        }

        // Manejador de eventos para crear y configurar el UserControl de Pacientes
        private UC_Pacientes CrearUCPaciente()
        {
            var ucPacientes = new UC_Pacientes();

            ucPacientes.RegistrarPacienteSolicitado += (_, __) => AbrirRegistrarPaciente();
            ucPacientes.VerPacienteSolicitado += (_, p) => AbrirVisualizarPaciente(p);

            return ucPacientes;
        }

        // Abrir el UserControl para registrar un nuevo paciente
        private void AbrirRegistrarPaciente()
        {
            var ucRegistrar = new UC_RegistrarPaciente();

            ucRegistrar.CancelarRegistroSolicitado += (_, __) => AbrirUserControl(CrearUCPaciente());

            AbrirUserControl(ucRegistrar);
        }

        // Abrir el UserControl para ver los detalles de un paciente
        private void AbrirVisualizarPaciente(PacienteDetalleDto pacienteDetalle)
        {
            var ucVisualizar = new UC_VisualizarPaciente(pacienteDetalle);

            ucVisualizar.CancelarVisualizacionSolicitada += (_, __) =>
                AbrirUserControl(CrearUCPaciente());

            AbrirUserControl(ucVisualizar);
        }

        // ======================= TURNOS =======================
        private void btnTurnos_Click(object sender, EventArgs e)
        {
            AbrirUserControl(CrearUCTurnos());
        }

        // Manejador de eventos para crear y configurar el UserControl de Turnos
        private UC_Turnos CrearUCTurnos()
        {
            var ucTurnos = new UC_Turnos();

            ucTurnos.RegistrarTurnoSolicitado += (_, __) => AbrirRegistrarTurno();
            ucTurnos.VerTurnoSolicitado += (_, t) => AbrirVisualizarTurnos(t);

            return ucTurnos;
        }

        // Abrir el UserControl para registrar un nuevo turno
        private void AbrirRegistrarTurno()
        {
            var ucRegistrar = new Turnos.UC_RegistrarTurno();

            ucRegistrar.CancelarTurnoSolicitado += (_, __) => AbrirUserControl(CrearUCTurnos());

            AbrirUserControl(ucRegistrar);
        }

        // Abrir el UserControl para ver los detalles de un turno
        private void AbrirVisualizarTurnos(TurnoDto turno)
        {
            var ucVisualizar = new Turnos.UC_VisualizarTurno(turno);

            ucVisualizar.CancelarVisualizacionSolicitada += (_, __) => AbrirUserControl(CrearUCTurnos());

            AbrirUserControl(ucVisualizar);
        }

        // ======================= HOSPITALIZACIÓN =======================
        private void btnHospitalizacion_Click(object sender, EventArgs e)
        {
            AbrirUserControl(CrearUCHospitalizacion());
        }

        // Manejador de eventos para crear y configurar el UserControl de Hospitalización
        private UC_Hospitalizacion CrearUCHospitalizacion()
        {
            var uc = new UC_Hospitalizacion();

            uc.RegistrarInternacionSolicitada += (_, __) => AbrirRegistrarInternacion();

            // Cuando el UC de Hospitalización pide finalizar una internación:
            uc.FinalizarInternacionSolicitada += (_, internacion) =>
                AbrirFinalizarInternacion(internacion);

            return uc;
        }

        // Abrir el UC_FinalizarInternacion con la internación seleccionada
        private void AbrirFinalizarInternacion(InternacionDto internacion)
        {
            var ucFinalizar = new UC_FinalizarInternacion(internacion);

            // Cuando cancela, volvés a la pantalla de hospitalización
            ucFinalizar.CancelarFinalizacionSolicitada += (_, __) =>
                AbrirUserControl(CrearUCHospitalizacion());

            AbrirUserControl(ucFinalizar);
        }


        // Abrir el UserControl para registrar una nueva internación
        private void AbrirRegistrarInternacion()
        {
            var ucRegistrar = new UC_RegistrarInternacion();

            ucRegistrar.CancelarInternacionSolicitada += (_, __) =>
                AbrirUserControl(CrearUCHospitalizacion());

            AbrirUserControl(ucRegistrar);
        }

        // ======================= SALIR =======================
        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("¿Seguro que desea salir?",
                                              "Confirmación",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
