using Sistema_Hospitalario.CapaDatos.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaNegocio.Servicios.BackupService
{
    /// <summary>
    /// Servicio encargado de gestionar las operaciones de respaldo (backup) y restauración 
    /// de la base de datos del sistema hospitalario.
    /// </summary>
    public class BackupService
    {
        private readonly IBackupRepository _repo;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="BackupService"/>.
        /// </summary>
        /// <param name="repo">Repositorio de backup inyectado para acceso a datos.</param>
        public BackupService(IBackupRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Realiza una copia de seguridad de la base de datos en la ruta especificada de forma asíncrona.
        /// </summary>
        /// <param name="destinoBak">Ruta absoluta del archivo .bak de destino.</param>
        /// <param name="progreso">Objeto opcional para reportar el progreso de la operación (0-100).</param>
        /// <param name="ct">Token de cancelación para abortar la operación si es necesario.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public Task HacerBackupAsync(string destinoBak, IProgress<int> progreso = null, CancellationToken ct = default)
            => _repo.BackupAsync(destinoBak, progreso, ct);

        /// <summary>
        /// Restaura la base de datos a partir de un archivo de respaldo de forma asíncrona.
        /// </summary>
        /// <param name="origenBak">Ruta absoluta del archivo .bak de origen.</param>
        /// <param name="progreso">Objeto opcional para reportar el progreso de la operación (0-100).</param>
        /// <param name="ct">Token de cancelación para abortar la operación si es necesario.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public Task RestaurarAsync(string origenBak, IProgress<int> progreso = null, CancellationToken ct = default)
            => _repo.RestoreAsync(origenBak, progreso, ct);
    }
}
