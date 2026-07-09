using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class MedicoServiceTests
    {
        private Mock<IMedicoRepository> _repoMock;
        private MedicoService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IMedicoRepository>();
            _service = new MedicoService(_repoMock.Object);

            _repoMock.Setup(r => r.ObtenerTodosParaMedico(It.IsAny<DateTime?>())).Returns(new List<PacienteListadoMedicoDto>
            {
                new PacienteListadoMedicoDto { Nombre = "Juan",  Apellido = "Zapata", Dni = "30111222" },
                new PacienteListadoMedicoDto { Nombre = "Ana",   Apellido = "Alvarez", Dni = "28999888" },
                new PacienteListadoMedicoDto { Nombre = "Juana", Apellido = "Alvarez", Dni = "30555666" }
            });
        }

        [TestMethod]
        public void Constructor_RepositorioNull_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MedicoService(null));
        }

        [TestMethod]
        public void ObtenerPacientes_FiltraPorNombreSinDistinguirMayusculas()
        {
            var resultado = _service.ObtenerPacientes("juan", null, null, null);

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.All(p => p.Nombre.ToLower().Contains("juan")));
        }

        [TestMethod]
        public void ObtenerPacientes_FiltraPorDniConComienzaCon()
        {
            var resultado = _service.ObtenerPacientes(null, null, "30", null);

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.All(p => p.Dni.StartsWith("30")));
        }

        [TestMethod]
        public void ObtenerPacientes_CombinaFiltrosDeApellidoYDni()
        {
            var resultado = _service.ObtenerPacientes(null, "alvarez", "30", null);

            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Juana", resultado[0].Nombre);
        }

        [TestMethod]
        public void ObtenerPacientes_OrdenaPorApellidoYLuegoNombre()
        {
            var resultado = _service.ObtenerPacientes(null, null, null, null);

            CollectionAssert.AreEqual(
                new[] { "Ana", "Juana", "Juan" },
                resultado.Select(p => p.Nombre).ToArray());
        }
    }
}
