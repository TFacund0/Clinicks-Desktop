using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace Sistema_Hospitalario.CapaNegocio.Util
{
    /// <summary>
    /// Clase de utilidad para la gestión y extracción de cadenas de conexión.
    /// Facilita la conversión entre conexiones de Entity Framework y conexiones nativas de SQL Client.
    /// </summary>
    public static class ConnectionHelper
    {
        /// <summary>
        /// Extrae la cadena de conexión del proveedor (SqlClient) y el nombre de la base de datos 
        /// a partir de una cadena de conexión de Entity Framework definida en el archivo de configuración.
        /// </summary>
        /// <param name="entityConnName">Nombre de la cadena de conexión en el App.config.</param>
        /// <returns>
        /// Una tupla que contiene:
        /// <list type="bullet">
        /// <item><description><c>providerCnn</c>: La cadena de conexión nativa para SQL Server.</description></item>
        /// <item><description><c>dbName</c>: El nombre del catálogo inicial (Base de Datos).</description></item>
        /// </list>
        /// </returns>
        public static (string providerCnn, string dbName) GetSqlClientCnnFromEntity(string entityConnName)
        {
            var entityCnn = System.Configuration.ConfigurationManager
                            .ConnectionStrings[entityConnName].ConnectionString;

            var ecb = new EntityConnectionStringBuilder(entityCnn);
            var providerCnn = ecb.ProviderConnectionString;

            var sb = new SqlConnectionStringBuilder(providerCnn);
            string dbName = sb.InitialCatalog;

            return (providerCnn, dbName);
        }
    }
}
