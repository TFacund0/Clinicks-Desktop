using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class PacienteServiceTests
    {
        private Mock<IPacienteRepository> _repoMock;
        private PacienteService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IPacienteRepository>();
            _service = new PacienteService(_repoMock.Object);

            _repoMock.Setup(r => r.GetEstados()).Returns(new List<EstadoPacienteDto>
            {
                new EstadoPacienteDto { Id = 1, Nombre = "Activo" },
                new EstadoPacienteDto { Id = 2, Nombre = "Internado" },
                new EstadoPacienteDto { Id = 3, Nombre = "Alta" }
            });
        }

        [TestMethod]
        public void Constructor_RepositorioNull_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new PacienteService(null));
        }

        // ---------- Alta ----------

        [TestMethod]
        public void Alta_DniDuplicado_DevuelveError()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>
            {
                new PacienteDto { Id = 1, Dni = 30111222, Estado_paciente = "activo" }
            });

            var resultado = _service.Alta(new PacienteAltaDto { Nombre = "Juan", Apellido = "Pérez", Dni = 30111222 });

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "DNI");
            _repoMock.Verify(r => r.Insertar(It.IsAny<PacienteDto>()), Times.Never);
        }

        [TestMethod]
        public void Alta_EstadoInexistente_DevuelveError()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>());

            var resultado = _service.Alta(new PacienteAltaDto
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Dni = 30111222,
                EstadoInicial = "congelado"
            });

            Assert.IsFalse(resultado.Ok);
            _repoMock.Verify(r => r.Insertar(It.IsAny<PacienteDto>()), Times.Never);
        }

        [TestMethod]
        public void Alta_SinEstadoInicial_UsaActivoPorDefecto()
        {
            PacienteDto insertado = null;
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>());
            _repoMock.Setup(r => r.Insertar(It.IsAny<PacienteDto>()))
                     .Callback<PacienteDto>(p => insertado = p);

            var resultado = _service.Alta(new PacienteAltaDto
            {
                Nombre = "Juan",
                Apellido = "Pérez",
                Dni = 30111222
            });

            Assert.IsTrue(resultado.Ok);
            Assert.IsNotNull(insertado);
            Assert.AreEqual(1, insertado.Id_estado_paciente);
            Assert.AreEqual("activo", insertado.Estado_paciente);
        }

        [TestMethod]
        public void Alta_RecortaEspaciosEnNombreYApellido()
        {
            PacienteDto insertado = null;
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>());
            _repoMock.Setup(r => r.Insertar(It.IsAny<PacienteDto>()))
                     .Callback<PacienteDto>(p => insertado = p);

            _service.Alta(new PacienteAltaDto
            {
                Nombre = "  Juan ",
                Apellido = " Pérez  ",
                Dni = 30111222
            });

            Assert.AreEqual("Juan", insertado.Nombre);
            Assert.AreEqual("Pérez", insertado.Apellido);
        }

        // ---------- Editar ----------

        [TestMethod]
        public void Editar_PacienteInexistente_DevuelveError()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>());

            var resultado = _service.Editar(new PacienteDetalleDto { Id = 99, DNI = 30111222 });

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "no encontrado");
        }

        [TestMethod]
        public void Editar_DniOcupadoPorOtroPaciente_DevuelveError()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>
            {
                new PacienteDto { Id = 1, Dni = 30111222, Estado_paciente = "activo" },
                new PacienteDto { Id = 2, Dni = 28999888, Estado_paciente = "activo" }
            });

            var resultado = _service.Editar(new PacienteDetalleDto { Id = 1, DNI = 28999888, Telefono = "123" });

            Assert.IsFalse(resultado.Ok);
            StringAssert.Contains(resultado.Error, "DNI");
            _repoMock.Verify(r => r.Actualizar(It.IsAny<int>(), It.IsAny<PacienteDto>()), Times.Never);
        }

        [TestMethod]
        public void Editar_DatosValidos_ActualizaElPaciente()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>
            {
                new PacienteDto { Id = 1, Dni = 30111222, Estado_paciente = "activo" }
            });

            var resultado = _service.Editar(new PacienteDetalleDto
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Pérez",
                DNI = 30111222,
                Estado = "Internado",
                Telefono = "3794-000000"
            });

            Assert.IsTrue(resultado.Ok, resultado.Error);
            _repoMock.Verify(r => r.Actualizar(1, It.Is<PacienteDto>(p => p.Id_estado_paciente == 2)), Times.Once);
        }

        // ---------- Consultas ----------

        [TestMethod]
        public void ContarPorEstadoId_IgnoraMayusculasYMinusculas()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>
            {
                new PacienteDto { Id = 1, Estado_paciente = "Activo" },
                new PacienteDto { Id = 2, Estado_paciente = "ACTIVO" },
                new PacienteDto { Id = 3, Estado_paciente = "Alta" }
            });

            Assert.AreEqual(2, _service.ContarPorEstadoId("activo"));
        }

        [TestMethod]
        public void ListarPacientes_ExcluyeEstadosNoOperativos()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>
            {
                new PacienteDto { Id = 1, Estado_paciente = "activo" },
                new PacienteDto { Id = 2, Estado_paciente = "internado" },
                new PacienteDto { Id = 3, Estado_paciente = "alta" },
                new PacienteDto { Id = 4, Estado_paciente = "fallecido" }
            });

            var resultado = _service.ListarPacientes();

            Assert.AreEqual(3, resultado.Count);
            Assert.IsFalse(resultado.Any(p => p.Id == 4));
        }

        [TestMethod]
        public void ObtenerDetalle_PacienteInexistente_DevuelveNull()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<PacienteDto>());

            Assert.IsNull(_service.ObtenerDetalle(99));
        }
    }
}
