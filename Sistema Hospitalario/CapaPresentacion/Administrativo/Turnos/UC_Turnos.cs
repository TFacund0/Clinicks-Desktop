using Sistema_Hospitalario.CapaNegocio.DTOs;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;
using Sistema_Hospitalario.CapaPresentacion.Administrativo.Turnos;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo
{
    /// <summary>
    /// Control de usuario que gestiona el listado y búsqueda de turnos para el personal administrativo.
    /// Permite visualizar turnos recientes, filtrar por paciente/fecha/estado y acceder al detalle o registro de citas.
    /// </summary>
    public partial class UC_Turnos : UserControl
    {
        /// <summary>Servicio para el manejo de lógica de turnos.</summary>
        TurnoService _turnoService = new TurnoService();

        /// <summary>Lista interna que almacena los turnos cargados.</summary>
        List<ListadoTurno> _listadoTurnos = new List<ListadoTurno>();

        /// <summary>Enlace de datos para la sincronización con la grilla de turnos.</summary>
        BindingSource enlaceTurnos = new BindingSource();

        /// <summary>Evento que notifica que se ha solicitado ver el detalle de un turno específico.</summary>
        public event EventHandler<TurnoDto> VerTurnoSolicitado;

        /// <summary>Evento que notifica que se ha solicitado el registro de un nuevo turno.</summary>
        public event EventHandler RegistrarTurnoSolicitado;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_Turnos"/>.
        /// Configura la grilla, carga los indicadores de estado y puebla los datos iniciales.
        /// </summary>
        public UC_Turnos()
        {
            InitializeComponent();     

            ConfigurarActividad();
            ConfigurarLabelsDatosTurno();

            ConfigurarEnlazadoDatosTurnoColumnas();
            CargarTurnosDGV();
            CargarOpcionesDeFiltro();

            dgvTurnos.CellContentClick += dgvTurnos_CellContentClick;
        }

        // ============== BOTÓN NUEVO TURNO ==============
        private void btnNuevoTurno_Click(object sender, EventArgs e)
        {
            RegistrarTurnoSolicitado?.Invoke(this, EventArgs.Empty);
        }

        // ============== CONFIGURACION DEL DATAGRIDVIEW ==============
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

        // ===================== CONFIGURAR DATOS CAJAS DE TEXTO =====================
        private void ConfigurarLabelsDatosTurno()
        {
            lblTurnosPendientes.Text = _turnoService.CantidadTurnosPendientes().ToString();
            lblTurnosCompletados.Text = _turnoService.CantidadTurnosPorEstado("Atendido").ToString();
            lblTurnosCurso.Text = _turnoService.CantidadTurnosPorEstado("En Proceso").ToString();
            lblTurnosHoy.Text = _turnoService.CantidadTurnosPorEstado("Pendiente").ToString();
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
            _listadoTurnos = _turnoService.ListarTurnos().Where(t => t.Fecha_Del_Turno >= DateTime.Now.AddDays(-7)).ToList();
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
            string busqueda = (texto ?? "").Trim().ToLowerInvariant();
            IEnumerable<ListadoTurno> query = _listadoTurnos;

            if (!string.IsNullOrEmpty(busqueda))
            {
                switch (campo)
                {
                    case "Paciente":
                        query = query.Where(t => (t.Paciente ?? "").ToLower().Contains(busqueda));
                        break;

                    case "Fecha":
                        // Intento 1: fecha completa (ej: 15/02/2025)
                        if (DateTime.TryParse(texto, out var fechaExacta))
                        {
                            query = query.Where(t => t.Fecha_Del_Turno.Date == fechaExacta.Date);
                        }
                        else
                        {
                            // Intento 2: mes/año (ej: 02/2025 o 2-2025)
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
                            // solo mes (1-12): filtra por mes sin importar el año
                            else if (int.TryParse(texto, out int mesSolo) &&
                                     mesSolo >= 1 && mesSolo <= 12)
                            {
                                query = query.Where(t => t.Fecha_Del_Turno.Month == mesSolo);
                            }
                            else
                            {
                                // fallback: filtra por texto sobre la fecha formateada
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

            enlaceTurnos.DataSource = query.OrderBy(t => t.Fecha_Del_Turno).ToList();
            enlaceTurnos.ResetBindings(false);
        }


        // ===================== EVENTO CLIC EN CELDA DEL DATAGRIDVIEW =====================
        private void dgvTurnos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignorar clics en encabezados
            if (e.RowIndex < 0) return;

            // Si se hizo clic en el botón "Ver" de la columna de acción
            if (e.RowIndex >= 0 && e.RowIndex < enlaceTurnos.Count)
                {
                var turno = enlaceTurnos[e.RowIndex] as ListadoTurno;
                if (turno == null) return;

                // Traer el detalle desde negocio/datos
                var detalle = _turnoService.ObtenerDetalle(turno.Id_turno);

                if (detalle == null)
                {
                    MessageBox.Show("No se encontró el paciente seleccionado.", "Atención",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Disparar el evento hacia el contenedor (Form/Padre) para que lo muestre
                VerTurnoSolicitado?.Invoke(this, detalle);
            }
        }
    }
}
