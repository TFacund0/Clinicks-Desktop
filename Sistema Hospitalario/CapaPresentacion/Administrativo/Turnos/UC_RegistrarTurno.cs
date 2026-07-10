using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.ProcedimientoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;



namespace Sistema_Hospitalario.CapaPresentacion.Administrativo.Turnos
{
    /// <summary>
    /// Control de usuario que gestiona el registro de nuevos turnos médicos.
    /// Incluye lógica avanzada de autofiltrado en ComboBoxes, validación de duplicados y filtrado de médicos por procedimiento.
    /// </summary>
    public partial class UC_RegistrarTurno : UserControl
    {
        /// <summary>Servicio de pacientes para obtener la lista de candidatos.</summary>
        private static readonly PacienteService pacienteService = new PacienteService();
        /// <summary>Servicio de pacientes local.</summary>
        private readonly PacienteService _servicioPaciente = pacienteService;
        /// <summary>Servicio de médicos para obtener profesionales disponibles.</summary>
        private readonly MedicoService _servicioMedico = new MedicoService();
        /// <summary>Servicio de procedimientos para catalogar el tipo de cita.</summary>
        private readonly ProcedimientoService _servicioProcedimiento = new ProcedimientoService();

        /// <summary>Maestro de nombres de pacientes para validación de entrada de texto.</summary>
        private readonly List<string> _maestroPaciente = new List<string>();
        /// <summary>Maestro de nombres de médicos para validación de entrada de texto.</summary>
        private readonly List<string> _maestroMedico = new List<string>();
        /// <summary>Maestro de procedimientos para validación de entrada de texto.</summary>
        private readonly List<string> _maestroProcedimiento = new List<string>();

        /// <summary>Caché local de DTOs de médicos para filtrado rápido sin reconsultar la base de datos.</summary>
        private List<MedicoDto> _medicosMaestroDtos = new List<MedicoDto>();
        /// <summary>Caché local de DTOs de procedimientos.</summary>
        private List<ProcedimientoDto> _procedimientosMaestroDtos = new List<ProcedimientoDto>();
        /// <summary>Caché local de DTOs de pacientes.</summary>
        private List<PacienteDto> _pacientesMaestroDtos = new List<PacienteDto>();

        /// <summary>Bandera para evitar recursividad infinita durante la actualización de texto en combos de autofiltrado.</summary>
        private bool _actualizandoInterno = false;

        /// <summary>Evento que notifica que se ha cancelado el registro del turno.</summary>
        public event EventHandler CancelarTurnoSolicitado;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_RegistrarTurno"/>.
        /// Configura las fechas permitidas y puebla los catálogos iniciales de pacientes, médicos y procedimientos.
        /// </summary>
        public UC_RegistrarTurno()
        {
            InitializeComponent();

            txtObservaciones.MaxLength = 200;
            txtCorreo.MaxLength = 150;
            txtTelefono.MaxLength = 30;
            txtTelefono.Validating += TxtTelefono_Validating;

            dtpFechaTurno.MinDate = DateTime.Today.AddDays(1);
            dtpFechaTurno.Value = DateTime.Today.AddDays(1);

            DatosComboBoxPaciente();
            DatosComboBoxMedico();
            DatosComboBoxProcedimiento();

            // Filtro médicos por procedimiento
            cbProcedimiento.SelectedIndexChanged -= CbProcedimiento_SelectedIndexChanged;
            cbProcedimiento.SelectedIndexChanged += CbProcedimiento_SelectedIndexChanged;

            cbProcedimiento.SelectionChangeCommitted -= CbProcedimiento_SelectionChangeCommitted;
            cbProcedimiento.SelectionChangeCommitted += CbProcedimiento_SelectionChangeCommitted;

            // <<< NUEVO: autofiltrado mientras escribís
            cbPaciente.TextUpdate += CbPaciente_TextUpdate;
            cbMedico.TextUpdate += CbMedico_TextUpdate;
            cbProcedimiento.TextUpdate += CbProcedimiento_TextUpdate;

            // (Opcional pero lindo: que se despliegue al entrar)
            cbPaciente.Enter += (s, e) => cbPaciente.DroppedDown = true;
            cbMedico.Enter += (s, e) => cbMedico.DroppedDown = true;
            cbProcedimiento.Enter += (s, e) => cbProcedimiento.DroppedDown = true;
        }

        // ========================= CARGA DE DATOS EN COMBOBOXES =========================
        // ==== Carga ComboBox Paciente ====
        private void DatosComboBoxPaciente()
        {
            List<PacienteDto> listaPacientes = _servicioPaciente.ListarPacientes();

            _pacientesMaestroDtos = listaPacientes
                .Where(p => p.Estado_paciente.ToLower() == "activo" || p.Estado_paciente.ToLower() == "alta")
                .ToList();

            var fuente = _pacientesMaestroDtos
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

            // <<< CLAVE: que no quede nada preseleccionado
            cbPaciente.SelectedIndex = -1;
            cbPaciente.Text = string.Empty;
        }

        // ==== Carga ComboBox Médico ====
        private void DatosComboBoxMedico()
        {
            List<MedicoDto> listaMedicos = _servicioMedico.ListarMedicos() ?? new List<MedicoDto>();

            _medicosMaestroDtos = listaMedicos.ToList();

            var fuente = listaMedicos
                .Select(m => new {
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

            cbMedico.SelectedIndex = -1;          // <<< ahí
            cbMedico.Text = string.Empty;
        }

        // ==== Carga ComboBox Procedimiento ====
        private void DatosComboBoxProcedimiento()
        {
            List<ProcedimientoDto> listaProcedimientos = _servicioProcedimiento.ListarProcedimientos() ?? new List<ProcedimientoDto>();

            _procedimientosMaestroDtos = listaProcedimientos.ToList();

            var fuente = listaProcedimientos
                .Select(p => new {
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

            cbProcedimiento.SelectedIndex = -1;   // <<< ahí
            cbProcedimiento.Text = string.Empty;
        }


        // ========================= VALIDACIONES =========================
        // ==== Validacion ComboBox Paciente ====
        private void CbPaciente_Validating(object sender, CancelEventArgs e)
        {
            if (!TextoCoincideConLista(cbPaciente, _maestroPaciente))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbPaciente, "Seleccione un paciente válido de la lista.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(cbPaciente, null);
            }
        }

        // ==== Validacion ComboBox Medico ====
        private void CbMedico_Validating(object sender, CancelEventArgs e)
        {
            if (!cbMedico.Enabled || !cbMedico.CausesValidation || _maestroMedico.Count == 0)
                return;

            if (!TextoCoincideConLista(cbMedico, _maestroMedico))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbMedico, "Seleccione un médico válido de la lista.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(cbMedico, null);
            }
        }

        // ==== Validacion ComboBox Procedimiento ====
        private void CbProcedimiento_Validating(object sender, CancelEventArgs e)
        {
            if (!TextoCoincideConLista(cbProcedimiento, _maestroProcedimiento))
            {
                e.Cancel = true;
                errorProvider1.SetError(cbProcedimiento, "Seleccione un procedimiento válido de la lista.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(cbProcedimiento, null);
            }
        }


        // ==== Validacion TextBox Observaciones ====
        private void TxtObservaciones_Validating(object sender, CancelEventArgs e)
        {
            var texto = txtObservaciones.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtObservaciones, "Las observaciones son obligatorias.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtObservaciones, null);
            }
        }

        // ==== Validacion Correo ====
        private void TxtCorreo_Validating(object sender, CancelEventArgs e)
        {
            var correo = txtCorreo.Text.Trim();

            if (string.IsNullOrWhiteSpace(correo))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtCorreo, "El correo es obligatorio.");
                return;
            }

            // Validación simple de formato de correo
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtCorreo, "El formato del correo no es válido.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtCorreo, null);
            }
        }

        // ==== Validacion TextBox Telefono (opcional) ====
        private void TxtTelefono_Validating(object sender, CancelEventArgs e)
        {
            var telefono = txtTelefono.Text.Trim();

            if (string.IsNullOrWhiteSpace(telefono))
            {
                e.Cancel = false;
                errorProvider1.SetError(txtTelefono, null);
                return;
            }

            if (!telefono.All(char.IsDigit))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTelefono, "El teléfono debe contener solo números.");
            }
            else if (telefono.Length < 6 || telefono.Length > 30)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTelefono, "El teléfono debe tener entre 6 y 30 dígitos.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtTelefono, null);
            }
        }

        // ==== Validacion DateTimePicker Fecha ====
        private void DtpFecha_Validating(object sender, CancelEventArgs e)
        {
            if (!ValidarFechaTurno())
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }

        // Validación específica de la fecha del turno
        private bool ValidarFechaTurno()
        {
            if (dtpFechaTurno.Value.Date <= DateTime.Today)
            {
                errorProvider1.SetError(dtpFechaTurno, "La fecha del turno debe ser posterior a hoy.");
                return false;
            }

            errorProvider1.SetError(dtpFechaTurno, null);
            return true;
        }


        // ========================= BOTONES =========================

        // ==== Boton Cancelar ====
        private void BtnCancelar_Click_1(object sender, EventArgs e)
        {
            CancelarTurnoSolicitado?.Invoke(this, EventArgs.Empty);
        }

        // ==== Boton Guardar ====
        private void BtnGuardar_Click_1(object sender, EventArgs e)
        {
            // Confirmación previa
            var confirmacion = MessageBox.Show(
                "¿Estás seguro de que deseas guardar este turno?",
                "Confirmar guardado",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmacion != DialogResult.Yes)
                return;

            // Ejecutar todas las validaciones de los controles (Validating de combos, correo, observaciones, etc.)
            bool controlesValidos = this.ValidateChildren();

            // Refuerzo explícito de la validación de fecha (por si el Validating no se disparó)
            bool fechaValida = ValidarFechaTurno();

            if (!controlesValidos || !fechaValida)
            {
                MessageBox.Show(
                    "Por favor, corrija los errores antes de guardar.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Chequeo defensivo extra por si algún SelectedValue viene en null
            if (cbPaciente.SelectedValue == null || cbMedico.SelectedValue == null || cbProcedimiento.SelectedValue == null)
            {
                MessageBox.Show(
                    "Debe seleccionar paciente, médico y procedimiento.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            int idPaciente = (int)cbPaciente.SelectedValue;
            int idMedico = (int)cbMedico.SelectedValue;
            DateTime fechaTurno = dtpFechaTurno.Value;

            // >>>>>> NUEVA VALIDACIÓN DE REGRA DE NEGOCIO <<<<<<
            var turnoService = new TurnoService();
            bool yaTieneTurnoEseDia = turnoService
                .ExisteTurnoMismoDiaMismoMedicoPaciente(idPaciente, idMedico, fechaTurno);

            if (yaTieneTurnoEseDia)
            {
                MessageBox.Show(
                    "Este paciente ya tiene un turno con el mismo médico para ese día.\n" +
                    "No se puede registrar otro turno igual.",
                    "Turno duplicado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            // >>>>>> FIN VALIDACIÓN <<<<<<

            // Construir el DTO del turno
            var nuevoTurno = new TurnoDto
            {
                Id_paciente = idPaciente,
                Id_medico = idMedico,
                Id_procedimiento = (int)cbProcedimiento.SelectedValue,
                FechaTurno = fechaTurno,
                Observaciones = txtObservaciones.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Correo = string.IsNullOrWhiteSpace(txtCorreo.Text) ? null : txtCorreo.Text.Trim()
            };

            // Guardar usando el servicio
            try
            {
                turnoService.RegistrarTurno(nuevoTurno);

                MessageBox.Show(
                    "Turno registrado con éxito.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al registrar el turno. Verifique los datos o inténtelo nuevamente. ",
                    "Error: " + ex,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // ==== Boton Limpiar ====
        private void BtnLimpiar_Click_1(object sender, EventArgs e)
        {
            cbPaciente.SelectedIndex = -1;
            cbMedico.SelectedIndex = -1;
            cbProcedimiento.SelectedIndex = -1;

            dtpFechaTurno.MinDate = DateTime.Today.AddDays(1);
            dtpFechaTurno.Value = DateTime.Today.AddDays(1);

            txtObservaciones.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            errorProvider1.Clear();

            // recargar médicos completos
            DatosComboBoxMedico();
        }


        // ========================= FILTRADO MÉDICOS POR PROCEDIMIENTO =========================
        private void RefrescarMedicosPorProcedimiento(string nombreProc)
        {
            nombreProc = (nombreProc ?? "").Trim();

            // Si el procedimiento es vacío o es una "consulta", mostramos todos
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

        // Determina si el nombre del procedimiento indica una "consulta"
        private bool EsConsulta(string nombreProc)
        {
            if (string.IsNullOrWhiteSpace(nombreProc)) return false;
            var t = nombreProc.Trim();
            return t.Equals("consulta", StringComparison.OrdinalIgnoreCase)
                || t.StartsWith("consulta", StringComparison.OrdinalIgnoreCase);
        }

        // Eventos para refrescar médicos al cambiar procedimiento
        private void CbProcedimiento_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefrescarMedicosPorProcedimiento(cbProcedimiento.Text);
        }

        // === Alternativo por si se usa SelectionChangeCommitted ===
        private void CbProcedimiento_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RefrescarMedicosPorProcedimiento(cbProcedimiento.Text);
        }

        // ========================= AUTOFILTRADO DE COMBOS =========================
        private bool TextoCoincideConLista(ComboBox cb, List<string> maestro)
        {
            var t = cb.Text?.Trim();
            if (string.IsNullOrEmpty(t) || t == "— sin coincidencias —") return false;

            return maestro.Any(x => string.Equals(x?.Trim(), t, StringComparison.OrdinalIgnoreCase));
        }

        // === Autofiltrado ComboBox Médico ===
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

            // Armamos la fuente para el combo (Id + Display)
            var fuente = dtosFiltrados
                .Select(p => new
                {
                    p.Id,
                    Display = $"{p.Apellido} {p.Nombre} ({p.Dni})"
                })
                .ToList();

            if (fuente.Count == 0)
            {
                // Mostramos solo un item informativo
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

                // actualizar maestro de strings para la validación
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

        // === Autofiltrado ComboBox Médico ===
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

        // === Autofiltrado ComboBox Procedimiento ===
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

    }
}
