using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.DTOs;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo.Pacientes
{
    /// <summary>
    /// Control de usuario que permite visualizar y editar el detalle de un paciente existente.
    /// Soporta la actualización de datos personales, contacto, observaciones y cambio de estado clínico.
    /// </summary>
    public partial class UC_VisualizarPaciente : UserControl
    {
        /// <summary>Servicio para operaciones de pacientes.</summary>
        private readonly PacienteService _pacienteService = new PacienteService();
        /// <summary>Servicio para gestión de estados de pacientes.</summary>
        private readonly EstadoPacienteService _estadoService = new EstadoPacienteService();

        /// <summary>DTO que mantiene la información detallada del paciente actual.</summary>
        private PacienteDetalleDto _pacienteDto;

        /// <summary>Bandera que indica si el control está en modo edición.</summary>
        private bool _modoEdicion = false;

        /// <summary>Evento que notifica que se ha cancelado la visualización para volver al listado.</summary>
        public event EventHandler CancelarVisualizacionSolicitada;

        /// <summary>Evento que notifica que los datos del paciente han sido actualizados con éxito.</summary>
        public event EventHandler<PacienteDetalleDto> PacienteActualizado;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_VisualizarPaciente"/>.
        /// </summary>
        /// <param name="paciente">DTO con los datos iniciales del paciente a visualizar.</param>
        public UC_VisualizarPaciente(PacienteDetalleDto paciente)
        {
            InitializeComponent();

            // Configuración de fecha de nacimiento
            dtpNacimiento.MaxDate = DateTime.Today;
            dtpNacimiento.MinDate = DateTime.Today.AddYears(-120);
            dtpNacimiento.Format = DateTimePickerFormat.Short;

            _pacienteDto = paciente ?? throw new ArgumentNullException(nameof(paciente));

            ConfigurarUiSoloLectura();
            CargarDatos(_pacienteDto);

            SelectEventosPaciente();
            CargarEstadosEnCombo();

            if (btnEditar != null)
            {
                btnEditar.Visible = true;
                btnEditar.Enabled = true;
                btnEditar.Text = "Editar";
                btnEditar.Click -= btnEditar_Click;
                btnEditar.Click += btnEditar_Click;
            }

            if (btnCancelar != null)
            {
                btnCancelar.Click -= btnCancelar_Click;
                btnCancelar.Click += btnCancelar_Click;
            }
        }


        // ========================= CARGA DE DATOS DEL PACIENTE =========================
        private void CargarDatos(PacienteDetalleDto p)
        {
            txtNombre.Text = p.Nombre ?? string.Empty;
            txtApellido.Text = p.Apellido ?? string.Empty;
            txtDni.Text = p.DNI.ToString();
            txtEmail.Text = p.Email ?? string.Empty;
            txtDireccion.Text = p.Direccion ?? string.Empty;
            txtTelefono.Text = p.Telefono ?? string.Empty;
            txtObservaciones.Text = p.Observaciones ?? string.Empty;

            // Mostrar en TextBox (modo lectura)
            txtEstado.Text = (p.Estado ?? string.Empty).Trim();

            // Preseleccionar en el Combo (modo edicion)
            SeleccionarEstadoPorNombre(txtEstado.Text);

            var fecha = (p.FechaNacimiento == DateTime.MinValue) ? DateTime.Today : p.FechaNacimiento;
            dtpNacimiento.Value = fecha;
        }


        // ========================= COMBOBOX ESTADO PACIENTE =========================

        // Eventos para abrir el combo automáticamente
        private void SelectEventosPaciente()
        {
            // Abrir automáticamente al ganar foco o click
            cbEstadoInicial.Enter += (s, e) => cbEstadoInicial.DroppedDown = true;
            cbEstadoInicial.MouseDown += (s, e) => cbEstadoInicial.DroppedDown = true;
        }

        // Cargar estados desde BD y configurar combo
        private void CargarEstadosEnCombo()
        {
            var estados = _estadoService.ListarEstados(); // List<EstadoPacienteDto> { Id, Nombre }

            var lista = new List<EstadoPacienteDto>
            {
                new EstadoPacienteDto { Id = 0, Nombre = "— Seleccioná —" }
            };
            lista.AddRange(estados);

            cbEstadoInicial.DisplayMember = nameof(EstadoPacienteDto.Nombre);
            cbEstadoInicial.ValueMember = nameof(EstadoPacienteDto.Id);
            cbEstadoInicial.DataSource = lista;

            cbEstadoInicial.SelectedIndex = 0;
 
            cbEstadoInicial.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        // Seleccionar en el combo el estado que coincide con el nombre dado
        private void SeleccionarEstadoPorNombre(string nombreEstado)
        {
            if (string.IsNullOrWhiteSpace(nombreEstado))
            {
                cbEstadoInicial.SelectedIndex = 0;
                return;
            }

            for (int i = 0; i < cbEstadoInicial.Items.Count; i++)
            {
                var it = cbEstadoInicial.Items[i] as EstadoPacienteDto;
                if (it != null && string.Equals(it.Nombre, nombreEstado, StringComparison.OrdinalIgnoreCase))
                {
                    cbEstadoInicial.SelectedIndex = i;
                    return;
                }
            }
        }


        // ========================= CONFIGURACIÓN UI =========================
        private void ConfigurarUiSoloLectura()
        {
            ToggleEdicion(false);
        }

        // Alternar entre modo edición y solo lectura
        private void ToggleEdicion(bool habilitar)
        {
            _modoEdicion = habilitar;

            // Textos editables
            txtNombre.ReadOnly = !habilitar;
            txtApellido.ReadOnly = !habilitar;
            txtDni.ReadOnly = !habilitar;
            txtEmail.ReadOnly = !habilitar;
            txtDireccion.ReadOnly = !habilitar;
            txtTelefono.ReadOnly = !habilitar;
            txtObservaciones.ReadOnly = !habilitar;
            dtpNacimiento.Enabled = habilitar;

            // Estado: en solo lectura mostramos txtEstado
            txtEstado.Visible = !habilitar;

            cbEstadoInicial.Visible = habilitar;
            cbEstadoInicial.Enabled = habilitar;

            if (habilitar)
            {
                // Cuando entro en edición, el combo debe reflejar lo que se estaba viendo
                SeleccionarEstadoPorNombre(txtEstado.Text);
            }

            if (btnEditar != null)
                btnEditar.Text = habilitar ? "Guardar" : "Editar";
        }


        // ========================= VALIDACIONES =========================
        private bool ValidarDatos(out string error)
        {
            error = null;

            // ===== Nombre (obligatorio, máx. 50) =====
            var nombre = txtNombre.Text.Trim();
            if (string.IsNullOrWhiteSpace(nombre))
            {
                error = "El nombre es obligatorio.";
                return false;
            }
            if (nombre.Length > 50)
            {
                error = "El nombre no puede superar 50 caracteres.";
                return false;
            }

            // ===== Apellido (obligatorio, máx. 50) =====
            var apellido = txtApellido.Text.Trim();
            if (string.IsNullOrWhiteSpace(apellido))
            {
                error = "El apellido es obligatorio.";
                return false;
            }
            if (apellido.Length > 50)
            {
                error = "El apellido no puede superar 50 caracteres.";
                return false;
            }

            // ===== DNI (obligatorio, numérico, 7–8 dígitos, positivo) =====
            var dniTexto = txtDni.Text.Trim();
            if (string.IsNullOrWhiteSpace(dniTexto))
            {
                error = "El DNI es obligatorio.";
                return false;
            }
            if (!dniTexto.All(char.IsDigit))
            {
                error = "El DNI debe contener solo números.";
                return false;
            }
            if (!int.TryParse(dniTexto, out var dniVal) || dniVal < 1_000_000 || dniVal > 99_999_999)
            {
                error = "El DNI debe estar entre 1.000.000 y 99.999.999.";
                return false;
            }

            // ===== Teléfono (obligatorio, numérico, 6 a 30 dígitos) =====
            var telTexto = txtTelefono.Text.Trim();
            if (string.IsNullOrWhiteSpace(telTexto))
            {
                error = "El teléfono es obligatorio.";
                return false;
            }
            if (!telTexto.All(char.IsDigit))
            {
                error = "El teléfono debe contener solo números.";
                return false;
            }
            if (telTexto.Length < 6 || telTexto.Length > 30)
            {
                error = "El teléfono debe tener entre 6 y 30 dígitos.";
                return false;
            }

            // ===== Dirección (obligatoria, máx. 100) =====
            var direccion = txtDireccion.Text.Trim();
            if (string.IsNullOrWhiteSpace(direccion))
            {
                error = "La dirección es obligatoria.";
                return false;
            }
            if (direccion.Length > 100)
            {
                error = "La dirección no puede superar 100 caracteres.";
                return false;
            }

            // ===== Observaciones (opcional, máx. 200) =====
            var obs = txtObservaciones.Text.Trim();
            if (obs.Length > 200)
            {
                error = "Observaciones no puede superar 200 caracteres.";
                return false;
            }

            // ===== Email (obligatorio, máx. 100, formato válido) =====
            var email = txtEmail.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                error = "El correo es obligatorio.";
                return false;
            }
            if (email.Length > 100)
            {
                error = "El correo no puede superar 100 caracteres.";
                return false;
            }
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                error = "Formato de correo inválido.";
                return false;
            }

            // ===== Estado (en modo edición, obligatorio) =====
            if (_modoEdicion)
            {
                var sel = cbEstadoInicial.SelectedItem as EstadoPacienteDto;
                if (sel == null || sel.Id == 0)
                {
                    error = "Seleccioná un estado válido.";
                    return false;
                }
            }

            // ===== Fecha de nacimiento (no futura, no más de 120 años) =====
            var fechaNac = dtpNacimiento.Value.Date;

            if (fechaNac > DateTime.Today)
            {
                error = "La fecha de nacimiento no puede ser futura.";
                return false;
            }
            if (fechaNac < DateTime.Today.AddYears(-120))
            {
                error = "La fecha de nacimiento no puede ser mayor a 120 años.";
                return false;
            }

            return true;
        }


        // ========================= EVENTOS BOTONES =========================

        // ========================= BOTÓN EDITAR / GUARDAR =========================
        // Alternar entre modo edición y guardar cambios
        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!_modoEdicion)
            {
                ToggleEdicion(true);
                return;
            }

            if (!ValidarDatos(out var msg))
            {
                MessageBox.Show(msg, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mapear cambios al DTO local
            _pacienteDto.Nombre = txtNombre.Text.Trim();
            _pacienteDto.Apellido = txtApellido.Text.Trim();
            _pacienteDto.DNI = int.TryParse(txtDni.Text.Trim(), out var dni) ? dni : 0;
            _pacienteDto.Direccion = txtDireccion.Text.Trim();
            _pacienteDto.Telefono = txtTelefono.Text.Trim();
            _pacienteDto.Observaciones = txtObservaciones.Text.Trim();
            _pacienteDto.Email = txtEmail.Text.Trim();
            _pacienteDto.FechaNacimiento = dtpNacimiento.Value;

            // Estado desde el combo
            var estadoSel = cbEstadoInicial.SelectedItem as EstadoPacienteDto;
            if (estadoSel != null && estadoSel.Id > 0)
            {
                _pacienteDto.Estado = estadoSel.Nombre;
            }
            else
            {
                MessageBox.Show("Seleccioná un estado válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Intentar guardar cambios via servicio
            try
            {
                var r = _pacienteService.Editar(_pacienteDto);
                if (!r.Ok)
                {
                    MessageBox.Show(r.Error ?? "No se pudo actualizar el paciente.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Volver a solo lectura y refrescar controles visibles
                ToggleEdicion(false);
                CargarDatos(_pacienteDto);

                PacienteActualizado?.Invoke(this, _pacienteDto);

                MessageBox.Show("Paciente actualizado correctamente.",
                                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al actualizar: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================= BOTÓN CANCELAR EDICIÓN =========================
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CargarDatos(_pacienteDto);
            ToggleEdicion(false);
            CancelarVisualizacionSolicitada?.Invoke(this, EventArgs.Empty);
        }

    }
}
