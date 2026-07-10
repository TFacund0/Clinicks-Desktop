-- ==========================================================================
-- Sistema_Hospitalario - Script de reseteo de datos
-- ==========================================================================
-- Motor: SQL Server
-- Objetivo: vaciar todas las tablas de datos y reiniciar los IDENTITY,
--           para poder re-ejecutar Sistema_Hospitalario-DML.sql desde cero.
--
-- Por qué DELETE y no TRUNCATE:
-- Todas las FK del esquema (ver Sistema_Hospitalario-DDL.sql) están
-- definidas como ON DELETE NO ACTION / ON UPDATE NO ACTION (sin CASCADE).
-- SQL Server no permite TRUNCATE sobre una tabla referenciada por una FK,
-- sin importar si la tabla hija está vacía. Como casi todas las tablas
-- "padre" de este esquema tienen al menos una FK apuntándoles, TRUNCATE
-- fallaría. Por eso se usa DELETE en el orden correcto (hijas -> padres)
-- seguido de DBCC CHECKIDENT para reiniciar el contador de IDENTITY.
--
-- Tablas que este script vacía (18 tablas de datos de la app):
--   Consulta, turno, internacion, telefono, usuario, cama,
--   subespecialidad, medico, paciente, habitacion,
--   especialidad, tipo_habitacion, procedimiento, rol,
--   estado_usuario, estado_paciente, estado_cama, estado_turno
--
-- Tabla NO tocada: [sysdiagrams] (tabla de sistema de SSMS, no es dato
--   de la aplicación; borrarla no aporta nada y podría romper diagramas
--   guardados en el Management Studio).
-- ==========================================================================

USE [Sistema_Hospitalario];
GO

BEGIN TRANSACTION;
GO

-- --------------------------------------------------------------------
-- 1) Borrado en orden de dependencia (hijas primero, padres al final)
-- --------------------------------------------------------------------

DELETE FROM [dbo].[Consulta];
DELETE FROM [dbo].[turno];
DELETE FROM [dbo].[internacion];
DELETE FROM [dbo].[telefono];
DELETE FROM [dbo].[usuario];
DELETE FROM [dbo].[cama];
DELETE FROM [dbo].[subespecialidad];
DELETE FROM [dbo].[medico];
DELETE FROM [dbo].[paciente];
DELETE FROM [dbo].[habitacion];
DELETE FROM [dbo].[especialidad];
DELETE FROM [dbo].[tipo_habitacion];
DELETE FROM [dbo].[procedimiento];
DELETE FROM [dbo].[rol];
DELETE FROM [dbo].[estado_usuario];
DELETE FROM [dbo].[estado_paciente];
DELETE FROM [dbo].[estado_cama];
DELETE FROM [dbo].[estado_turno];
GO

-- --------------------------------------------------------------------
-- 2) Reinicio de los contadores IDENTITY (solo tablas que tienen IDENTITY)
--    RESEED a 0 hace que el próximo INSERT use 1, replicando el estado
--    original tras correr la DDL.
-- --------------------------------------------------------------------

DBCC CHECKIDENT ('[dbo].[Consulta]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[turno]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[internacion]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[telefono]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[usuario]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[cama]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[subespecialidad]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[medico]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[paciente]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[habitacion]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[especialidad]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[tipo_habitacion]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[procedimiento]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[rol]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[estado_usuario]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[estado_paciente]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[estado_cama]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[estado_turno]', RESEED, 0);
GO

COMMIT TRANSACTION;
GO

-- --------------------------------------------------------------------
-- 3) Verificación rápida: todas las tablas deben quedar en 0 filas
-- --------------------------------------------------------------------

SELECT 'Consulta' AS tabla, COUNT(*) AS filas FROM [dbo].[Consulta]
UNION ALL SELECT 'turno', COUNT(*) FROM [dbo].[turno]
UNION ALL SELECT 'internacion', COUNT(*) FROM [dbo].[internacion]
UNION ALL SELECT 'telefono', COUNT(*) FROM [dbo].[telefono]
UNION ALL SELECT 'usuario', COUNT(*) FROM [dbo].[usuario]
UNION ALL SELECT 'cama', COUNT(*) FROM [dbo].[cama]
UNION ALL SELECT 'subespecialidad', COUNT(*) FROM [dbo].[subespecialidad]
UNION ALL SELECT 'medico', COUNT(*) FROM [dbo].[medico]
UNION ALL SELECT 'paciente', COUNT(*) FROM [dbo].[paciente]
UNION ALL SELECT 'habitacion', COUNT(*) FROM [dbo].[habitacion]
UNION ALL SELECT 'especialidad', COUNT(*) FROM [dbo].[especialidad]
UNION ALL SELECT 'tipo_habitacion', COUNT(*) FROM [dbo].[tipo_habitacion]
UNION ALL SELECT 'procedimiento', COUNT(*) FROM [dbo].[procedimiento]
UNION ALL SELECT 'rol', COUNT(*) FROM [dbo].[rol]
UNION ALL SELECT 'estado_usuario', COUNT(*) FROM [dbo].[estado_usuario]
UNION ALL SELECT 'estado_paciente', COUNT(*) FROM [dbo].[estado_paciente]
UNION ALL SELECT 'estado_cama', COUNT(*) FROM [dbo].[estado_cama]
UNION ALL SELECT 'estado_turno', COUNT(*) FROM [dbo].[estado_turno];
GO

-- --------------------------------------------------------------------
-- Ahora podés ejecutar Sistema_Hospitalario-DML.sql para recargar los
-- datos iniciales (incluye el usuario admin con password_hash SHA-256
-- legacy que la app migra automáticamente a PBKDF2 en el primer login).
-- --------------------------------------------------------------------
