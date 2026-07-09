using Sistema_Hospitalario.CapaDatos.Repositories;
using Sistema_Hospitalario.CapaNegocio.Servicios.BackupService;
using Sistema_Hospitalario.CapaNegocio.Util;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Administrador.Backups
{
    /// <summary>
    /// Control de usuario que proporciona herramientas para la gestión de copias de seguridad (Backup) y restauración (Restore).
    /// Utiliza procesos asíncronos y reporta el progreso a través de una barra de progreso.
    /// </summary>
    public partial class UC_Backups : UserControl
    {
        /// <summary>Servicio para operaciones de backup y restauración.</summary>
        private BackupService _service;
        /// <summary>Token para cancelación de operaciones asíncronas.</summary>
        private CancellationTokenSource _cts;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_Backups"/>.
        /// Configura el servicio de backup obteniendo la cadena de conexión del helper.
        /// </summary>
        public UC_Backups()
        {
            InitializeComponent();
            InicializarServicio();
        }

        private void InicializarServicio()
        {
            var (providerCnn, dbName) = ConnectionHelper
                .GetSqlClientCnnFromEntity("Sistema_HospitalarioEntities_Conexion");

            var repo = new BackupRepository(providerCnn, dbName);
            _service = new BackupService(repo);
        }

        private void ToggleUI(bool enabled)
        {
            btnBackup.Enabled = enabled;
            btnRestore.Enabled = enabled;
            Cursor = enabled ? Cursors.Default : Cursors.WaitCursor;
        }

        // =============== BOTÓN HACER BACKUP ===============
        private async void BtnBackup_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Guardar copia de seguridad";
                sfd.Filter = "Backup SQL Server (*.bak)|*.bak";
                sfd.FileName = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                _cts = new CancellationTokenSource();
                var progress = new Progress<int>(p =>
                {
                    if (p < 0) p = 0;
                    if (p > 100) p = 100;
                    pbProgreso.Value = p;
                    lblProgreso.Text = $"Progreso: {p}%";
                });

                try
                {
                    ToggleUI(false);
                    await _service.HacerBackupAsync(sfd.FileName, progress, _cts.Token);
                    MessageBox.Show("Backup generado correctamente.", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar el backup:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ToggleUI(true);
                    pbProgreso.Value = 0;
                    lblProgreso.Text = "Progreso";
                    _cts.Dispose();
                    _cts = null;
                }
            }
        }

        // =============== BOTÓN RESTAURAR BACKUP ===============
        private async void BtnRestore_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                "Esto restaurará la base de datos y sobrescribirá los datos actuales.\n\n¿Desea continuar?",
                "Confirmar restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Seleccionar archivo .bak";
                ofd.Filter = "Backup SQL Server (*.bak)|*.bak";

                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                _cts = new CancellationTokenSource();
                var progress = new Progress<int>(p =>
                {
                    if (p < 0) p = 0;
                    if (p > 100) p = 100;
                    pbProgreso.Value = p;
                    lblProgreso.Text = $"Progreso: {p}%";
                });

                try
                {
                    ToggleUI(false);
                    await _service.RestaurarAsync(ofd.FileName, progress, _cts.Token);
                    MessageBox.Show("Base restaurada correctamente.", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al restaurar la base:\n" + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    ToggleUI(true);
                    pbProgreso.Value = 0;
                    lblProgreso.Text = "Progreso";
                    _cts.Dispose();
                    _cts = null;
                }
            }
        }
    }
}
