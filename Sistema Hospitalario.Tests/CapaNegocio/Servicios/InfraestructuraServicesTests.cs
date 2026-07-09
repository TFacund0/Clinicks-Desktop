using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Camas;
using Sistema_Hospitalario.CapaNegocio.DTOs.Habitaciones;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.EspecialidadService;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService;
using Sistema_Hospitalario.CapaNegocio.Servicios.HabitacionService.CamaService;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class EspecialidadServiceTests
    {
        private Mock<IEspecialidadRepository> _repoMock;
        private EspecialidadService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IEspecialidadRepository>();
            _service = new EspecialidadService(_repoMock.Object);
        }

        [TestMethod]
        public void AgregarEspecialidad_NombreVacioONulo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => _service.AgregarEspecialidad(null));
            Assert.ThrowsException<ArgumentException>(() => _service.AgregarEspecialidad("   "));
            _repoMock.Verify(r => r.Insertar(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void AgregarEspecialidad_NombreValido_DelegaEnElRepositorio()
        {
            _service.AgregarEspecialidad("Cardiología");

            _repoMock.Verify(r => r.Insertar("Cardiología"), Times.Once);
        }
    }

    [TestClass]
    public class HabitacionServiceTests
    {
        private Mock<IHabitacionRepository> _repoMock;
        private HabitacionService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IHabitacionRepository>();
            _service = new HabitacionService(_repoMock.Object);
        }

        [TestMethod]
        public void AgregarHabitacion_PisoNegativo_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => _service.AgregarHabitacion(-1, 2));
            _repoMock.Verify(r => r.Insertar(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void AgregarHabitacion_PisoValido_DelegaEnElRepositorio()
        {
            _service.AgregarHabitacion(3, 2);

            _repoMock.Verify(r => r.Insertar(3, 2), Times.Once);
        }

        [TestMethod]
        public void TotalHabitaciones_DevuelveLaCantidadDelRepositorio()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<MostrarHabitacionDto>
            {
                new MostrarHabitacionDto(), new MostrarHabitacionDto(), new MostrarHabitacionDto()
            });

            Assert.AreEqual(3, _service.TotalHabitaciones());
        }

        [TestMethod]
        public void ListarHabitacionesXPiso_TextoInvalido_DevuelveListaVaciaSinTocarLaBase()
        {
            // "abc", vacío, cero o negativo no deben llegar a la base de datos.
            Assert.AreEqual(0, _service.ListarHabitacionesXPiso("abc").Count);
            Assert.AreEqual(0, _service.ListarHabitacionesXPiso("").Count);
            Assert.AreEqual(0, _service.ListarHabitacionesXPiso("0").Count);
            Assert.AreEqual(0, _service.ListarHabitacionesXPiso("-2").Count);
        }
    }

    [TestClass]
    public class CamaServiceTests
    {
        private Mock<ICamaRepository> _repoMock;
        private CamaService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<ICamaRepository>();
            _service = new CamaService(_repoMock.Object);

            _repoMock.Setup(r => r.GetAll()).Returns(new List<MostrarCamaDto>
            {
                new MostrarCamaDto { NroCama = 1, NroHabitacion = 101, Estado = "Disponible", IdEstadoCama = 1 },
                new MostrarCamaDto { NroCama = 2, NroHabitacion = 101, Estado = "OCUPADA",    IdEstadoCama = 2 },
                new MostrarCamaDto { NroCama = 3, NroHabitacion = 202, Estado = "disponible", IdEstadoCama = 1 }
            });
        }

        [TestMethod]
        public void AgregarCama_HabitacionNegativa_LanzaArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => _service.AgregarCama(-5));
            _repoMock.Verify(r => r.Insertar(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void TotalCamasXEstado_IgnoraMayusculasYMinusculas()
        {
            Assert.AreEqual(2, _service.TotalCamasXEstado("DISPONIBLE"));
            Assert.AreEqual(1, _service.TotalCamasXEstado("ocupada"));
        }

        [TestMethod]
        public void ListarCamasXHabitacion_DevuelveSoloLasDeEsaHabitacion()
        {
            var resultado = _service.ListarCamasXHabitacion("101");

            Assert.AreEqual(2, resultado.Count);
            Assert.IsTrue(resultado.All(c => c.NroHabitacion == 101));
        }

        [TestMethod]
        public void ListarEstadosCama_MapeaLasEntidadesACamaDto()
        {
            _repoMock.Setup(r => r.GetEstadosCama()).Returns(new List<estado_cama>
            {
                new estado_cama { id_estado_cama = 1, disponibilidad = "Disponible" },
                new estado_cama { id_estado_cama = 2, disponibilidad = "Ocupada" }
            });

            var resultado = _service.ListarEstadosCama();

            Assert.AreEqual(2, resultado.Count);
            Assert.AreEqual("Disponible", resultado[0].EstadoCama);
            Assert.AreEqual(1, resultado[0].IdEstadoCama);
        }
    }

    [TestClass]
    public class EstadoPacienteServiceTests
    {
        [TestMethod]
        public void ListarEstados_DevuelveLosEstadosOrdenadosAlfabeticamente()
        {
            var repoMock = new Mock<IPacienteRepository>();
            repoMock.Setup(r => r.GetEstados()).Returns(new List<EstadoPacienteDto>
            {
                new EstadoPacienteDto { Id = 1, Nombre = "Internado" },
                new EstadoPacienteDto { Id = 2, Nombre = "Activo" },
                new EstadoPacienteDto { Id = 3, Nombre = "Alta" }
            });
            var service = new EstadoPacienteService(repoMock.Object);

            var resultado = service.ListarEstados();

            CollectionAssert.AreEqual(
                new[] { "Activo", "Alta", "Internado" },
                resultado.Select(e => e.Nombre).ToArray());
        }
    }
}
