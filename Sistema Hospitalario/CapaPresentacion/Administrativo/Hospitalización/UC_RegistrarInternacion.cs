using Sistema_Hospitalario.CapaNegocio.DTOs.Habitaciones;
using Sistema_Hospitalario.CapaNegocio.DTOs.Camas;
using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;
using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;

using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService.CamaService;
using Sistema_Hospitalario.CapaNegocio.Servicios.InternacionService;
using Sistema_Hospitalario.CapaNegocio.Servicios.ProcedimientoService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo.Hospitalización
{
    /// <summary>
    /// Control de usuario que gestiona el registro de nuevas internaciones de pacientes.
    /// Maneja la asignación de médicos, procedimientos, pisos, habitaciones y camas disponibles,
    /// con lógica de filtrado dinámico y validaciones de negocio.
    /// </summary>
    public partial class UC_RegistrarInternacion : UserControl
    {
        /// <summary>Servicio para operaciones de pacientes.</summary>
        private readonly PacienteService _servicioPaciente = new PacienteService();
        /// <summary>Servicio para gestión de habitaciones.</summary>
        private readonly HabitacionService _servicioHabitacion = new HabitacionService();
        /// <summary>Servicio para gestión de camas.</summary>
        private readonly CamaService _servicioCama = new CamaService();
        /// <summary>Servicio para gestión de médicos.</summary>
        private readonly MedicoService _servicioMedico = new MedicoService();
        /// <summary>Servicio para gestión de procedimientos médicos.</summary>
        private readonly ProcedimientoService _servicioProcedimiento = new ProcedimientoService();

        /// <summary>Lista de pacientes candidatos para internación.</summary>
        private List<PacienteDto> listaPacientes = new List<PacienteDto>();
        /// <summary>Lista de habitaciones filtradas.</summary>
        private List<HabitacionDto> listaHabitaciones = new List<HabitacionDto>();
        /// <summary>Lista de camas disponibles en la habitación seleccionada.</summary>
        private List<CamaDto> listaCamas = new List<CamaDto>();
        /// <summary>Lista de médicos disponibles.</summary>
        private List<MedicoDto> listaMedicos = new List<MedicoDto>();
        /// <summary>Lista de procedimientos disponibles.</summary>
        private List<ProcedimientoDto> listaProcedimientos = new List<ProcedimientoDto>();

        /// <summary>Maestro de visualización de pacientes.</summary>
        private readonly List<string> _maestroPaciente = new List<string>();
        /// <summary>Maestro de visualización de habitaciones.</summary>
        private readonly List<string> _maestroHabitacion = new List<string>();
        /// <summary>Maestro de visualización de camas.</summary>
        private readonly List<string> _maestroCama = new List<string>();
        /// <summary>Maestro de visualización de médicos.</summary>
        private readonly List<string> _maestroMedico = new List<string>();
        /// <summary>Maestro de visualización de procedimientos.</summary>
        private readonly List<string> _maestroProcedimiento = new List<string>();

        /// <summary>Cached para filtrar médicos sin consultar BD.</summary>
        private List<MedicoDto> _medicosMaestro = new List<MedicoDto>();
        /// <summary>Cached para filtrar procedimientos sin consultar BD.</summary>
        private List<ProcedimientoDto> _procedimientosMaestro = new List<ProcedimientoDto>();


        /// <summary>Bandera para evitar reentrancia en eventos de texto.</summary>
        private bool _actualizandoInterno = false;

        /// <summary>Evento que notifica que se ha cancelado el registro de internación.</summary>
        public event EventHandler CancelarInternacionSolicitada;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_RegistrarInternacion"/>.
        /// Configura los eventos de filtrado, carga los catálogos iniciales y prepara los controles de fecha.
        /// </summary>
        public UC_RegistrarInternacion()
        {
            InitializeComponent();

            txtObservaciones.MaxLength = 200;

            // Inputs
            // txtPiso y cbPaciente ya se suscriben en el Designer; no volver a suscribirlos aquí
            // para evitar que sus handlers se disparen dos veces por evento.

            cbHabitacion.TextUpdate -= CbHabitacion_TextUpdate;
            cbHabitacion.TextUpdate += CbHabitacion_TextUpdate;

            cbHabitacion.Enter += (s, ev) => cbHabitacion.DroppedDown = true;
            cbHabitacion.MouseDown += (s, ev) => cbHabitacion.DroppedDown = true;

            cbCama.Enter += (s, ev) => cbCama.DroppedDown = true;
            cbCama.MouseDown += (s, ev) => cbCama.DroppedDown = true;

            cbMedico.TextUpdate += CbMedico_TextUpdate;
            cbProcedimiento.TextUpdate += CbProcedimiento_TextUpdate;

            // Cargar fuentes
            DatosComboBoxPaciente();
            DatosComboBoxMedico();
            DatosComboBoxProcedimiento();

            // Limpiar selecciones iniciales
            LimpiarSeleccionCombo(cbPaciente);
            LimpiarSeleccionCombo(cbMedico);
            LimpiarSeleccionCombo(cbProcedimiento);

            // Habitaciones / camas se controlan por piso, arrancan deshabilitadas
            SincronizarHabilitacionControles();

            ConfigurarFechaInicioConHora();
            ConfigurarFechaFinNull();
        }


        // ============================= COMBO PACIENTE =============================
        private void DatosComboBoxPaciente()
        {
            listaPacientes = _servicioPaciente.ListarPacientes();

            var fuente = listaPacientes
                .Where(p => p.Estado_paciente == "activo")
                .Select(p => new {
                    p.Id,
                    Display = $"{p.Apellido} {p.Nombre} ({p.Dni})"
                })
                .ToList();

            _maestroPaciente.Clear();
            _maestroPaciente.AddRange(fuente.Select(x => x.Display));

            cbPaciente.DropDownStyle = ComboBoxStyle.DropDown;
            cbPaciente.DataSource = fuente;
            cbPaciente.DisplayMember = "Display";
            cbPaciente.ValueMember = "Id";
        }

        // ============================= COMBO MEDICO =============================
        private void DatosComboBoxMedico()
        {
            listaMedicos = _servicioMedico.ListarMedicos() ?? new List<MedicoDto>();
            _medicosMaestro = listaMedicos.ToList(); // cache

            var fuente = _medicosMaestro
                .Select(m => new {
                    m.Id,
                    Display = $"{m.Apellido} {m.Nombre} ({m.Especialidad})",
                    m.Especialidad
                })
                .ToList();

            _maestroMedico.Clear();
            _maestroMedico.AddRange(fuente.Select(x => x.Display));

            cbMedico.DropDownStyle = ComboBoxStyle.DropDown;
            cbMedico.DataSource = fuente;
            cbMedico.DisplayMember = "Display";
            cbMedico.ValueMember = "Id";
        }

        // ============================= COMBO PROCEDIMIENTO =============================
        private void DatosComboBoxProcedimiento()
        {
            listaProcedimientos = _servicioProcedimiento.ListarProcedimientos() ?? new List<ProcedimientoDto>();
            _procedimientosMaestro = listaProcedimientos.ToList(); // cache

            var fuente = _procedimientosMaestro
                .Select(p => new { p.Id, Display = p.Name })
                .ToList();

            _maestroProcedimiento.Clear();
            _maestroProcedimiento.AddRange(fuente.Select(x => x.Display));

            cbProcedimiento.DropDownStyle = ComboBoxStyle.DropDown;
            cbProcedimiento.DataSource = fuente;
            cbProcedimiento.DisplayMember = "Display";
            cbProcedimiento.ValueMember = "Id";

            // engancho eventos 1 sola vez
            cbProcedimiento.SelectedIndexChanged -= CbProcedimiento_SelectedIndexChanged;
            cbProcedimiento.SelectedIndexChanged += CbProcedimiento_SelectedIndexChanged;

            cbProcedimiento.SelectionChangeCommitted -= CbProcedimiento_SelectionChangeCommitted;
            cbProcedimiento.SelectionChangeCommitted += CbProcedimiento_SelectionChangeCommitted;
        }


        // ============================= COMBO HABITACIÓN =============================
        private void CargarHabitacionesPorPiso(string pisoTexto)
        {
            _maestroHabitacion.Clear(); // FIX: limpiamos en todos los casos

            if (!string.IsNullOrWhiteSpace(pisoTexto) && int.TryParse(pisoTexto, out var piso) && piso > 0)
            {
                listaHabitaciones = _servicioHabitacion.ListarHabitacionesXPiso(pisoTexto) ?? new List<HabitacionDto>();

                foreach (var unaHabitacion in listaHabitaciones)
                    _maestroHabitacion.Add(unaHabitacion.Nro_habitacion.ToString());
            }

            var seleccionadoAntes = cbHabitacion.Text; // FIX: conservamos por texto (porque trabajamos con string)

            try
            {
                _actualizandoInterno = true;
                cbHabitacion.BeginUpdate();

                cbHabitacion.DataSource = null;
                cbHabitacion.DataSource = new BindingList<string>(_maestroHabitacion);

                cbHabitacion.SelectedIndex = -1;
                cbHabitacion.Text = string.Empty;
                
                cbHabitacion.Enabled = _maestroHabitacion.Count > 0;
                cbHabitacion.CausesValidation = _maestroHabitacion.Count > 0;

                if (_maestroHabitacion.Count == 0)
                {
                    errorProvider1.SetError(cbHabitacion, "No hay habitaciones disponibles para el piso seleccionado.");
                    // si no hay habitaciones, también vaciamos/deshabilitamos camas
                    VaciarCombo(cbCama);
                    cbCama.Enabled = false;
                    cbCama.CausesValidation = false;
                    errorProvider1.SetError(cbCama, "");
                }
                else
                {
                    errorProvider1.SetError(cbHabitacion, "");
                }

            }
            finally
            {
                cbHabitacion.EndUpdate();
                _actualizandoInterno = false;
            }
        }

        // ============================= COMBO CAMA =============================
        private void CargarCamaPorHabitacion(string habitacionText)
        {
            _maestroCama.Clear();

            if (!string.IsNullOrWhiteSpace(habitacionText) &&
                int.TryParse(habitacionText, out var habitacion) && habitacion > 0)
            {
                listaCamas = _servicioCama.ListarCamasXHabitacion(habitacionText) ?? new List<CamaDto>();

                foreach (var unaCama in listaCamas.Where(c => c.EstadoCama.ToLower() == "disponible"))
                    _maestroCama.Add(unaCama.NroCama.ToString());
            }

            try
            {
                _actualizandoInterno = true;
                cbCama.BeginUpdate();

                cbCama.DataSource = null;
                cbCama.DataSource = new BindingList<string>(_maestroCama);
                cbCama.SelectedIndex = -1;
                cbCama.Text = string.Empty;
                
                // después de setear DataSource/Text...
                cbCama.Enabled = _maestroCama.Count > 0;
                cbCama.CausesValidation = _maestroCama.Count > 0;

                if (_maestroCama.Count == 0)
                {
                    // aviso informativo (no es bloqueo de validación)
                    errorProvider1.SetError(cbCama, "No hay camas disponibles en la habitación seleccionada.");
                }
                else
                {
                    errorProvider1.SetError(cbCama, "");
                }
            }
            finally
            {
                cbCama.EndUpdate();
                _actualizandoInterno = false;
            }
        }

        // ============================= FILTROS =============================

        // ============== FILTRO PACIENTE ==============
        private void CbPaciente_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;

            string texto = cbPaciente.Text?.Trim() ?? "";
            int caret = cbPaciente.SelectionStart;

            var filtrados = string.IsNullOrEmpty(texto)
                ? _maestroPaciente
                : _maestroPaciente
                    .Where(p => p.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            if (filtrados.Count == 0)
                filtrados.Add("— sin coincidencias —");

            try
            {
                _actualizandoInterno = true;
                cbPaciente.BeginUpdate();

                cbPaciente.DataSource = null;
                cbPaciente.DataSource = new BindingList<string>(filtrados);

                cbPaciente.DroppedDown = true;
                Cursor.Current = Cursors.Default;
            }
            finally
            {
                cbPaciente.EndUpdate();
                cbPaciente.Text = texto;
                cbPaciente.SelectionStart = Math.Min(caret, cbPaciente.Text.Length);
                _actualizandoInterno = false;
            }
        }

        // ============== FILTRO HABITACION ===============
        private void CbHabitacion_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;

            string texto = cbHabitacion.Text?.Trim() ?? "";
            int caret = cbHabitacion.SelectionStart;

            var filtrados = string.IsNullOrEmpty(texto)
                ? _maestroHabitacion
                : _maestroHabitacion
                    .Where(p => p.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            try
            {
                _actualizandoInterno = true;
                cbHabitacion.BeginUpdate();

                cbHabitacion.DataSource = null;
                cbHabitacion.DataSource = new BindingList<string>(filtrados);

                cbHabitacion.DroppedDown = true;
                Cursor.Current = Cursors.Default;
            }
            finally
            {
                cbHabitacion.EndUpdate();
                cbHabitacion.Text = texto;
                cbHabitacion.SelectionStart = Math.Min(caret, cbHabitacion.Text.Length);
                _actualizandoInterno = false;
            }
        }

        // ================= FILTRO MÉDICO ===================
        private void CbMedico_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;
            string texto = cbMedico.Text?.Trim() ?? "";
            int caret = cbMedico.SelectionStart;
            var filtrados = string.IsNullOrEmpty(texto)
                ? _maestroMedico
                : _maestroMedico
                    .Where(p => p.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            if (filtrados.Count == 0)
                filtrados.Add("— sin coincidencias —");
            try
            {
                _actualizandoInterno = true;
                cbMedico.BeginUpdate();
                cbMedico.DataSource = null;
                cbMedico.DataSource = new BindingList<string>(filtrados);
                cbMedico.DroppedDown = true;
                Cursor.Current = Cursors.Default;
            }
            finally
            {
                cbMedico.EndUpdate();
                cbMedico.Text = texto;
                cbMedico.SelectionStart = Math.Min(caret, cbMedico.Text.Length);
                _actualizandoInterno = false;
            }
        }

        // ================= FILTRO PROCEDIMIENTO ===================
        private void CbProcedimiento_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;
            
            string texto = cbProcedimiento.Text?.Trim() ?? "";
            int caret = cbProcedimiento.SelectionStart;
            
            var filtrados = string.IsNullOrEmpty(texto)
                ? _maestroProcedimiento
                : _maestroProcedimiento
                    .Where(p => p.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
            
            if (filtrados.Count == 0)
                filtrados.Add("— sin coincidencias —");
            
            try
            {
                _actualizandoInterno = true;
                cbProcedimiento.BeginUpdate();
                cbProcedimiento.DataSource = null;
                cbProcedimiento.DataSource = new BindingList<string>(filtrados);
                cbProcedimiento.DroppedDown = true;
                Cursor.Current = Cursors.Default;
            }
            finally
            {
                cbProcedimiento.EndUpdate();
                cbProcedimiento.Text = texto;
                cbProcedimiento.SelectionStart = Math.Min(caret, cbProcedimiento.Text.Length);
                _actualizandoInterno = false;
            }
        }

        // ============================= EVENTOS DE TEXTO QUE AFECTAN A OTROS CONTROLES =============================
        private void TxtPiso_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(txtPiso.Text, out var piso) && piso > 0)
                CargarHabitacionesPorPiso(txtPiso.Text);
            else
                CargarHabitacionesPorPiso("");

            SincronizarHabilitacionControles();
        }

        private void CbHabitacion_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(cbHabitacion.Text, out var habitacion) && habitacion > 0)
                CargarCamaPorHabitacion(cbHabitacion.Text);
            else
                CargarCamaPorHabitacion("");

            SincronizarHabilitacionControles(); // ← agregar
        }

        // ============================= HELPERS (opcionales, pero útiles) =============================
        private bool TextoCoincideConLista(ComboBox cb, List<string> maestro)
        {
            var t = cb.Text?.Trim();
            if (string.IsNullOrEmpty(t) || t == "— sin coincidencias —") return false;

            return maestro.Any(x => string.Equals(x?.Trim(), t, StringComparison.OrdinalIgnoreCase));
        }

        private bool TextoEsEnteroPositivo(string s, out int valor)
            => int.TryParse(s, out valor) && valor > 0;


        // ============================= VALIDACION PACIENTE =============================
        private void CbPaciente_Validating(object sender, CancelEventArgs e)
        {
            if (!TextoCoincideConLista(cbPaciente, _maestroPaciente))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbPaciente, "Seleccioná un paciente válido de la lista.");
            }
            else
            {
                errorProvider1.SetError(cbPaciente, "");
            }
        }

        // ============================= VALIDACION MÉDICO =============================
        private void CbMedico_Validating(object sender, CancelEventArgs e)
        {
            // Si no hay médicos cargados para el procedimiento actual
            if (!cbMedico.Enabled || !cbMedico.CausesValidation || _maestroMedico.Count == 0)
            {
                return;
            }

            if (!TextoCoincideConLista(cbMedico, _maestroMedico))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbMedico, "Seleccioná un médico válido de la lista.");
            }
            else
            {
                errorProvider1.SetError(cbMedico, "");
            }
        }


        // ============================= VALIDACION PROCEDIMIENTO =============================
        private void CbProcedimiento_Validating(object sender, CancelEventArgs e)
        {
            var texto = cbProcedimiento.Text?.Trim() ?? "";

            // 3.1 primero: validar contra el maestro cargado
            bool coincide = _maestroProcedimiento.Any(x =>
                string.Equals(x?.Trim(), texto, StringComparison.OrdinalIgnoreCase));

            if (!coincide)
            {
                e.Cancel = true;
                errorProvider1.SetError(cbProcedimiento, "Seleccioná un procedimiento válido de la lista.");
                return;
            }

            if (cbProcedimiento.SelectedValue == null)
            {
                var proc = listaProcedimientos.FirstOrDefault(p =>
                    string.Equals(p.Name?.Trim(), texto, StringComparison.OrdinalIgnoreCase));

                if (proc != null)
                {
                    var fuente = listaProcedimientos.Select(p => new { p.Id, Display = p.Name }).ToList();
                    cbProcedimiento.BeginUpdate();
                    try
                    {
                        cbProcedimiento.DataSource = fuente;
                        cbProcedimiento.DisplayMember = "Display";
                        cbProcedimiento.ValueMember = "Id";
                        cbProcedimiento.SelectedValue = proc.Id; // ← ahora sí queda vinculado
                    }
                    finally
                    {
                        cbProcedimiento.EndUpdate();
                    }
                }
            }

            errorProvider1.SetError(cbProcedimiento, "");
        }

        // ============================= VALIDACION PISO =============================
        private void TxtPiso_Validating(object sender, CancelEventArgs e)
        {
            if (!TextoEsEnteroPositivo(txtPiso.Text, out _))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPiso, "Piso obligatorio y numérico (>0).");
                return;
            }

            if (txtPiso.Text.Length > 3)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPiso, "Máximo 3 dígitos para el piso.");
                return;
            }

            errorProvider1.SetError(txtPiso, "");
        }


        // ============================= VALIDACION HABITACIÓN =============================
        private void CbHabitacion_Validating(object sender, CancelEventArgs e)
        {
            if (!cbHabitacion.Enabled || !cbHabitacion.CausesValidation)
            {
                errorProvider1.SetError(cbHabitacion, "");
                return;
            }

            if (!TextoCoincideConLista(cbHabitacion, _maestroHabitacion) || !TextoEsEnteroPositivo(cbHabitacion.Text, out _))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbHabitacion, "Seleccioná una habitación válida (número > 0 de la lista).");
            }
            else
            {
                errorProvider1.SetError(cbHabitacion, "");
            }
        }



        // ============================= VALIDACION CAMA =============================
        private void CbCama_Validating(object sender, CancelEventArgs e)
        {
            if (!cbCama.Enabled || !cbCama.CausesValidation)
            {
                // no hay camas -> no validamos este control
                errorProvider1.SetError(cbCama, "");
                return;
            }

            if (!TextoCoincideConLista(cbCama, _maestroCama) || !TextoEsEnteroPositivo(cbCama.Text, out _))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbCama, "Seleccioná una cama válida (número > 0 de la lista).");
            }
            else
            {
                errorProvider1.SetError(cbCama, "");
            }
        }


        // ============================= VALIDACION FECHA INICIO =============================
        private void DtpFechaInicio_Validating(object sender, CancelEventArgs e)
        {
            var fecha = dtpFechaInicio.Value;

            // Permitimos fechas pasadas y futuras,
            // pero acotadas a un rango razonable (±10 años)
            var limiteInferior = DateTime.Today.AddYears(-10);
            var limiteSuperior = DateTime.Today.AddYears(10);

            if (fecha < limiteInferior)
            {
                e.Cancel = true;
                errorProvider1.SetError(dtpFechaInicio,
                    "La fecha de inicio es demasiado antigua (máx. 10 años atrás).");
                return;
            }

            if (fecha > limiteSuperior)
            {
                e.Cancel = true;
                errorProvider1.SetError(dtpFechaInicio,
                    "La fecha de inicio no puede ser más de 10 años en el futuro.");
                return;
            }

            errorProvider1.SetError(dtpFechaInicio, "");
        }


        // ============================= VALIDACION FECHA FIN =============================
        private void DtpFechaFin_Validating(object sender, CancelEventArgs e)
        {
            if (!dtpFechaFin.Checked)
            {
                errorProvider1.SetError(dtpFechaFin, "");
                return;
            }

            if (dtpFechaFin.Value < dtpFechaInicio.Value)
            {
                e.Cancel = true;
                errorProvider1.SetError(dtpFechaFin, "La fecha de egreso debe ser posterior al inicio.");
                return;
            }

            errorProvider1.SetError(dtpFechaFin, "");
        }

        // ============================= VALIDACION OBSERVACIONES =============================
        private void TxtObservaciones_Validating(object sender, CancelEventArgs e)
        {
            var texto = txtObservaciones.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(texto))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtObservaciones,
                    "El diagnóstico / observaciones es obligatorio.");
                return;
            }

            if (texto.Length > 200)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtObservaciones,
                    "Máximo 200 caracteres.");
                return;
            }

            errorProvider1.SetError(txtObservaciones, "");
        }

        private int? ObtenerIdPacienteSeleccionado()
        {
            // Si SelectedValue ya es int, genial
            if (cbPaciente.SelectedValue is int idInt)
                return idInt;

            var texto = cbPaciente.Text?.Trim();
            if (string.IsNullOrEmpty(texto))
                return null;

            // El display lo armás así en DatosComboBoxPaciente
            var paciente = listaPacientes.FirstOrDefault(p =>
                string.Equals(
                    $"{p.Apellido} {p.Nombre} ({p.Dni})".Trim(),
                    texto,
                    StringComparison.OrdinalIgnoreCase));

            return paciente?.Id;
        }

        
        private int? ObtenerIdMedicoSeleccionado()
        {
            if (cbMedico.SelectedValue is int idInt)
                return idInt;

            var texto = cbMedico.Text?.Trim();
            if (string.IsNullOrEmpty(texto))
                return null;

            // Display: Apellido Nombre (Especialidad)
            var medico = listaMedicos.FirstOrDefault(m =>
                string.Equals(
                    $"{m.Apellido} {m.Nombre} ({m.Especialidad})".Trim(),
                    texto,
                    StringComparison.OrdinalIgnoreCase));

            return medico?.Id;
        }

        private int? ObtenerIdProcedimientoSeleccionado()
        {
            if (cbProcedimiento.SelectedValue is int idInt)
                return idInt;

            var texto = cbProcedimiento.Text?.Trim();
            if (string.IsNullOrEmpty(texto))
                return null;

            var proc = listaProcedimientos.FirstOrDefault(p =>
                string.Equals(p.Name?.Trim(), texto, StringComparison.OrdinalIgnoreCase));

            return proc?.Id;
        }



        // ============================= REESTRICCIONES DE ENTRADA DE DATOS =============================
        private void SoloLetras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // ============================= BOTONES =============================
        // Valida todo el formulario y simula guardar (aquí iría la lógica real)
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Por favor, corrija los errores antes de guardar.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validaciones básicas: que haya selección
            if (cbPaciente.SelectedValue == null || cbMedico.SelectedValue == null || cbProcedimiento.SelectedValue == null)
            {
                MessageBox.Show("Falta seleccionar Paciente, Médico o Procedimiento.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int? idPaciente = ObtenerIdPacienteSeleccionado();
            int? idMedico = ObtenerIdMedicoSeleccionado();
            int? idProcedimiento = ObtenerIdProcedimientoSeleccionado();

            if (idPaciente == null || idMedico == null || idProcedimiento == null)
            {
                MessageBox.Show("Falta seleccionar un Paciente, Médico o Procedimiento válido.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int nroHabitacion = int.Parse(cbHabitacion.Text);
            int nroCama = int.Parse(cbCama.Text);

            var dto = new InternacionDto
            {
                Id_paciente = idPaciente.Value,
                Id_medico = idMedico.Value,
                Id_procedimiento = idProcedimiento.Value,
                Nro_habitacion = nroHabitacion,
                Id_cama = nroCama,
                Fecha_ingreso = dtpFechaInicio.Value,
                Fecha_egreso = dtpFechaFin.Checked ? (DateTime?)dtpFechaFin.Value : null,
                Diagnostico = txtObservaciones.Text?.Trim()
            };

            // Llamá a tu capa de negocio
            var servicio = new InternacionService();
            servicio.AltaInternacion(dto);

            MessageBox.Show("Internación registrada con éxito.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            // refrescar UI y fuentes para que desaparezca el paciente y la cama ocupada
            PostGuardarRefrescarUI();

        }


        // Limpia todos los campos del formulario
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            ResetFormulario();
        }

        // Notifica al menú administrativo que se solicitó cancelar
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            CancelarInternacionSolicitada?.Invoke(this, EventArgs.Empty);
        }

        // ============================= RESETEA TODA LA INFORMACION DEL FORMULARIO =============================
        private void ResetFormulario()
        {
            _actualizandoInterno = true;
            try
            {
                // Textos
                txtPiso.Clear();
                txtObservaciones.Clear();

                // Fechas
                dtpFechaInicio.Value = DateTime.Now; 
                dtpFechaFin.Value = DateTime.Today;
                dtpFechaFin.Checked = false;
                dtpFechaFin.CustomFormat = " "; // que quede visualmente vacío

                // Paciente / Médico / Procedimiento
                LimpiarSeleccionCombo(cbPaciente);
                LimpiarSeleccionCombo(cbMedico);
                LimpiarSeleccionCombo(cbProcedimiento);

                // Habitación y Cama (vaciar, limpiar y deshabilitar en cascada)
                VaciarCombo(cbHabitacion);
                VaciarCombo(cbCama);
                errorProvider1.Clear();

                // restaurar médicos completos y procedimientos
                DatosComboBoxMedico();

                SincronizarHabilitacionControles();
            }
            finally
            {
                _actualizandoInterno = false;
            }
        }

        // ============================= HELPERS DE COMBOBOX =============================
        // Limpia la selección de un combo
        private void LimpiarSeleccionCombo(ComboBox cb)
        {
            cb.SelectedIndex = -1;
            cb.Text = string.Empty;
        }

        // Vacía un combo (sin tocar el maestro)
        private void VaciarCombo(ComboBox cb)
        {
            cb.DataSource = null;
            cb.Items.Clear();
            cb.SelectedIndex = -1;
            cb.Text = string.Empty;
        }

        // ============================= SINCRONIZACIÓN DE HABILITACIÓN DE CONTROLES =============================
        private void SincronizarHabilitacionControles()
        {
            bool pisoValido = !string.IsNullOrWhiteSpace(txtPiso.Text)
                              && int.TryParse(txtPiso.Text, out var p) && p > 0;

            cbHabitacion.Enabled = pisoValido;

            bool habitacionValida = pisoValido
                                    && !string.IsNullOrWhiteSpace(cbHabitacion.Text)
                                    && int.TryParse(cbHabitacion.Text, out var h) && h > 0;

            cbCama.Enabled = habitacionValida;

            if (!pisoValido)
            {
                VaciarCombo(cbHabitacion);
                VaciarCombo(cbCama);
            }
            else if (!habitacionValida)
            {
                VaciarCombo(cbCama);
            }
        }

        // ============================= CONFIGURACIONES ESPECIALES DE CONTROLES DE FECHA =============================
        // Fecha inicio con hora (sin calendario desplegable)
        private void ConfigurarFechaInicioConHora()
        {
            dtpFechaInicio.Format = DateTimePickerFormat.Custom;
            dtpFechaInicio.CustomFormat = "dd/MM/yyyy HH:mm"; 
            dtpFechaInicio.ShowUpDown = true;                 
        }

        // Fecha fin opcional (check para "sin fecha")
        private void ConfigurarFechaFinNull()
        {
            dtpFechaFin.ShowCheckBox = true;                 
            dtpFechaFin.Checked = false;                     
            dtpFechaFin.Format = DateTimePickerFormat.Custom;
            dtpFechaFin.CustomFormat = " ";                  
            dtpFechaFin.ValueChanged -= DtpFechaFin_ValueChanged;
            dtpFechaFin.ValueChanged += DtpFechaFin_ValueChanged;
        }
        
        // Evento para actualizar el formato cuando se tilda/desmarca el check
        private void DtpFechaFin_ValueChanged(object sender, EventArgs e)
        {
            if (dtpFechaFin.Checked)
                dtpFechaFin.CustomFormat = "dd/MM/yyyy HH:mm"; // o el formato que uses
            else
                dtpFechaFin.CustomFormat = " ";                // sin fecha visible
        }

        private void PostGuardarRefrescarUI()
        {
            // Limpia los valores ingresados y estados visuales
            ResetFormulario();

            // Recarga las fuentes desde BD para que se apliquen los nuevos estados
            _actualizandoInterno = true;
            try
            {
                DatosComboBoxPaciente();       // quita pacientes ya internados
                DatosComboBoxMedico();         // por si cambió algo de médicos
                DatosComboBoxProcedimiento();  // idem procedimientos

            }
            finally
            {
                _actualizandoInterno = false;
            }

            // Re-sincroniza habilitaciones (quedan deshabilitados hasta que seleccione piso/habitación)
            SincronizarHabilitacionControles();
        }

        // ============================= FILTRADO DE MÉDICOS POR PROCEDIMIENTO =============================
        private void RefrescarMedicosPorProcedimiento(string nombreProc)
        {
            nombreProc = (nombreProc ?? "").Trim();

            var medicosFiltrados =
                (string.IsNullOrEmpty(nombreProc) || EsConsulta(nombreProc))
                ? _medicosMaestro
                : _medicosMaestro.Where(m =>
                      string.Equals(m.Especialidad?.Trim(), nombreProc, StringComparison.OrdinalIgnoreCase)
                  ).ToList();

            cbMedico.BeginUpdate();
            try
            {
                if (medicosFiltrados.Count == 0)
                {
                    cbMedico.DataSource = null;
                    cbMedico.Items.Clear();
                    cbMedico.Text = "";
                    _maestroMedico.Clear();

                    // CLAVE: deshabilitás y evitás validación bloqueante
                    cbMedico.Enabled = false;
                    cbMedico.CausesValidation = false;

                    errorProvider1.SetError(cbMedico, $"No hay médicos para «{nombreProc}».");
                }
                else
                {
                    var fuente = medicosFiltrados.Select(m => new
                    {
                        m.Id,
                        Display = $"{m.Apellido} {m.Nombre} ({m.Especialidad})"
                    }).ToList();

                    cbMedico.DataSource = fuente;
                    cbMedico.DisplayMember = "Display";
                    cbMedico.ValueMember = "Id";
                    cbMedico.SelectedIndex = 0;

                    _maestroMedico.Clear();
                    _maestroMedico.AddRange(fuente.Select(x => x.Display));

                    // Volvemos a habilitar y validar
                    cbMedico.Enabled = true;
                    cbMedico.CausesValidation = true;

                    errorProvider1.SetError(cbMedico, "");
                }
            }
            finally
            {
                cbMedico.EndUpdate();
            }
        }



        private void CbProcedimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            // filtra por el texto visible del procedimiento
            RefrescarMedicosPorProcedimiento(cbProcedimiento.Text);
        }

        private void CbProcedimiento_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RefrescarMedicosPorProcedimiento(cbProcedimiento.Text);
        }

        private bool EsConsulta(string nombreProc)
        {
            if (string.IsNullOrWhiteSpace(nombreProc)) return false;
            var t = nombreProc.Trim();
            // por si viene como "Consulta", "consulta", "Consulta médica", etc.
            return t.Equals("consulta", StringComparison.OrdinalIgnoreCase)
                || t.StartsWith("consulta", StringComparison.OrdinalIgnoreCase);
        }
    }
}
