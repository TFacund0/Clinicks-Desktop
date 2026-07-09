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

        // ---------- RegistrarConsulta (validaciones previas a la BD) ----------

        [TestMethod]
        public void RegistrarConsulta_SinDni_DevuelveError()
        {
            var resultado = _service.RegistrarConsulta(new Sistema_Hospitalario.CapaNegocio.DTOs.Consultas.ConsultaAltaDto
            {
                DniPaciente = "  ",
                Motivo = "Control"
            }, idMedicoLogueado: 1);

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "DNI");
        }

        [TestMethod]
        public void RegistrarConsulta_SinMotivo_DevuelveError()
        {
            var resultado = _service.RegistrarConsulta(new Sistema_Hospitalario.CapaNegocio.DTOs.Consultas.ConsultaAltaDto
            {
                DniPaciente = "30111222",
                Motivo = ""
            }, idMedicoLogueado: 1);

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "Motivo");
        }

        [TestMethod]
        public void RegistrarConsulta_DniNoNumerico_DevuelveError()
        {
            var resultado = _service.RegistrarConsulta(new Sistema_Hospitalario.CapaNegocio.DTOs.Consultas.ConsultaAltaDto
            {
                DniPaciente = "3O111Z22",
                Motivo = "Control"
            }, idMedicoLogueado: 1);

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "DNI");
        }

        [TestMethod]
        public void RegistrarConsulta_PacienteInexistente_DevuelveError()
        {
            _repoMock.Setup(r => r.ObtenerIdPacientePorDni(30111222)).Returns((int?)null);

            var resultado = _service.RegistrarConsulta(new Sistema_Hospitalario.CapaNegocio.DTOs.Consultas.ConsultaAltaDto
            {
                DniPaciente = "30111222",
                Motivo = "Control"
            }, idMedicoLogueado: 1);

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "30111222");
        }

        [TestMethod]
        public void RegistrarConsulta_DatosValidos_InsertaLaConsulta()
        {
            _repoMock.Setup(r => r.ObtenerIdPacientePorDni(30111222)).Returns(7);
            var dto = new Sistema_Hospitalario.CapaNegocio.DTOs.Consultas.ConsultaAltaDto
            {
                DniPaciente = "30111222",
                Motivo = "Control"
            };

            var resultado = _service.RegistrarConsulta(dto, idMedicoLogueado: 4);

            Assert.IsTrue(resultado.Ok, resultado.Error);
            _repoMock.Verify(r => r.InsertarConsulta(dto, 4, 7), Times.Once);
        }

        // ---------- ObtenerMedicos ----------

        [TestMethod]
        public void ObtenerMedicos_ConFiltroPorNombre_DevuelveSoloCoincidencias()
        {
            _repoMock.Setup(r => r.ObtenerMedicos()).Returns(new List<Sistema_Hospitalario.CapaNegocio.DTOs.Medicos.MostrarMedicoDto>
            {
                new Sistema_Hospitalario.CapaNegocio.DTOs.Medicos.MostrarMedicoDto { IdMedico = 1, Nombre = "Carlos" },
                new Sistema_Hospitalario.CapaNegocio.DTOs.Medicos.MostrarMedicoDto { IdMedico = 2, Nombre = "Carla" },
                new Sistema_Hospitalario.CapaNegocio.DTOs.Medicos.MostrarMedicoDto { IdMedico = 3, Nombre = "Pedro" }
            });

            var resultado = _service.ObtenerMedicos("Nombre", "car");

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.All(m => m.Nombre.ToLower().StartsWith("car")));
        }
    }
}
