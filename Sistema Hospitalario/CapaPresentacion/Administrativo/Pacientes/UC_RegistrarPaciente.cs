using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo.Pacientes
{
    /// <summary>
    /// Control de usuario que gestiona el alta de nuevos pacientes en el sistema.
    /// Realiza validaciones exhaustivas de datos personales, contacto y asignación de estado inicial.
    /// </summary>
    public partial class UC_RegistrarPaciente : UserControl
    {
        /// <summary>Servicio para la gestión de lógica de pacientes.</summary>
        private readonly PacienteService _pacienteService = new PacienteService();
        /// <summary>Servicio para la gestión de estados de pacientes.</summary>
        private readonly EstadoPacienteService _estadoService = new EstadoPacienteService();

        /// <summary>Evento que notifica que se ha cancelado el registro del paciente.</summary>
        public event EventHandler CancelarRegistroSolicitado;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_RegistrarPaciente"/>.
        /// Configura los rangos de fecha permitidos y carga el catálogo de estados iniciales.
        /// </summary>
        public UC_RegistrarPaciente()
        {
            InitializeComponent();

            // Rango razonable para fecha de nacimiento
            dtpNacimiento.MaxDate = DateTime.Today;
            dtpNacimiento.MinDate = DateTime.Today.AddYears(-120);
            dtpNacimiento.Format = DateTimePickerFormat.Short;

            SelectEventosPaciente();
            CargarEstadosEnCombo();
        }


        // ============================= COMBOBOX ESTADO INICIAL =============================
        // Seleccionar el ComboBox al hacer foco o click
        private void SelectEventosPaciente()
        {
            // Abrir automáticamente al ganar foco o click
            cbEstadoInicial.Enter += (s, e) => cbEstadoInicial.DroppedDown = true;
            cbEstadoInicial.MouseDown += (s, e) => cbEstadoInicial.DroppedDown = true;
        }

        // Cargar los estados de paciente en el ComboBox
        private void CargarEstadosEnCombo()
        {
            var estados = _estadoService.ListarEstados() ?? new List<EstadoPacienteDto>();

            // sin “— Seleccioná —” porque no queremos que el usuario cambie nada
            cbEstadoInicial.DataSource = estados;
            cbEstadoInicial.DisplayMember = nameof(EstadoPacienteDto.Nombre);
            cbEstadoInicial.ValueMember = nameof(EstadoPacienteDto.Id);

            // buscar el estado ACTIVO (case-insensitive)
            var activo = estados.FirstOrDefault(e =>
                string.Equals(e.Nombre?.Trim(), "activo", StringComparison.OrdinalIgnoreCase));

            if (activo != null)
            {
                cbEstadoInicial.SelectedValue = activo.Id;
                cbEstadoInicial.Text = activo.Nombre;
            }
            else
            {
                // fallback: primer estado disponible, por si no existe "activo" en BD
                if (estados.Count > 0) cbEstadoInicial.SelectedIndex = 0;
            }

            // bloquear edición/selección
            cbEstadoInicial.DropDownStyle = ComboBoxStyle.DropDownList;
            cbEstadoInicial.Enabled = false;      // o cbEstadoInicial.Visible = false; si querés ocultarlo
        }



        // ============================= VALIDACIONES DE CAMPOS PACIENTE =============================

        // ========== VALIDACIÓN NOMBRE ==========
        private void TxtNombre_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNombre, "El nombre es obligatorio.");
            }
            else if (txtNombre.Text.Length > 50)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtNombre, "Máximo 50 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtNombre, "");
            }
        }

        // ========== VALIDACIÓN APELLIDO ==========
        private void TxtApellido_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtApellido, "El apellido es obligatorio.");
            }
            else if (txtApellido.Text.Length > 50)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtApellido, "Máximo 50 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtApellido, "");
            }
        }

        // ========== VALIDACIÓN DIRECCIÓN ==========
        private void TxtDireccion_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDireccion, "La dirección es obligatoria.");
            }
            else if (txtDireccion.Text.Length > 100)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDireccion, "Máximo 100 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtDireccion, "");
            }
        }

        // ========== VALIDACIÓN FECHA DE NACIMIENTO ==========
        private void DtpNacimiento_Validating(object sender, CancelEventArgs e)
        {
            var fecha = dtpNacimiento.Value.Date;

            if (fecha > DateTime.Today)
            {
                e.Cancel = true;
                errorProvider1.SetError(dtpNacimiento, "La fecha de nacimiento no puede ser futura.");
            }
            else if (fecha < DateTime.Today.AddYears(-120))
            {
                e.Cancel = true;
                errorProvider1.SetError(dtpNacimiento, "La fecha de nacimiento no puede ser mayor a 120 años.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(dtpNacimiento, "");
            }
        }


        // ========== VALIDACIÓN DNI ==========
        private void TxtDni_Validating(object sender, CancelEventArgs e)
        {
            string dniTexto = txtDni.Text.Trim();

            if (string.IsNullOrWhiteSpace(dniTexto))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDni, "El DNI es obligatorio.");
            }
            else if (!dniTexto.All(char.IsDigit))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDni, "El DNI debe contener solo números.");
            }
            else if (!int.TryParse(dniTexto, out int dni) || dni < 1_000_000 || dni > 99_999_999)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDni, "El DNI debe estar entre 1.000.000 y 99.999.999.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtDni, "");
            }
        }


        // ========== VALIDACIÓN TELÉFONO ==========
        private void TxtTelefono_Validating(object sender, CancelEventArgs e)
        {
            string telTexto = txtTelefono.Text.Trim();

            if (string.IsNullOrWhiteSpace(telTexto))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTelefono, "El teléfono es obligatorio.");
            }
            else if (!telTexto.All(char.IsDigit))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTelefono, "El teléfono debe contener solo números.");
            }
            else if (telTexto.Length < 6 || telTexto.Length > 30)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTelefono, "El teléfono debe tener entre 6 y 30 dígitos.");
            }
            else
            {
                e.Cancel = false;
                errorProvider1.SetError(txtTelefono, "");
            }
        }


        // ========= VALIDACIÓN CORREO ==========
        private void TxtEmail_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "El correo es obligatorio.");
            }
            else if (txtEmail.Text.Length > 100)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "Máximo 100 caracteres.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(txtEmail.Text,
                     @"^[^@\s]+@[^@\s]+\.[^@\s]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtEmail, "Formato de correo inválido.");
            }
            else
            {
                errorProvider1.SetError(txtEmail, "");
            }
        }

        // ========== VALIDACIÓN OBSERVACIONES ==========
        private void TxtObservaciones_Validating(object sender, CancelEventArgs e)
        {
            if (txtObservaciones.Text.Length > 200)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtObservaciones, "Máximo 200 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtObservaciones, "");
            }
        }


        //============================= RESTRICCIONES DE TECLADO =============================

        // Permitir solo letras, espacios y teclas de control
        private void SoloLetras(object s, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        // Permitir solo números y teclas de control
        private void SoloNumeros(object s, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }


        // ============================ BOTÓN GUARDAR =============================
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            // Ejecutar todas las validaciones de los controles
            bool controlesValidos = this.ValidateChildren();

            if (!controlesValidos)
            {
                MessageBox.Show(
                    "Por favor, corregí los errores antes de guardar.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // Chequeo extra por si el combo quedó raro
            if (cbEstadoInicial.SelectedValue == null || (int)cbEstadoInicial.SelectedValue == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un estado inicial.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            var dto = new PacienteAltaDto
            {
                Nombre = txtNombre.Text.Trim(),
                Apellido = txtApellido.Text.Trim(),
                Dni = int.TryParse(txtDni.Text.Trim(), out int dni) ? dni : 0,
                Telefono = txtTelefono.Text.Trim(),
                Direccion = txtDireccion.Text.Trim(),
                FechaNacimiento = dtpNacimiento.Value,
                Email = txtEmail.Text.Trim(),
                Observaciones = txtObservaciones.Text.Trim(),
                EstadoInicial = "activo" // <-- forzado
            };

            var (Ok, IdGenerado, Error) = _pacienteService.Alta(dto);

            if (Ok)
            {
                MessageBox.Show($"Paciente registrado con éxito.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                BtnLimpiar_Click(null, EventArgs.Empty);
            }
            else
            {
                MessageBox.Show(Error, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ============================ BOTÓN LIMPIAR =============================
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Clear();
            txtApellido.Clear();
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtDni.Clear();
            txtDireccion.Clear();            
            txtDni.Clear();
            txtEmail.Clear();
            txtObservaciones.Clear();
            dtpNacimiento.Value = DateTime.Today;
            cbEstadoInicial.SelectedIndex = cbEstadoInicial.SelectedIndex; 

            errorProvider1.Clear();
        }

        // ============================ BOTÓN CANCELAR =============================
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            CancelarRegistroSolicitado?.Invoke(this, EventArgs.Empty);
        }
    }
}
