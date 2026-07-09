using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Sistema_Hospitalario.CapaPresentacion.Administrativo.UC_Pacientes;


namespace Sistema_Hospitalario.CapaPresentacion.Medico
{

    /// <summary>
    /// Control de usuario que muestra el listado de pacientes para el perfil médico.
    /// Incluye funcionalidades de filtrado por nombre, apellido o DNI y acceso al detalle del paciente.
    /// </summary>
    public partial class UC_PacientesM : UserControl
    {
        /// <summary>Servicio de operaciones médicas.</summary>
        private MedicoService _service = new MedicoService();

        /// <summary>Fuente de datos para el enlace con el DataGridView.</summary>
        private readonly BindingSource _enlacePacientes = new BindingSource();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_PacientesM"/>.
        /// Configura la interfaz, carga los filtros iniciales y puebla la grilla.
        /// </summary>
        public UC_PacientesM()
        {
            InitializeComponent();

            ConfigurarEstilosGrilla();
            CargarOpcionesDeFiltro();
            RefrescarGrilla();
            CargarContadores();

            // Suscripción al evento de doble clic para abrir detalles
            dgvPacientes.CellDoubleClick += DgvPacientes_CellDoubleClick;
        }

        /// <summary>
        /// Actualiza el contenido de la grilla de pacientes aplicando los filtros especificados.
        /// </summary>
        /// <param name="nombre">Filtro por nombre.</param>
        /// <param name="apellido">Filtro por apellido.</param>
        /// <param name="dni">Filtro por DNI.</param>
        /// <param name="fechaTurno">Filtro por fecha de turno (opcional).</param>
        private void RefrescarGrilla(string nombre = null, string apellido = null, string dni = null, DateTime? fechaTurno = null)
        {
            try
            {
                var lista = _service.ObtenerPacientes(nombre, apellido, dni, fechaTurno);

                _enlacePacientes.DataSource = lista;
                dgvPacientes.DataSource = _enlacePacientes;

                if (dgvPacientes.Columns.Contains("IdPaciente"))
                {
                    dgvPacientes.Columns["IdPaciente"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pacientes: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarOpcionesDeFiltro()
        {
            if (cboCampo == null) return;

            cboCampo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCampo.Items.Clear();

            // Los textos son los que va a ver el usuario
            cboCampo.Items.AddRange(new[] {
                "Todos",
                "Nombre",
                "Apellido",
                "DNI"
                // si después querés, agregamos "Fecha último turno"
            });

            cboCampo.SelectedIndex = 0;
        }


        private void ConfigurarEstilosGrilla()
        {
            dgvPacientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPacientes.RowHeadersVisible = false;
            dgvPacientes.BackgroundColor = Color.White;
            dgvPacientes.BorderStyle = BorderStyle.None;
            dgvPacientes.EnableHeadersVisualStyles = false;

            dgvPacientes.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvPacientes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPacientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvPacientes.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvPacientes.DefaultCellStyle.ForeColor = Color.Black;
            dgvPacientes.DefaultCellStyle.Font = new Font("Segoe UI", 9);
            
            if (dgvPacientes.Columns.Contains("Historial"))
            {
                dgvPacientes.Columns["Historial"].DefaultCellStyle.NullValue = "Ver";
            }
        }

        private void DgvPacientes_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                DataGridViewRow fila = dgvPacientes.Rows[e.RowIndex];
                
                MenuMedicos menu = this.FindForm() as MenuMedicos;
                PacienteListadoMedicoDto pacienteSeleccionado = fila.DataBoundItem as PacienteListadoMedicoDto;

                if (menu != null)
                {
                    menu.AbrirUserControl(new UC_DetallePaciente(pacienteSeleccionado));
                }
            }
        }
        private void CargarContadores()
        {
            try
            {
                lblTotalPacientes.Text = _service.ObtenerConteoTotalPacientes().ToString();
            }
            catch (Exception ex)
            {
                lblTotalPacientes.Text = "N/A";
                MessageBox.Show("Error al cargar contadores: " + ex.Message);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string campo = cboCampo.SelectedItem?.ToString() ?? "Todos";
            string valor = (txtBuscar.Text ?? "").Trim();

            // Parámetros para el service
            string nombre = null;
            string apellido = null;
            string dni = null;
            DateTime? fechaTurno = null;   // por ahora no lo usamos

            if (!string.IsNullOrWhiteSpace(valor))
            {
                switch (campo)
                {
                    case "Nombre":
                        nombre = valor;
                        break;

                    case "Apellido":
                        apellido = valor;
                        break;

                    case "DNI":
                        dni = valor;
                        break;

                    case "Todos":
                    default:
                        // Todos: usamos el mismo valor para buscar en varios campos
                        nombre = valor;
                        apellido = valor;
                        dni = valor;
                        break;
                }
            }

            RefrescarGrilla(nombre, apellido, dni, fechaTurno);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cboCampo != null) cboCampo.SelectedIndex = 0;

            // Volver a mostrar todo
            RefrescarGrilla();
        }
    }
}
