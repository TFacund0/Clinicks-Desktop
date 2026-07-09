using Sistema_Hospitalario.CapaDatos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

using Sistema_Hospitalario.CapaNegocio.Servicios.EspecialidadService;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;

namespace Sistema_Hospitalario.CapaPresentacion.Administrador.medicos
{
    /// <summary>
    /// Control de usuario que permite el registro de nuevos médicos en el sistema.
    /// Incluye validaciones de campos obligatorios, formatos de DNI y correo electrónico.
    /// </summary>
    public partial class UC_agregarMedico : System.Windows.Forms.UserControl
    {
        /// <summary>Instancia estática del servicio de médicos.</summary>
        private static readonly MedicoService medicoService = new MedicoService();
        /// <summary>Instancia local del servicio de médicos.</summary>
        private readonly MedicoService _service = medicoService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_agregarMedico"/>.
        /// Carga las especialidades disponibles desde la base de datos para su selección.
        /// </summary>
        public UC_agregarMedico()
        {
            InitializeComponent();
            CargarEspecialidades();
        }

        // ============================= VALIDACIONES =============================
        
        // ====== Validacion Nombre ======
        private void TBNOMBRE_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBNOMBRE.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBNOMBRE, "El nombre es obligatorio.");
            }
            else if (TBNOMBRE.Text.Length > 50)
            {
                e.Cancel = true;
                errorProvider1.SetError(TBNOMBRE, "Máximo 50 caracteres.");
            }
            else
            {
                errorProvider1.SetError(TBNOMBRE, "");
            }
        }

        // ====== Validacion Apellido ======
        private void TBAPELLIDO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBAPELLIDO.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBAPELLIDO, "El apellido es obligatorio.");
            }
            else if (TBAPELLIDO.Text.Length > 50)
            {
                e.Cancel = true;
                errorProvider1.SetError(TBAPELLIDO, "Máximo 50 caracteres.");
            }
            else
            {
                errorProvider1.SetError(TBAPELLIDO, "");
            }
        }

        // ====== Validacion DNI ======
        private void TBDNI_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBDNI.Text) || !long.TryParse(TBDNI.Text, out _))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBDNI, "El DNI es obligatorio y numérico.");
            }
            else if (TBDNI.Text.Length < 7 || TBDNI.Text.Length > 15)
            {
                e.Cancel = true;
                errorProvider1.SetError(TBDNI, "El DNI debe tener entre 7 y 15 dígitos.");
            }
            else
            {
                errorProvider1.SetError(TBDNI, "");
            }
        }

        // ====== Validacion Dirección ======
        private void TBDIRECCION_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBDIRECCION.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBDIRECCION, "La dirección es obligatoria.");
            }
            else if (TBDIRECCION.Text.Length > 100)
            {
                e.Cancel = true;
                errorProvider1.SetError(TBDIRECCION, "Máximo 100 caracteres.");
            }
            else
            {
                errorProvider1.SetError(TBDIRECCION, "");
            }
        }

        // ====== Validacion Matrícula ======
        private void TBMATRICULA_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBMATRICULA.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBMATRICULA, "La matrícula es obligatoria.");
            }
            else if (TBMATRICULA.Text.Length > 20)
            {
                e.Cancel = true;
                errorProvider1.SetError(TBMATRICULA, "Máximo 20 caracteres.");
            }
            else
            {
                errorProvider1.SetError(TBMATRICULA, "");
            }
        }

        // ====== Validacion Correo Electrónico ======
        private void TBCORREO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBCORREO.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBCORREO, "El correo electrónico es obligatorio.");
            }
            else if (!Regex.IsMatch(TBCORREO.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                e.Cancel = true;
                errorProvider1.SetError(TBCORREO, "Formato de correo inválido.");
            }
            else if (TBCORREO.Text.Length > 100)
            {
                e.Cancel = true;
                errorProvider1.SetError(TBCORREO, "Máximo 100 caracteres.");
            }
            else
            {
                errorProvider1.SetError(TBCORREO, "");
            }
        }


        // ============================= BOTONES =============================
        // ====== Botón Guardar ======
        private void BtnGuardar_Click_1(object sender, EventArgs e) 
        {
            try
            {
                if (!this.ValidateChildren())
                {
                    MessageBox.Show("Por favor, corrija los errores antes de guardar.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string nombre = TBNOMBRE.Text.Trim();
                string apellido = TBAPELLIDO.Text.Trim();
                string dni = TBDNI.Text.Trim();
                string direccion = TBDIRECCION.Text.Trim();
                string matricula = TBMATRICULA.Text.Trim();
                string correo = TBCORREO.Text.Trim();
        
                int idEspecialidad = (int)comboBox1.SelectedValue;  

                // Guardar en base de datos
                _service.AgregarMedico(nombre, apellido, dni, direccion, matricula, correo, idEspecialidad);

                MessageBox.Show("Médico registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar el formulario después de guardar
                BtnLimpiar_Click_1(sender, e); 

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el médico: " + ex.Message);
            }
        }

        // ====== Botón Limpiar ======
        private void BtnLimpiar_Click_1(object sender, EventArgs e)
        {
            TBNOMBRE.Clear();
            TBAPELLIDO.Clear();
            TBDNI.Clear();
            TBDIRECCION.Clear();
            TBMATRICULA.Clear();
            TBCORREO.Clear();
            TBCORREO.Clear();
            TBDIRECCION.Clear();
            TBMATRICULA.Clear();

            errorProvider1.Clear();
        }

        // ====== Botón Cancelar ======
        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            MenuModer parentForm = this.FindForm() as MenuModer;

            parentForm.AbrirUserControl(new UC_Medicos()); 
        }

        // ============================= MÉTODOS AUXILIARES =============================
        // Cargar especialidades en el ComboBox
        private void CargarEspecialidades()
        {
            var especialidades = new EspecialidadService().ObtenerEspecialidades();

            comboBox1.DisplayMember = "Nombre";
            comboBox1.ValueMember = "Id";
            comboBox1.DataSource = especialidades;

            if (comboBox1.Items.Count > 10)
                comboBox1.SelectedIndex = 10;
        }

    }
}
