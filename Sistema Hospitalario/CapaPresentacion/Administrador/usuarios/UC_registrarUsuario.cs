using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.UsuarioService;

namespace Sistema_Hospitalario.CapaPresentacion.Administrador.usuarios
{
    /// <summary>
    /// Control de usuario que permite la creación y alta de nuevos usuarios en el sistema.
    /// Soporta la asignación de roles, estados y la vinculación opcional con un perfil médico,
    /// incluyendo validaciones de seguridad para contraseñas y nombres de usuario únicos.
    /// </summary>
    public partial class UC_registrarUsuario : UserControl
    {
        /// <summary>Servicio para la gestión de lógica de usuarios.</summary>
        private static readonly UsuarioService usuarioService = new UsuarioService();
        /// <summary>Referencia local al servicio de usuarios.</summary>
        private readonly UsuarioService _service = usuarioService;

        /// <summary>Servicio para la gestión de médicos (usado para vinculación de cuentas).</summary>
        private readonly MedicoService _medicoService = new MedicoService();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_registrarUsuario"/>.
        /// Carga los catálogos de roles y estados, e inhabilita inicialmente la selección de médicos.
        /// </summary>
        public UC_registrarUsuario()
        {
            InitializeComponent();
            CargarRoles();
            CargarEstados();
            cboMedicos.Enabled = false;
        }

        // ============================= MÉTODOS AUXILIARES ===========================
        // Carga de roles en el ComboBox
        private void CargarRoles()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var roles = db.rol
                              .Select(r => new { IdRol = r.id_rol, NombreRol = r.nombre })
                              .ToList();
                cboRol.DisplayMember = "NombreRol";
                cboRol.ValueMember = "IdRol";
                cboRol.DataSource = roles;
            }
        }

        // Carga de estados en el ComboBox
        private void CargarEstados()
        {
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                var estados = db.estado_usuario
                                .Select(e => new { IdEstado = e.id_estado_usuario, NombreEstado = e.nombre })
                                .ToList();
                cboEstado.DisplayMember = "NombreEstado";
                cboEstado.ValueMember = "IdEstado";
                cboEstado.DataSource = estados;
            }
        }

        // Carga de médicos en el ComboBox
        private void CargarMedicos()
        {
            try
            {
                var medicos = _medicoService.ObtenerMedicosParaComboBox();

                cboMedicos.DisplayMember = "NombreCompletoYDNI";
                cboMedicos.ValueMember = "Id";
                cboMedicos.DataSource = medicos;

                cboMedicos.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la lista de médicos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Deshabilitar el combo si no se pudo cargar
                cboMedicos.Enabled = false;
            }
        }

        // ============================= EVENTOS DEL FORMULARIO ===========================
        // Evento al cambiar la selección del rol
        private void cboRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboRol.SelectedItem != null && cboRol.Text == "medico") 
            {
                cboMedicos.Visible = true;

                if (cboMedicos.DataSource == null)
                {
                    CargarMedicos();
                }
                cboMedicos.Enabled = true;
            }
            else
            {
                cboMedicos.Enabled = false;
                cboMedicos.SelectedIndex = -1;
            }
        }


        // ============================= Validaciones ===========================
        // Validaciones de los campos del formulario
        private void TBNOMBRE_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBNOMBRE.Text))
            {  // Validar que no esté vacío o solo espacios
                e.Cancel = true;
                errorProvider1.SetError(TBNOMBRE, "El nombre es obligatorio.");
            }
            else if (TBNOMBRE.Text.Length > 50)
            { // Validar longitud máxima
                e.Cancel = true;
                errorProvider1.SetError(TBNOMBRE, "Máximo 50 caracteres.");
            }
            else // Si todo está bien, limpiar el error
            {
                errorProvider1.SetError(TBNOMBRE, "");
            }
        }

        // Validaciones de los campos del formulario
        private void TBAPELLIDO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBAPELLIDO.Text))
            { // Validar que no esté vacío o solo espacios
                e.Cancel = true;
                errorProvider1.SetError(TBAPELLIDO, "El apellido es obligatorio.");
            }
            else if (TBAPELLIDO.Text.Length > 50)
            { // Validar longitud máxima
                e.Cancel = true;
                errorProvider1.SetError(TBAPELLIDO, "Máximo 50 caracteres.");
            }
            else    // Si todo está bien, limpiar el error
            {
                errorProvider1.SetError(TBAPELLIDO, "");
            }
        }

        // Validaciones de los campos del formulario
        private void TBCORREO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBCORREO.Text))
            { // Validar que no esté vacío
                e.Cancel = true;
                errorProvider1.SetError(TBCORREO, "El correo electrónico es obligatorio.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(
                         TBCORREO.Text,
                         @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            { // Validar formato básico de correo
                e.Cancel = true;
                errorProvider1.SetError(TBCORREO, "Formato de correo inválido.");
            }
            else if (TBCORREO.Text.Length > 100)
            { // Validar longitud máxima
                e.Cancel = true;
                errorProvider1.SetError(TBCORREO, "Máximo 100 caracteres.");
            }
            else // Si todo está bien, limpiar el error
            {
                errorProvider1.SetError(TBCORREO, "");
            }
        }

        // Validaciones de los campos del formulario
        private void TBUSERNAME_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBUSERNAME.Text))
            { // Validar que no esté vacío o solo espacios
                e.Cancel = true;
                errorProvider1.SetError(TBUSERNAME, "El nombre de usuario es obligatorio.");
            }
            else if (TBUSERNAME.Text.Length > 50)
            { // Validar longitud máxima
                e.Cancel = true;
                errorProvider1.SetError(TBUSERNAME, "Máximo 50 caracteres.");
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(
                         TBUSERNAME.Text, @"^[a-zA-Z0-9_]+$"))
            { // Validar caracteres permitidos 
                e.Cancel = true;
                errorProvider1.SetError(TBUSERNAME, "Solo se permiten letras, números y guión bajo.");
            }
            else // Si todo está bien, limpiar el error
            {
                errorProvider1.SetError(TBUSERNAME, "");
            }
        }

        // Validaciones de los campos del formulario
        private void TBPASSWORD_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBPASSWORD.Text))
            { // Validar que no esté vacío
                e.Cancel = true;
                errorProvider1.SetError(TBPASSWORD, "La contraseña es obligatoria.");
            }
            else if (TBPASSWORD.Text.Length > 65)
            { // Validar longitud máxima
                e.Cancel = true;
                errorProvider1.SetError(TBPASSWORD, "Máximo 60 caracteres.");
            }
            else if (TBPASSWORD.Text.Length < 6)
            { // Validar longitud mínima básica
                e.Cancel = true;
                errorProvider1.SetError(TBPASSWORD, "Debe tener al menos 6 caracteres.");
            }
            else // Si todo está bien, limpiar el error
            {
                errorProvider1.SetError(TBPASSWORD, "");
            }
        }

        // ============================= BOTONES =============================

        // Botón Guardar
        private void BtnGuardar_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!this.ValidateChildren())
                {
                    MessageBox.Show("Por favor, corrija los errores antes de guardar.", "Error de Validación",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int idRol = (int)cboRol.SelectedValue; 
                int idEstado = (int)cboEstado.SelectedValue;
                string rolSeleccionado = cboRol.Text;

                int? idMedicoAsociado = null;
                if (rolSeleccionado == "medico")
                {
                    if (cboMedicos.SelectedValue == null || (int)cboMedicos.SelectedValue == 0)
                    {
                        // Si el rol es Médico, es obligatorio seleccionar uno
                        MessageBox.Show("Debe seleccionar un médico para asociar a este usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        errorProvider1.SetError(cboMedicos, "Selección requerida."); // Marcar error en el combo
                        return; // Detener guardado
                    }
                    idMedicoAsociado = (int)cboMedicos.SelectedValue;
                    errorProvider1.SetError(cboMedicos, ""); // Limpiar error si todo OK
                }
                else
                {
                    errorProvider1.SetError(cboMedicos, ""); // Limpiar error si no es rol médico
                }

                // Crear el DTO de alta
                UsuarioAltaDto nuevoUsuario = new UsuarioAltaDto
                {
                    Nombre = TBNOMBRE.Text.Trim(),
                    Apellido = TBAPELLIDO.Text.Trim(),
                    NombreUsuario = TBUSERNAME.Text.Trim(),
                    Password = TBPASSWORD.Text.Trim(),
                    Correo = TBCORREO.Text.Trim(),
                    IdRol = idRol,
                    IdEstado = idEstado,
                    IdMedico = idMedicoAsociado
                };

                // Llamar al servicio para agregar el usuario
                var (Ok, IdGenerado, Error) = _service.AgregarUsuario(nuevoUsuario);

                if (Ok)
                {
                    MessageBox.Show("Usuario registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    BtnLimpiar_Click_1(sender, e); // Limpiar el formulario
                }
                else
                {
                    // Mostrar el error específico devuelto por el repositorio/servicio
                    MessageBox.Show("Error al registrar el usuario: " + Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado al registrar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Botón Limpiar
        private void BtnLimpiar_Click_1(object sender, EventArgs e)
        {
            TBNOMBRE.Clear();
            TBAPELLIDO.Clear();
            TBUSERNAME.Clear();
            TBPASSWORD.Clear();
            TBCORREO.Clear();
            errorProvider1.Clear();
            cboRol.SelectedIndex = -1; // Seleccionar el primer rol por defecto
            cboEstado.SelectedIndex = -1;
            cboRol.SelectedIndex = -1;
            errorProvider1.Clear();
        }

        // Botón Cancelar
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            MenuModer parentForm = this.FindForm() as MenuModer;

            parentForm.AbrirUserControl(new UC_usuarios());
        }

    }
}
