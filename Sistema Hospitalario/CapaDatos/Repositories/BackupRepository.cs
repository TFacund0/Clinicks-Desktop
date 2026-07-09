using Sistema_Hospitalario.CapaDatos.Interfaces;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Implementación concreta de <see cref="IBackupRepository"/> para SQL Server.
    /// Utiliza comandos T-SQL nativos para gestionar el respaldo y restauración de la base de datos.
    /// </summary>
    /// <remarks>
    /// Esta clase requiere conexión a la base de datos 'master' para realizar operaciones de restauración
    /// que requieren poner la base de datos principal en modo de usuario único.
    /// </remarks>
    public sealed class BackupRepository : IBackupRepository
    {
        private readonly string _providerCnn; // connection string SqlClient
        private readonly string _dbName;      // nombre de la BD
        private readonly string _masterCnn;   // misma cnn pero a master

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="BackupRepository"/>.
        /// </summary>
        /// <param name="providerConnectionString">Cadena de conexión nativa (SqlClient) a la base de datos.</param>
        /// <param name="databaseName">Nombre de la base de datos a respaldar/restaurar.</param>
        public BackupRepository(string providerConnectionString, string databaseName)
        {
            _providerCnn = providerConnectionString;
            _dbName = databaseName;

            var b = new SqlConnectionStringBuilder(_providerCnn);
            b.InitialCatalog = "master";
            _masterCnn = b.ConnectionString;
        }

        /// <inheritdoc />
        public async Task BackupAsync(string backupFullPath, IProgress<int> progreso = null, CancellationToken ct = default)
        {
            string sql = @"
DECLARE @db sysname = @_db;
DECLARE @path nvarchar(4000) = @_path;

DECLARE @sql nvarchar(max) = N'
BACKUP DATABASE ' + QUOTENAME(@db) + N'
TO DISK = @path
WITH INIT, STATS = 5;';   -- 👈 SIN COMPRESSION

EXEC sp_executesql @sql, N'@path nvarchar(4000)', @path;
";

            using (var cnn = new SqlConnection(_masterCnn))
            using (var cmd = new SqlCommand(sql, cnn))
            {
                cnn.InfoMessage += (s, e) =>
                {
                    foreach (var line in e.Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.EndsWith("percent processed.", StringComparison.OrdinalIgnoreCase))
                        {
                            var pctStr = line.Split(' ')[0];
                            if (int.TryParse(pctStr, out int pct))
                                progreso?.Report(pct);
                        }
                    }
                };

                cmd.Parameters.AddWithValue("@_db", _dbName);
                cmd.Parameters.AddWithValue("@_path", backupFullPath);

                await cnn.OpenAsync(ct);
                await cmd.ExecuteNonQueryAsync(ct);
                progreso?.Report(100);
            }
        }



        /// <inheritdoc />
        public async Task RestoreAsync(string backupFullPath, IProgress<int> progreso = null, CancellationToken ct = default)
        {
            string sql = @"
DECLARE @db sysname = @_db;
DECLARE @path nvarchar(4000) = @_path;

IF DB_ID(@db) IS NULL
    THROW 50001, 'La base de datos no existe en este servidor.', 1;

DECLARE @sql nvarchar(max);

SET @sql = N'ALTER DATABASE ' + QUOTENAME(@db) + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE;';
EXEC(@sql);

BEGIN TRY
    SET @sql = N'
    RESTORE DATABASE ' + QUOTENAME(@db) + N'
    FROM DISK = @path
    WITH REPLACE, STATS = 5;';
    EXEC sp_executesql @sql, N'@path nvarchar(4000)', @path;
END TRY
BEGIN CATCH
    SET @sql = N'ALTER DATABASE ' + QUOTENAME(@db) + N' SET MULTI_USER;';
    EXEC(@sql);
    THROW;
END CATCH

SET @sql = N'ALTER DATABASE ' + QUOTENAME(@db) + N' SET MULTI_USER;';
EXEC(@sql);";

            using (var cnn = new SqlConnection(_masterCnn))
            using (var cmd = new SqlCommand(sql, cnn))
            {
                cnn.InfoMessage += (s, e) =>
                {
                    foreach (var line in e.Message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (line.EndsWith("percent processed.", StringComparison.OrdinalIgnoreCase))
                        {
                            var pctStr = line.Split(' ')[0];
                            if (int.TryParse(pctStr, out int pct))
                                progreso?.Report(pct);
                        }
                    }
                };

                cmd.Parameters.AddWithValue("@_db", _dbName);
                cmd.Parameters.AddWithValue("@_path", backupFullPath);

                await cnn.OpenAsync(ct);
                await cmd.ExecuteNonQueryAsync(ct);
                progreso?.Report(100);
            }
        }
    }
}
