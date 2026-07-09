using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.DTOs.Consultas;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Medico.Pacientes
{
    /// <summary>
    /// Control de usuario que gestiona el registro de nuevas consultas médicas.
    /// Permite al médico ingresar diagnóstico, tratamiento y motivo de la consulta para un paciente identificado por DNI.
    /// </summary>
    public partial class ConsultaMedica : UserControl
    {
        /// <summary>Servicio para el manejo de operaciones médicas.</summary>
        private MedicoService _service = new MedicoService();

        /// <summary>ID del médico logueado recuperado de la sesión.</summary>
        private int _idMedicoLogueado;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="ConsultaMedica"/>.
        /// Prepara el formulario para el registro de una consulta.
        /// </summary>
        public ConsultaMedica()
        {
            InitializeComponent();
            registroConsulta();
        }

        private void registroConsulta()
        {
            // Obtenemos el ID del médico de la sesión
            if (!SesionUsuario.IdMedicoAsociado.HasValue)
            {
                MessageBox.Show("Error fatal: No se pudo identificar al médico. Cierre sesión y vuelva a intentarlo.", "Error de Sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardar.Enabled = false; // Deshabilitamos el guardado
                return;
            }
            _idMedicoLogueado = SesionUsuario.IdMedicoAsociado.Value;

            dtpFecha.Value = DateTime.Now;
            dtpFecha.Enabled = false;
        }
        // ============================= VALIDACIONES =============================

        private void TBDNI_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDniPaciente.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDniPaciente, "Ingrese DNI ");
            }
            else if (!string.IsNullOrWhiteSpace(txtDniPaciente.Text))
            {
                if (!int.TryParse(txtDniPaciente.Text, out _) || txtDniPaciente.Text.Length > 15)
                {
                    e.Cancel = true;
                    errorProvider1.SetError(txtDniPaciente, "DNI numérico (máx. 15).");
                }
                else
                {
                    errorProvider1.SetError(txtDniPaciente, "");
                }
            }
            else
            {
                errorProvider1.SetError(txtDniPaciente, "");
            }
        }


        private void TBMOTIVO_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMotivo.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtMotivo, "El motivo es obligatorio.");
            }
            else if (txtMotivo.Text.Length > 300)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtMotivo, "Máximo 300 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtMotivo, "");
            }
        }

        private void TBDX_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDiagnostico.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDiagnostico, "El diagnóstico es obligatorio.");
            }
            else if (txtDiagnostico.Text.Length > 300)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDiagnostico, "Máximo 300 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtDiagnostico, "");
            }
        }

        private void TBTYR_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTratamiento.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTratamiento, "El tratamiento es obligatorio.");
            }
            else if (txtTratamiento.Text.Length > 300)
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTratamiento, "Máximo 300 caracteres.");
            }
            else
            {
                errorProvider1.SetError(txtTratamiento, "");
            }
        }

        private void dateTimePicker1_Validating(object sender, CancelEventArgs e)
        {
            if (dtpFecha.Value.Date > DateTime.Today)
            {
             e.Cancel = true;
             errorProvider1.SetError(dtpFecha, "La fecha de la consulta no puede ser futura.");
            }
            else
            {
                errorProvider1.SetError(dtpFecha, "");
            }
        }

        /// <summary>
        /// Procesa el guardado de la consulta médica.
        /// Valida los datos, crea el DTO y utiliza el servicio para persistir la información.
        /// </summary>
        /// <param name="sender">Objeto que dispara el evento.</param>
        /// <param name="e">Argumentos del evento.</param>
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Recolectamos los datos del formulario
                var dto = new ConsultaAltaDto
                {
                    DniPaciente = txtDniPaciente.Text.Trim(),
                    Fecha = dtpFecha.Value,
                    Motivo = txtMotivo.Text.Trim(),
                    Diagnostico = txtDiagnostico.Text.Trim(),
                    Tratamiento = txtTratamiento.Text.Trim()
                };

                // 2. Llamamos al servicio para que haga la magia
                var resultado = _service.RegistrarConsulta(dto, _idMedicoLogueado);

                // 3. Mostramos el resultado
                if (resultado.Ok)
                {
                    MessageBox.Show("Consulta registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnLimpiar_Click(sender, e); // Limpiamos el formulario
                }
                else
                {
                    // Mostramos el error específico que devolvió el servicio
                    MessageBox.Show(resultado.Error, "Error de Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error inesperado al guardar: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtDniPaciente.Clear();
            txtMotivo.Clear();
            txtDiagnostico.Clear();
            txtTratamiento.Clear();
            panel1.Focus();
        }

    }
}
