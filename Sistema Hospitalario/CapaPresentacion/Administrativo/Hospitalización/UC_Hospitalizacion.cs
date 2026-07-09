using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService.CamaService;
using Sistema_Hospitalario.CapaNegocio.Servicios.InternacionService;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo
{
    /// <summary>
    /// Control de usuario que gestiona el módulo de hospitalización para el personal administrativo.
    /// Permite monitorear la ocupación de camas, registrar nuevas internaciones y procesar altas.
    /// </summary>
    public partial class UC_Hospitalizacion : UserControl
    {
        /// <summary>Servicio para operaciones relacionadas con pacientes.</summary>
        private readonly PacienteService pacienteService = new PacienteService();
        /// <summary>Servicio para la gestión de internaciones.</summary>
        private readonly InternacionService internacionService = new InternacionService();
        /// <summary>Servicio para la gestión de habitaciones.</summary>
        private readonly HabitacionService habitacionService = new HabitacionService();
        /// <summary>Servicio para la gestión de camas.</summary>
        private readonly CamaService camaService = new CamaService();

        /// <summary>Lista para almacenar las internaciones cargadas desde la base de datos.</summary>
        private List<InternacionDto> listaInternaciones = new List<InternacionDto>();

        /// <summary>BindingSource para el enlace de datos con la grilla de internaciones.</summary>
        private readonly BindingSource enlaceInternaciones = new BindingSource();

        /// <summary>Evento que notifica la solicitud de registrar una nueva internación.</summary>
        public event EventHandler RegistrarInternacionSolicitada;

        /// <summary>Evento que notifica la solicitud de finalizar (dar el alta) una internación específica.</summary>
        public event EventHandler<InternacionDto> FinalizarInternacionSolicitada;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_Hospitalizacion"/>.
        /// Configura la visualización de estadísticas de ocupación y carga el listado de internados.
        /// </summary>
        public UC_Hospitalizacion()
        {
            InitializeComponent();

            ConfigurarInformacionCajas();

            ConfigurarTablaInternaciones();
            ConfigurarEnlazadoDatosPacienteColumnas();
            CargarDatosDGV();
            CargarOpcionesDeFiltro();

            dgvInternaciones.CellDoubleClick += dgvInternaciones_CellDoubleClick;
        }

        // ============================ BOTÓN REGISTRAR INTERNACIÓN ============================
        private void BtnRegistrarInternacion_Click(object sender, EventArgs e)
        {
            RegistrarInternacionSolicitada?.Invoke(this, EventArgs.Empty);
        }

        // ============================ CONFIGURACION DE LAS CAJAS DE RESUMEN ============================
        private void ConfigurarInformacionCajas()
        {
            int totalHabitaciones = habitacionService.TotalHabitaciones();
            lblTotalHabitaciones.Text = totalHabitaciones.ToString();

            int totalCamasDisponibles = camaService.TotalCamasXEstado("Disponible");
            lblDisponibles.Text = totalCamasDisponibles.ToString();

            int totalCamas = camaService.TotalCamas();

            float porcentajeDisponible = totalCamas > 0 ? ((float)totalCamasDisponibles / totalCamas) * 100 : 0;
            lblPorcentajeDisponibles.Text = $"{porcentajeDisponible:F2}% de camas disponibles";

            int totalCamasOcupadas = camaService.TotalCamasXEstado("Ocupada");
            lblOcupadas.Text = totalCamasOcupadas.ToString();

            float porcentajeOcupadas = totalCamas > 0 ? ((float)totalCamasOcupadas / totalCamas) * 100 : 0;
            lblPorcentajeOcupadas.Text = $"{porcentajeOcupadas:F2}% de camas ocupadas";

            int totalPacientesInternados = pacienteService.ContarPorEstadoId("internado");
            lblPacientesInternados.Text = totalPacientesInternados.ToString();
        }

        // ============================ CONFIGURACION DEL DATAGRIDVIEW ============================

        private void ConfigurarTablaInternaciones()
        {
            dgvInternaciones.AutoGenerateColumns = false;
            dgvInternaciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvInternaciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInternaciones.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvInternaciones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            dgvInternaciones.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvInternaciones.ColumnHeadersHeight = 35;
            dgvInternaciones.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
        }

        // Método que carga los datos en el DataGridView de Internaciones
        private void CargarDatosDGV()
        {
            listaInternaciones = internacionService.ListadoInternacionDtos();
            enlaceInternaciones.DataSource = listaInternaciones;
            dgvInternaciones.DataSource = enlaceInternaciones;
        }

        // Enlazado de columnas
        private void ConfigurarEnlazadoDatosPacienteColumnas()
        {
            dgvInternaciones.AutoGenerateColumns = false;

            dgvInternaciones.Columns["colHabitacion"].DataPropertyName = "Nro_habitacion";
            dgvInternaciones.Columns["colPiso"].DataPropertyName = "Nro_piso";
            dgvInternaciones.Columns["colInternado"].DataPropertyName = "Internado";
            dgvInternaciones.Columns["colFechaIngreso"].DataPropertyName = "Fecha_ingreso";
            dgvInternaciones.Columns["colFechaEgreso"].DataPropertyName = "Fecha_egreso";
            dgvInternaciones.Columns["colCama"].DataPropertyName = "Id_Cama";

            // FORMATO SOLO FECHA
            dgvInternaciones.Columns["colFechaEgreso"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvInternaciones.Columns["colFechaIngreso"].DefaultCellStyle.Format = "dd/MM/yyyy";
        }

        // Carga las opciones de filtro en el ComboBox
        private void CargarOpcionesDeFiltro()
        {
            if (cboCampo == null) return;

            cboCampo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCampo.Items.Clear();
            cboCampo.Items.AddRange(new[] {
                "Todos",
                "Habitación",
                "Piso",
                "Internado",
                "Fecha ingreso",
                "Cama"
            });
            cboCampo.SelectedIndex = 0;
        }

        // ============================ APLICAR FILTRO (TEXTO + FECHA INGRESO + FECHA EGRESO) ============================

        private void AplicarFiltro(
            string campo,
            string texto,
            DateTime? fechaIngreso,
            DateTime? fechaEgreso)
        {
            string busqueda = (texto ?? "").Trim().ToLowerInvariant();
            IEnumerable<InternacionDto> query = listaInternaciones;

            // ===== 1) FILTRO POR FECHA DE INGRESO =====
            if (fechaIngreso.HasValue)
            {
                var f = fechaIngreso.Value.Date;
                query = query.Where(i => i.Fecha_ingreso.Date == f);
            }

            // ===== 2) FILTRO POR FECHA DE EGRESO =====
            if (fechaEgreso.HasValue)
            {
                var f = fechaEgreso.Value.Date;
                query = query.Where(i =>
                    i.Fecha_egreso.HasValue &&
                    i.Fecha_egreso.Value.Date == f);
            }

            // ===== 3) FILTRO POR TEXTO (COMBO + TXT) =====
            if (!string.IsNullOrEmpty(busqueda))
            {
                switch (campo)
                {
                    case "Habitación":
                        if (int.TryParse(busqueda, out int numHab))
                            query = query.Where(i => i.Nro_habitacion == numHab);
                        else
                            query = Enumerable.Empty<InternacionDto>();
                        break;

                    case "Piso":
                        if (int.TryParse(busqueda, out int piso))
                            query = query.Where(i => i.Nro_piso == piso);
                        else
                            query = Enumerable.Empty<InternacionDto>();
                        break;

                    case "Internado":
                        query = query.Where(i => (i.Internado ?? "")
                            .ToLowerInvariant()
                            .Contains(busqueda));
                        break;

                    case "Fecha ingreso":
                        // mantengo tu lógica textual de fecha
                        if (DateTime.TryParse(texto, out DateTime fechaExacta))
                        {
                            var fdi = fechaExacta.Date;
                            query = query.Where(i => i.Fecha_ingreso.Date == fdi);
                        }
                        else
                        {
                            char[] separadores = new[] { '/', '-', ' ' };
                            var partes = texto.Split(separadores, StringSplitOptions.RemoveEmptyEntries);

                            if (partes.Length == 2 &&
                                int.TryParse(partes[0], out int mes) &&
                                int.TryParse(partes[1], out int anio) &&
                                mes >= 1 && mes <= 12)
                            {
                                query = query.Where(i =>
                                    i.Fecha_ingreso.Month == mes &&
                                    i.Fecha_ingreso.Year == anio);
                            }
                            else if (int.TryParse(texto, out int mesSolo) &&
                                     mesSolo >= 1 && mesSolo <= 12)
                            {
                                query = query.Where(i =>
                                    i.Fecha_ingreso.Month == mesSolo);
                            }
                            else
                            {
                                query = query.Where(i =>
                                    i.Fecha_ingreso.ToShortDateString()
                                    .ToLowerInvariant()
                                    .Contains(busqueda));
                            }
                        }
                        break;

                    case "Cama":
                        if (int.TryParse(busqueda, out int idCama))
                            query = query.Where(i => i.Id_cama == idCama);
                        else
                            query = Enumerable.Empty<InternacionDto>();
                        break;

                    case "Todos":
                    default:
                        query = query.Where(i =>
                            i.Nro_habitacion.ToString().Contains(busqueda) ||
                            i.Nro_piso.ToString().Contains(busqueda) ||
                            (i.Internado ?? "").ToLowerInvariant().Contains(busqueda) ||
                            i.Fecha_ingreso.ToShortDateString().ToLowerInvariant().Contains(busqueda) ||
                            i.Id_cama.ToString().Contains(busqueda)
                        );
                        break;
                }
            }

            // ===== 4) ACTUALIZAR GRILLA =====
            enlaceInternaciones.DataSource = query
                .OrderBy(i => i.Nro_habitacion)
                .ThenBy(i => i.Id_cama)
                .ToList();

            enlaceInternaciones.ResetBindings(false);
        }

        // ============================ EVENTOS DE BOTONES ============================

        // Botón Buscar
        private void btnBuscar_Click_1(object sender, EventArgs e)
        {
            var campo = cboCampo.SelectedItem?.ToString() ?? "Todos";
            var texto = txtBuscar.Text;

            DateTime? fechaIngreso = dtpIngreso.Checked ? dtpIngreso.Value.Date : (DateTime?)null;
            DateTime? fechaEgreso = dtpEgreso.Checked ? dtpEgreso.Value.Date : (DateTime?)null;

            AplicarFiltro(campo, texto, fechaIngreso, fechaEgreso);
        }

        // Botón Limpiar
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cboCampo != null) cboCampo.SelectedIndex = 0;

            dtpIngreso.Checked = false;
            dtpEgreso.Checked = false;

            // Sin texto ni fechas → lista completa ordenada
            AplicarFiltro("Todos", "", null, null);
        }

        private void dgvInternaciones_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var internacionSeleccionada = dgvInternaciones.Rows[e.RowIndex].DataBoundItem as InternacionDto;

            if (internacionSeleccionada != null)
            {
                FinalizarInternacionSolicitada?.Invoke(this, internacionSeleccionada);
            }
        }
    }
}
