<div align="center">

# 🏥 Clinicks — Sistema de Gestión Hospitalaria

**Aplicación de escritorio en C# / WinForms con arquitectura en capas, construida sobre .NET Framework y Entity Framework 6, para administrar el ciclo operativo completo de un hospital: pacientes, turnos, internaciones, infraestructura y estadísticas gerenciales.**

[<img src="https://img.shields.io/badge/Descargar-Ejecutable_v1.0-orange?style=for-the-badge&logo=windows" />](https://github.com/TFacund0/Clinicks-Desktop/releases/tag/v1.0)

[![CI](https://github.com/TFacund0/Clinicks-Desktop/actions/workflows/ci.yml/badge.svg)](https://github.com/TFacund0/Clinicks-Desktop/actions/workflows/ci.yml)
[![.NET Framework](https://img.shields.io/badge/.NET_Framework-4.7.2-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)](https://learn.microsoft.com/dotnet/csharp/)
[![Entity Framework 6](https://img.shields.io/badge/Entity_Framework-6.0-512BD4)](https://learn.microsoft.com/ef/ef6/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server)
[![Tests](https://img.shields.io/badge/tests-74%20passing-brightgreen)](.github/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-yellow.svg)](LICENSE)

[Capturas](#-capturas-de-pantalla) · [Arquitectura](#-arquitectura) · [Instalación](#-instalación) · [Tests](#-tests-y-calidad) · [Roadmap](#-roadmap)

</div>

<p align="center">
  <img src="screenshots/Login.png" width="70%" alt="Pantalla de acceso" />
</p>

---

## 📖 Sobre el proyecto

Clinicks nació como proyecto académico para la facultad y se transformó en un proyecto que mantengo y refactorizo activamente para aplicar buenas prácticas de arquitectura de software: separación de responsabilidades, inyección de dependencias, testing unitario e integración continua.

El sistema modela un flujo hospitalario real con **4 roles de usuario** (Administrador, Recepcionista, Médico y Gerente), cada uno con permisos y pantallas propias, sobre una base de datos relacional normalizada en SQL Server.

> 📌 Este repositorio documenta tanto la **implementación original** como el **proceso de refactor** posterior (ver [Evolución del proyecto](#-evolución-del-proyecto)) — una decisión deliberada para mostrar criterio técnico, no solo el resultado final.

---

## ✨ Funcionalidades principales

| Rol | Funcionalidades |
|---|---|
| 👮 **Administrador** | Gestión de usuarios y roles · Administración de infraestructura (habitaciones, camas, especialidades) · Backup y restauración de la base de datos |
| 📝 **Recepcionista** | Alta de pacientes · Gestión de turnos con validación de disponibilidad y colisiones · Internaciones y altas médicas |
| 👨‍⚕️ **Médico** | Agenda diaria personalizada · Registro de consultas e historial clínico · Cambio de estado de turnos |
| 📈 **Gerente** | Dashboard con gráficos en tiempo real · Estadísticas de ocupación de camas · Reportes de afluencia de pacientes y efectividad de turnos |

---

## 🏗️ Arquitectura

El sistema sigue una **arquitectura en 3 capas (N-Layer)** con separación estricta de responsabilidades, verificada en CI:

```
CapaPresentacion (WinForms)
        │  solo UI: nunca accede a Entity Framework directamente
        ▼
CapaNegocio (Servicios + DTOs)
        │  reglas de negocio, validaciones; depende de interfaces, no de clases concretas
        ▼
CapaDatos (Repositorios + Entity Framework 6, Database First)
        │  única capa con acceso a la base de datos
        ▼
   SQL Server
```

**Decisiones de diseño clave:**

- **Repository Pattern + Inyección de dependencias por constructor** — cada servicio depende de una interfaz de repositorio (`IUsuarioRepository`, `ITurnoRepository`, `ICamaRepository`...), lo que permite sustituirlas por mocks en los tests sin tocar base de datos.
- **DTOs en cada frontera de capa** — las entidades de Entity Framework nunca cruzan hacia la UI; cada operación expone un DTO específico (`MostrarUsuariosDto`, `TurnoAgendaDto`, `PacienteDetalleDto`...).
- **Seguridad de contraseñas con PBKDF2** — salt aleatorio por usuario + 100.000 iteraciones (`Rfc2898DeriveBytes`), con **migración transparente** de los hashes SHA-256 heredados: al iniciar sesión, si el hash almacenado es legacy y la contraseña es correcta, se regenera automáticamente en el formato seguro.
- **Cero acceso a UI desde capas internas** — un hallazgo del proceso de refactor fue codigo que mostraba `MessageBox` desde el repositorio; se corrigió para que la capa de datos solo lance excepciones y sea la UI quien decida cómo comunicarlas.

<details>
<summary><b>Ver stack tecnológico completo</b></summary>

| Categoría | Tecnología |
|---|---|
| Lenguaje | C# 7.3 |
| Framework | .NET Framework 4.7.2 |
| UI | Windows Forms |
| ORM | Entity Framework 6 (Database First) |
| Base de datos | Microsoft SQL Server |
| Gráficos | LiveCharts |
| Seguridad | PBKDF2 (`Rfc2898DeriveBytes`) |
| Testing | MSTest + Moq |
| CI/CD | GitHub Actions |

</details>

---

## 🧪 Tests y calidad

**74 tests unitarios** sobre la capa de negocio, con repositorios mockeados (sin dependencia de base de datos):

- **Seguridad** — formato del hash PBKDF2, verificación de contraseñas, compatibilidad y migración desde hashes legacy SHA-256.
- **Reglas de negocio** — unicidad de DNI/username, validación de turnos duplicados por paciente/médico/fecha, coherencia de fechas de egreso en internaciones, protección del usuario administrador principal.
- **Consultas y agregaciones** — filtros combinados, ordenamientos, conteos case-insensitive y series estadísticas semanales.

```bash
# Ejecutar la suite completa
dotnet test "Sistema Hospitalario.Tests"
```

Cada push y pull request a `master` dispara el workflow de **[GitHub Actions](.github/workflows/ci.yml)**, que compila la solución en Release con MSBuild y corre la suite completa en un runner Windows — el badge de estado arriba refleja el resultado del último build.

---

## 🚀 Instalación

### Requisitos previos
- Visual Studio 2019+ con carga de trabajo **.NET desktop development**
- SQL Server (Express o superior)

### Pasos

```bash
git clone https://github.com/TFacund0/Clinicks-Desktop.git
```

1. **Base de datos**: restaurar el backup de [`database/Database-Backup/`](database/Database-Backup) en tu instancia de SQL Server.
2. **Conexión**: ajustar la cadena de conexión en `App.config` (proyecto `Sistema Hospitalario`) según tu instancia local.
3. **Compilación**: abrir `Sistema Hospitalario.sln`, restaurar paquetes NuGet y ejecutar.

> 💡 ¿Solo querés probarlo sin compilar? Descargá el [**ejecutable de la última release**](https://github.com/TFacund0/Clinicks-Desktop/releases/tag/v1.0).

---

## 📸 Capturas de pantalla

<details open>
<summary><b>👮 Administrador</b></summary>
<p align="center">
  <img src="screenshots/Moderador/Pantalla inicial.png" width="45%" alt="Panel de administrador" />
  <img src="screenshots/Moderador/Usuario.png" width="45%" alt="Gestión de usuarios" />
</p>
</details>

<details>
<summary><b>📝 Recepcionista</b></summary>
<p align="center">
  <img src="screenshots/Recepcionista/Turnos.png" width="45%" alt="Gestión de turnos" />
  <img src="screenshots/Recepcionista/Hospitalización.png" width="45%" alt="Internaciones" />
</p>
</details>

<details>
<summary><b>👨‍⚕️ Médico</b></summary>
<p align="center">
  <img src="screenshots/Médico/Agenda de Turnos.png" width="45%" alt="Agenda médica" />
  <img src="screenshots/Médico/HistorialClínico.png" width="45%" alt="Historial clínico" />
</p>
</details>

<details>
<summary><b>📈 Gerente</b></summary>
<p align="center">
  <img src="screenshots/Gerente/Dashboard.png" width="45%" alt="Dashboard gerencial" />
  <img src="screenshots/Gerente/EstadísticasTurnos.png" width="45%" alt="Estadísticas de turnos" />
</p>
</details>

---

## 🔄 Evolución del proyecto

Este repositorio muestra el proyecto original **y** un proceso posterior de refactor aplicando prácticas que no formaban parte de mi formación inicial:

- [x] Inyección de dependencias por constructor sobre interfaces de repositorio (antes: instanciación directa con `new`)
- [x] Reemplazo de SHA-256 sin salt por PBKDF2, con migración transparente de hashes existentes
- [x] Eliminación de accesos a Entity Framework y a `MessageBox` desde capas que no correspondían (Separation of Concerns)
- [x] Unificación de convenciones de nombres en DTOs y namespaces
- [x] Suite de 74 tests unitarios con MSTest + Moq
- [x] Pipeline de CI con GitHub Actions (build + tests en cada push/PR)

### Roadmap

- [ ] Migrar de Database First a Code First con migraciones versionadas
- [ ] Tests de integración contra una base de datos efímera (LocalDB/contenedor)
- [ ] Explorar migración de la capa de presentación a una arquitectura Clean Architecture / MVVM

---

## 📄 Licencia

Distribuido bajo licencia MIT. Ver [LICENSE](LICENSE) para más detalles.

---

<p align="center">
<sub>Desarrollado originalmente para el Taller de Programación II — Facultad de Ciencias Exactas y Naturales y Agrimensura (Universidad Nacional del Nordeste, UNNE).</sub>
</p>
