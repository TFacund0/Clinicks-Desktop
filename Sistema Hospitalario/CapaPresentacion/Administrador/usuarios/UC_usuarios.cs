using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Sistema_Hospitalario.CapaNegocio.Servicios.UsuarioService;

namespace Sistema_Hospitalario.CapaPresentacion.Administrador.usuarios
{
    /// <summary>
    /// Control de usuario que gestiona la administración de usuarios del sistema.
    /// Permite visualizar, buscar, filtrar por roles y eliminar cuentas de usuario.
    /// </summary>
    public partial class UC_usuarios : UserControl
    {
        /// <summary>Instancia estática del servicio de usuarios para operaciones transversales.</summary>
        private static readonly UsuarioService usuarioService = new UsuarioService();
        /// <summary>Instancia local del servicio para manejo de lógica de usuarios.</summary>
        private readonly UsuarioService _service = usuarioService;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_usuarios"/>.
        /// Configura los filtros, carga el listado de usuarios y actualiza los contadores de roles.
        /// </summary>
        public UC_usuarios()
        {
            InitializeComponent();
            CargarComboOrdenamiento();
            RefrescarGrilla(null, null);
            ConfigurarEstilosGrilla();
            ActualizarContadoresRoles();
        }

        // ===================== MÉTODOS AUXILIARES =====================
        // Carga de usuarios en el DataGridView
        private void RefrescarGrilla(string campo = null, string valor = null)
        {
            try
            {
                var lista = _service.ObtenerUsuarios(campo, valor);
                dgvUsuarios.DataSource = lista;

                if (dgvUsuarios.Columns["IdUsuario"] != null)
                {
                    dgvUsuarios.Columns["IdUsuario"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Configuración de estilos del DataGridView
        private void ConfigurarEstilosGrilla()
        {
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUsuarios.DefaultCellStyle.BackColor = Color.WhiteSmoke;
            dgvUsuarios.DefaultCellStyle.ForeColor = Color.Black;
            dgvUsuarios.DefaultCellStyle.Font = new Font("Segoe UI", 9);
        }

        // Carga de campos del DTO en el ComboBox para ordenamiento
        private void CargarComboOrdenamiento()
        {
            var tipoDelDto = typeof(MostrarUsuariosDto);
            var propiedades = tipoDelDto.GetProperties();
            var listaDeNombres = propiedades.Select(p => p.Name)
                                            .ToList();
            cboCampo.DataSource = listaDeNombres;
        }

        // ===================== EVENTOS DEL FORMULARIO =====================
        // Nuevo usuario
        private void BtnNuevoUsuario_Click(object sender, EventArgs e)
        {
            MenuModer parentForm = this.FindForm() as MenuModer;

            parentForm.AbrirUserControl(new UC_registrarUsuario());
        }

        // Búsqueda de usuarios
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string campo = (string)cboCampo.SelectedItem;
            string valor = txtBuscar.Text.Trim();

            if (string.IsNullOrEmpty(campo))
            {
                MessageBox.Show("Por favor, seleccione un campo para buscar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RefrescarGrilla(campo, valor);
        }

        // Limpiar búsqueda
        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscar.Clear();
            RefrescarGrilla(null, null);
            txtBuscar.Focus();
        }

        // Doble clic para eliminar usuario
        private void DgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dgvUsuarios.Rows[e.RowIndex];
            int idUsuario = Convert.ToInt32(row.Cells["IdUsuario"].Value);

            string nombreUsuario = row.Cells["NombreUsuario"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"¿Desea eliminar al usuario '{nombreUsuario}'?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    _service.EliminarUsuario(idUsuario);
                    RefrescarGrilla(cboCampo.Text, txtBuscar.Text);
                    ActualizarContadoresRoles();
                    MessageBox.Show("Usuario eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Actualizar contadores de usuarios por rol
        private void ActualizarContadoresRoles()
        {
            try
            {
                var conteos = _service.ObtenerConteoUsuariosPorRol();
               
                if (conteos.ContainsKey("medico"))
                {
                    lblTotalMedicos.Text = conteos["medico"].ToString(); 
                }
                else
                {
                    lblTotalMedicos.Text = "0";
                }

                if (conteos.ContainsKey("administrativo")) 
                {
                    lblTotalAdministrativos.Text = conteos["administrativo"].ToString();
                }
                else
                {
                    lblTotalAdministrativos.Text = "0";
                }

                if (conteos.ContainsKey("gerente")) 
                {
                    lblTotalGerentes.Text = conteos["gerente"].ToString();
                }
                else
                {
                    lblTotalGerentes.Text = "0";
                }

                if (conteos.ContainsKey("administrador")) 
                {
                    lblTotalAdministradores.Text = conteos["administrador"].ToString();
                }
                else
                {
                    lblTotalAdministradores.Text = "0";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar los contadores de roles: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblTotalMedicos.Text = "N/A";
                lblTotalAdministrativos.Text = "N/A";
                lblTotalGerentes.Text = "N/A";
                lblTotalAdministradores.Text = "N/A";
            }
        }
    }
}
