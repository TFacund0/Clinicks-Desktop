
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/14/2025 17:58:06
-- Generated from EDMX file: C:\Users\USUARIO\Desktop\LSI\TERCER A�O\Segundo cuatrimestre\Taller de Programaci�n II\Proyecto\Sistema Hospitalario\Sistema Hospitalario\CapaDatos\Sistema_Hospitalario_BDD.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Sistema_Hospitalario];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[fk_cama_estado_cama]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[cama] DROP CONSTRAINT [fk_cama_estado_cama];
GO
IF OBJECT_ID(N'[dbo].[fk_cama_habitacion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[cama] DROP CONSTRAINT [fk_cama_habitacion];
GO
IF OBJECT_ID(N'[dbo].[FK_Consulta_Medico]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Consulta] DROP CONSTRAINT [FK_Consulta_Medico];
GO
IF OBJECT_ID(N'[dbo].[FK_Consulta_Paciente]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Consulta] DROP CONSTRAINT [FK_Consulta_Paciente];
GO
IF OBJECT_ID(N'[dbo].[fk_habitacion_tipo_habitacion]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[habitacion] DROP CONSTRAINT [fk_habitacion_tipo_habitacion];
GO
IF OBJECT_ID(N'[dbo].[fk_internacion_cama]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[internacion] DROP CONSTRAINT [fk_internacion_cama];
GO
IF OBJECT_ID(N'[dbo].[fk_internacion_medico]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[internacion] DROP CONSTRAINT [fk_internacion_medico];
GO
IF OBJECT_ID(N'[dbo].[fk_internacion_paciente]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[internacion] DROP CONSTRAINT [fk_internacion_paciente];
GO
IF OBJECT_ID(N'[dbo].[fk_internacion_procedimiento]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[internacion] DROP CONSTRAINT [fk_internacion_procedimiento];
GO
IF OBJECT_ID(N'[dbo].[fk_medico_especialidad]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[medico] DROP CONSTRAINT [fk_medico_especialidad];
GO
IF OBJECT_ID(N'[dbo].[fk_paciente_estado_paciente]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[paciente] DROP CONSTRAINT [fk_paciente_estado_paciente];
GO
IF OBJECT_ID(N'[dbo].[fk_subespecialidad_especialidad]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[subespecialidad] DROP CONSTRAINT [fk_subespecialidad_especialidad];
GO
IF OBJECT_ID(N'[dbo].[fk_telefono_paciente]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[telefono] DROP CONSTRAINT [fk_telefono_paciente];
GO
IF OBJECT_ID(N'[dbo].[fk_turno_estado_turno]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[turno] DROP CONSTRAINT [fk_turno_estado_turno];
GO
IF OBJECT_ID(N'[dbo].[fk_turno_medico]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[turno] DROP CONSTRAINT [fk_turno_medico];
GO
IF OBJECT_ID(N'[dbo].[fk_turno_paciente]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[turno] DROP CONSTRAINT [fk_turno_paciente];
GO
IF OBJECT_ID(N'[dbo].[fk_turno_procedimiento]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[turno] DROP CONSTRAINT [fk_turno_procedimiento];
GO
IF OBJECT_ID(N'[dbo].[fk_usuario_estado_usuario]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[usuario] DROP CONSTRAINT [fk_usuario_estado_usuario];
GO
IF OBJECT_ID(N'[dbo].[FK_Usuario_Medico]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[usuario] DROP CONSTRAINT [FK_Usuario_Medico];
GO
IF OBJECT_ID(N'[dbo].[fk_usuario_rol]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[usuario] DROP CONSTRAINT [fk_usuario_rol];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[cama]', 'U') IS NOT NULL
    DROP TABLE [dbo].[cama];
GO
IF OBJECT_ID(N'[dbo].[Consulta]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Consulta];
GO
IF OBJECT_ID(N'[dbo].[especialidad]', 'U') IS NOT NULL
    DROP TABLE [dbo].[especialidad];
GO
IF OBJECT_ID(N'[dbo].[estado_cama]', 'U') IS NOT NULL
    DROP TABLE [dbo].[estado_cama];
GO
IF OBJECT_ID(N'[dbo].[estado_paciente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[estado_paciente];
GO
IF OBJECT_ID(N'[dbo].[estado_turno]', 'U') IS NOT NULL
    DROP TABLE [dbo].[estado_turno];
GO
IF OBJECT_ID(N'[dbo].[estado_usuario]', 'U') IS NOT NULL
    DROP TABLE [dbo].[estado_usuario];
GO
IF OBJECT_ID(N'[dbo].[habitacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[habitacion];
GO
IF OBJECT_ID(N'[dbo].[internacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[internacion];
GO
IF OBJECT_ID(N'[dbo].[medico]', 'U') IS NOT NULL
    DROP TABLE [dbo].[medico];
GO
IF OBJECT_ID(N'[dbo].[paciente]', 'U') IS NOT NULL
    DROP TABLE [dbo].[paciente];
GO
IF OBJECT_ID(N'[dbo].[procedimiento]', 'U') IS NOT NULL
    DROP TABLE [dbo].[procedimiento];
GO
IF OBJECT_ID(N'[dbo].[rol]', 'U') IS NOT NULL
    DROP TABLE [dbo].[rol];
GO
IF OBJECT_ID(N'[dbo].[subespecialidad]', 'U') IS NOT NULL
    DROP TABLE [dbo].[subespecialidad];
GO
IF OBJECT_ID(N'[dbo].[sysdiagrams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[sysdiagrams];
GO
IF OBJECT_ID(N'[dbo].[telefono]', 'U') IS NOT NULL
    DROP TABLE [dbo].[telefono];
GO
IF OBJECT_ID(N'[dbo].[tipo_habitacion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tipo_habitacion];
GO
IF OBJECT_ID(N'[dbo].[turno]', 'U') IS NOT NULL
    DROP TABLE [dbo].[turno];
GO
IF OBJECT_ID(N'[dbo].[usuario]', 'U') IS NOT NULL
    DROP TABLE [dbo].[usuario];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'cama'
CREATE TABLE [dbo].[cama] (
    [id_cama] int IDENTITY(1,1) NOT NULL,
    [nro_habitacion] int  NOT NULL,
    [id_estado_cama] int  NOT NULL
);
GO

-- Creating table 'especialidad'
CREATE TABLE [dbo].[especialidad] (
    [id_especialidad] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(50)  NOT NULL
);
GO

-- Creating table 'estado_cama'
CREATE TABLE [dbo].[estado_cama] (
    [id_estado_cama] int IDENTITY(1,1) NOT NULL,
    [disponibilidad] varchar(30)  NOT NULL
);
GO

-- Creating table 'estado_paciente'
CREATE TABLE [dbo].[estado_paciente] (
    [id_estado_paciente] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(30)  NOT NULL
);
GO

-- Creating table 'estado_turno'
CREATE TABLE [dbo].[estado_turno] (
    [id_estado_turno] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(30)  NOT NULL
);
GO

-- Creating table 'estado_usuario'
CREATE TABLE [dbo].[estado_usuario] (
    [id_estado_usuario] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(30)  NOT NULL
);
GO

-- Creating table 'habitacion'
CREATE TABLE [dbo].[habitacion] (
    [nro_habitacion] int IDENTITY(1,1) NOT NULL,
    [nro_piso] int  NOT NULL,
    [id_tipo_habitacion] int  NOT NULL
);
GO

-- Creating table 'internacion'
CREATE TABLE [dbo].[internacion] (
    [id_internacion] int IDENTITY(1,1) NOT NULL,
    [fecha_inicio] datetime  NOT NULL,
    [fecha_fin] datetime  NULL,
    [motivo] varchar(200)  NOT NULL,
    [id_cama] int  NOT NULL,
    [nro_habitacion] int  NOT NULL,
    [id_paciente] int  NOT NULL,
    [id_medico] int  NOT NULL,
    [id_procedimiento] int  NOT NULL
);
GO

-- Creating table 'medico'
CREATE TABLE [dbo].[medico] (
    [id_medico] int IDENTITY(1,1) NOT NULL,
    [matricula] varchar(50)  NOT NULL,
    [nombre] varchar(50)  NOT NULL,
    [apellido] varchar(50)  NOT NULL,
    [direccion] varchar(150)  NOT NULL,
    [correo_electronico] varchar(150)  NOT NULL,
    [id_especialidad] int  NOT NULL,
    [DNI] int  NOT NULL
);
GO

-- Creating table 'paciente'
CREATE TABLE [dbo].[paciente] (
    [id_paciente] int IDENTITY(1,1) NOT NULL,
    [dni] int  NOT NULL,
    [nombre] varchar(50)  NOT NULL,
    [apellido] varchar(50)  NOT NULL,
    [fecha_nacimiento] datetime  NOT NULL,
    [observaciones] varchar(200)  NULL,
    [direccion] varchar(200)  NOT NULL,
    [correo_electronico] varchar(150)  NOT NULL,
    [id_estado_paciente] int  NOT NULL,
    [fecha_registracion] datetime  NOT NULL
);
GO

-- Creating table 'procedimiento'
CREATE TABLE [dbo].[procedimiento] (
    [id_procedimiento] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(100)  NOT NULL
);
GO

-- Creating table 'rol'
CREATE TABLE [dbo].[rol] (
    [id_rol] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(20)  NOT NULL
);
GO

-- Creating table 'subespecialidad'
CREATE TABLE [dbo].[subespecialidad] (
    [id_subespecialidad] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(30)  NOT NULL,
    [id_especialidad] int  NOT NULL
);
GO

-- Creating table 'sysdiagrams'
CREATE TABLE [dbo].[sysdiagrams] (
    [name] nvarchar(128)  NOT NULL,
    [principal_id] int  NOT NULL,
    [diagram_id] int IDENTITY(1,1) NOT NULL,
    [version] int  NULL,
    [definition] varbinary(max)  NULL
);
GO

-- Creating table 'telefono'
CREATE TABLE [dbo].[telefono] (
    [numero_telefono] varchar(30)  NOT NULL,
    [id_nroTelefono] int IDENTITY(1,1) NOT NULL,
    [id_paciente] int  NOT NULL
);
GO

-- Creating table 'tipo_habitacion'
CREATE TABLE [dbo].[tipo_habitacion] (
    [id_tipo_habitacion] int IDENTITY(1,1) NOT NULL,
    [nombre] varchar(30)  NOT NULL
);
GO

-- Creating table 'turno'
CREATE TABLE [dbo].[turno] (
    [id_turno] int IDENTITY(1,1) NOT NULL,
    [fecha_turno] datetime  NOT NULL,
    [fecha_registracion] datetime  NOT NULL,
    [id_procedimiento] int  NOT NULL,
    [id_paciente] int  NOT NULL,
    [id_medico] int  NOT NULL,
    [id_estado_turno] int  NOT NULL,
    [telefono] varchar(30)  NULL,
    [correo_electronico] varchar(150)  NULL,
    [motivo] varchar(200)  NULL
);
GO

-- Creating table 'usuario'
CREATE TABLE [dbo].[usuario] (
    [id_usuario] int IDENTITY(1,1) NOT NULL,
    [username] varchar(50)  NOT NULL,
    [password] varchar(200)  NOT NULL,
    [nombre] varchar(30)  NOT NULL,
    [apellido] varchar(30)  NOT NULL,
    [email] varchar(100)  NOT NULL,
    [id_estado_usuario] int  NOT NULL,
    [id_rol] int  NOT NULL,
    [id_medico] int  NULL
);
GO

-- Creating table 'Consulta'
CREATE TABLE [dbo].[Consulta] (
    [id_consulta] int IDENTITY(1,1) NOT NULL,
    [motivo] varchar(max)  NULL,
    [diagnostico] varchar(max)  NULL,
    [tratamiento] varchar(max)  NULL,
    [id_medico] int  NOT NULL,
    [id_paciente] int  NOT NULL,
    [fecha_consulta] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id_cama], [nro_habitacion] in table 'cama'
ALTER TABLE [dbo].[cama]
ADD CONSTRAINT [PK_cama]
    PRIMARY KEY CLUSTERED ([id_cama], [nro_habitacion] ASC);
GO

-- Creating primary key on [id_especialidad] in table 'especialidad'
ALTER TABLE [dbo].[especialidad]
ADD CONSTRAINT [PK_especialidad]
    PRIMARY KEY CLUSTERED ([id_especialidad] ASC);
GO

-- Creating primary key on [id_estado_cama] in table 'estado_cama'
ALTER TABLE [dbo].[estado_cama]
ADD CONSTRAINT [PK_estado_cama]
    PRIMARY KEY CLUSTERED ([id_estado_cama] ASC);
GO

-- Creating primary key on [id_estado_paciente] in table 'estado_paciente'
ALTER TABLE [dbo].[estado_paciente]
ADD CONSTRAINT [PK_estado_paciente]
    PRIMARY KEY CLUSTERED ([id_estado_paciente] ASC);
GO

-- Creating primary key on [id_estado_turno] in table 'estado_turno'
ALTER TABLE [dbo].[estado_turno]
ADD CONSTRAINT [PK_estado_turno]
    PRIMARY KEY CLUSTERED ([id_estado_turno] ASC);
GO

-- Creating primary key on [id_estado_usuario] in table 'estado_usuario'
ALTER TABLE [dbo].[estado_usuario]
ADD CONSTRAINT [PK_estado_usuario]
    PRIMARY KEY CLUSTERED ([id_estado_usuario] ASC);
GO

-- Creating primary key on [nro_habitacion] in table 'habitacion'
ALTER TABLE [dbo].[habitacion]
ADD CONSTRAINT [PK_habitacion]
    PRIMARY KEY CLUSTERED ([nro_habitacion] ASC);
GO

-- Creating primary key on [id_internacion] in table 'internacion'
ALTER TABLE [dbo].[internacion]
ADD CONSTRAINT [PK_internacion]
    PRIMARY KEY CLUSTERED ([id_internacion] ASC);
GO

-- Creating primary key on [id_medico] in table 'medico'
ALTER TABLE [dbo].[medico]
ADD CONSTRAINT [PK_medico]
    PRIMARY KEY CLUSTERED ([id_medico] ASC);
GO

-- Creating primary key on [id_paciente] in table 'paciente'
ALTER TABLE [dbo].[paciente]
ADD CONSTRAINT [PK_paciente]
    PRIMARY KEY CLUSTERED ([id_paciente] ASC);
GO

-- Creating primary key on [id_procedimiento] in table 'procedimiento'
ALTER TABLE [dbo].[procedimiento]
ADD CONSTRAINT [PK_procedimiento]
    PRIMARY KEY CLUSTERED ([id_procedimiento] ASC);
GO

-- Creating primary key on [id_rol] in table 'rol'
ALTER TABLE [dbo].[rol]
ADD CONSTRAINT [PK_rol]
    PRIMARY KEY CLUSTERED ([id_rol] ASC);
GO

-- Creating primary key on [id_subespecialidad], [id_especialidad] in table 'subespecialidad'
ALTER TABLE [dbo].[subespecialidad]
ADD CONSTRAINT [PK_subespecialidad]
    PRIMARY KEY CLUSTERED ([id_subespecialidad], [id_especialidad] ASC);
GO

-- Creating primary key on [diagram_id] in table 'sysdiagrams'
ALTER TABLE [dbo].[sysdiagrams]
ADD CONSTRAINT [PK_sysdiagrams]
    PRIMARY KEY CLUSTERED ([diagram_id] ASC);
GO

-- Creating primary key on [id_nroTelefono], [id_paciente] in table 'telefono'
ALTER TABLE [dbo].[telefono]
ADD CONSTRAINT [PK_telefono]
    PRIMARY KEY CLUSTERED ([id_nroTelefono], [id_paciente] ASC);
GO

-- Creating primary key on [id_tipo_habitacion] in table 'tipo_habitacion'
ALTER TABLE [dbo].[tipo_habitacion]
ADD CONSTRAINT [PK_tipo_habitacion]
    PRIMARY KEY CLUSTERED ([id_tipo_habitacion] ASC);
GO

-- Creating primary key on [id_turno] in table 'turno'
ALTER TABLE [dbo].[turno]
ADD CONSTRAINT [PK_turno]
    PRIMARY KEY CLUSTERED ([id_turno] ASC);
GO

-- Creating primary key on [id_usuario] in table 'usuario'
ALTER TABLE [dbo].[usuario]
ADD CONSTRAINT [PK_usuario]
    PRIMARY KEY CLUSTERED ([id_usuario] ASC);
GO

-- Creating primary key on [id_consulta] in table 'Consulta'
ALTER TABLE [dbo].[Consulta]
ADD CONSTRAINT [PK_Consulta]
    PRIMARY KEY CLUSTERED ([id_consulta] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [id_estado_cama] in table 'cama'
ALTER TABLE [dbo].[cama]
ADD CONSTRAINT [fk_cama_estado_cama]
    FOREIGN KEY ([id_estado_cama])
    REFERENCES [dbo].[estado_cama]
        ([id_estado_cama])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_cama_estado_cama'
CREATE INDEX [IX_fk_cama_estado_cama]
ON [dbo].[cama]
    ([id_estado_cama]);
GO

-- Creating foreign key on [nro_habitacion] in table 'cama'
ALTER TABLE [dbo].[cama]
ADD CONSTRAINT [fk_cama_habitacion]
    FOREIGN KEY ([nro_habitacion])
    REFERENCES [dbo].[habitacion]
        ([nro_habitacion])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_cama_habitacion'
CREATE INDEX [IX_fk_cama_habitacion]
ON [dbo].[cama]
    ([nro_habitacion]);
GO

-- Creating foreign key on [id_cama], [nro_habitacion] in table 'internacion'
ALTER TABLE [dbo].[internacion]
ADD CONSTRAINT [fk_internacion_cama]
    FOREIGN KEY ([id_cama], [nro_habitacion])
    REFERENCES [dbo].[cama]
        ([id_cama], [nro_habitacion])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_internacion_cama'
CREATE INDEX [IX_fk_internacion_cama]
ON [dbo].[internacion]
    ([id_cama], [nro_habitacion]);
GO

-- Creating foreign key on [id_especialidad] in table 'medico'
ALTER TABLE [dbo].[medico]
ADD CONSTRAINT [fk_medico_especialidad]
    FOREIGN KEY ([id_especialidad])
    REFERENCES [dbo].[especialidad]
        ([id_especialidad])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_medico_especialidad'
CREATE INDEX [IX_fk_medico_especialidad]
ON [dbo].[medico]
    ([id_especialidad]);
GO

-- Creating foreign key on [id_especialidad] in table 'subespecialidad'
ALTER TABLE [dbo].[subespecialidad]
ADD CONSTRAINT [fk_subespecialidad_especialidad]
    FOREIGN KEY ([id_especialidad])
    REFERENCES [dbo].[especialidad]
        ([id_especialidad])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_subespecialidad_especialidad'
CREATE INDEX [IX_fk_subespecialidad_especialidad]
ON [dbo].[subespecialidad]
    ([id_especialidad]);
GO

-- Creating foreign key on [id_estado_paciente] in table 'paciente'
ALTER TABLE [dbo].[paciente]
ADD CONSTRAINT [fk_paciente_estado_paciente]
    FOREIGN KEY ([id_estado_paciente])
    REFERENCES [dbo].[estado_paciente]
        ([id_estado_paciente])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_paciente_estado_paciente'
CREATE INDEX [IX_fk_paciente_estado_paciente]
ON [dbo].[paciente]
    ([id_estado_paciente]);
GO

-- Creating foreign key on [id_estado_turno] in table 'turno'
ALTER TABLE [dbo].[turno]
ADD CONSTRAINT [fk_turno_estado_turno]
    FOREIGN KEY ([id_estado_turno])
    REFERENCES [dbo].[estado_turno]
        ([id_estado_turno])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_turno_estado_turno'
CREATE INDEX [IX_fk_turno_estado_turno]
ON [dbo].[turno]
    ([id_estado_turno]);
GO

-- Creating foreign key on [id_estado_usuario] in table 'usuario'
ALTER TABLE [dbo].[usuario]
ADD CONSTRAINT [fk_usuario_estado_usuario]
    FOREIGN KEY ([id_estado_usuario])
    REFERENCES [dbo].[estado_usuario]
        ([id_estado_usuario])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_usuario_estado_usuario'
CREATE INDEX [IX_fk_usuario_estado_usuario]
ON [dbo].[usuario]
    ([id_estado_usuario]);
GO

-- Creating foreign key on [id_tipo_habitacion] in table 'habitacion'
ALTER TABLE [dbo].[habitacion]
ADD CONSTRAINT [fk_habitacion_tipo_habitacion]
    FOREIGN KEY ([id_tipo_habitacion])
    REFERENCES [dbo].[tipo_habitacion]
        ([id_tipo_habitacion])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_habitacion_tipo_habitacion'
CREATE INDEX [IX_fk_habitacion_tipo_habitacion]
ON [dbo].[habitacion]
    ([id_tipo_habitacion]);
GO

-- Creating foreign key on [id_medico] in table 'internacion'
ALTER TABLE [dbo].[internacion]
ADD CONSTRAINT [fk_internacion_medico]
    FOREIGN KEY ([id_medico])
    REFERENCES [dbo].[medico]
        ([id_medico])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_internacion_medico'
CREATE INDEX [IX_fk_internacion_medico]
ON [dbo].[internacion]
    ([id_medico]);
GO

-- Creating foreign key on [id_paciente] in table 'internacion'
ALTER TABLE [dbo].[internacion]
ADD CONSTRAINT [fk_internacion_paciente]
    FOREIGN KEY ([id_paciente])
    REFERENCES [dbo].[paciente]
        ([id_paciente])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_internacion_paciente'
CREATE INDEX [IX_fk_internacion_paciente]
ON [dbo].[internacion]
    ([id_paciente]);
GO

-- Creating foreign key on [id_procedimiento] in table 'internacion'
ALTER TABLE [dbo].[internacion]
ADD CONSTRAINT [fk_internacion_procedimiento]
    FOREIGN KEY ([id_procedimiento])
    REFERENCES [dbo].[procedimiento]
        ([id_procedimiento])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_internacion_procedimiento'
CREATE INDEX [IX_fk_internacion_procedimiento]
ON [dbo].[internacion]
    ([id_procedimiento]);
GO

-- Creating foreign key on [id_medico] in table 'turno'
ALTER TABLE [dbo].[turno]
ADD CONSTRAINT [fk_turno_medico]
    FOREIGN KEY ([id_medico])
    REFERENCES [dbo].[medico]
        ([id_medico])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_turno_medico'
CREATE INDEX [IX_fk_turno_medico]
ON [dbo].[turno]
    ([id_medico]);
GO

-- Creating foreign key on [id_paciente] in table 'telefono'
ALTER TABLE [dbo].[telefono]
ADD CONSTRAINT [fk_telefono_paciente]
    FOREIGN KEY ([id_paciente])
    REFERENCES [dbo].[paciente]
        ([id_paciente])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_telefono_paciente'
CREATE INDEX [IX_fk_telefono_paciente]
ON [dbo].[telefono]
    ([id_paciente]);
GO

-- Creating foreign key on [id_paciente] in table 'turno'
ALTER TABLE [dbo].[turno]
ADD CONSTRAINT [fk_turno_paciente]
    FOREIGN KEY ([id_paciente])
    REFERENCES [dbo].[paciente]
        ([id_paciente])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_turno_paciente'
CREATE INDEX [IX_fk_turno_paciente]
ON [dbo].[turno]
    ([id_paciente]);
GO

-- Creating foreign key on [id_procedimiento] in table 'turno'
ALTER TABLE [dbo].[turno]
ADD CONSTRAINT [fk_turno_procedimiento]
    FOREIGN KEY ([id_procedimiento])
    REFERENCES [dbo].[procedimiento]
        ([id_procedimiento])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_turno_procedimiento'
CREATE INDEX [IX_fk_turno_procedimiento]
ON [dbo].[turno]
    ([id_procedimiento]);
GO

-- Creating foreign key on [id_rol] in table 'usuario'
ALTER TABLE [dbo].[usuario]
ADD CONSTRAINT [fk_usuario_rol]
    FOREIGN KEY ([id_rol])
    REFERENCES [dbo].[rol]
        ([id_rol])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'fk_usuario_rol'
CREATE INDEX [IX_fk_usuario_rol]
ON [dbo].[usuario]
    ([id_rol]);
GO

-- Creating foreign key on [id_medico] in table 'usuario'
ALTER TABLE [dbo].[usuario]
ADD CONSTRAINT [FK_Usuario_Medico]
    FOREIGN KEY ([id_medico])
    REFERENCES [dbo].[medico]
        ([id_medico])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Usuario_Medico'
CREATE INDEX [IX_FK_Usuario_Medico]
ON [dbo].[usuario]
    ([id_medico]);
GO

-- Creating foreign key on [id_medico] in table 'Consulta'
ALTER TABLE [dbo].[Consulta]
ADD CONSTRAINT [FK_Consulta_Medico]
    FOREIGN KEY ([id_medico])
    REFERENCES [dbo].[medico]
        ([id_medico])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Consulta_Medico'
CREATE INDEX [IX_FK_Consulta_Medico]
ON [dbo].[Consulta]
    ([id_medico]);
GO

-- Creating foreign key on [id_paciente] in table 'Consulta'
ALTER TABLE [dbo].[Consulta]
ADD CONSTRAINT [FK_Consulta_Paciente]
    FOREIGN KEY ([id_paciente])
    REFERENCES [dbo].[paciente]
        ([id_paciente])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Consulta_Paciente'
CREATE INDEX [IX_FK_Consulta_Paciente]
ON [dbo].[Consulta]
    ([id_paciente]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------