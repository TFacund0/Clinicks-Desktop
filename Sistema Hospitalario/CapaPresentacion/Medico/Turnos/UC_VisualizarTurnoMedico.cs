using Sistema_Hospitalario.CapaNegocio.DTOs.Medicos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Pacientes;
using Sistema_Hospitalario.CapaNegocio.DTOs.Procedimientos;
using Sistema_Hospitalario.CapaNegocio.DTOs.Turnos;
using Sistema_Hospitalario.CapaNegocio.Servicios.MedicoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.PacienteService;
using Sistema_Hospitalario.CapaNegocio.Servicios.ProcedimientoService;
using Sistema_Hospitalario.CapaNegocio.Servicios.TurnoService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Hospitalario.CapaPresentacion.Medico.Turnos
{
    /// <summary>
    /// Control de usuario que permite visualizar y editar el detalle de un turno desde la perspectiva médica.
    /// Soporta cambio de paciente, médico, procedimiento, estado y observaciones del turno.
    /// </summary>
    public partial class UC_VisualizarTurnoMedico : UserControl
    {
        /// <summary>DTO que contiene los datos del turno que se está visualizando.</summary>
        private readonly TurnoDto _turno;
        
        /// <summary>Bandera que indica si el control se encuentra en modo edición.</summary>
        private bool _modoEdicion = false;

        /// <summary>Evento que notifica que se ha cancelado o finalizado la visualización del turno.</summary>
        public event EventHandler CancelarVisualizacionSolicitada;

        /// <summary>
        /// Inicializa una nueva instancia de <see cref="UC_VisualizarTurnoMedico"/>.
        /// </summary>
        /// <param name="turno">El DTO del turno a cargar.</param>
        public UC_VisualizarTurnoMedico(TurnoDto turno)
        {
            InitializeComponent();
            _turno = turno ?? throw new ArgumentNullException(nameof(turno));

            ConfigurarUISoloLectura();
        }

        // ======================== CONFIGURACION DEL FORMULARIO ========================
        // Configura la UI para modo solo lectura
        private void ConfigurarUISoloLectura()
        {
            ToggleEdicion(false);
            CargarDatosLectura(_turno);
            CargarCombosBox(_turno);
        }

        // Carga los datos del turno en los controles de solo lectura
        private void CargarDatosLectura(TurnoDto p_turno)
        {
            txtPaciente.Text = p_turno.Paciente;
            txtMedico.Text = p_turno.Medico;
            txtProcedimiento.Text = p_turno.Procedimiento;
            txtCorreo.Text = p_turno.Correo ?? string.Empty;
            txtTelefono.Text = p_turno.Telefono ?? string.Empty;
            dtpFechaTurno.Value = p_turno.FechaTurno;
            txtObservaciones.Text = p_turno.Observaciones ?? string.Empty;
        }

        // Carga los datos en los ComboBox
        private void CargarCombosBox(TurnoDto p_turno)
        {
            CargarComboPaciente(p_turno);
            CargarComboMedico(p_turno);
            CargarComboProcedimiento(p_turno);
            CargarComboBoxTurno();
        }

        // Habilita o deshabilita los controles para edición
        private void ToggleEdicion(bool habilitar)
        {
            _modoEdicion = habilitar;

            txtPaciente.ReadOnly = !habilitar;
            txtMedico.ReadOnly = !habilitar;
            txtProcedimiento.ReadOnly = !habilitar;
            txtCorreo.ReadOnly = !habilitar;
            txtTelefono.ReadOnly = !habilitar;
            txtObservaciones.ReadOnly = !habilitar;
            dtpFechaTurno.Enabled = habilitar;

            txtPaciente.Visible = !habilitar;
            txtMedico.Visible = !habilitar;
            txtProcedimiento.Visible = !habilitar;

            cbPaciente.Visible = habilitar;
            cbPaciente.Enabled = habilitar;
            lblEstado.Visible = habilitar;
            cbEstadoTurno.Visible = habilitar;
            cbEstadoTurno.Enabled = habilitar;
            cbMedico.Visible = habilitar;
            cbMedico.Enabled = habilitar;
            cbProcedimiento.Visible = habilitar;
            cbProcedimiento.Enabled = habilitar;

            btnModificar.Text = habilitar ? "Guardar" : "Modificar";
            btnEliminar.Enabled = !habilitar;
        }

        // ======================== CARGA DE COMBOS ========================
        // Carga los pacientes en el ComboBox
        private void CargarComboPaciente(TurnoDto p_turno)
        {
            PacienteService _servicioPaciente = new PacienteService();
            
            List<PacienteDto> listaPacientes = _servicioPaciente.ListarPacientes();

            var fuente = listaPacientes
                .Where(p => p.Estado_paciente == "activo" || p.Estado_paciente == "internado")
                .Select(p => new {
                    p.Id,
                    Display = $"{p.Nombre} ({p.Dni})"
                })
                .ToList();

            cbPaciente.DropDownStyle = ComboBoxStyle.DropDown;
            cbPaciente.DataSource = fuente;
            cbPaciente.DisplayMember = "Display";
            cbPaciente.ValueMember = "Id";

            if (p_turno != null && p_turno.Id_paciente > 0)
            {
                cbPaciente.SelectedValue = p_turno.Id_paciente;
                
                cbPaciente.Text = ((dynamic)cbPaciente.SelectedItem)?.Display ?? string.Empty;
            }
        }

        // Carga los médicos en el ComboBox
        private void CargarComboMedico(TurnoDto p_turno)
        {
            MedicoService _servicioMedico = new MedicoService();

            List<MedicoDto> listaMedicos = _servicioMedico.ListarMedicos() ?? new List<MedicoDto>();
            
            var fuente = listaMedicos
                .Select(m => new {
                    m.Id,
                    Display = $"{m.Apellido} {m.Nombre} ({m.Especialidad})"
                })
                .ToList();
            
            cbMedico.DropDownStyle = ComboBoxStyle.DropDown;
            cbMedico.DataSource = fuente;
            cbMedico.DisplayMember = "Display";
            cbMedico.ValueMember = "Id";

            if (p_turno != null && p_turno.Id_medico > 0)
            {
                cbMedico.SelectedValue = p_turno.Id_medico;

                cbMedico.Text = ((dynamic)cbMedico.SelectedItem)?.Display ?? string.Empty;
            }
        }

        // Carga los procedimientos en el ComboBox
        private void CargarComboProcedimiento(TurnoDto p_turno)
        {
            ProcedimientoService _servicioProcedimiento = new ProcedimientoService();
            List<ProcedimientoDto> listaProcedimientos = _servicioProcedimiento.ListarProcedimientos() ?? new List<ProcedimientoDto>();

            var fuente = listaProcedimientos
                .Select(p => new {
                    p.Id,
                    Display = p.Name
                })
                .ToList();

            cbProcedimiento.DropDownStyle = ComboBoxStyle.DropDown;
            cbProcedimiento.DataSource = fuente;
            cbProcedimiento.DisplayMember = "Display";
            cbProcedimiento.ValueMember = "Id";

            if (p_turno != null && p_turno.Id_procedimiento > 0)
            {
                cbProcedimiento.SelectedValue = p_turno.Id_procedimiento;

                cbProcedimiento.Text = ((dynamic)cbProcedimiento.SelectedItem)?.Display ?? string.Empty;
            }
        }

        // Carga los estados de turno en el ComboBox
        private void CargarComboBoxTurno()
        {
            var turnoService = new TurnoService();
            var listaEstadosTurnos = turnoService.ListadoEstadosTurnos();

            var lista = new List<ListadoEstadoTurno>
            {
                new ListadoEstadoTurno { Id_estado = "0", Estado = "— Seleccioná —" }
            };

            lista.AddRange(listaEstadosTurnos);

            cbEstadoTurno.DropDownStyle = ComboBoxStyle.DropDown;
            cbEstadoTurno.DataSource = lista;
            cbEstadoTurno.DisplayMember = nameof(ListadoEstadoTurno.Estado);
            cbEstadoTurno.ValueMember = nameof(ListadoEstadoTurno.Id_estado);
            cbEstadoTurno.SelectedIndex = 0;
        }

        // ======================== GUARDAR CAMBIOS ========================
        // Actualiza el objeto turno con los datos de los controles
        private void ActualizarTurno()
        {
            _turno.Paciente = cbPaciente.Text.Trim();
            _turno.Medico = cbMedico.Text.Trim();
            _turno.Procedimiento = cbProcedimiento.Text.Trim();
            _turno.Id_paciente = (int)(cbPaciente.SelectedValue ?? 0);
            _turno.Id_medico = (int)(cbMedico.SelectedValue ?? 0);
            _turno.Id_procedimiento = (int)(cbProcedimiento.SelectedValue ?? 0);
            _turno.Correo = txtCorreo.Text.Trim();

            _turno.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text)
                ? null
                : txtTelefono.Text.Trim();

            _turno.FechaTurno = dtpFechaTurno.Value;
            _turno.Observaciones = txtObservaciones.Text.Trim();
            _turno.Estado = cbEstadoTurno.SelectedIndex > 0
                ? ((ListadoEstadoTurno)cbEstadoTurno.SelectedItem).Estado
                : _turno.Estado;
        }


        // Guarda los cambios del turno en la base de datos
        private void GuardarCambios()
        {
            TurnoService turnoService = new TurnoService();
            turnoService.ActualizarTurno(_turno.Id_turno, _turno);
        }

        // ======================== EVENTOS BOTONES ========================
        // Botón Modificar/Guardar
        private void BtnModificar_Click(object sender, EventArgs e)
        {
            if (!_modoEdicion)
            {
                // Pasar a edición
                ToggleEdicion(true);
            }
            else
            {
                // Mensaje de confirmación
                var confirmacion = MessageBox.Show(
                    "¿Estás seguro de que deseas guardar este turno?",
                    "Confirmar guardado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirmacion != DialogResult.Yes)
                    return;

                // alidar datos alineado con RegistrarTurno
                if (!ValidarDatosTurno(out var msg))
                {
                    MessageBox.Show(msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Guardar cambios
                ActualizarTurno();
                GuardarCambios();
                ConfigurarUISoloLectura();

                MessageBox.Show("Cambios guardados.", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Botón Eliminar
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            TurnoService turnoService = new TurnoService();
            var dr = MessageBox.Show("¿Eliminar este turno?",
                                     "Confirmación",
                                     MessageBoxButtons.YesNo,
                                     MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                turnoService.EliminarTurno(_turno.Id_turno);
                CancelarVisualizacionSolicitada?.Invoke(this, EventArgs.Empty);
            }            
        }

        // Botón Volver
        private void BtnVolver_Click(object sender, EventArgs e)
        {
            CancelarVisualizacionSolicitada?.Invoke(this, EventArgs.Empty);
        }

        // ======================== VALIDACIONES ========================
        private bool ValidarDatosTurno(out string error)
        {
            error = null;

            // Paciente obligatorio
            if (cbPaciente.SelectedValue == null)
            {
                error = "Debe seleccionar un paciente.";
                return false;
            }

            // Médico obligatorio
            if (cbMedico.SelectedValue == null)
            {
                error = "Debe seleccionar un médico.";
                return false;
            }

            // Procedimiento obligatorio
            if (cbProcedimiento.SelectedValue == null)
            {
                error = "Debe seleccionar un procedimiento.";
                return false;
            }

            // Correo obligatorio + formato
            var correo = txtCorreo.Text.Trim();
            if (string.IsNullOrWhiteSpace(correo))
            {
                error = "El correo es obligatorio.";
                return false;
            }
            if (!Regex.IsMatch(correo, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                error = "El formato del correo no es válido.";
                return false;
            }

            // Observaciones obligatorias (igual que en registrar turno)
            var obs = txtObservaciones.Text.Trim();
            if (string.IsNullOrWhiteSpace(obs))
            {
                error = "Las observaciones son obligatorias.";
                return false;
            }
            if (obs.Length > 200)
            {
                error = "Las observaciones no pueden superar 200 caracteres.";
                return false;
            }

            // Fecha de turno: debe ser posterior a hoy (no hoy, no pasado)
            var fecha = dtpFechaTurno.Value.Date;
            if (fecha <= DateTime.Today)
            {
                error = "La fecha del turno debe ser posterior a hoy.";
                return false;
            }

            return true;
        }

    }

}
