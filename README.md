# SITRAM — Sistema Integrado de Trámites Municipales

Plataforma web para **iniciar, pagar y dar seguimiento a trámites municipales** de forma
remota, con énfasis en **seguridad de la información** y **protección de datos personales**
(Ley N.° 29733 y D.S. 016-2024-JUS).

Proyecto del curso **Pruebas y Aseguramiento de la Calidad de Software** · UNSCH · 2026.

## Estructura del repositorio

| Carpeta | Contenido |
|---------|-----------|
| [`docs/`](docs/) | Documentación de ingeniería: requisitos, arquitectura, ADR, modelo de datos, convenciones, flujo de trabajo |
| `src/` | Código fuente de la solución .NET (Clean Architecture) |
| `tests/` | Pruebas unitarias y de integración |
| [`informe-02/`](informe-02/) | Informe formal (Markdown → `.docx`, APA 7) |
| `Sitram.slnx` | Solución de Visual Studio / .NET |

## Arquitectura (Clean Architecture + DDD)

```
src/
├── Sitram.Domain/          # Entidades, agregados, value objects, reglas (sin dependencias)
├── Sitram.Application/     # Casos de uso (CQRS/MediatR), puertos, validadores
├── Sitram.Infrastructure/  # EF Core, PostgreSQL (Supabase), Identity, servicios externos
└── Sitram.Api/             # Blazor + Web API: controllers, middlewares, DI
tests/
├── Sitram.Domain.Tests/        # Unitarias del dominio
├── Sitram.Application.Tests/    # Casos de uso (con mocks)
└── Sitram.Integration.Tests/   # Integración (API + BD de prueba)
```

La **regla de dependencia** apunta siempre hacia el dominio. Detalle en
[docs/arquitectura.md](docs/arquitectura.md).

## Requisitos previos

- **.NET SDK 10** (`dotnet --version` ≥ 10.0)
- **PostgreSQL** (Supabase en desarrollo/producción) para la persistencia — cadena de
  conexión real en User Secrets (`ConnectionStrings:SitramDb`), nunca en `appsettings.json`

## Cómo compilar, probar y ejecutar

```bash
dotnet build Sitram.slnx          # compilar toda la solución
dotnet test  Sitram.slnx          # ejecutar las pruebas
dotnet run --project src/Sitram.Api   # levantar la API (health: /api/health)
```

En desarrollo, la documentación OpenAPI se expone en `/openapi/v1.json`.

## Stack tecnológico

C# 14 · .NET 10 (LTS) · Blazor · ASP.NET Core Web API · PostgreSQL 17 (Supabase) · EF Core 10
(Npgsql) · MediatR · FluentValidation · Serilog · xUnit · Moq · FluentAssertions · Respawn.

## Metodología

**Spec-Driven Development (SDD)** como núcleo (GitHub Spec Kit: `/specify → /plan → /tasks →
/implement`), complementado con **Scrum** y **XP**. Ver
[docs/flujo-de-trabajo.md](docs/flujo-de-trabajo.md).
