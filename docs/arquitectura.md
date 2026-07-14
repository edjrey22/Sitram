# Arquitectura — SITRAM

> Versión 1.0 · Documento vivo. Cambios estructurales se registran como
> [ADR](decisiones/).

## 1. Estilo arquitectónico

SITRAM adopta **Clean Architecture** (arquitectura en capas concéntricas) combinada con
**Domain-Driven Design (DDD) táctico**. La regla fundamental es la **regla de dependencia**:
las dependencias del código fuente solo pueden apuntar **hacia adentro**. El dominio no
conoce a la infraestructura.

```
        ┌───────────────────────────────────────────────┐
        │                  API / Presentation            │  ← ASP.NET Core Web API
        │   Controllers · Middlewares · Filtros · DTOs   │
        │  ┌─────────────────────────────────────────┐   │
        │  │             Application                  │   │  ← Casos de uso
        │  │  Commands · Queries · Handlers · Ports   │   │
        │  │  ┌───────────────────────────────────┐   │   │
        │  │  │            Domain                  │   │   │  ← Núcleo (sin dependencias)
        │  │  │  Entidades · Value Objects ·       │   │   │
        │  │  │  Agregados · Reglas · Eventos      │   │   │
        │  │  └───────────────────────────────────┘   │   │
        │  └─────────────────────────────────────────┘   │
        │                Infrastructure                  │  ← EF Core · PostgreSQL (Supabase) · Serilog
        │   Repositorios · DbContext · Servicios ext.    │
        └───────────────────────────────────────────────┘
             Las flechas de dependencia apuntan hacia el Domain
```

### ¿Por qué Clean Architecture?

- **Testabilidad**: el dominio y los casos de uso se prueban sin base de datos ni framework.
- **Independencia tecnológica**: cambiar de motor de base de datos solo afecta a
  Infrastructure — así se migró de SQL Server a PostgreSQL sin tocar Domain/Application
  (ver [ADR-0007](decisiones/ADR-0007-migracion-postgresql-supabase.md)).
- **Protección del dominio**: las reglas municipales (estados de un trámite, validaciones)
  viven aisladas y no se contaminan con detalles de EF Core o HTTP.

Justificación completa en [ADR-0002](decisiones/ADR-0002-clean-architecture.md).

## 2. Proyectos de la solución

Estructura física de la solución `.NET` (`Sitram.sln`):

```
src/
├── Sitram.Domain/           # Entidades, Value Objects, Agregados, interfaces de dominio
├── Sitram.Application/      # Casos de uso (CQRS), DTOs, validadores, puertos (interfaces)
├── Sitram.Infrastructure/   # EF Core, repositorios, Identity, Serilog, servicios externos
└── Sitram.Api/              # Web API: controllers, middlewares, configuración, DI
tests/
├── Sitram.Domain.Tests/         # Pruebas unitarias del dominio
├── Sitram.Application.Tests/    # Pruebas de casos de uso (con mocks)
└── Sitram.Integration.Tests/    # Pruebas de integración (API + BD real de prueba)
```

| Proyecto | Depende de | Responsabilidad |
|----------|-----------|-----------------|
| `Domain` | *(nada)* | Modelo de negocio puro |
| `Application` | `Domain` | Orquestación de casos de uso |
| `Infrastructure` | `Application`, `Domain` | Implementación de puertos (BD, correo, pagos) |
| `Api` | `Application`, `Infrastructure` | Exposición HTTP y arranque |

## 3. Patrón CQRS (Command Query Responsibility Segregation)

La capa `Application` separa **comandos** (escritura, mutan estado) de **consultas**
(lectura). Se implementa con **MediatR**.

```
Ciudadano ──HTTP POST /tramites──► TramitesController
                                        │
                                        ▼
                           IniciarTramiteCommand ──► MediatR
                                                        │
                                        ┌───────────────┘
                                        ▼
                           IniciarTramiteCommandHandler
                                        │  (valida, crea Agregado Tramite)
                                        ▼
                           ITramiteRepository.AddAsync()
                                        │
                                        ▼
                            PostgreSQL (Supabase)
```

- **Commands**: `IniciarTramiteCommand`, `AprobarTramiteCommand`, `RechazarTramiteCommand`.
- **Queries**: `ObtenerEstadoTramiteQuery`, `ListarTramitesCiudadanoQuery`.
- Cada handler tiene una **única responsabilidad**; los validadores (`FluentValidation`) se
  ejecutan en un *pipeline behavior* antes del handler.

## 4. Agregados y modelo de dominio (DDD)

El **agregado raíz** central es `Tramite`. Encapsula la máquina de estados y garantiza las
invariantes de negocio (un trámite no puede pasar de `Borrador` a `Aprobado` sin pasar por
`EnRevision`).

```
Agregado Tramite (raíz)
├── TramiteId (Value Object)
├── CiudadanoId (referencia a otro agregado)
├── TipoTramite
├── EstadoTramite  ─────► máquina de estados
├── List<Documento>       (entidades hijas)
├── List<Pago>
└── List<EventoAuditoria> (historial inmutable)
```

### Máquina de estados del trámite

```
  ┌──────────┐   enviar    ┌────────────┐  revisar   ┌──────────────┐
  │ Borrador │ ──────────► │  Recibido  │ ─────────► │  EnRevision  │
  └──────────┘             └────────────┘            └──────┬───────┘
                                                            │
                         ┌──────────────────┬──────────────┤
                         ▼                  ▼              ▼
                   ┌──────────┐      ┌────────────┐  ┌────────────┐
                   │ Aprobado │      │  Rechazado │  │ Observado  │
                   └──────────┘      └────────────┘  └─────┬──────┘
                                                           │ subsanar
                                                           ▼
                                                    ┌──────────────┐
                                                    │  EnRevision  │
                                                    └──────────────┘
```

Las transiciones inválidas lanzan una excepción de dominio (`TransicionInvalidaException`)
— nunca se permiten desde fuera del agregado.

## 5. Vista de componentes y seguridad

```
                         Internet (HTTPS/TLS 1.3)
                                  │
                                  ▼
                    ┌──────────────────────────┐
                    │   Reverse Proxy / WAF     │
                    └────────────┬─────────────┘
                                 ▼
        ┌────────────────────────────────────────────────┐
        │                 Sitram.Api                      │
        │  ┌──────────────────────────────────────────┐  │
        │  │ Middleware pipeline                       │  │
        │  │  1. Manejo global de excepciones          │  │
        │  │  2. Autenticación JWT                     │  │
        │  │  3. Autorización RBAC (políticas)         │  │
        │  │  4. Auditoría (Serilog enrichers)         │  │
        │  │  5. Rate limiting                         │  │
        │  └──────────────────────────────────────────┘  │
        └───────┬───────────────────────────┬────────────┘
                ▼                            ▼
     ┌───────────────────────┐  ┌───────────────────────────┐
     │  PostgreSQL (Supabase)│  │  Servicios externos        │
     │  - Datos cifrados     │  │  - Pasarela de pagos       │
     │    a nivel de columna │  │  - Notificaciones (correo) │
     │    (AES-256, app)     │  └───────────────────────────┘
     │  - Cifrado en reposo  │
     │    del proveedor      │
     │  - Auditoría inmutable│
     └───────────────────────┘
```

Los detalles de seguridad y protección de datos se desarrollan en
[ADR-0004](decisiones/ADR-0004-seguridad-proteccion-datos.md) y
[ADR-0005](decisiones/ADR-0005-autenticacion-autorizacion.md).

## 6. Decisiones arquitectónicas clave (resumen)

| # | Decisión | Estado |
|---|----------|--------|
| [0001](decisiones/ADR-0001-eleccion-metodologia-sdd.md) | GitHub Spec Kit como herramienta SDD | Aceptada |
| [0002](decisiones/ADR-0002-clean-architecture.md) | Clean Architecture + DDD | Aceptada |
| [0003](decisiones/ADR-0003-sql-server-ef-core.md) | SQL Server + EF Core como persistencia | Reemplazada por 0007 |
| [0004](decisiones/ADR-0004-seguridad-proteccion-datos.md) | Estrategia de seguridad y Ley 29733 | Aceptada |
| [0005](decisiones/ADR-0005-autenticacion-autorizacion.md) | Identity + JWT + RBAC | Aceptada |
| [0006](decisiones/ADR-0006-frontend-blazor.md) | Frontend con Blazor (render interactivo en servidor) | Aceptada |
| [0007](decisiones/ADR-0007-migracion-postgresql-supabase.md) | Migración de SQL Server a PostgreSQL/Supabase | Aceptada |

## 7. Atributos de calidad priorizados

1. **Seguridad** (crítico) — datos personales sensibles bajo Ley 29733.
2. **Auditabilidad** (crítico) — cada acción sobre un trámite debe ser trazable.
3. **Mantenibilidad** (alto) — separación de capas y pruebas automatizadas.
4. **Disponibilidad** (medio) — objetivo de servicio para atención ciudadana.
5. **Rendimiento** (medio) — respuestas < 500 ms en consultas de estado.
