using Sistema_Hospitalario.CapaNegocio.DTOs.Camas;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService.CamaService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrador.misceláneo
{
    public partial class Form_CambiarEstadoCama : Form
    {
        // Propiedad pública para exponer el ID seleccionado
        public int NuevoEstadoIdSeleccionado { get; private set; }

        // Campo privado para almacenar el estado actual
        private string _estadoActual; 

        // Modificamos el "Constructor" para que acepte el estado actual
        public Form_CambiarEstadoCama(string estadoActual)
        {
            InitializeComponent();
            _estadoActual = estadoActual;
        }

        // ===================== EVENTOS DEL FORMULARIO =====================
        // Carga del formulario
        private void Form_CambiarEstadoCama_Load(object sender, EventArgs e)
        {
            CargarComboBox();
        }

        // ===================== MÉTODOS AUXILIARES =====================
        // Carga del ComboBox con los estados de cama
        private void CargarComboBox()
        {
            var _camaService = new CamaService();
            var estados = _camaService.ListarEstadosCama();

            comboBox1.DataSource = estados;
            comboBox1.DisplayMember = "Nombre";
            comboBox1.ValueMember = "Id";

            comboBox1.Text = _estadoActual;
        }

        // Botón Guardar
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            this.NuevoEstadoIdSeleccionado = (int)comboBox1.SelectedValue;

            this.Close();
        }

        // Botón Agregar
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            var valorSeleccionado = (int)comboBox1.SelectedValue;

            if (valorSeleccionado == 0) // '0' es el ID que le dimos a "ninguna"
            {
                MessageBox.Show("Por favor, seleccione un estado válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Guardamos el ID seleccionado en nuestra propiedad pública
            this.NuevoEstadoIdSeleccionado = valorSeleccionado;

            // 3. ¡IMPORTANTE! Asignamos el DialogResult manualmente
            this.DialogResult = DialogResult.OK;

            // 4. Cerramos el formulario
            this.Close();
        }
    }
}
