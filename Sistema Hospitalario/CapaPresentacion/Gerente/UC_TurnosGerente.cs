using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios.EstadisticasService;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Sistema_Hospitalario.CapaPresentacion.Gerente
{
    /// <summary>
    /// Control de usuario que proporciona un panel de gestión de turnos para el perfil de Gerente.
    /// Incluye métricas rápidas (pendientes, completados, cancelados), listado con filtros y gráficos de tendencias de citas.
    /// </summary>
    public partial class UC_TurnosGerente : UserControl
    {
        /// <summary>Servicio para la gestión de lógica de negocio de turnos.</summary>
        TurnoService _turnoService = new TurnoService();

        /// <summary>Lista interna que almacena los turnos cargados.</summary>
        List<ListadoTurno> _listadoTurnos = new List<ListadoTurno>();

        /// <summary>BindingSource para el enlace de datos entre la lista de turnos y la grilla.</summary>
        BindingSource enlaceTurnos = new BindingSource();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_TurnosGerente"/>.
        /// Configura los indicadores, la tabla de datos y procesa la generación de gráficos de turnos por día y estado.
        /// </summary>
        public UC_TurnosGerente()
        {
            InitializeComponent();

            ConfigurarLabelsDatosTurno();
            ConfigurarActividad();

            ConfigurarEnlazadoDatosTurnoColumnas();
            CargarTurnosDGV();
            CargarOpcionesDeFiltro();

            CargarGraficoTurnosPorDia();
            CargarGraficoEstadoTurnos();
        }

        // ===================== CONFIGURACIÓN DE CONTROLES DE DATOS =====================
        private void ConfigurarLabelsDatosTurno()
        {
            lblTurnosPendientes.Text = _turnoService.CantidadTurnosPendientes().ToString();
            lblTurnosCompletados.Text = _turnoService.CantidadTurnosPorEstado("Atendido").ToString();
            lblTurnosCancelados.Text = _turnoService.CantidadTurnosPorEstado("Cancelado").ToString();
        }

        // ===================== CONFIGURACIÓN DEL DATAGRIDVIEW =====================
        private void ConfigurarActividad()
        {
            dgvTurnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvTurnos.DataSource = null;

            dgvTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgvTurnos.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);
            dgvTurnos.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvTurnos.ColumnHeadersHeight = 35;
            dgvTurnos.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke;
        }

        // ===================== ENLAZADO DE DATOS =====================
        // Configura el enlace de datos entre las columnas del DataGridView y las propiedades del objeto ListadoTurno
        private void ConfigurarEnlazadoDatosTurnoColumnas()
        {
            dgvTurnos.AutoGenerateColumns = false;

            dgvTurnos.Columns["colPaciente"].DataPropertyName = "Paciente";
            dgvTurnos.Columns["colMedico"].DataPropertyName = "Medico";
            dgvTurnos.Columns["colHora"].DataPropertyName = "FechaTurno";
            dgvTurnos.Columns["colEstado"].DataPropertyName = "Estado";
        }

        // ===================== CARGA DE DATOS =====================
        // Carga los turnos en el DataGridView
        public void CargarTurnosDGV()
        {
            _listadoTurnos = _turnoService.ListarTurnos();
            enlaceTurnos.DataSource = _listadoTurnos;
            dgvTurnos.DataSource = enlaceTurnos;
        }

        // ===================== BOTÓN BUSCAR TURNO =====================
        // Filtra los turnos según el campo y el texto ingresado
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var campo = cboCampoFiltroTurno.SelectedItem?.ToString() ?? "Todos";
            var texto = txtBuscarTurno.Text;
            AplicarFiltro(campo, texto);
        }

        // ===================== BOTÓN LIMPIAR FILTRO =====================
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarTurno.Clear();
            if (cboCampoFiltroTurno != null) cboCampoFiltroTurno.SelectedIndex = 0;

            enlaceTurnos.DataSource = _listadoTurnos.OrderBy(t => t.Fecha_Del_Turno).ToList();
            enlaceTurnos.ResetBindings(false);
        }


        // ===================== MÉTODOS DE FILTRADO =====================
        // Carga las opciones de filtro en el ComboBox
        private void CargarOpcionesDeFiltro()
        {
            if (cboCampoFiltroTurno == null) return;

            cboCampoFiltroTurno.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCampoFiltroTurno.Items.Clear();
            cboCampoFiltroTurno.Items.AddRange(new string[] { "Todos", "Paciente", "Fecha", "Estado" });
            cboCampoFiltroTurno.SelectedIndex = 0;
        }

        // Aplica el filtro basado en el campo y el texto ingresado
        private void AplicarFiltro(string campo, string texto)
        {
            // Normaliza el texto para comparación
            string busqueda = (texto ?? "").Trim().ToLowerInvariant();
            IEnumerable<ListadoTurno> query = _listadoTurnos;

            // Si hay texto, aplica el filtro según el campo seleccionado
            if (!string.IsNullOrEmpty(busqueda))
            {
                switch (campo)
                {
                    case "Paciente":
                        query = query.Where(t => (t.Paciente ?? "").ToLower().Contains(busqueda));
                        break;

                    case "Fecha":
                        // 1) Intento: fecha completa (ej: 15/02/2025)
                        if (DateTime.TryParse(texto, out var fechaExacta))
                        {
                            query = query.Where(t => t.Fecha_Del_Turno.Date == fechaExacta.Date);
                        }
                        else
                        {
                            // 2) Intento: mes/año (ej: 02/2025, 2-2025, 2 2025)
                            char[] sep = new[] { '/', '-', ' ' };
                            var partes = texto.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                            // mes + año
                            if (partes.Length == 2 &&
                                int.TryParse(partes[0], out int mes) &&
                                int.TryParse(partes[1], out int anio) &&
                                mes >= 1 && mes <= 12)
                            {
                                query = query.Where(t =>
                                    t.Fecha_Del_Turno.Month == mes &&
                                    t.Fecha_Del_Turno.Year == anio);
                            }
                            // 3) Intento: solo mes (1-12) → sin importar el año
                            else if (int.TryParse(texto, out int mesSolo) &&
                                     mesSolo >= 1 && mesSolo <= 12)
                            {
                                query = query.Where(t => t.Fecha_Del_Turno.Month == mesSolo);
                            }
                            else
                            {
                                // Fallback: filtra por texto sobre la fecha formateada
                                query = query.Where(t =>
                                    t.Fecha_Del_Turno.ToString("g").ToLower().Contains(busqueda));
                            }
                        }
                        break;

                    case "Estado":
                        query = query.Where(t => (t.Estado ?? "").ToLower().Contains(busqueda));
                        break;

                    default: // "Todos"
                        query = query.Where(t =>
                            (t.Paciente ?? "").ToLower().Contains(busqueda) ||
                            t.Fecha_Del_Turno.ToString("g").ToLower().Contains(busqueda) ||
                            (t.Estado ?? "").ToLower().Contains(busqueda));
                        break;
                }
            }

            // Actualiza el BindingSource con los resultados filtrados
            enlaceTurnos.DataSource = query.OrderBy(t => t.Fecha_Del_Turno).ToList();
            enlaceTurnos.ResetBindings(false);
        }


        // ===================== CARGA DE GRÁFICOS TURNOS POR DIA =====================
        private void CargarGraficoTurnosPorDia()
        {
            var _estadisticasService = new EstadisticasService();
            // cambia chartTurnosPorDia por el nombre real de tu Chart
            chartTurnosPorDia.Series.Clear();
            chartTurnosPorDia.ChartAreas[0].AxisX.Title = "Día";
            chartTurnosPorDia.ChartAreas[0].AxisY.Title = "Cantidad de Turnos";

            var datos = _estadisticasService.ObtenerTurnosPorDiaUltimaSemana();

            Series serieTurnos = new Series("Turnos")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (var d in datos.OrderBy(x => x.Fecha))
            {
                string etiqueta = d.Fecha.ToString("dd/MM"); // eje X
                serieTurnos.Points.AddXY(etiqueta, d.Cantidad);
            }

            chartTurnosPorDia.Series.Add(serieTurnos);

            // opcional
            serieTurnos["PointWidth"] = "0.4";
        }

        // ===================== CARGA DE GRÁFICOS ESTADO DE TURNOS =====================
        private void CargarGraficoEstadoTurnos()
        {
            var _estadisticasService = new EstadisticasService();

            chartEstadoTurnos.Series.Clear();

            Series serieEstados = new Series("Estados")
            {
                ChartType = SeriesChartType.Doughnut,   // Donut
                IsValueShownAsLabel = true
            };

            var dist = _estadisticasService.ObtenerDistribucionEstadosTurnosUltimaSemana();

            int total = dist.Pendientes + dist.Atendidos + dist.Cancelados;
            if (total == 0)
                return;

            // Agregamos los puntos; el Chart calcula los % solo
            if (dist.Pendientes > 0)
                serieEstados.Points.AddXY("Pendientes", dist.Pendientes);

            if (dist.Atendidos > 0)
                serieEstados.Points.AddXY("Atendidos", dist.Atendidos);

            if (dist.Cancelados > 0)
                serieEstados.Points.AddXY("Cancelados", dist.Cancelados);

            // Etiquetas: nombre + porcentaje
            serieEstados.Label = "#VALX\n#PERCENT{P0}";
            serieEstados.LegendText = "#VALX";

            if (chartEstadoTurnos.Legends.Count > 0)
                chartEstadoTurnos.Legends[0].Enabled = true;

            chartEstadoTurnos.Series.Add(serieEstados);
        }
    }
}
