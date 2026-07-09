using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Internaciones;
using Sistema_Hospitalario.CapaNegocio.Servicios.InternacionService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class InternacionServiceTests
    {
        private Mock<IInternacionRepository> _repoMock;
        private InternacionService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IInternacionRepository>();
            _service = new InternacionService(_repoMock.Object);
        }

        [TestMethod]
        public void Constructor_RepositorioNull_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new InternacionService(null));
        }

        [TestMethod]
        public void FinalizarInternacion_FechaEgresoAnteriorAlIngreso_LanzaExcepcion()
        {
            var dto = new FinalizarInternacionDto
            {
                FechaIngreso = new DateTime(2026, 7, 10),
                FechaEgreso = new DateTime(2026, 7, 5),
                DiagnosticoEgreso = "Recuperado"
            };

            Assert.ThrowsException<InvalidOperationException>(() => _service.FinalizarInternacion(dto));
            _repoMock.Verify(r => r.FinalizarInternacion(It.IsAny<FinalizarInternacionDto>()), Times.Never);
        }

        [TestMethod]
        public void FinalizarInternacion_SinDiagnosticoDeEgreso_LanzaExcepcion()
        {
            var dto = new FinalizarInternacionDto
            {
                FechaIngreso = new DateTime(2026, 7, 5),
                FechaEgreso = new DateTime(2026, 7, 10),
                DiagnosticoEgreso = "   "
            };

            Assert.ThrowsException<InvalidOperationException>(() => _service.FinalizarInternacion(dto));
        }

        [TestMethod]
        public void FinalizarInternacion_DatosValidos_DelegaEnElRepositorio()
        {
            var dto = new FinalizarInternacionDto
            {
                FechaIngreso = new DateTime(2026, 7, 5),
                FechaEgreso = new DateTime(2026, 7, 10),
                DiagnosticoEgreso = "Recuperado"
            };

            _service.FinalizarInternacion(dto);

            _repoMock.Verify(r => r.FinalizarInternacion(dto), Times.Once);
        }

        [TestMethod]
        public void TotalInternacionesXProcedimiento_CuentaSoloLasDelProcedimiento()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<InternacionDto>
            {
                new InternacionDto { Id_internacion = 1, Id_procedimiento = 5 },
                new InternacionDto { Id_internacion = 2, Id_procedimiento = 5 },
                new InternacionDto { Id_internacion = 3, Id_procedimiento = 8 }
            });

            Assert.AreEqual(2, _service.TotalInternacionesXProcedimiento(5));
        }
    }
}
