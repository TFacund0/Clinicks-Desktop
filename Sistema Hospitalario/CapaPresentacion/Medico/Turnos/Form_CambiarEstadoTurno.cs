using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;

namespace Sistema_Hospitalario.CapaPresentacion.Medico.Turnos
{
    /// <summary>
    /// Ventana de diálogo que permite al médico cambiar el estado de un turno (ej: de Pendiente a En Proceso o Atendido).
    /// </summary>
    public partial class Form_CambiarEstadoTurno : Form
    {
        /// <summary>ID del nuevo estado seleccionado por el usuario.</summary>
        public int NuevoEstadoId { get; private set; }
        /// <summary>Estado actual del turno antes del cambio.</summary>
        private string _estadoActual;

        /// <summary>
        /// Inicializa una nueva instancia del formulario <see cref="Form_CambiarEstadoTurno"/>.
        /// </summary>
        /// <param name="estadoActual">Nombre del estado actual del turno para pre-seleccionarlo en el combo.</param>
        public Form_CambiarEstadoTurno(string estadoActual)
        {
            InitializeComponent();
            CargarEstados();
            _estadoActual = estadoActual;

            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.Text = "Cambiar Estado del Turno";
        }
        private void CargarEstados()
        {
            var estados = new TurnoService().ListadoEstadosTurnos();

            cboEstadosTurno.DataSource = estados;
            cboEstadosTurno.DisplayMember = "Estado";
            cboEstadosTurno.ValueMember = "Id_estado";
            cboEstadosTurno.Text = _estadoActual; // Selecciona el estado actual
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            this.NuevoEstadoId = Convert.ToInt32(cboEstadosTurno.SelectedValue);
            this.DialogResult = DialogResult.OK; // Lo asignamos manualmente
            this.Close();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
