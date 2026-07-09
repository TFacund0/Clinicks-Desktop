using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sistema_Hospitalario.CapaDatos.Interfaces;
using Sistema_Hospitalario.CapaNegocio.DTOs.Home;

namespace Sistema_Hospitalario.CapaDatos.Repositories
{
    /// <summary>
    /// Repositorio encargado de consolidar información de diversas fuentes para la vista de inicio.
    /// Combina datos de turnos, internaciones y registros de pacientes.
    /// </summary>
    public class HomeRepository : IHomeRepository
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="HomeRepository"/>.
        /// </summary>
        public HomeRepository()
        {
        }

        /// <inheritdoc />
        public List<HomeDto> ListarActividad(int cantidad)
        { 
            using (var db = new Sistema_Hospitalario.CapaDatos.Sistema_HospitalarioEntities_Conexion())
            {
                // 1) Turnos
                var turnos = db.turno
                    .AsNoTracking()
                    .Select(t => new HomeDto
                    {
                        Nombre = t.paciente.nombre,
                        Apellido = t.paciente.apellido,
                        Accion = "Turno registrado",
                        Horario = t.fecha_registracion,
                        Tipo = t.procedimiento.nombre // si FK es opcional: Tipo = t.procedimiento != null ? t.procedimiento.nombre : null
                    });

                // 2) Internaciones
                var internaciones = db.internacion
                    .AsNoTracking()
                    .Select(i => new HomeDto
                    {
                        Nombre = i.paciente.nombre,
                        Apellido = i.paciente.apellido,
                        Accion = "Internación registrada",
                        Horario = i.fecha_inicio,      // o i.fecha_registracion si la tenés
                        Tipo = i.procedimiento != null ? i.procedimiento.nombre : null
                    });

                // 3) Pacientes registrados
                var pacientes = db.paciente
                    .AsNoTracking()
                    .Select(p => new HomeDto
                    {
                        Nombre = p.nombre,
                        Apellido = p.apellido,
                        Accion = "Paciente registrado",
                        Horario = p.fecha_registracion,
                        Tipo = "Registración"
                    });

                // Unificamos, ordenamos por más reciente y limitamos
                var query = turnos
                    .Concat(internaciones)
                    .Concat(pacientes)
                    .OrderByDescending(x => x.Horario)
                    .Take(cantidad);

                return query.ToList();
            }
        }
    }
}
