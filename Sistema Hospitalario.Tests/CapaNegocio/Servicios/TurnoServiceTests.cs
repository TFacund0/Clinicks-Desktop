using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;

namespace Sistema_Hospitalario.Tests.CapaNegocio.Servicios
{
    [TestClass]
    public class TurnoServiceTests
    {
        private Mock<ITurnoRepository> _repoMock;
        private TurnoService _service;

        [TestInitialize]
        public void Inicializar()
        {
            _repoMock = new Mock<ITurnoRepository>();
            _service = new TurnoService(_repoMock.Object);
        }

        [TestMethod]
        public void Constructor_RepositorioNull_LanzaArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TurnoService(null));
        }

        // ---------- CantidadTurnosPorEstado ----------

        [TestMethod]
        public void CantidadTurnosPorEstado_IgnoraMayusculasYMinusculas()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<TurnoDto>
            {
                new TurnoDto { Estado = "Atendido", FechaTurno = DateTime.Today },
                new TurnoDto { Estado = "ATENDIDO", FechaTurno = DateTime.Today.AddDays(-1) },
                new TurnoDto { Estado = "Cancelado", FechaTurno = DateTime.Today }
            });

            Assert.AreEqual(2, _service.CantidadTurnosPorEstado("atendido"));
        }

        [TestMethod]
        public void CantidadTurnosPorEstado_Pendiente_SoloCuentaLosDeHoy()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<TurnoDto>
            {
                new TurnoDto { Estado = "Pendiente", FechaTurno = DateTime.Today.AddHours(10) },
                new TurnoDto { Estado = "Pendiente", FechaTurno = DateTime.Today.AddDays(1) },
                new TurnoDto { Estado = "Pendiente", FechaTurno = DateTime.Today.AddDays(-1) }
            });

            Assert.AreEqual(1, _service.CantidadTurnosPorEstado("Pendiente"));
        }

        [TestMethod]
        public void CantidadTurnosPendientes_CuentaTodosSinFiltrarPorFecha()
        {
            _repoMock.Setup(r => r.GetAll()).Returns(new List<TurnoDto>
            {
                new TurnoDto { Estado = "Pendiente", FechaTurno = DateTime.Today },
                new TurnoDto { Estado = "pendiente", FechaTurno = DateTime.Today.AddDays(5) },
                new TurnoDto { Estado = "Atendido", FechaTurno = DateTime.Today }
            });

            Assert.AreEqual(2, _service.CantidadTurnosPendientes());
        }

        // ---------- Validación de colisiones ----------

        [TestMethod]
        public void ExisteTurnoMismoDiaMismoMedicoPaciente_DelegaEnElRepositorio()
        {
            var fecha = new DateTime(2026, 7, 10);
            _repoMock.Setup(r => r.ExisteTurnoMismoDiaMismoMedicoPaciente(3, 7, fecha)).Returns(true);

            Assert.IsTrue(_service.ExisteTurnoMismoDiaMismoMedicoPaciente(3, 7, fecha));
            _repoMock.Verify(r => r.ExisteTurnoMismoDiaMismoMedicoPaciente(3, 7, fecha), Times.Once);
        }

        [TestMethod]
        public void ExisteTurnoExcluyendo_DelegaEnElRepositorioConElIdExcluido()
        {
            var fecha = new DateTime(2026, 7, 10);
            _repoMock.Setup(r => r.ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(99, 3, 7, fecha)).Returns(false);

            Assert.IsFalse(_service.ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(99, 3, 7, fecha));
            _repoMock.Verify(r => r.ExisteTurnoMismoDiaMismoMedicoPacienteExcluyendo(99, 3, 7, fecha), Times.Once);
        }

        // ---------- Operaciones básicas ----------

        [TestMethod]
        public void RegistrarTurno_DelegaEnElRepositorio()
        {
            var turno = new TurnoDto { Id_paciente = 3, Id_medico = 7, FechaTurno = DateTime.Today };

            _service.RegistrarTurno(turno);

            _repoMock.Verify(r => r.Insertar(turno), Times.Once);
        }

        [TestMethod]
        public void ActualizarEstadoTurno_DevuelveElResultadoDelRepositorio()
        {
            _repoMock.Setup(r => r.ActualizarEstadoTurno(5, 2)).Returns(true);

            Assert.IsTrue(_service.ActualizarEstadoTurno(5, 2));
        }
    }
}
