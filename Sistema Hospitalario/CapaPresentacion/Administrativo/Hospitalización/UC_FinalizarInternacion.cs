using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;
using Sistema_Hospitalario.CapaNegocio.Servicios.InternacionService;

namespace Sistema_Hospitalario.CapaPresentacion.Administrativo.Hospitalización
{
    /// <summary>
    /// Control de usuario que gestiona la finalización de una internación (alta médica).
    /// Permite registrar la fecha de egreso, el diagnóstico de salida y libera automáticamente la cama ocupada.
    /// </summary>
    public partial class UC_FinalizarInternacion : UserControl
    {
        /// <summary>Evento que notifica que se ha cancelado la operación de finalización.</summary>
        public event EventHandler CancelarFinalizacionSolicitada;

        /// <summary>DTO que contiene los datos de la internación activa que se va a finalizar.</summary>
        private readonly InternacionDto _internacion;

        /// <summary>Servicio para la gestión de lógica de internaciones.</summary>
        private readonly InternacionService _internacionService = new InternacionService();

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_FinalizarInternacion"/>.
        /// </summary>
        public UC_FinalizarInternacion()
        {
            InitializeComponent();
            this.Load += UC_FinalizarInternacion_Load;
        }

        private void UC_FinalizarInternacion_Load(object sender, EventArgs e)
        {
            // TextBox solo lectura
            txtPaciente.ReadOnly = true;
            txtMedico.ReadOnly = true;
            txtProcedimiento.ReadOnly = true;
            txtHabitacion.ReadOnly = true;
            txtPiso.ReadOnly = true;
            txtCama.ReadOnly = true;

            // Que no entren con TAB
            txtPaciente.TabStop = false;
            txtMedico.TabStop = false;
            txtProcedimiento.TabStop = false;
            txtHabitacion.TabStop = false;
            txtPiso.TabStop = false;
            txtCama.TabStop = false;

            // DateTimePicker de inicio deshabilitado (solo se muestra)
            dtpFechaInicio.Enabled = false;
            dtpFechaEgreso.Enabled = false;

            txtMotivo.ReadOnly = false;
        }


        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_FinalizarInternacion"/> con los datos de una internación específica.
        /// </summary>
        /// <param name="internacion">Datos de la internación a finalizar.</param>
        public UC_FinalizarInternacion(InternacionDto internacion) : this()
        {
            _internacion = internacion ?? throw new ArgumentNullException(nameof(internacion));
            CargarDatosInternacion();
        }

        private void CargarDatosInternacion()
        {
            txtPaciente.Text = _internacion.Internado;
            txtMedico.Text = _internacion.NombreCompletoMedico;
            txtHabitacion.Text = _internacion.Nro_habitacion.ToString();
            txtPiso.Text = _internacion.Nro_piso.ToString();
            txtCama.Text = _internacion.Id_cama.ToString();
            txtProcedimiento.Text = _internacion.procedimiento;

            dtpFechaInicio.Value = _internacion.Fecha_ingreso;

            // Si YA está finalizada
            if (_internacion.Fecha_egreso != null)
            {
                // Seteamos la fecha de egreso real
                dtpFechaEgreso.Value = _internacion.Fecha_egreso.Value;

                // Bloqueamos edición
                dtpFechaEgreso.Enabled = false;
                txtMotivo.ReadOnly = true;

                // Deshabilitamos el botón para prevenir doble finalización
                btnGuardar.Enabled = false;
            }
            else
            {
                // Caso normal: internación activa → se puede finalizar
                dtpFechaEgreso.Value = DateTime.Now;
                txtMotivo.Text = _internacion.Diagnostico;

                // Aseguramos edición
                dtpFechaEgreso.Enabled = true;
                txtMotivo.ReadOnly = false;

                btnGuardar.Enabled = true;
            }
        }


        // Botón Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            CancelarFinalizacionSolicitada?.Invoke(this, EventArgs.Empty);
        }

        // Botón Guardar / Finalizar internación
        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (_internacion.Fecha_egreso != null)
            {
                MessageBox.Show("Esta internación ya fue finalizada previamente.",
                    "Operación no permitida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (dtpFechaEgreso.Value < dtpFechaInicio.Value)
                {
                    MessageBox.Show("La fecha de egreso no puede ser anterior a la fecha de ingreso.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show("Debe ingresar un diagnóstico/motivo de egreso.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 👉 Confirmación antes de finalizar
                var dr = MessageBox.Show(
                    "¿Está seguro que desea finalizar esta internación?\n" +
                    "El paciente será dado de alta y la cama quedará disponible.",
                    "Confirmar finalización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (dr != DialogResult.Yes)
                {
                    // Si el usuario elige No, no hacemos nada
                    return;
                }

                // Armar el DTO que viaja a la capa de negocio
                var dto = new FinalizarInternacionDto
                {
                    IdInternacion = _internacion.Id_internacion,
                    IdCama = _internacion.Id_cama,
                    NroHabitacion = _internacion.Nro_habitacion,

                    FechaIngreso = _internacion.Fecha_ingreso,
                    FechaEgreso = dtpFechaEgreso.Value,

                    DiagnosticoIngreso = _internacion.Diagnostico,
                    DiagnosticoEgreso = txtMotivo.Text
                };

                // Llamar al service para que haga la magia (actualizar internación + liberar cama)
                _internacionService.FinalizarInternacion(dto);

                // Mostrar mensaje de éxito
                MessageBox.Show("Internación finalizada correctamente.",
                    "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Volver a la lista de internaciones
                CancelarFinalizacionSolicitada?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al finalizar la internación: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
