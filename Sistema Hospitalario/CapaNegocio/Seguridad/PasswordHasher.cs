using System;
using System.Security.Cryptography;
using System.Text;

namespace Sistema_Hospitalario.CapaNegocio.Seguridad
{
    /// <summary>
    /// Proporciona hashing y verificación de contraseñas mediante PBKDF2 (RFC 2898)
    /// con salt aleatorio e iteraciones configurables.
    /// Mantiene compatibilidad de lectura con los hashes legacy en SHA-256 sin salt,
    /// permitiendo su migración transparente al iniciar sesión.
    /// </summary>
    public static class PasswordHasher
    {
        private const int Iteraciones = 100_000;
        private const int TamanioSalt = 16;   // bytes
        private const int TamanioHash = 32;   // bytes
        private const string Prefijo = "PBKDF2";

        /// <summary>
        /// Genera un hash PBKDF2 de la contraseña con salt aleatorio.
        /// </summary>
        /// <param name="password">Contraseña en texto plano.</param>
        /// <returns>Cadena con formato <c>PBKDF2$iteraciones$saltBase64$hashBase64</c>.</returns>
        public static string Hash(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            byte[] salt = new byte[TamanioSalt];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] hash = DerivarClave(password, salt, Iteraciones);

            return $"{Prefijo}${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifica una contraseña contra un hash almacenado (PBKDF2 o legacy SHA-256).
        /// </summary>
        /// <param name="password">Contraseña en texto plano ingresada por el usuario.</param>
        /// <param name="hashAlmacenado">Hash guardado en la base de datos.</param>
        /// <returns>True si la contraseña coincide con el hash.</returns>
        public static bool Verificar(string password, string hashAlmacenado)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashAlmacenado))
                return false;

            if (EsHashLegacy(hashAlmacenado))
                return CompararSeguro(
                    Encoding.UTF8.GetBytes(HashLegacySha256(password)),
                    Encoding.UTF8.GetBytes(hashAlmacenado));

            var partes = hashAlmacenado.Split('$');
            if (partes.Length != 4 || partes[0] != Prefijo)
                return false;

            if (!int.TryParse(partes[1], out int iteraciones))
                return false;

            byte[] salt, hashEsperado;
            try
            {
                salt = Convert.FromBase64String(partes[2]);
                hashEsperado = Convert.FromBase64String(partes[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            byte[] hashCalculado = DerivarClave(password, salt, iteraciones);
            return CompararSeguro(hashCalculado, hashEsperado);
        }

        /// <summary>
        /// Indica si un hash almacenado usa el formato legacy (SHA-256 hexadecimal sin salt)
        /// y por lo tanto debe ser regenerado con PBKDF2.
        /// </summary>
        /// <param name="hashAlmacenado">Hash guardado en la base de datos.</param>
        /// <returns>True si el hash es legacy y conviene re-hashearlo.</returns>
        public static bool EsHashLegacy(string hashAlmacenado)
        {
            return !string.IsNullOrEmpty(hashAlmacenado)
                && !hashAlmacenado.StartsWith(Prefijo + "$", StringComparison.Ordinal);
        }

        private static byte[] DerivarClave(string password, byte[] salt, int iteraciones)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iteraciones, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(TamanioHash);
            }
        }

        private static string HashLegacySha256(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        // Comparación en tiempo constante para evitar ataques de temporización.
        private static bool CompararSeguro(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int diferencia = 0;
            for (int i = 0; i < a.Length; i++)
                diferencia |= a[i] ^ b[i];
            return diferencia == 0;
        }
    }
}
