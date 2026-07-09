using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using Sistema_Hospitalario.CapaNegocio.Servicios.ProcedimientoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo.Turnos
{
    /// <summary>
    /// Control de usuario que permite visualizar y editar el detalle de un turno existente.
    /// Soporta la actualización de paciente, médico, procedimiento, fecha, estado y observaciones,
    /// incluyendo lógica de validación para evitar duplicidad de citas.
    /// </summary>
    public partial class UC_VisualizarTurno : UserControl
    {
        /// <summary>DTO que contiene los datos del turno que se está visualizando.</summary>
        private readonly TurnoDto _turno;
        /// <summary>Bandera que indica si el control se encuentra en modo edición.</summary>
        private bool _modoEdicion = false;

        /// <summary>Servicio para gestión de pacientes.</summary>
        private readonly PacienteService _servicioPaciente = new PacienteService();
        /// <summary>Servicio para gestión de médicos.</summary>
        private readonly MedicoService _servicioMedico = new MedicoService();
        /// <summary>Servicio para gestión de procedimientos.</summary>
        private readonly ProcedimientoService _servicioProcedimiento = new ProcedimientoService();

        /// <summary>Maestro de visualización de pacientes.</summary>
        private readonly List<string> _maestroPaciente = new List<string>();
        /// <summary>Maestro de visualización de médicos.</summary>
        private readonly List<string> _maestroMedico = new List<string>();
        /// <summary>Maestro de visualización de procedimientos.</summary>
        private readonly List<string> _maestroProcedimiento = new List<string>();

        /// <summary>Caché de DTOs de pacientes filtrados.</summary>
        private List<PacienteDto> _pacientesMaestroDtos = new List<PacienteDto>();
        /// <summary>Caché de DTOs de médicos filtrados.</summary>
        private List<MedicoDto> _medicosMaestroDtos = new List<MedicoDto>();
        /// <summary>Caché de DTOs de procedimientos filtrados.</summary>
        private List<ProcedimientoDto> _procedimientosMaestroDtos = new List<ProcedimientoDto>();

        /// <summary>Bandera para evitar recursividad en eventos de actualización de texto.</summary>
        private bool _actualizandoInterno = false;

        /// <summary>Evento que notifica que se ha cancelado la visualización del turno.</summary>
        public event EventHandler CancelarVisualizacionSolicitada;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_VisualizarTurno"/>.
        /// </summary>
        /// <param name="turno">DTO con la información del turno a cargar.</param>
        public UC_VisualizarTurno(TurnoDto turno)
        {
            InitializeComponent();
            _turno = turno ?? throw new ArgumentNullException(nameof(turno));

            InicializarCombosYFiltros();
            ConfigurarUISoloLectura();
        }

        // ================== CONFIG INICIAL ==================
        private void InicializarCombosYFiltros()
        {
            // Cargar combos con datos de BD
            DatosComboBoxPaciente();
            DatosComboBoxMedico();
            DatosComboBoxProcedimiento();

            // Seleccionar valores del turno actual
            SeleccionarValoresIniciales();

            // Filtro médicos por procedimiento
            cbProcedimiento.SelectedIndexChanged -= CbProcedimiento_SelectedIndexChanged;
            cbProcedimiento.SelectedIndexChanged += CbProcedimiento_SelectedIndexChanged;

            cbProcedimiento.SelectionChangeCommitted -= CbProcedimiento_SelectionChangeCommitted;
            cbProcedimiento.SelectionChangeCommitted += CbProcedimiento_SelectionChangeCommitted;

            // Filtro en vivo mientras escribís
            cbPaciente.TextUpdate += CbPaciente_TextUpdate;
            cbMedico.TextUpdate += CbMedico_TextUpdate;
            cbProcedimiento.TextUpdate += CbProcedimiento_TextUpdate;

            // Auto desplegar al entrar
            cbPaciente.Enter += (s, e) => cbPaciente.DroppedDown = true;
            cbMedico.Enter += (s, e) => cbMedico.DroppedDown = true;
            cbProcedimiento.Enter += (s, e) => cbProcedimiento.DroppedDown = true;
        }

        private void ConfigurarUISoloLectura()
        {
            ToggleEdicion(false);
            CargarDatosLectura(_turno);
            CargarComboBoxTurno();
        }

        // ================== CARGA INICIAL DE DATOS ==================
        private void CargarDatosLectura(TurnoDto p_turno)
        {
            txtPaciente.Text = p_turno.Paciente;
            txtMedico.Text = p_turno.Medico;
            txtProcedimiento.Text = p_turno.Procedimiento;
            txtCorreo.Text = p_turno.Correo ?? string.Empty;
            txtTelefono.Text = p_turno.Telefono ?? string.Empty;
            dtpFechaTurno.Value = p_turno.FechaTurno;
            txtObservaciones.Text = p_turno.Observaciones ?? string.Empty;
        }

        // Hace que los combos queden con el turno actual seleccionado
        private void SeleccionarValoresIniciales()
        {
            // Procedimiento primero (porque filtra médicos)
            if (_turno.Id_procedimiento > 0)
            {
                var proc = _procedimientosMaestroDtos
                    .FirstOrDefault(p => p.Id == _turno.Id_procedimiento);

                if (proc != null)
                {
                    cbProcedimiento.SelectedValue = proc.Id;
                    // filtrar médicos según este procedimiento
                    RefrescarMedicosPorProcedimiento(proc.Name);
                }
                else
                {
                    RefrescarMedicosPorProcedimiento("");
                }
            }
            else
            {
                RefrescarMedicosPorProcedimiento("");
            }

            // Paciente
            if (_turno.Id_paciente > 0)
            {
                cbPaciente.SelectedValue = _turno.Id_paciente;
                if (cbPaciente.SelectedItem != null)
                    cbPaciente.Text = ((dynamic)cbPaciente.SelectedItem).Display;
            }

            // Médico
            if (_turno.Id_medico > 0)
            {
                cbMedico.SelectedValue = _turno.Id_medico;
                if (cbMedico.SelectedItem != null)
                    cbMedico.Text = ((dynamic)cbMedico.SelectedItem).Display;
            }
        }

        // ================== TOGGLE EDICIÓN ==================
        private void ToggleEdicion(bool habilitar)
        {
            _modoEdicion = habilitar;

            txtPaciente.ReadOnly = !habilitar;
            txtMedico.ReadOnly = !habilitar;
            txtProcedimiento.ReadOnly = !habilitar;
            txtCorreo.ReadOnly = !habilitar;
            txtTelefono.ReadOnly = !habilitar;
            txtObservaciones.ReadOnly = !habilitar;
            dtpFechaTurno.Enabled = habilitar;

            txtPaciente.Visible = !habilitar;
            txtMedico.Visible = !habilitar;
            txtProcedimiento.Visible = !habilitar;

            cbPaciente.Visible = habilitar;
            cbPaciente.Enabled = habilitar;
            cbMedico.Visible = habilitar;
            cbMedico.Enabled = habilitar;
            cbProcedimiento.Visible = habilitar;
            cbProcedimiento.Enabled = habilitar;

            lblEstado.Visible = habilitar;
            cbEstadoTurno.Visible = habilitar;
            cbEstadoTurno.Enabled = habilitar;

            btnModificar.Text = habilitar ? "Guardar" : "Modificar";
            btnEliminar.Enabled = !habilitar;
        }

        // ================== CARGA DE COMBOS (ESTILO REGISTRAR TURNO) ==================
        private void DatosComboBoxPaciente()
        {
            List<PacienteDto> listaPacientes = _servicioPaciente.ListarPacientes();

            _pacientesMaestroDtos = listaPacientes
                .Where(p => p.Estado_paciente == "activo" || p.Estado_paciente == "internado")
                .ToList();

            var fuente = _pacientesMaestroDtos
                .Select(p => new
                {
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

        private void DatosComboBoxMedico()
        {
            List<MedicoDto> listaMedicos = _servicioMedico.ListarMedicos() ?? new List<MedicoDto>();

            _medicosMaestroDtos = listaMedicos.ToList();

            var fuente = listaMedicos
                .Select(m => new
                {
                    m.Id,
                    Display = $"{m.Apellido} {m.Nombre} ({m.Especialidad})"
                })
                .ToList();

            _maestroMedico.Clear();
            _maestroMedico.AddRange(fuente.Select(x => x.Display));

            cbMedico.DropDownStyle = ComboBoxStyle.DropDown;
            cbMedico.DataSource = fuente;
            cbMedico.DisplayMember = "Display";
            cbMedico.ValueMember = "Id";
        }

        private void DatosComboBoxProcedimiento()
        {
            List<ProcedimientoDto> listaProcedimientos = _servicioProcedimiento.ListarProcedimientos() ?? new List<ProcedimientoDto>();

            _procedimientosMaestroDtos = listaProcedimientos.ToList();

            var fuente = listaProcedimientos
                .Select(p => new
                {
                    p.Id,
                    Display = p.Name
                })
                .ToList();

            _maestroProcedimiento.Clear();
            _maestroProcedimiento.AddRange(fuente.Select(x => x.Display));

            cbProcedimiento.DropDownStyle = ComboBoxStyle.DropDown;
            cbProcedimiento.DataSource = fuente;
            cbProcedimiento.DisplayMember = "Display";
            cbProcedimiento.ValueMember = "Id";
        }

        private void CargarComboBoxTurno()
        {
            var turnoService = new TurnoService();
            var listaEstadosTurnos = turnoService.ListadoEstadosTurnos();

            var lista = new List<ListadoEstadoTurno>
            {
                new ListadoEstadoTurno { Id_estado = "0", Estado = "— Seleccioná —" }
            };

            lista.AddRange(listaEstadosTurnos);

            cbEstadoTurno.DropDownStyle = ComboBoxStyle.DropDown;
            cbEstadoTurno.DataSource = lista;
            cbEstadoTurno.DisplayMember = nameof(ListadoEstadoTurno.Estado);
            cbEstadoTurno.ValueMember = nameof(ListadoEstadoTurno.Id_estado);

            // seleccionar el estado actual si existe
            if (!string.IsNullOrEmpty(_turno.Estado))
            {
                var idx = lista.FindIndex(e =>
                    string.Equals(e.Estado, _turno.Estado, StringComparison.OrdinalIgnoreCase));
                cbEstadoTurno.SelectedIndex = idx >= 0 ? idx : 0;
            }
            else
            {
                cbEstadoTurno.SelectedIndex = 0;
            }
        }

        // ================== FILTRADO MÉDICOS POR PROCEDIMIENTO ==================
        private void RefrescarMedicosPorProcedimiento(string nombreProc)
        {
            nombreProc = (nombreProc ?? "").Trim();

            var medicosFiltrados =
                (string.IsNullOrEmpty(nombreProc) || EsConsulta(nombreProc))
                ? _medicosMaestroDtos
                : _medicosMaestroDtos.Where(m =>
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

                    cbMedico.Enabled = false;
                    cbMedico.CausesValidation = false;
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

                    _maestroMedico.Clear();
                    _maestroMedico.AddRange(fuente.Select(x => x.Display));

                    cbMedico.Enabled = true;
                    cbMedico.CausesValidation = true;
                }
            }
            finally
            {
                cbMedico.EndUpdate();
            }
        }

        private bool EsConsulta(string nombreProc)
        {
            if (string.IsNullOrWhiteSpace(nombreProc)) return false;
            var t = nombreProc.Trim();
            return t.Equals("consulta", StringComparison.OrdinalIgnoreCase)
                || t.StartsWith("consulta", StringComparison.OrdinalIgnoreCase);
        }

        private void CbProcedimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefrescarMedicosPorProcedimiento(cbProcedimiento.Text);
        }

        private void CbProcedimiento_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RefrescarMedicosPorProcedimiento(cbProcedimiento.Text);
        }

        // ================== AUTOFILTRADO DE COMBOS ==================
        private bool TextoCoincideConLista(ComboBox cb, List<string> maestro)
        {
            var t = cb.Text?.Trim();
            if (string.IsNullOrEmpty(t) || t == "— sin coincidencias —") return false;

            return maestro.Any(x => string.Equals(x?.Trim(), t, StringComparison.OrdinalIgnoreCase));
        }

        private void CbPaciente_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;

            string texto = cbPaciente.Text?.Trim() ?? "";
            int caret = cbPaciente.SelectionStart;

            var dtosFiltrados = string.IsNullOrEmpty(texto)
                ? _pacientesMaestroDtos
                : _pacientesMaestroDtos
                    .Where(p =>
                    {
                        var display = $"{p.Apellido} {p.Nombre} ({p.Dni})";
                        return display.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0;
                    })
                    .ToList();

            var fuente = dtosFiltrados
                .Select(p => new
                {
                    p.Id,
                    Display = $"{p.Apellido} {p.Nombre} ({p.Dni})"
                })
                .ToList();

            if (fuente.Count == 0)
            {
                fuente.Add(new { Id = 0, Display = "— sin coincidencias —" });
            }

            try
            {
                _actualizandoInterno = true;
                cbPaciente.BeginUpdate();

                cbPaciente.DataSource = null;
                cbPaciente.DataSource = fuente;
                cbPaciente.DisplayMember = "Display";
                cbPaciente.ValueMember = "Id";

                _maestroPaciente.Clear();
                _maestroPaciente.AddRange(fuente.Select(x => x.Display));

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

        private void CbMedico_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;

            string texto = cbMedico.Text?.Trim() ?? "";
            int caret = cbMedico.SelectionStart;

            var dtosFiltrados = string.IsNullOrEmpty(texto)
                ? _medicosMaestroDtos
                : _medicosMaestroDtos
                    .Where(m =>
                    {
                        var display = $"{m.Apellido} {m.Nombre} ({m.Especialidad})";
                        return display.IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0;
                    })
                    .ToList();

            var fuente = dtosFiltrados
                .Select(m => new
                {
                    m.Id,
                    Display = $"{m.Apellido} {m.Nombre} ({m.Especialidad})"
                })
                .ToList();

            if (fuente.Count == 0)
            {
                fuente.Add(new { Id = 0, Display = "— sin coincidencias —" });
            }

            try
            {
                _actualizandoInterno = true;
                cbMedico.BeginUpdate();

                cbMedico.DataSource = null;
                cbMedico.DataSource = fuente;
                cbMedico.DisplayMember = "Display";
                cbMedico.ValueMember = "Id";

                _maestroMedico.Clear();
                _maestroMedico.AddRange(fuente.Select(x => x.Display));

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

        private void CbProcedimiento_TextUpdate(object sender, EventArgs e)
        {
            if (_actualizandoInterno) return;

            string texto = cbProcedimiento.Text?.Trim() ?? "";
            int caret = cbProcedimiento.SelectionStart;

            var dtosFiltrados = string.IsNullOrEmpty(texto)
                ? _procedimientosMaestroDtos
                : _procedimientosMaestroDtos
                    .Where(p => (p.Name ?? "")
                        .IndexOf(texto, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            var fuente = dtosFiltrados
                .Select(p => new
                {
                    p.Id,
                    Display = p.Name
                })
                .ToList();

            if (fuente.Count == 0)
            {
                fuente.Add(new { Id = 0, Display = "— sin coincidencias —" });
            }

            try
            {
                _actualizandoInterno = true;
                cbProcedimiento.BeginUpdate();

                cbProcedimiento.DataSource = null;
                cbProcedimiento.DataSource = fuente;
                cbProcedimiento.DisplayMember = "Display";
                cbProcedimiento.ValueMember = "Id";

                _maestroProcedimiento.Clear();
                _maestroProcedimiento.AddRange(fuente.Select(x => x.Display));

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

        // ================== GUARDAR CAMBIOS ==================
        private void ActualizarTurno()
        {
            _turno.Paciente = cbPaciente.Text.Trim();
            _turno.Medico = cbMedico.Text.Trim();
            _turno.Procedimiento = cbProcedimiento.Text.Trim();

            _turno.Id_paciente = (int)(cbPaciente.SelectedValue ?? 0);
            _turno.Id_medico = (int)(cbMedico.SelectedValue ?? 0);
            _turno.Id_procedimiento = (int)(cbProcedimiento.SelectedValue ?? 0);

            _turno.Correo = txtCorreo.Text.Trim();
            _turno.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text)
                ? null
                : txtTelefono.Text.Trim();

            _turno.FechaTurno = dtpFechaTurno.Value;
            _turno.Observaciones = txtObservaciones.Text.Trim();

            _turno.Estado = cbEstadoTurno.SelectedIndex > 0
                ? ((ListadoEstadoTurno)cbEstadoTurno.SelectedItem).Estado
                : _turno.Estado;
        }

        private void GuardarCambios()
        {
            TurnoService turnoService = new TurnoService();
            turnoService.ActualizarTurno(_turno.Id_turno, _turno);
        }

        // ================== BOTONES ==================
        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (!_modoEdicion)
            {
                ToggleEdicion(true);
            }
            else
            {
                var confirmacion = MessageBox.Show(
                    "¿Estás seguro de que deseas guardar este turno?",
                    "Confirmar guardado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                if (!ValidarDatosTurno(out var msg))
                {
                    MessageBox.Show(msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ActualizarTurno();
                GuardarCambios();
                ConfigurarUISoloLectura();

                MessageBox.Show("Cambios guardados.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            TurnoService turnoService = new TurnoService();
            var dr = MessageBox.Show("¿Eliminar este turno?",
                                     "Confirmación",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                turnoService.EliminarTurno(_turno.Id_turno);
                CancelarVisualizacionSolicitada?.Invoke(this, EventArgs.Empty);
            }
        }

        private void BtnVolver_Click(object sender, EventArgs e)
        {
            CancelarVisualizacionSolicitada?.Invoke(this, EventArgs.Empty);
        }

        // ================== VALIDACIONES (alineadas con RegistrarTurno) ==================
        private bool ValidarDatosTurno(out string error)
        {
            error = null;

            // Paciente obligatorio + tiene que coincidir con la lista
            if (cbPaciente.SelectedValue == null || !TextoCoincideConLista(cbPaciente, _maestroPaciente))
            {
                error = "Debe seleccionar un paciente válido.";
                return false;
            }

            // Médico obligatorio + lista
            if (cbMedico.SelectedValue == null ||
                (cbMedico.Enabled && !TextoCoincideConLista(cbMedico, _maestroMedico)))
            {
                error = "Debe seleccionar un médico válido.";
                return false;
            }

            // Procedimiento obligatorio + lista
            if (cbProcedimiento.SelectedValue == null ||
                !TextoCoincideConLista(cbProcedimiento, _maestroProcedimiento))
            {
                error = "Debe seleccionar un procedimiento válido.";
                return false;
            }

            // Correo obligatorio + formato
            var correo = txtCorreo.Text.Trim();
            if (string.IsNullOrWhiteSpace(correo))
            {
                error = "El correo es obligatorio.";
                return false;
            }
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                error = "El formato del correo no es válido.";
                return false;
            }

            // Observaciones obligatorias
            var obs = txtObservaciones.Text.Trim();
            if (string.IsNullOrWhiteSpace(obs))
            {
                error = "Las observaciones son obligatorias.";
                return false;
            }
            if (obs.Length > 200)
            {
                error = "Las observaciones no pueden superar 200 caracteres.";
                return false;
            }

            // Fecha del turno: posterior a hoy
            var fecha = dtpFechaTurno.Value.Date;
            if (fecha <= DateTime.Today)
            {
                error = "La fecha del turno debe ser posterior a hoy.";
                return false;
            }

            // ========== NUEVA REGLA: evitar duplicado excluyendo este turno ==========
            var turnoService = new TurnoService();

            int idPaciente = (int)cbPaciente.SelectedValue;
            int idMedico = (int)cbMedico.SelectedValue;
            DateTime fechaTurno = dtpFechaTurno.Value;

            bool yaExiste = turnoService
                .ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(
                    _turno.Id_turno,    // <<< excluimos este mismo turno
                    idPaciente,
                    idMedico,
                    fechaTurno);

            if (yaExiste)
            {
                error = "Este paciente ya tiene un turno con el mismo médico para ese día.";
                return false;
            }
            // ========================================================================

            return true;
        }

    }
}
