using Sistema_Hospitalario.CapaNegocio;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using Sistema_Hospitalario.CapaPresentacion.Medico.Pacientes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Sistema_Hospitalario.CapaPresentacion.Medico
{
    /// <summary>
    /// Control de usuario que muestra el historial clínico detallado de un paciente.
    /// Utiliza un componente WebBrowser para renderizar el historial en formato HTML.
    /// </summary>
    public partial class UC_DetallePaciente : UserControl
    {
        /// <summary>Servicio de operaciones médicas.</summary>
        private readonly MedicoService service = new MedicoService();

        /// <summary>Datos del paciente seleccionado.</summary>
        private PacienteListadoMedicoDto Paciente;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_DetallePaciente"/>.
        /// </summary>
        /// <param name="paciente">DTO del paciente del cual se desea ver el historial.</param>
        public UC_DetallePaciente(PacienteListadoMedicoDto paciente)
        {
            InitializeComponent();
            this.Paciente = paciente; 
            CargarHistorial();
        }
        /// <summary>
        /// Genera y carga el historial clínico consolidado en el control WebBrowser.
        /// Construye un documento HTML dinámico con la información de consultas e internaciones.
        /// </summary>
        private void CargarHistorial()
        {
            // 1. Recolectamos los filtros
            int idPaciente = this.Paciente.IdPaciente;
            int _idMedicoLogueado = (int)SesionUsuario.IdMedicoAsociado;
            try
            {
                var listaHistorial = service.ObtenerHistorial(idPaciente, _idMedicoLogueado);

                // 3. Mostramos el resultado en el TextBox grande
                var sb = new StringBuilder();

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<style>");
                sb.AppendLine("  body { font-family: 'Segoe UI', Arial, sans-serif; }");
                sb.AppendLine("  .item { border: 1px solid #ccc; border-radius: 8px; margin-bottom: 15px; padding: 15px; background-color: #f9f9f9; }");
                sb.AppendLine("  .header { font-size: 1.2em; font-weight: bold; color: #005A96; text-align: center; border-bottom: 1px solid #ccc; padding-bottom: 5px; }");
                sb.AppendLine("  .label { font-weight: bold; color: #333; }");
                sb.AppendLine("  .content { color: #555; padding-left: 10px; }");
                sb.AppendLine("  .patient-header { border-bottom: 2px solid #005A96; padding-bottom: 10px; margin-bottom: 20px; }");
                sb.AppendLine("  .patient-header h1 { color: #005A96; margin: 0; padding: 0; font-size: 1.8em; }");
                sb.AppendLine("  .patient-details p { margin: 2px 0; font-size: 1.1em; color: #333; }");
                sb.AppendLine("  .patient-details .label { font-weight: bold; color: #000; }");

                
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                sb.AppendLine("<div class='patient-header'>");
                sb.AppendLine("<h1>Historial Clínico del Paciente</h1>");
                sb.AppendLine("<div class='patient-details'>");
                sb.AppendLine($"<p><span class='label'>Paciente:</span> {Paciente.Nombre} {Paciente.Apellido}</p>");
                sb.AppendLine($"<p><span class='label'>DNI:</span> {Paciente.Dni}</p>");
                sb.AppendLine($"<p><span class='label'>Direccion:</span> {Paciente.Direccion}</p>");
                sb.AppendLine($"<p><span class='label'>Fecha de Nacimiento:</span> {Paciente.FechaNacim.ToString("dd/MM/yyyy")}</p>");
                sb.AppendLine("</div>");
                sb.AppendLine("</div>");

                if (!listaHistorial.Any())
                {
                    sb.AppendLine("<h3 style='text-align: center;'>--- No se encontraron registros ---</h3>");
                }
                else
                {
                    // 3. Recorremos la lista y creamos un "div" por cada item
                    foreach (var item in listaHistorial)
                    {
                        sb.AppendLine("<div class='item'>");

                        // Título
                        sb.AppendLine($"<div class='header'>{item.Tipo.ToUpper()} - {item.Fecha.ToString("dd/MM/yyyy")}</div>");

                        // Médico
                        sb.AppendLine($"<p><span class='label'>Médico:</span> <span class='content'>{item.NombreMedico} (DNI: {item.DniMedico})</span></p>");

                        // Motivo
                        sb.AppendLine($"<p><span class='label'>Motivo/Obs:</span> <span class='content'>{item.Motivo}</span></p>");

                        // Diagnóstico
                        sb.AppendLine($"<p><span class='label'>Diagnóstico/Proced:</span> <span class='content'>{item.Diagnostico}</span></p>");

                        // Tratamiento
                        sb.AppendLine($"<p><span class='label'>Tratamiento:</span> <span class='content'>{item.Tratamiento}</span></p>");

                        if (item.FechaFin != null)
                        {
                            sb.AppendLine($"<p><span class='label'>Fecha de Finalización:</span> <span class='content'>{item.FechaFin?.ToString("dd/MM/yyyy")}</span></p>");
                        }
                        else
                        {
                            sb.AppendLine($"<p><span class='label'>Fecha de Finalización:</span> <span class='content'> Sin finalizar.</span></p>");
                        }

                            sb.AppendLine("</div>");
                    }
                }

                sb.AppendLine("</body></html>");

                // 4. Cargamos el string HTML en el WebBrowser
                webBrowserHistorial.DocumentText = sb.ToString();
            }
            catch (Exception ex)
            {
                webBrowserHistorial.DocumentText = $"<html><body><h1>Error al cargar el historial</h1><p>{ex.Message}</p></body></html>";
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            webBrowserHistorial.ShowPrintDialog();
        }
    }
}
