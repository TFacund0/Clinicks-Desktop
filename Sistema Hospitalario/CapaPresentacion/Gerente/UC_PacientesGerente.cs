using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.EstadisticasService;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
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
    /// Control de usuario que proporciona una visión analítica de los pacientes para el perfil de Gerente.
    /// Incluye listado de pacientes con filtros y gráficos estadísticos de registros por día y distribución por estado.
    /// </summary>
    public partial class UC_PacientesGerente : UserControl
    {
        /// <summary>Servicio para operaciones relacionadas con pacientes.</summary>
        private readonly PacienteService pacienteService = new PacienteService();

        /// <summary>BindingSource para el enlace de datos entre la lista de pacientes y la grilla.</summary>
        private readonly BindingSource enlacePacientes = new BindingSource();

        /// <summary>Lista interna que contiene los DTOs de pacientes cargados.</summary>
        private List<PacienteDto> listaPacientes = new List<PacienteDto>();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_PacientesGerente"/>.
        /// Configura la grilla, los filtros y procesa la generación de gráficos estadísticos de pacientes.
        /// </summary>
        public UC_PacientesGerente()
        {
            InitializeComponent();

            try
            {
                ConfigurarTablaActividad();

                CargarOpcionesDeFiltro();
                ConfigurarEnlazadoDatosPacienteColumnas();
                CargarPacientesDatagridview();

                CargarGraficoPacientesRegistradosPorDia();
                CargarGraficoPacientesPorEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al inicializar UC_PacientesGerente: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===================== CONFIGURACIÓN Y CARGA DATAGRIDVIEW =====================
        private void ConfigurarTablaActividad()
        {
            dgvPacientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Selección de fila completa

            dgvPacientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Ajustar columnas al ancho del control
            dgvPacientes.DefaultCellStyle.Font = new Font("Segoe UI", 10F); // Fuente para las celdas
            dgvPacientes.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248); // Color alternativo para filas

            dgvPacientes.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold); // Fuente para encabezados
            dgvPacientes.ColumnHeadersHeight = 35; // Altura de encabezado
            dgvPacientes.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke; // Color de fondo del encabezado
        }

        // Carga la lista de pacientes desde el servicio y la enlaza al DataGridView
        private void CargarPacientesDatagridview()
        {
            listaPacientes = pacienteService.ListadoPacientesDGV();
            enlacePacientes.DataSource = listaPacientes;
            dgvPacientes.DataSource = enlacePacientes;
        }

        // ===================== CONFIGURAR ENLAZADO DE DATOS =====================
        // Configura el enlace entre las columnas del DataGridView y las propiedades del DTO
        private void ConfigurarEnlazadoDatosPacienteColumnas()
        {
            dgvPacientes.AutoGenerateColumns = false;

            dgvPacientes.Columns["colPaciente"].DataPropertyName = "Paciente";
            dgvPacientes.Columns["colDni"].DataPropertyName = "DNI";
            dgvPacientes.Columns["colEdad"].DataPropertyName = "Edad";
            dgvPacientes.Columns["colEstado"].DataPropertyName = "Estado_paciente";
        }

        // ===================== BUSQUEDA DE PACIENTE POR FILTRADO =====================
        // Carga las opciones de filtro en el ComboBox
        private void CargarOpcionesDeFiltro()
        {
            if (cboCampo == null) return;

            cboCampo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCampo.Items.Clear();
            cboCampo.Items.AddRange(new[] { "Todos", "Paciente", "DNI", "Estado" });
            cboCampo.SelectedIndex = 0;
        }

        // Aplica el filtro basado en el campo y el texto ingresado
        private void AplicarFiltro(string campo, string texto)
        {
            // Normaliza el texto para comparación
            string busqueda = (texto ?? "").Trim().ToLowerInvariant();
            IEnumerable<PacienteDto> query = listaPacientes;

            // Si hay texto, aplica el filtro según el campo seleccionado
            if (!string.IsNullOrEmpty(busqueda))
            {
                switch (campo)
                {
                    case "Paciente":
                        query = query.Where(t => (t.Paciente ?? "").ToLower().Contains(busqueda));
                        break;
                    case "DNI":
                        if (int.TryParse(busqueda, out int dniBuscado))
                            query = query.Where(t => t.Dni == dniBuscado); // o Dni según el DTO que uses
                        else
                            query = Enumerable.Empty<PacienteDto>(); // o no aplicar filtro
                        break;
                    case "Estado":
                        query = query.Where(t => (t.Estado_paciente ?? "").ToLower().Contains(busqueda));
                        break;
                    default:
                        query = query.Where(t =>
                            (t.Nombre ?? "").ToLower().Contains(busqueda) ||
                            t.Dni.ToString().Contains(busqueda) ||
                            (t.Estado_paciente ?? "").ToLower().Contains(busqueda));
                        break;
                }
            }

            // Actualiza el BindingSource con los resultados filtrados
            enlacePacientes.DataSource = query.OrderBy(t => t.Paciente).ToList();
            enlacePacientes.ResetBindings(false);
        }

        // ===================== BOTÓN BUSCAR PACIENTE =====================
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var campo = cboCampo.SelectedItem?.ToString() ?? "Todos";
            var texto = txtBuscar.Text;
            AplicarFiltro(campo, texto);
        }

        // ===================== BOTÓN LIMPIAR BUSCADOR PACIENTE =====================
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cboCampo != null) cboCampo.SelectedIndex = 0;

            enlacePacientes.DataSource = listaPacientes.OrderBy(t => t.Paciente).ToList();
            enlacePacientes.ResetBindings(false);
        }

        // ===================== CARGAR GRÁFICO PACIENTES REGISTRADOS POR DÍA =====================
        private void CargarGraficoPacientesRegistradosPorDia()
        {
            var _estadisticasService = new EstadisticasService();
            chartPacientesPorDia.Series.Clear();
            chartPacientesPorDia.ChartAreas[0].AxisX.Title = "Día";
            chartPacientesPorDia.ChartAreas[0].AxisY.Title = "Pacientes registrados";

            // Pedimos los datos ya procesados al servicio
            var datos = _estadisticasService.ObtenerPacientesRegistradosPorDiaUltimaSemana();

            Series serie = new Series("Pacientes")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (var d in datos.OrderBy(x => x.Fecha))
            {
                string etiqueta = d.Fecha.ToString("dd/MM"); // eje X
                serie.Points.AddXY(etiqueta, d.Cantidad);
            }

            chartPacientesPorDia.Series.Add(serie);
            serie["PointWidth"] = "0.4"; // opcional, para que no queden barras súper anchas
        }

        // ===================== CARGAR GRÁFICO PACIENTES POR ESTADO =====================
        private void CargarGraficoPacientesPorEstado()
        {
            var _estadisticasService = new EstadisticasService();
            chartPacientesPorEstado.Series.Clear();

            Series serieEstados = new Series("Estados")
            {
                ChartType = SeriesChartType.Doughnut,  // Donut
                IsValueShownAsLabel = true
            };

            // Datos desde la capa de negocio
            var dist = _estadisticasService.ObtenerDistribucionPacientesPorEstado();

            int total = dist.Activos + dist.Internados + dist.Altas;
            if (total == 0)
                return;

            if (dist.Activos > 0)
                serieEstados.Points.AddXY("Activos", dist.Activos);

            if (dist.Internados > 0)
                serieEstados.Points.AddXY("Internados", dist.Internados);

            if (dist.Altas > 0)
                serieEstados.Points.AddXY("Altas", dist.Altas);

            // Etiquetas: nombre + porcentaje
            serieEstados.Label = "#VALX\n#PERCENT{P0}";
            serieEstados.LegendText = "#VALX";

            if (chartPacientesPorEstado.Legends.Count > 0)
                chartPacientesPorEstado.Legends[0].Enabled = true;

            chartPacientesPorEstado.Series.Add(serieEstados);
        }

    }
}
