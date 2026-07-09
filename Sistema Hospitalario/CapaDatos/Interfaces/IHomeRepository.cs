using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sistema_Hospitalario.CapaNegocio.DTOs.Home;

namespace Sistema_Hospitalario.CapaDatos.Interfaces
{
    /// <summary>
    /// Define las operaciones de acceso a datos para la pantalla principal o tablero de resumen.
    /// </summary>
    public interface IHomeRepository
    {
        /// <summary>
        /// Obtiene un listado de la actividad reciente consolidada (turnos e internaciones).
        /// </summary>
        /// <param name="cantidad">Número máximo de registros a recuperar.</param>
        /// <returns>Lista de <see cref="HomeDto"/> con la actividad cronológica.</returns>
        List<HomeDto> ListarActividad(int cantidad);
    }
}
