using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sistema_Hospitalario.CapaNegocio.Seguridad;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Seguridad
{
    [TestClass]
    public class PasswordHasherTests
    {
        [TestMethod]
        public void Hash_GeneraFormatoPbkdf2ConCuatroPartes()
        {
            string hash = PasswordHasher.Hash("MiClave123");

            StringAssert.StartsWith(hash, "PBKDF2$");
            Assert.AreEqual(4, hash.Split('$').Length);
        }

        [TestMethod]
        public void Hash_MismaPassword_GeneraHashesDistintosPorElSalt()
        {
            string hash1 = PasswordHasher.Hash("MiClave123");
            string hash2 = PasswordHasher.Hash("MiClave123");

            Assert.AreNotEqual(hash1, hash2);
        }

        [TestMethod]
        public void Verificar_PasswordCorrecta_DevuelveTrue()
        {
            string hash = PasswordHasher.Hash("MiClave123");

            Assert.IsTrue(PasswordHasher.Verificar("MiClave123", hash));
        }

        [TestMethod]
        public void Verificar_PasswordIncorrecta_DevuelveFalse()
        {
            string hash = PasswordHasher.Hash("MiClave123");

            Assert.IsFalse(PasswordHasher.Verificar("OtraClave", hash));
        }

        [TestMethod]
        public void Verificar_HashLegacySha256_PasswordCorrecta_DevuelveTrue()
        {
            string hashLegacy = Sha256Hex("MiClave123");

            Assert.IsTrue(PasswordHasher.Verificar("MiClave123", hashLegacy));
        }

        [TestMethod]
        public void Verificar_HashLegacySha256_PasswordIncorrecta_DevuelveFalse()
        {
            string hashLegacy = Sha256Hex("MiClave123");

            Assert.IsFalse(PasswordHasher.Verificar("OtraClave", hashLegacy));
        }

        [TestMethod]
        public void Verificar_PasswordOHashVacios_DevuelveFalse()
        {
            Assert.IsFalse(PasswordHasher.Verificar(null, PasswordHasher.Hash("x")));
            Assert.IsFalse(PasswordHasher.Verificar("", PasswordHasher.Hash("x")));
            Assert.IsFalse(PasswordHasher.Verificar("x", null));
            Assert.IsFalse(PasswordHasher.Verificar("x", ""));
        }

        [TestMethod]
        public void Verificar_HashConFormatoInvalido_DevuelveFalse()
        {
            Assert.IsFalse(PasswordHasher.Verificar("x", "PBKDF2$abc$###$###"));
        }

        [TestMethod]
        public void EsHashLegacy_DetectaSha256YExcluyePbkdf2()
        {
            Assert.IsTrue(PasswordHasher.EsHashLegacy(Sha256Hex("MiClave123")));
            Assert.IsFalse(PasswordHasher.EsHashLegacy(PasswordHasher.Hash("MiClave123")));
        }

        // Replica el algoritmo legacy (SHA-256 hexadecimal sin salt) usado antes de PBKDF2.
        private static string Sha256Hex(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
