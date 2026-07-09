using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;
using Sistema_Hospitalario.CapaPresentacion.Medico.Turnos;
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
    /// Control de usuario que gestiona la agenda de turnos del médico logueado.
    /// Permite visualizar, filtrar por paciente/estado/fecha y actualizar el estado de los turnos.
    /// </summary>
    public partial class panel1 : UserControl
    {
        /// <summary>Servicio para la gestión de turnos.</summary>
        private TurnoService _turnoService = new TurnoService(new TurnoRepository());

        /// <summary>ID del médico que inició sesión.</summary>
        private int _idMedicoLogueado;

        /// <summary>Lista base que contiene todos los turnos del médico sin filtrar.</summary>
        private List<ListadoTurno> _turnosBase = new List<ListadoTurno>();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="panel1"/>.
        /// Configura la grilla, los filtros y carga los turnos iniciales.
        /// </summary>
        public panel1()
        {
            InitializeComponent();

            ConfigurarEstilosGrilla();
            CargarOpcionesDeFiltro();
            CargarTurnos();        // carga base y aplica filtro “Todos”
        }


        // ===================== CARGAR TURNOS INICIALES =====================
        private void CargarTurnos()
        {
            if (SesionUsuario.IdMedicoAsociado.HasValue)
            {
                _idMedicoLogueado = SesionUsuario.IdMedicoAsociado.Value;
            }
            else
            {
                MessageBox.Show("Error: No se pudo identificar al médico logueado.",
                                "Error de Sesión",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _turnosBase = _turnoService.ListarTurnos()
                                           .Where(t => t.Id_medico == _idMedicoLogueado)
                                           .ToList();

                AplicarFiltro("Todos", "", null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la agenda: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void CargarOpcionesDeFiltro()
        {
            if (cboCampo == null) return;

            cboCampo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCampo.Items.Clear();
            cboCampo.Items.AddRange(new[] { "Todos", "Paciente", "Estado" });
            cboCampo.SelectedIndex = 0;
        }

        // ===================== REFRESCAR AGENDA Y CONTADORES =====================
        private void RefrescarAgenda()
        {
            try
            {
                _turnosBase = _turnoService.ListarTurnos()
                                           .Where(t => t.Id_medico == _idMedicoLogueado)
                                           .ToList();

                string campo = cboCampo.SelectedItem?.ToString() ?? "Todos";
                string texto = txtBuscar.Text;

                DateTime? desde = dtpDesde.Checked ? dtpDesde.Value.Date : (DateTime?)null;

                AplicarFiltro(campo, texto, desde);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la agenda: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ===================== CONFIGURACIÓN ESTILOS GRILLA =====================
        private void ConfigurarEstilosGrilla()
        {

            dgvTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.RowHeadersVisible = false;
            dgvTurnos.BackgroundColor = Color.White;
            dgvTurnos.BorderStyle = BorderStyle.None;
            dgvTurnos.EnableHeadersVisualStyles = false;

            dgvTurnos.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvTurnos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTurnos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dgvTurnos.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvTurnos.DefaultCellStyle.ForeColor = Color.Black;
            dgvTurnos.DefaultCellStyle.Font = new Font("Segoe UI", 9);

        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cboCampo != null) cboCampo.SelectedIndex = 0;

            dtpDesde.Checked = false;

            AplicarFiltro("Todos", "", null);
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string campo = cboCampo.SelectedItem?.ToString() ?? "Todos";
            string texto = txtBuscar.Text;

            DateTime? desde = dtpDesde.Checked ? dtpDesde.Value.Date : (DateTime?)null;

            AplicarFiltro(campo, texto, desde);
        }


        private void dgvTurnos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Si hace clic en el header

            int idTurno = (int)dgvTurnos.Rows[e.RowIndex].Cells["Id_turno"].Value;
            string estadoActual = (string)dgvTurnos.Rows[e.RowIndex].Cells["Estado"].Value;

            Form_CambiarEstadoTurno formDialogo = new Form_CambiarEstadoTurno(estadoActual);
            DialogResult resultado = formDialogo.ShowDialog();

            if (resultado == DialogResult.OK)
            {
                try
                {
                    int nuevoEstadoId = formDialogo.NuevoEstadoId;
                    if (_turnoService.ActualizarEstadoTurno(idTurno, nuevoEstadoId))
                    {
                        RefrescarAgenda();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el turno (quizás fue eliminado).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el estado: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Aplica los criterios de filtrado sobre la lista base de turnos y actualiza la visualización de la grilla.
        /// </summary>
        /// <param name="campo">Nombre del campo por el cual filtrar (Paciente, Estado, Todos).</param>
        /// <param name="texto">Texto de búsqueda.</param>
        /// <param name="fechaDesde">Fecha específica para filtrar el día de los turnos.</param>
        private void AplicarFiltro(string campo, string texto, DateTime? fechaDesde)
        {
            IEnumerable<ListadoTurno> query = _turnosBase;

            // --- Filtro por fecha (día exacto) ---
            if (fechaDesde.HasValue)
            {
                var dia = fechaDesde.Value.Date;
                var diaSiguiente = dia.AddDays(1);

                // Turnos cuyo Fecha_Del_Turno esté en [dia, diaSiguiente)
                query = query.Where(t =>
                    t.Fecha_Del_Turno >= dia &&
                    t.Fecha_Del_Turno < diaSiguiente);
            }

            // --- Filtro por texto ---
            string busqueda = (texto ?? "").Trim().ToLowerInvariant();

            if (!string.IsNullOrEmpty(busqueda))
            {
                switch (campo)
                {
                    case "Paciente":
                        query = query.Where(t =>
                            (t.Paciente ?? "").ToLowerInvariant().Contains(busqueda));
                        break;

                    case "Estado":
                        query = query.Where(t =>
                            (t.Estado ?? "").ToLowerInvariant().Contains(busqueda));
                        break;

                    case "Todos":
                    default:
                        query = query.Where(t =>
                            (t.Paciente ?? "").ToLowerInvariant().Contains(busqueda) ||
                            (t.Estado ?? "").ToLowerInvariant().Contains(busqueda) ||
                            t.Fecha_Del_Turno.ToString("dd/MM/yyyy").Contains(busqueda));
                        break;
                }
            }

            var listaFinal = query.OrderBy(t => t.Fecha_Del_Turno).ToList();

            // --- Actualizar grilla ---
            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = listaFinal;

            if (dgvTurnos.Columns["Id_turno"] != null)
                dgvTurnos.Columns["Id_turno"].Visible = false;

            if (dgvTurnos.Columns["Id_medico"] != null)
                dgvTurnos.Columns["Id_medico"].Visible = false;

            if (dgvTurnos.Columns["FechaTurno"] != null)
                dgvTurnos.Columns["FechaTurno"].Visible = false;

            // --- Contadores ---
            lblTotalPendientes.Text = listaFinal.Count(t =>
                t.Estado.Equals("pendiente", StringComparison.OrdinalIgnoreCase)).ToString();

            lblTotalCompletadas.Text = listaFinal.Count(t =>
                t.Estado.Equals("atendido", StringComparison.OrdinalIgnoreCase)).ToString();

            lblTotalRecanceladas.Text = listaFinal.Count(t =>
                t.Estado.Equals("cancelado", StringComparison.OrdinalIgnoreCase)).ToString();
        }
    }
}
