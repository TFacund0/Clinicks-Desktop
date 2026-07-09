using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sistema_Hospitalario.CapaNegocio.DTOs.Home;
using Sistema_Hospitalario.CapaNegocio.Servicios.HomeService;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService.CamaService;
using Sistema_Hospitalario.CapaNegocio.Servicios.InternacionService;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo
{
    /// <summary>
    /// Control de usuario que representa la pantalla de inicio para el personal administrativo.
    /// Muestra un resumen de estadísticas (pacientes, camas e internaciones) y el listado de actividad reciente.
    /// </summary>
    public partial class UC_HomeGerente : UserControl
    {
        /// <summary>Lista que almacena la actividad reciente del sistema.</summary>
        private List<HomeDto> listaActividad = new List<HomeDto>();
        /// <summary>Enlace de datos para el DataGridView de actividad.</summary>
        private BindingSource enlaceActividad = new BindingSource();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_HomeGerente"/> (Panel de Inicio Administrativo).
        /// Carga los paneles estadísticos y configura la tabla de actividad reciente.
        /// </summary>
        public UC_HomeGerente()
        {
            InitializeComponent();

            CargarInformacionPaneles();
            ConfigurarTablaActividad();

            // Asegura que se ejecute el Load
            this.Load += Home_Load;
        }

        // Método que configuran la información de las estadísticas en el UC Home Gerente
        private void CargarInformacionPaneles()
        {
            PacienteService pacienteService = new PacienteService();
            int cantidadPacientes = pacienteService.ContarPorEstadoId("activo");

            CamaService camaService = new CamaService();
            int cantidadCamasOcupadas = camaService.TotalCamasXEstado("ocupada");
            int cantidadCamas = camaService.TotalCamas();

            InternacionService internacionService = new InternacionService();
            int cantidadInternaciones = internacionService.ListadoInternacionDtos().Count();

            string cantPacientes = cantidadPacientes.ToString();
            string cantCamasOcupadas = cantidadCamasOcupadas.ToString();
            string totalCamas = cantidadCamas.ToString();
            string cantInternaciones = cantidadInternaciones.ToString();

            if (
                int.TryParse(cantPacientes, out int valorPaciente) && valorPaciente >= 0 && valorPaciente < 1000 &&
                int.TryParse(cantCamasOcupadas, out int valorCamasOcupadas) && valorCamasOcupadas >= 0 && valorCamasOcupadas < 1000 &&
                int.TryParse(totalCamas, out int valorCamas) && valorCamas >= 0 && valorCamas < 1000 && valorCamasOcupadas <= valorCamas &&
                int.TryParse(cantInternaciones, out int valorInternaciones) && valorInternaciones >= 0 && valorInternaciones < 1000
                )
            {
                string porcentajeCamas = (((float)valorCamasOcupadas / (float)valorCamas) * 100).ToString() + "%";

                lblPacientesActivos.Text = cantPacientes;
                lblCamasOcupadas.Text = cantCamasOcupadas + "/" + totalCamas;
                lblPorcentajeCamas.Text = porcentajeCamas + " de ocupación";
                lblInternaciones.Text = cantInternaciones;
            }
            else
            {
                MessageBox.Show("Error: Los valores de los paneles deben ser números enteros no negativos y dentro del rango permitido.", "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método que configura el DataGridView de Actividad Reciente en el UC Home Gerente
        private void ConfigurarTablaActividad()
        {
            dgvActividad.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Selección de fila completa

            dgvActividad.DefaultCellStyle.Font = new Font("Segoe UI", 10F); // Establece la fuente predeterminada para las celdas
            dgvActividad.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248); // Color de fondo para filas alternas

            dgvActividad.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold); // Fuente en negrita para los encabezados de columna
            dgvActividad.ColumnHeadersHeight = 35; // Altura de los encabezados de columna
            dgvActividad.ColumnHeadersDefaultCellStyle.BackColor = Color.WhiteSmoke; // Color de fondo para los encabezados de columna
            
            ConfigurarEnlazadoDatosColumnas();
        }

        // Método que configura el DataPropertyName de cada columna del DataGridView
        private void ConfigurarEnlazadoDatosColumnas()
        {
            dgvActividad.AutoGenerateColumns = false;

            dgvActividad.Columns["colNombre"].DataPropertyName = "Nombre";
            dgvActividad.Columns["colApellido"].DataPropertyName = "Apellido";
            dgvActividad.Columns["colAccion"].DataPropertyName = "Accion";
            dgvActividad.Columns["colHorario"].DataPropertyName = "Horario";
            dgvActividad.Columns["colTipo"].DataPropertyName = "Tipo";
        }

        // ===================== CARGAR ACTIVIDAD RECIENTE =====================
        private void CargarActividadReciente()
        {
            var homeService = new HomeService();

            // Cargamos la lista de la clase
            var datos = homeService.ListarActividadReciente(100);
            listaActividad = datos.Where(e => e.Horario >= DateTime.Now.AddMonths(-1)).ToList();

            // Usamos el BindingSource de la clase
            enlaceActividad.DataSource = listaActividad
                .OrderByDescending(t => t.Horario)
                .ToList();

            dgvActividad.AutoGenerateColumns = false;
            dgvActividad.DataSource = enlaceActividad;
            dgvActividad.Columns["colHorario"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
        }

        // ===================== EVENTO LOAD DEL UC HOME =====================
        private void Home_Load(object sender, EventArgs e)
        {
            CargarActividadReciente();
            CargarOpcionesDeFiltroHome();
        }

        // ===================== BOTÓN BUSCAR =====================
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            var campo = cboCampo.SelectedItem?.ToString() ?? "Todos";
            var texto = txtBuscar.Text;
            AplicarFiltroHome(campo, texto);
        }

        // ===================== BOTÓN LIMPIAR =====================
        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            if (cboCampo != null) cboCampo.SelectedIndex = 0;

            enlaceActividad.DataSource = listaActividad
                .OrderByDescending(t => t.Horario)
                .ToList();
            enlaceActividad.ResetBindings(false);
        }

        // ===================== OPCIONES DE FILTRO =====================
        private void CargarOpcionesDeFiltroHome()
        {
            if (cboCampo == null) return;

            cboCampo.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCampo.Items.Clear();
            // Mismos nombres que verás en el Combo
            cboCampo.Items.AddRange(new[] { "Todos", "Paciente", "Acción", "Tipo", "Fecha" });
            cboCampo.SelectedIndex = 0;
        }

        // ===================== APLICAR FILTRO =====================
        private void AplicarFiltroHome(string campo, string texto)
        {
            string busqueda = (texto ?? "").Trim().ToLowerInvariant();
            IEnumerable<HomeDto> query = listaActividad;

            if (!string.IsNullOrEmpty(busqueda))
            {
                switch (campo)
                {
                    case "Paciente":
                        query = query.Where(t =>
                        {
                            var nombre = (t.Nombre ?? "").ToLowerInvariant();
                            var apellido = (t.Apellido ?? "").ToLowerInvariant();
                            var full1 = (nombre + " " + apellido).Trim();
                            var full2 = (apellido + " " + nombre).Trim();
                            return nombre.Contains(busqueda) ||
                                   apellido.Contains(busqueda) ||
                                   full1.Contains(busqueda) ||
                                   full2.Contains(busqueda);
                        });
                        break;

                    case "Acción":
                        query = query.Where(t => (t.Accion ?? "").ToLowerInvariant().Contains(busqueda));
                        break;

                    case "Tipo":
                        query = query.Where(t => (t.Tipo ?? "").ToLowerInvariant().Contains(busqueda));
                        break;

                    case "Fecha":
                        // 1) Intento: fecha completa (ej: 15/02/2025)
                        if (DateTime.TryParse(texto, out var fechaExacta))
                        {
                            var f = fechaExacta.Date;
                            query = query.Where(t => t.Horario.Date == f);
                        }
                        else
                        {
                            // 2) Intento: mes/año (ej: 02/2025, 2-2025, 2 2025)
                            char[] sep = new[] { '/', '-', ' ' };
                            var partes = texto.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                            if (partes.Length == 2 &&
                                int.TryParse(partes[0], out int mes) &&
                                int.TryParse(partes[1], out int anio) &&
                                mes >= 1 && mes <= 12)
                            {
                                query = query.Where(t =>
                                    t.Horario.Month == mes &&
                                    t.Horario.Year == anio);
                            }
                            // 3) Intento: solo mes (1–12) → todos los años
                            else if (int.TryParse(texto, out int mesSolo) &&
                                     mesSolo >= 1 && mesSolo <= 12)
                            {
                                query = query.Where(t => t.Horario.Month == mesSolo);
                            }
                            else
                            {
                                // Fallback: coincidencia de texto en la fecha formateada
                                query = query.Where(t =>
                                    t.Horario.ToString("dd/MM/yyyy HH:mm").ToLowerInvariant().Contains(busqueda) ||
                                    t.Horario.ToString("dd/MM/yyyy").ToLowerInvariant().Contains(busqueda));
                            }
                        }
                        break;

                    default: // "Todos"
                        query = query.Where(t =>
                            ((t.Nombre ?? "").ToLowerInvariant() + " " + (t.Apellido ?? "").ToLowerInvariant())
                                .Contains(busqueda)
                            || (t.Accion ?? "").ToLowerInvariant().Contains(busqueda)
                            || (t.Tipo ?? "").ToLowerInvariant().Contains(busqueda)
                            || t.Horario.ToString("dd/MM/yyyy HH:mm").ToLowerInvariant().Contains(busqueda)
                        );
                        break;
                }
            }

            // Siempre mostrar lo más reciente primero
            enlaceActividad.DataSource = query
                .OrderByDescending(t => t.Horario)
                .ToList();

            enlaceActividad.ResetBindings(false);
        }
    }
}