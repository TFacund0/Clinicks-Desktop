using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Usuarios;
using Sistema_Hospitalario.CapaNegocio.Servicios.UsuarioService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class UsuarioServiceTests
    {
        private Mock<IUsuarioRepository> _repoMock;
        private UsuarioService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IUsuarioRepository>();
            _service = new UsuarioService(_repoMock.Object);
        }

        [TestMethod]
        public void Constructor_RepositorioNull_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new UsuarioService(null));
        }

        // ---------- AgregarUsuario ----------

        [TestMethod]
        public void AgregarUsuario_UsernameYaExistente_DevuelveError()
        {
            _repoMock.Setup(r => r.ExisteUsername("jperez")).Returns(true);

            var resultado = _service.AgregarUsuario(new UsuarioAltaDto
            {
                NombreUsuario = "jperez",
                Password = "Clave123"
            });

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "jperez");
            _repoMock.Verify(r => r.Insertar(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<int?>()), Times.Never);
        }

        [TestMethod]
        public void AgregarUsuario_HasheaLaPasswordAntesDeInsertar()
        {
            string passwordAlmacenada = null;
            _repoMock.Setup(r => r.ExisteUsername(It.IsAny<string>())).Returns(false);
            _repoMock.Setup(r => r.Insertar(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<int?>()))
                .Callback<string, string, string, int, int, string, string, int?>(
                    (n, a, u, e, rol, pass, c, m) => passwordAlmacenada = pass)
                .Returns((true, 10, null));

            var resultado = _service.AgregarUsuario(new UsuarioAltaDto
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                NombreUsuario = "jperez",
                Password = "Clave123",
                IdEstado = 1,
                IdRol = 2
            });

            Assert.IsTrue(resultado.Ok);
            Assert.IsNotNull(passwordAlmacenada);
            Assert.AreNotEqual("Clave123", passwordAlmacenada, "La contraseña no debe guardarse en texto plano.");
            StringAssert.StartsWith(passwordAlmacenada, "PBKDF2$");
        }

        // ---------- EliminarUsuario ----------

        [TestMethod]
        public void EliminarUsuario_AdministradorPrincipal_LanzaExcepcion()
        {
            Assert.ThrowsException<InvalidOperationException>(() => _service.EliminarUsuario(1));
            _repoMock.Verify(r => r.Eliminar(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void EliminarUsuario_UsuarioComun_DelegaEnElRepositorio()
        {
            _service.EliminarUsuario(5);

            _repoMock.Verify(r => r.Eliminar(5), Times.Once);
        }

        // ---------- ValidarCredenciales ----------

        [TestMethod]
        public void ValidarCredenciales_Correctas_DevuelveLoginExitosoConDatos()
        {
            _repoMock.Setup(r => r.ObtenerUsuarioParaLogin("jperez")).Returns(new DatosLoginUsuarioDto
            {
                IdUsuario = 7,
                Username = "jperez",
                PasswordHashAlmacenado = Sistema_Hospitalario.CapaNegocio.Seguridad.PasswordHasher.Hash("Clave123"),
                NombreRol = "Médico",
                IdMedicoAsociado = 3
            });

            var resultado = _service.ValidarCredenciales("jperez", "Clave123");

            Assert.IsTrue(resultado.LoginExitoso);
            Assert.AreEqual(7, resultado.IdUsuario);
            Assert.AreEqual("Médico", resultado.NombreRol);
            Assert.AreEqual(3, resultado.IdMedicoAsociado);
        }

        [TestMethod]
        public void ValidarCredenciales_PasswordIncorrecta_DevuelveLoginFallido()
        {
            _repoMock.Setup(r => r.ObtenerUsuarioParaLogin("jperez")).Returns(new DatosLoginUsuarioDto
            {
                IdUsuario = 7,
                PasswordHashAlmacenado = Sistema_Hospitalario.CapaNegocio.Seguridad.PasswordHasher.Hash("Clave123")
            });

            var resultado = _service.ValidarCredenciales("jperez", "ClaveIncorrecta");

            Assert.IsFalse(resultado.LoginExitoso);
        }

        [TestMethod]
        public void ValidarCredenciales_UsuarioInexistente_DevuelveLoginFallido()
        {
            _repoMock.Setup(r => r.ObtenerUsuarioParaLogin("fantasma")).Returns((DatosLoginUsuarioDto)null);

            var resultado = _service.ValidarCredenciales("fantasma", "Clave123");

            Assert.IsFalse(resultado.LoginExitoso);
        }

        [TestMethod]
        public void ValidarCredenciales_HashLegacy_MigraAPbkdf2()
        {
            string hashLegacy;
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes("Clave123"));
                var sb = new System.Text.StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                hashLegacy = sb.ToString();
            }

            _repoMock.Setup(r => r.ObtenerUsuarioParaLogin("jperez")).Returns(new DatosLoginUsuarioDto
            {
                IdUsuario = 7,
                PasswordHashAlmacenado = hashLegacy
            });

            var resultado = _service.ValidarCredenciales("jperez", "Clave123");

            Assert.IsTrue(resultado.LoginExitoso);
            _repoMock.Verify(r => r.ActualizarPasswordHash(7,
                It.Is<string>(h => h.StartsWith("PBKDF2$"))), Times.Once);
        }

        [TestMethod]
        public void ValidarCredenciales_HashYaEnPbkdf2_NoVuelveAHashear()
        {
            _repoMock.Setup(r => r.ObtenerUsuarioParaLogin("jperez")).Returns(new DatosLoginUsuarioDto
            {
                IdUsuario = 7,
                PasswordHashAlmacenado = Sistema_Hospitalario.CapaNegocio.Seguridad.PasswordHasher.Hash("Clave123")
            });

            _service.ValidarCredenciales("jperez", "Clave123");

            _repoMock.Verify(r => r.ActualizarPasswordHash(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        // ---------- ObtenerUsuarios ----------

        [TestMethod]
        public void ObtenerUsuarios_ConFiltroPorNombreUsuario_DevuelveSoloCoincidencias()
        {
            _repoMock.Setup(r => r.ObtenerUsuarios()).Returns(new List<MostrarUsuariosDto>
            {
                new MostrarUsuariosDto { IdUsuario = 1, NombreUsuario = "jperez" },
                new MostrarUsuariosDto { IdUsuario = 2, NombreUsuario = "mgarcia" },
                new MostrarUsuariosDto { IdUsuario = 3, NombreUsuario = "jptorres" }
            });

            var resultado = _service.ObtenerUsuarios("NombreUsuario", "jp");

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.TrueForAll(u => u.NombreUsuario.StartsWith("jp")));
        }
    }
}
