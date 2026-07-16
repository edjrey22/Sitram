# SITRAM — Sistema Integrado de Trámites Municipales

> Documentación de ingeniería del proyecto. Estos artefactos son la base sobre la que se
> redactará el informe formal exigido por el curso (Capítulos I–V + Anexos).

## 1. Descripción del proyecto

**SITRAM** es una plataforma web que permite a la ciudadanía **iniciar, pagar y dar
seguimiento a trámites municipales** (licencias de funcionamiento, permisos, certificados,
constancias) de forma remota, y a los funcionarios **gestionarlos mediante flujos de
aprobación** con trazabilidad completa.

El proyecto se desarrolla para el curso de **Pruebas y Aseguramiento de la Calidad de
Software** y pone énfasis en dos ejes transversales —**Seguridad de la información** y
**Protección de datos personales**—:

- **Seguridad de la información**: autenticación robusta, autorización basada en roles
  (RBAC), cifrado en tránsito y en reposo, y registro de auditoría inviolable.
- **Protección de datos personales**: cumplimiento de la **Ley N.° 29733 – Ley de
  Protección de Datos Personales (Perú)** y su reglamento vigente (D.S. N.° 016-2024-JUS,
  que reemplaza al D.S. 003-2013-JUS desde 2024), equivalente nacional del GDPR.

## 2. Stack tecnológico

| Capa | Tecnología |
|------|------------|
| Lenguaje / Runtime | C# 14 · .NET 10 (LTS) |
| Frontend | Blazor Web App (.NET 10) · render interactivo en servidor (ver [ADR-0006](decisiones/ADR-0006-frontend-blazor.md)) |
| API | ASP.NET Core Web API |
| Persistencia | PostgreSQL 17 (Supabase, gestionado) · Entity Framework Core 10 (Npgsql) — ver [ADR-0007](decisiones/ADR-0007-migracion-postgresql-supabase.md), reemplaza a [ADR-0003](decisiones/ADR-0003-sql-server-ef-core.md) |
| Arquitectura | Clean Architecture + DDD táctico |
| Seguridad | ASP.NET Core Identity · JWT · RBAC |
| Validación | FluentValidation |
| Logging / Auditoría | Serilog |
| Pruebas | xUnit · Moq · FluentAssertions · Respawn |
| SDD | GitHub Spec Kit (ver [ADR-0001](decisiones/ADR-0001-eleccion-metodologia-sdd.md)) |

## 3. Índice de documentación

| Documento | Contenido |
|-----------|-----------|
| [analisis-problematica.md](analisis-problematica.md) | Diagnóstico con evidencia (local/nacional/internacional) que justifica el proyecto |
| [requisitos.md](requisitos.md) | Requisitos funcionales (RF) y no funcionales (RNF) trazables |
| [arquitectura.md](arquitectura.md) | Estilo arquitectónico, capas, componentes, flujos y diagramas |
| [modelo-datos.md](modelo-datos.md) | Diagrama Entidad-Relación, entidades, cifrado, índices e integridad |
| [decisiones/](decisiones/) | Registro de Decisiones de Arquitectura (ADR) |
| [frontend.md](frontend.md) | Mapa de páginas Blazor, rutas, políticas de autorización |
| [convenciones.md](convenciones.md) | Normas de código C#/.NET, nombres, estructura, commits |
| [glosario.md](glosario.md) | Términos del dominio municipal y técnicos |
| [flujo-de-trabajo.md](flujo-de-trabajo.md) | Git Flow, ramas, PRs, CI/CD, ciclo SDD |
| [errores-conocidos.md](errores-conocidos.md) | Problemas frecuentes y soluciones probadas |
| [presentacion.md](presentacion.md) | Diapositivas (15 hojas) para la exposición oral (formato Marp/reveal-md) |
| [guion-exposicion.md](guion-exposicion.md) | Monólogo completo hoja por hoja, con qué código y qué documentos mostrar en cada momento |

## 4. Metodología: Spec-Driven Development (SDD)

El desarrollo se rige por **especificaciones primero, código después**. Cada funcionalidad
nace de una especificación (`spec`) que describe *qué* y *por qué* antes de *cómo*. La
herramienta elegida es **GitHub Spec Kit**; la justificación está en
[ADR-0001](decisiones/ADR-0001-eleccion-metodologia-sdd.md).

El SDD es el **núcleo obligatorio**, complementado con **Scrum** (gestión iterativa por
*sprints*) y **Extreme Programming / XP** (TDD, integración continua y calidad de código). El
detalle de cómo se combinan las tres está en
[flujo-de-trabajo.md § 8](flujo-de-trabajo.md#8-marco-metodológico-combinado-sdd--scrum--xp).

## 5. Estado

| Fase | Estado |
|------|--------|
| Documentación de ingeniería | 🟢 Completa (Must + Should), actualizada 2026-07-13 |
| Estructura del proyecto .NET | 🟢 Completa (Clean Architecture, 4 proyectos de código + 3 de pruebas) |
| Implementación (backend) | 🟢 Completa: Must-have + Should-have (única excepción documentada: Always Encrypted real, ver [ADR-0007](decisiones/ADR-0007-migracion-postgresql-supabase.md)) |
| Implementación (frontend Blazor) | 🟢 Completa: registro, login, recuperación/confirmación de email, ciclo de vida del trámite, administración de trámites y funcionarios |
| Pruebas | 🟢 146/146 en verde (47 Domain + 37 Application + 62 Integration) |
| Informe formal | 🟡 En progreso — pendiente actualizar cifras y sección de persistencia tras la migración a PostgreSQL |
