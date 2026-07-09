-- =========================================================================
-- Datos iniciales para una instalación nueva del Sistema Hospitalario.
-- Ejecutar DESPUÉS de Sistema_Hospitalario-DDL.sql.
--
-- Siembra los catálogos que la aplicación necesita para funcionar (roles,
-- estados, especialidades, tipos de habitación) y crea un único usuario
-- administrador para el primer inicio de sesión. Los demás usuarios,
-- pacientes, médicos, turnos, etc. se cargan desde la propia aplicación
-- una vez logueado.
-- =========================================================================

-- Roles de usuario.
-- Los nombres deben coincidir exactamente (en minúscula) con los que
-- espera la capa de negocio (UsuarioService.ObtenerConteoUsuariosPorRol).
INSERT INTO rol (nombre) VALUES
    ('administrador'),  -- id_rol = 1
    ('medico'),         -- id_rol = 2
    ('administrativo'), -- id_rol = 3
    ('gerente');        -- id_rol = 4

-- Estados de una cuenta de usuario.
-- UsuarioService.ValidarCredenciales exige NombreEstado = 'Activo'
-- (comparación sin distinción de mayúsculas) para permitir el login.
INSERT INTO estado_usuario (nombre) VALUES
    ('Activo'),        -- id_estado_usuario = 1
    ('Deshabilitado'); -- id_estado_usuario = 2

-- Estados clínicos de un paciente (PacienteService).
INSERT INTO estado_paciente (nombre) VALUES
    ('Activo'),
    ('Internado'),
    ('Alta');

-- Estados de disponibilidad de una cama (CamaService / EstadisticasService).
INSERT INTO estado_cama (disponibilidad) VALUES
    ('Disponible'),
    ('Ocupada'),
    ('Mantenimiento');

-- Estados de un turno médico (TurnoService).
INSERT INTO estado_turno (nombre) VALUES
    ('Pendiente'),
    ('Atendido'),
    ('Cancelado');

-- Catálogo mínimo de especialidades médicas, editable luego desde la UI.
INSERT INTO especialidad (nombre) VALUES
    ('Clínica Médica'),
    ('Pediatría'),
    ('Cardiología'),
    ('Traumatología');

-- Catálogo mínimo de tipos de habitación, editable luego desde la UI.
INSERT INTO tipo_habitacion (nombre) VALUES
    ('Individual'),
    ('Compartida'),
    ('Terapia Intensiva');
GO

-- Usuario administrador inicial.
-- El hash se guarda como cadena hexadecimal en minúsculas, igual que la
-- calcula Sistema_Hospitalario.CapaNegocio.Seguridad.PasswordHasher en su
-- formato legacy SHA-256 (HashLegacySha256 usa byte.ToString("x2"), que es
-- minúscula). HASHBYTES por sí solo inserta el binario crudo reinterpretado
-- como texto, y CONVERT(..., 2) devuelve hex en MAYÚSCULAS: cualquiera de
-- los dos por separado produce un hash que no coincide byte a byte con el
-- que compara PasswordHasher.Verificar, y el login fallaría.
-- PasswordHasher migra este hash a PBKDF2 con salt automáticamente la
-- primera vez que el usuario inicia sesión.
INSERT INTO usuario (username, password, nombre, apellido, email, id_estado_usuario, id_rol, id_medico)
VALUES (
    'admin',
    LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256', 'admin1234'), 2)),
    'Administrador', 'Sistema', 'admin@clinicks.local', 1, 1, NULL
);
GO

-- Credenciales del usuario administrador creado arriba:
--   usuario:    admin
--   contraseña: admin1234
--
-- Desde ese usuario se pueden dar de alta médicos, recepcionistas y
-- gerentes utilizando las pantallas de administración del sistema.
