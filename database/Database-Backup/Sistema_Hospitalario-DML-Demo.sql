-- =========================================================================
-- Datos de DEMOSTRACIÓN para Sistema_Hospitalario.
-- Ejecutar DESPUÉS de Sistema_Hospitalario-DDL.sql (sobre una base vacía,
-- por ejemplo tras correr Sistema_Hospitalario-Reset.sql).
--
-- A diferencia de Sistema_Hospitalario-DML.sql (mínimo, solo catálogos +
-- admin), este script carga datos de ejemplo en TODAS las tablas:
-- especialidades, médicos, pacientes, teléfonos, habitaciones, camas,
-- internaciones, turnos, consultas y un usuario por cada rol.
--
-- Los IDs generados por IDENTITY se capturan con OUTPUT en variables de
-- tabla, así que el script funciona sin importar en qué número arrancan
-- los contadores.
-- =========================================================================

USE [Sistema_Hospitalario];
GO

BEGIN TRANSACTION;
GO

-- --------------------------------------------------------------------
-- 1) Catálogos
-- --------------------------------------------------------------------

INSERT INTO rol (nombre) VALUES
    ('administrador'),
    ('medico'),
    ('administrativo'),
    ('gerente');

INSERT INTO estado_usuario (nombre) VALUES
    ('Activo'),
    ('Deshabilitado');

INSERT INTO estado_paciente (nombre) VALUES
    ('Activo'),
    ('Internado'),
    ('Alta');

-- Los 5 valores admitidos por CK_estado_cama_dispon
INSERT INTO estado_cama (disponibilidad) VALUES
    ('Disponible'),
    ('Ocupada'),
    ('Mantenimiento'),
    ('Limpieza'),
    ('Fuera de servicio');

INSERT INTO estado_turno (nombre) VALUES
    ('Pendiente'),
    ('Atendido'),
    ('Cancelado');

INSERT INTO especialidad (nombre) VALUES
    ('Clínica Médica'),
    ('Pediatría'),
    ('Cardiología'),
    ('Traumatología'),
    ('Neurología'),
    ('Ginecología');

INSERT INTO tipo_habitacion (nombre) VALUES
    ('Individual'),
    ('Compartida'),
    ('Terapia Intensiva');

-- Un procedimiento por cada especialidad (el filtro de médicos en la
-- pantalla de turnos compara el nombre del procedimiento contra el
-- nombre de la especialidad del médico) + una "Consulta" genérica que
-- muestra todos los médicos sin filtrar.
INSERT INTO procedimiento (nombre) VALUES
    ('Consulta'),
    ('Clínica Médica'),
    ('Pediatría'),
    ('Cardiología'),
    ('Traumatología'),
    ('Neurología'),
    ('Ginecología');
GO

-- --------------------------------------------------------------------
-- 2) Médicos (uno por especialidad)
-- --------------------------------------------------------------------

DECLARE @Medicos TABLE (Etiqueta varchar(50), id_medico int);

INSERT INTO medico (nombre, apellido, DNI, direccion, matricula, correo_electronico, id_especialidad)
OUTPUT 'Dra. Ferrari (Clínica Médica)', INSERTED.id_medico INTO @Medicos(Etiqueta, id_medico)
VALUES ('Lucía', 'Ferrari', 28111222, 'Av. Rivadavia 1200', 'MP-1001', 'lferrari@clinicks.local',
        (SELECT id_especialidad FROM especialidad WHERE nombre = 'Clínica Médica'));

INSERT INTO medico (nombre, apellido, DNI, direccion, matricula, correo_electronico, id_especialidad)
OUTPUT 'Dr. Gómez (Pediatría)', INSERTED.id_medico INTO @Medicos(Etiqueta, id_medico)
VALUES ('Martín', 'Gómez', 27555333, 'Calle San Martín 450', 'MP-1002', 'mgomez@clinicks.local',
        (SELECT id_especialidad FROM especialidad WHERE nombre = 'Pediatría'));

INSERT INTO medico (nombre, apellido, DNI, direccion, matricula, correo_electronico, id_especialidad)
OUTPUT 'Dra. Celeste Acevedo (Cardiología)', INSERTED.id_medico INTO @Medicos(Etiqueta, id_medico)
VALUES ('Celeste', 'Acevedo', 30111222, 'Belgrano 780', 'MP-1003', 'cacevedo@clinicks.local',
        (SELECT id_especialidad FROM especialidad WHERE nombre = 'Cardiología'));

INSERT INTO medico (nombre, apellido, DNI, direccion, matricula, correo_electronico, id_especialidad)
OUTPUT 'Dr. Torres (Traumatología)', INSERTED.id_medico INTO @Medicos(Etiqueta, id_medico)
VALUES ('Federico', 'Torres', 26999888, 'Mitre 90', 'MP-1004', 'ftorres@clinicks.local',
        (SELECT id_especialidad FROM especialidad WHERE nombre = 'Traumatología'));

INSERT INTO medico (nombre, apellido, DNI, direccion, matricula, correo_electronico, id_especialidad)
OUTPUT 'Dra. Ibáñez (Neurología)', INSERTED.id_medico INTO @Medicos(Etiqueta, id_medico)
VALUES ('Valeria', 'Ibáñez', 29444555, 'Sarmiento 233', 'MP-1005', 'vibanez@clinicks.local',
        (SELECT id_especialidad FROM especialidad WHERE nombre = 'Neurología'));

INSERT INTO medico (nombre, apellido, DNI, direccion, matricula, correo_electronico, id_especialidad)
OUTPUT 'Dra. Núñez (Ginecología)', INSERTED.id_medico INTO @Medicos(Etiqueta, id_medico)
VALUES ('Romina', 'Núñez', 25777666, 'Alberdi 1010', 'MP-1006', 'rnunez@clinicks.local',
        (SELECT id_especialidad FROM especialidad WHERE nombre = 'Ginecología'));

-- --------------------------------------------------------------------
-- 3) Pacientes + teléfonos
-- --------------------------------------------------------------------

DECLARE @Pacientes TABLE (Etiqueta varchar(50), id_paciente int);

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Juan Pérez', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (31222333, 'Juan', 'Pérez', '1985-04-12', NULL, 'Av. Libertad 100', 'juan.perez@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Ana García', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (32444555, 'Ana', 'García', '1990-08-23', 'Alergia a la penicilina', 'Independencia 250', 'ana.garcia@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Carlos Ramírez', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (33666777, 'Carlos', 'Ramírez', '1978-01-05', NULL, '9 de Julio 340', 'carlos.ramirez@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Internado'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'María López', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (34888999, 'María', 'López', '1995-11-30', NULL, 'Colón 560', 'maria.lopez@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Diego Fernández', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (35111000, 'Diego', 'Fernández', '1982-06-18', 'Diabético tipo 2', 'Moreno 780', 'diego.fernandez@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Internado'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Lucía Martínez', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (36222111, 'Lucía', 'Martínez', '2001-03-09', NULL, 'Rivadavia 900', 'lucia.martinez@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Roberto Sosa', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (37333222, 'Roberto', 'Sosa', '1970-09-14', NULL, 'San Juan 120', 'roberto.sosa@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Alta'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Sofía Herrera', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (38444333, 'Sofía', 'Herrera', '1999-12-01', NULL, 'Entre Ríos 45', 'sofia.herrera@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Pablo Díaz', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (39555444, 'Pablo', 'Díaz', '1988-07-22', NULL, 'Urquiza 670', 'pablo.diaz@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

INSERT INTO paciente (dni, nombre, apellido, fecha_nacimiento, observaciones, direccion, correo_electronico, id_estado_paciente, fecha_registracion)
OUTPUT 'Valentina Castro', INSERTED.id_paciente INTO @Pacientes(Etiqueta, id_paciente)
VALUES (40666555, 'Valentina', 'Castro', '1993-02-17', NULL, 'Pellegrini 310', 'valentina.castro@mail.com',
        (SELECT id_estado_paciente FROM estado_paciente WHERE nombre = 'Activo'), GETDATE());

-- Un teléfono por paciente
INSERT INTO telefono (numero_telefono, id_paciente)
SELECT '11' + RIGHT('0000000' + CAST(1000000 + id_paciente AS varchar(10)), 8), id_paciente
FROM @Pacientes;

-- --------------------------------------------------------------------
-- 4) Habitaciones + camas
-- --------------------------------------------------------------------

DECLARE @Habitaciones TABLE (Etiqueta varchar(50), nro_habitacion int);

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 1 - Individual A', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (1, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Individual'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 1 - Individual B', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (1, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Individual'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 2 - Compartida A', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (2, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Compartida'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 2 - Compartida B', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (2, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Compartida'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 3 - Terapia Intensiva A', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (3, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Terapia Intensiva'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 3 - Terapia Intensiva B', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (3, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Terapia Intensiva'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 0 - Individual C', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (0, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Individual'));

INSERT INTO habitacion (nro_piso, id_tipo_habitacion)
OUTPUT 'Piso 4 - Compartida C', INSERTED.nro_habitacion INTO @Habitaciones(Etiqueta, nro_habitacion)
VALUES (4, (SELECT id_tipo_habitacion FROM tipo_habitacion WHERE nombre = 'Compartida'));

-- Dos camas por habitación (una Disponible, otra Ocupada)
DECLARE @Camas TABLE (Etiqueta varchar(50), id_cama int, nro_habitacion int);

INSERT INTO cama (nro_habitacion, id_estado_cama)
OUTPUT 'Cama A', INSERTED.id_cama, INSERTED.nro_habitacion INTO @Camas(Etiqueta, id_cama, nro_habitacion)
SELECT h.nro_habitacion, (SELECT id_estado_cama FROM estado_cama WHERE disponibilidad = 'Disponible')
FROM @Habitaciones h;

INSERT INTO cama (nro_habitacion, id_estado_cama)
OUTPUT 'Cama B', INSERTED.id_cama, INSERTED.nro_habitacion INTO @Camas(Etiqueta, id_cama, nro_habitacion)
SELECT h.nro_habitacion, (SELECT id_estado_cama FROM estado_cama WHERE disponibilidad = 'Ocupada')
FROM @Habitaciones h;

-- --------------------------------------------------------------------
-- 5) Internaciones (usa las camas "B" que quedaron Ocupada)
-- --------------------------------------------------------------------

INSERT INTO internacion (fecha_inicio, fecha_fin, motivo, id_cama, nro_habitacion, id_paciente, id_medico, id_procedimiento)
SELECT DATEADD(day, -5, GETDATE()), NULL,
       'Internación por control clínico',
       c.id_cama, c.nro_habitacion,
       (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Carlos Ramírez'),
       (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ferrari (Clínica Médica)'),
       (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Clínica Médica')
FROM @Camas c WHERE c.Etiqueta = 'Cama B' AND c.nro_habitacion = (SELECT MIN(nro_habitacion) FROM @Habitaciones);

INSERT INTO internacion (fecha_inicio, fecha_fin, motivo, id_cama, nro_habitacion, id_paciente, id_medico, id_procedimiento)
SELECT DATEADD(day, -10, GETDATE()), DATEADD(day, -2, GETDATE()),
       'Internación por traumatismo, ya con alta',
       c.id_cama, c.nro_habitacion,
       (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Roberto Sosa'),
       (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dr. Torres (Traumatología)'),
       (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Traumatología')
FROM @Camas c
JOIN @Habitaciones h ON h.nro_habitacion = c.nro_habitacion
WHERE c.Etiqueta = 'Cama B' AND h.Etiqueta = 'Piso 2 - Compartida A';

INSERT INTO internacion (fecha_inicio, fecha_fin, motivo, id_cama, nro_habitacion, id_paciente, id_medico, id_procedimiento)
SELECT DATEADD(day, -3, GETDATE()), NULL,
       'Internación por control diabetológico',
       c.id_cama, c.nro_habitacion,
       (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Diego Fernández'),
       (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ferrari (Clínica Médica)'),
       (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Clínica Médica')
FROM @Camas c
JOIN @Habitaciones h ON h.nro_habitacion = c.nro_habitacion
WHERE c.Etiqueta = 'Cama B' AND h.Etiqueta = 'Piso 3 - Terapia Intensiva A';

-- --------------------------------------------------------------------
-- 6) Turnos (fechas futuras relativas a hoy, exigido por CK_turno_no_pasado)
-- --------------------------------------------------------------------

INSERT INTO turno (fecha_turno, fecha_registracion, id_procedimiento, id_paciente, id_medico, id_estado_turno, telefono, correo_electronico, motivo)
VALUES
    (DATEADD(day, 3, GETDATE()), GETDATE(),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Cardiología'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Juan Pérez'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Celeste Acevedo (Cardiología)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Pendiente'),
     '1145551234', 'juan.perez@mail.com', 'Control de rutina'),

    (DATEADD(day, 5, GETDATE()), GETDATE(),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Pediatría'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Sofía Herrera'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dr. Gómez (Pediatría)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Pendiente'),
     '1145551235', 'sofia.herrera@mail.com', 'Consulta pediátrica'),

    (DATEADD(day, 7, GETDATE()), GETDATE(),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Traumatología'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Pablo Díaz'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dr. Torres (Traumatología)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Pendiente'),
     '1145551236', 'pablo.diaz@mail.com', 'Dolor lumbar'),

    (DATEADD(day, 2, GETDATE()), GETDATE(),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Consulta'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Valentina Castro'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ferrari (Clínica Médica)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Pendiente'),
     '1145551237', 'valentina.castro@mail.com', 'Consulta general'),

    (DATEADD(day, 10, GETDATE()), GETDATE(),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Neurología'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'María López'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ibáñez (Neurología)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Pendiente'),
     '1145551238', 'maria.lopez@mail.com', 'Cefaleas frecuentes'),

    (DATEADD(day, 14, GETDATE()), GETDATE(),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Ginecología'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Lucía Martínez'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Núñez (Ginecología)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Pendiente'),
     '1145551239', 'lucia.martinez@mail.com', 'Control anual'),

    (DATEADD(day, 6, GETDATE()), DATEADD(day, -20, GETDATE()),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Consulta'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Ana García'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ferrari (Clínica Médica)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Atendido'),
     '1145551240', 'ana.garcia@mail.com', 'Consulta ya atendida (histórico)'),

    (DATEADD(day, 8, GETDATE()), DATEADD(day, -15, GETDATE()),
     (SELECT id_procedimiento FROM procedimiento WHERE nombre = 'Traumatología'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Roberto Sosa'),
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dr. Torres (Traumatología)'),
     (SELECT id_estado_turno FROM estado_turno WHERE nombre = 'Cancelado'),
     '1145551241', 'roberto.sosa@mail.com', 'Turno cancelado por el paciente');

-- Nota: CK_turno_no_pasado exige fecha_turno >= GETDATE(), por eso incluso
-- los turnos "Atendido"/"Cancelado" de ejemplo llevan fecha futura (la
-- app no distingue histórico real; es solo para tener variedad de
-- estados en la demo).

-- --------------------------------------------------------------------
-- 7) Consultas médicas (historial clínico)
-- --------------------------------------------------------------------

INSERT INTO Consulta (motivo, diagnostico, tratamiento, id_medico, id_paciente, fecha_consulta)
VALUES
    ('Dolor torácico leve', 'Sin hallazgos patológicos en ECG', 'Reposo y control en 15 días',
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Celeste Acevedo (Cardiología)'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Juan Pérez'),
     DATEADD(day, -30, GETDATE())),

    ('Control pediátrico de rutina', 'Desarrollo acorde a la edad', 'Continuar esquema de vacunación',
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dr. Gómez (Pediatría)'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Sofía Herrera'),
     DATEADD(day, -25, GETDATE())),

    ('Dolor lumbar tras esfuerzo físico', 'Contractura muscular', 'Antiinflamatorios y kinesiología',
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dr. Torres (Traumatología)'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Pablo Díaz'),
     DATEADD(day, -18, GETDATE())),

    ('Cefaleas recurrentes', 'Migraña sin aura', 'Analgésicos y seguimiento neurológico',
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ibáñez (Neurología)'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'María López'),
     DATEADD(day, -12, GETDATE())),

    ('Control ginecológico anual', 'Sin hallazgos relevantes', 'Control en 12 meses',
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Núñez (Ginecología)'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Lucía Martínez'),
     DATEADD(day, -7, GETDATE())),

    ('Seguimiento por internación clínica', 'Evolución favorable', 'Alta médica progresiva',
     (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ferrari (Clínica Médica)'),
     (SELECT id_paciente FROM @Pacientes WHERE Etiqueta = 'Carlos Ramírez'),
     DATEADD(day, -4, GETDATE()));

-- --------------------------------------------------------------------
-- 8) Un usuario por cada rol, mismo esquema que "admin"/"admin1234":
--    username = nombre del rol, password = <rol>1234.
--    El hash se guarda igual que en Sistema_Hospitalario-DML.sql: SHA-256
--    hexadecimal en minúsculas (formato legacy). PasswordHasher lo migra
--    automáticamente a PBKDF2 con salt la primera vez que cada usuario
--    inicia sesión.
-- --------------------------------------------------------------------

INSERT INTO usuario (username, password, nombre, apellido, email, id_estado_usuario, id_rol, id_medico)
VALUES (
    'admin',
    LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256', 'admin1234'), 2)),
    'Administrador', 'Sistema', 'admin@clinicks.local',
    (SELECT id_estado_usuario FROM estado_usuario WHERE nombre = 'Activo'),
    (SELECT id_rol FROM rol WHERE nombre = 'administrador'),
    NULL
);

INSERT INTO usuario (username, password, nombre, apellido, email, id_estado_usuario, id_rol, id_medico)
VALUES (
    'medico',
    LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256', 'medico1234'), 2)),
    'Lucía', 'Ferrari', 'lferrari@clinicks.local',
    (SELECT id_estado_usuario FROM estado_usuario WHERE nombre = 'Activo'),
    (SELECT id_rol FROM rol WHERE nombre = 'medico'),
    (SELECT id_medico FROM @Medicos WHERE Etiqueta = 'Dra. Ferrari (Clínica Médica)')
);

INSERT INTO usuario (username, password, nombre, apellido, email, id_estado_usuario, id_rol, id_medico)
VALUES (
    'administrativo',
    LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256', 'administrativo1234'), 2)),
    'Recepción', 'Administrativa', 'administrativo@clinicks.local',
    (SELECT id_estado_usuario FROM estado_usuario WHERE nombre = 'Activo'),
    (SELECT id_rol FROM rol WHERE nombre = 'administrativo'),
    NULL
);

INSERT INTO usuario (username, password, nombre, apellido, email, id_estado_usuario, id_rol, id_medico)
VALUES (
    'gerente',
    LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256', 'gerente1234'), 2)),
    'Gerencia', 'General', 'gerente@clinicks.local',
    (SELECT id_estado_usuario FROM estado_usuario WHERE nombre = 'Activo'),
    (SELECT id_rol FROM rol WHERE nombre = 'gerente'),
    NULL
);

COMMIT TRANSACTION;
GO

-- --------------------------------------------------------------------
-- Credenciales creadas por este script:
--   admin           / admin1234           (rol administrador)
--   medico          / medico1234          (rol medico, vinculado a Dra. Ferrari)
--   administrativo  / administrativo1234  (rol administrativo)
--   gerente         / gerente1234         (rol gerente)
-- --------------------------------------------------------------------
