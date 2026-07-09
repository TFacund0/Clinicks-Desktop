using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.Servicios.EstadisticasService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class EstadisticasServiceTests
    {
        private Mock<IEstadisticasRepository> _repoMock;
        private EstadisticasService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<IEstadisticasRepository>();
            _service = new EstadisticasService(_repoMock.Object);
        }

        [TestMethod]
        public void Constructor_RepositorioNull_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new EstadisticasService(null));
        }

        [TestMethod]
        public void ObtenerPacientesSemana_DevuelveExactamenteSieteDiasConsecutivos()
        {
            _repoMock.Setup(r => r.ContarPacientesPorEstadoYFecha(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(1);

            var resultado = _service.ObtenerPacientesSemana();

            Assert.AreEqual(7, resultado.Count);
            Assert.AreEqual(DateTime.Today.AddDays(-6), resultado.First().Fecha);
            Assert.AreEqual(DateTime.Today, resultado.Last().Fecha);
        }

        [TestMethod]
        public void ObtenerTurnosPorDiaUltimaSemana_CompletaConCeroLosDiasSinTurnos()
        {
            var hoy = DateTime.Today;
            _repoMock.Setup(r => r.ObtenerConteoTurnosPorDia(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                     .Returns(new Dictionary<DateTime, int> { { hoy, 4 } });

            var resultado = _service.ObtenerTurnosPorDiaUltimaSemana();

            Assert.AreEqual(7, resultado.Count);
            Assert.AreEqual(4, resultado.Single(d => d.Fecha == hoy).Cantidad);
            Assert.IsTrue(resultado.Where(d => d.Fecha != hoy).All(d => d.Cantidad == 0));
        }

        [TestMethod]
        public void ObtenerDistribucionCamas_MapeaOcupadasYDisponibles()
        {
            _repoMock.Setup(r => r.ContarCamasPorDisponibilidad("ocupada")).Returns(12);
            _repoMock.Setup(r => r.ContarCamasPorDisponibilidad("disponible")).Returns(8);

            var resultado = _service.ObtenerDistribucionCamas();

            Assert.AreEqual(12, resultado.Ocupadas);
            Assert.AreEqual(8, resultado.Disponibles);
        }
    }
}
