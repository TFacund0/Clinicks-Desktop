# 🏥 Sistema de Gestión Hospitalaria (N-Layer)

[<img src="https://img.shields.io/badge/Descargar-Ejecutable_v1.0-orange?style=for-the-badge&logo=windows" />](https://github.com/TFacund0/Sistema-Hospitalario/releases/tag/v1.0)

[![.NET](https://img.shields.io/badge/.NET-v4.8-blueviolet.svg)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23-blue.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/Database-SQL%20Server-red.svg)](https://www.microsoft.com/en-us/sql-server/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 🌟 Descripción General

Este proyecto es un **Sistema Integral de Gestión Hospitalaria** desarrollado como proyecto académico para la facultad. Fue mi primer acercamiento a una **Arquitectura por Capas (N-Layer)**, diseñada para separar las responsabilidades de persistencia, lógica de negocio e interfaz de usuario.

El sistema permite gestionar todo el ciclo operativo de un hospital, desde el ingreso de pacientes y la asignación de camas hasta la visualización de estadísticas gerenciales y la seguridad de la información.

<p align="center">
  <img src="screenshots/Login.png" width="70%" alt="Pantalla de Acceso" />
</p>

---

## 🛠️ Arquitectura y Tecnologías

El sistema sigue el patrón de diseño de **Arquitectura en Capas**:

-   **Capa de Presentación (UI)**: Desarrollada en **Windows Forms (C#)**. Utiliza controles de usuario (UserControls) para una navegación fluida y moderna.
-   **Capa de Negocio (BLL)**: Contiene la lógica central, validaciones y orquestación de servicios. Implementa el uso de **DTOs (Data Transfer Objects)** para la comunicación segura entre capas.
-   **Capa de Datos (DAL)**: Gestión de persistencia mediante **Entity Framework (Database First)** y el patrón **Repositorio con interfaces** (`IUsuarioRepository`, `ITurnoRepository`, etc.).

Los servicios de la capa de negocio reciben sus repositorios por **inyección de dependencias por constructor**, lo que desacopla las capas y permite reemplazarlos por mocks en pruebas unitarias.

### Tecnologías Clave:
-   **Lenguaje**: C# (.NET Framework 4.8)
-   **Persistencia**: Entity Framework 6.0
-   **Base de Datos**: Microsoft SQL Server
-   **Gráficos**: LiveCharts (para el módulo de estadísticas)
-   **Seguridad**: Hashing de contraseñas con **PBKDF2** (salt aleatorio + 100.000 iteraciones), con migración transparente de los hashes legacy SHA-256 al iniciar sesión
-   **Testing**: **MSTest + Moq** — tests unitarios de la capa de negocio con repositorios mockeados

---

## 📋 Módulos del Sistema

El sistema está dividido en 4 roles principales, cada uno con acceso restringido y funcionalidades específicas:

### 1. 👮 Administrador (IT/Infraestructura)
-   Gestión de usuarios y asignación de roles.
-   Administración de infraestructura (Habitaciones, Camas, Especialidades).
-   Gestión de **Backups y Restauración** de la base de datos.

<p align="center">
  <img src="screenshots/Moderador/Pantalla%20inicial.png" width="45%" alt="Menu Administrador" />
  <img src="screenshots/Moderador/Usuario.png" width="45%" alt="Gestion de Usuarios" />
</p>

### 2. 📝 Administrativo (Gestión de Pacientes)
-   Registro y alta de pacientes.
-   Gestión de **Turnos Médicos** con validación de disponibilidad.
-   **Hospitalización**: Gestión de ingresos (internaciones) y altas médicas.

<p align="center">
  <img src="screenshots/Recepcionista/Turnos.png" width="45%" alt="Gestion de Turnos" />
  <img src="screenshots/Recepcionista/Hospitalización.png" width="45%" alt="Internaciones" />
</p>

### 3. 👨‍⚕️ Médico (Agenda Clínica)
-   Visualización de agenda diaria personalizada.
-   Atención de turnos y registro de notas médicas.
-   Cambio de estados de citas (Atendido, Pendiente, Cancelado).

<p align="center">
  <img src="screenshots/Médico/Agenda%20de%20Turnos.png" width="45%" alt="Agenda Médica" />
  <img src="screenshots/Médico/HistorialClínico.png" width="45%" alt="Historial Clínico" />
</p>

### 4. 📈 Gerente (Analítica de Datos)
-   Dashboard dinámico con gráficos en tiempo real.
-   Estadísticas de ocupación de camas.
-   Reportes de afluencia de pacientes y efectividad de turnos.

<p align="center">
  <img src="screenshots/Gerente/Dashboard.png" width="45%" alt="Dashboard Gerencial" />
  <img src="screenshots/Gerente/EstadísticasTurnos.png" width="45%" alt="Estadísticas" />
</p>

---

## 🚀 Instalación y Configuración

1.  **Clonar el repositorio**:
    ```bash
    git clone https://github.com/TFacund0/Sistema-Hospitalario.git
    ```
2.  **Base de Datos**:
    -   Importar el backup ubicado en `/database/Database-Backup/` en SQL Server.
    -   Actualizar la cadena de conexión en el archivo `App.config` de la **Capa de Presentación**.
3.  **Compilación**:
    -   Abrir `Sistema Hospitalario.sln` en Visual Studio (2019 o superior).
    -   Restaurar paquetes NuGet.
    -   Ejecutar el proyecto.

---

## 🧪 Tests

El proyecto `Sistema Hospitalario.Tests` (MSTest + Moq) cubre la lógica de negocio sin necesidad de base de datos, gracias a la inyección de repositorios mockeados:

-   **PasswordHasher**: formato PBKDF2, salt aleatorio, verificación de contraseñas correctas/incorrectas y compatibilidad con hashes legacy SHA-256.
-   **UsuarioService**: alta de usuarios (hashing y unicidad de username), validación de credenciales, migración automática de hashes legacy al iniciar sesión y reglas de negocio (no se puede eliminar al administrador principal).

Se ejecutan desde el **Test Explorer** de Visual Studio (`Ctrl+E, T` → Run All).

---

## 💡 Aprendizajes y Evolución

Este proyecto representa un hito importante en mi formación como desarrollador:
-   **Primera implementación de N-Layer**: Aprendí a separar responsabilidades para lograr un código más mantenible.
-   **Refactor hacia buenas prácticas**: Con el tiempo incorporé mejoras al proyecto original, como inyección de dependencias por constructor sobre interfaces de repositorio y el reemplazo de SHA-256 por PBKDF2 con salt para el almacenamiento de contraseñas.
-   **Manejo de Transacciones**: Gestión de cambios concurrentes en pacientes, camas e internaciones.
-   **Documentación Profesional**: Implementación de comentarios XML para soporte de IntelliSense.

> [!NOTE]
> Aunque este proyecto fue desarrollado durante mis inicios con arquitecturas complejas, lo sigo manteniendo y mejorando: ya incorpora **interfaces de repositorio con Inyección de Dependencias** y **PBKDF2** para contraseñas. En proyectos más recientes aplico además **JWT** para seguridad y **Clean Architecture**.

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Consulta el archivo [LICENSE](LICENSE) para más detalles.

---
*Desarrollado para el Taller de Programación II - 
Facultad de Ciencias Exactas y Naturales y Agrimensura (Universidad Nacional del Nordeste - UNNE).*.