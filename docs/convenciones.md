# Convenciones de código — SITRAM

> Objetivo: que todo el código se lea como si lo hubiera escrito una sola persona. Base:
> [Microsoft C# Coding Conventions](https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions)
> + reglas propias del proyecto.

## 1. Nomenclatura

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Clases, structs, records | `PascalCase` | `TramiteRepository`, `Dni` |
| Interfaces | `I` + `PascalCase` | `ITramiteRepository` |
| Métodos | `PascalCase` | `IniciarTramiteAsync` |
| Métodos asíncronos | sufijo `Async` | `ObtenerPorIdAsync` |
| Parámetros y variables locales | `camelCase` | `tramiteId` |
| Campos privados | `_` + `camelCase` | `_dbContext` |
| Constantes | `PascalCase` | `MaxIntentosLogin` |
| Propiedades | `PascalCase` | `EstadoActual` |
| Tipos genéricos | `T` + descriptivo | `TEntidad`, `TResultado` |
| Archivos | 1 tipo por archivo, mismo nombre | `Tramite.cs` |

**Idioma**: el **dominio se nombra en español** (lenguaje ubicuo: `Tramite`, `Ciudadano`,
`Aprobar`), coherente con el [glosario](glosario.md). Los términos puramente técnicos de
frameworks se mantienen como vienen (`Controller`, `Repository`, `Handler`, `Async`).

## 2. Estructura de carpetas por proyecto

```
Sitram.Domain/
├── Tramites/            # Agregado Tramite: entidad, VOs, eventos, ITramiteRepository
├── Ciudadanos/
├── Common/              # Clases base: Entity, AggregateRoot, ValueObject
└── Exceptions/          # Excepciones de dominio

Sitram.Application/
├── Tramites/
│   ├── Commands/        # IniciarTramite/, AprobarTramite/ (Command + Handler + Validator)
│   └── Queries/         # ObtenerEstado/ (Query + Handler)
├── Common/
│   ├── Behaviors/       # Pipeline MediatR (validación, logging)
│   └── Interfaces/      # Puertos: IEmailService, IPagoService
└── DependencyInjection.cs

Sitram.Infrastructure/
├── Persistence/         # DbContext, configuraciones EF, repositorios, migraciones
├── Identity/            # Configuración de Identity y JWT
├── Services/            # Adaptadores: correo, pagos, auditoría
└── DependencyInjection.cs

Sitram.Api/
├── Controllers/
├── Middlewares/
├── Extensions/
└── Program.cs
```

## 3. Principios de diseño (SOLID)

- **S** — Una clase, una razón para cambiar. Handlers con una sola responsabilidad.
- **O** — Abierto a extensión, cerrado a modificación. Nuevos tipos de trámite sin tocar los existentes.
- **L** — Las implementaciones respetan el contrato de sus interfaces.
- **I** — Interfaces pequeñas y específicas (`ITramiteRepository`, no un `IRepository` gigante).
- **D** — Se depende de abstracciones (puertos), no de implementaciones concretas.

## 4. Reglas de codificación

- **`var`** cuando el tipo es evidente por la derecha; tipo explícito si aporta claridad.
- **Nullable reference types** habilitado (`<Nullable>enable</Nullable>`). Evitar `null`;
  preferir `Result`/excepciones de dominio para errores esperables.
- **Async de extremo a extremo**: nada de `.Result` ni `.Wait()` (riesgo de *deadlock*).
- **Inmutabilidad**: los value objects son `record` inmutables.
- **Sin lógica de negocio en los controllers**: solo reciben, delegan a MediatR y responden.
- **Sin lógica de negocio en EF**: las entidades no conocen el `DbContext`.
- **Guard clauses** al inicio de los métodos para validar precondiciones.
- **Un método hace una cosa**; si supera ~30 líneas, revisar si debe dividirse.

## 5. Manejo de errores

| Situación | Mecanismo |
|-----------|-----------|
| Violación de invariante de dominio | Lanzar excepción de dominio (`TransicionInvalidaException`) |
| Validación de entrada | `FluentValidation` en el pipeline (antes del handler) |
| Recurso no encontrado | `NotFoundException` → HTTP 404 |
| Acceso no autorizado | Autorización por política → HTTP 403 |
| Error inesperado | Middleware global → HTTP 500 + log (sin filtrar datos personales) |

Todas las respuestas de error siguen el formato **RFC 7807 (Problem Details)**.

## 6. Comentarios y documentación

- El **código autoexplicativo** es preferible a comentarios que expliquen *qué* hace.
- Comentar el **porqué**, no el qué, cuando la intención no es obvia.
- **XML docs** (`/// <summary>`) en las interfaces públicas y en las reglas de dominio no triviales.
- **TODO/FIXME** con responsable y referencia: `// TODO(SITRAM-123): ...`.

## 7. Convención de commits (Conventional Commits)

```
<tipo>(<ámbito>): <descripción en imperativo, minúscula>

[cuerpo opcional]
```

| Tipo | Uso |
|------|-----|
| `feat` | Nueva funcionalidad |
| `fix` | Corrección de error |
| `docs` | Solo documentación |
| `refactor` | Cambio interno sin alterar comportamiento |
| `test` | Añadir o corregir pruebas |
| `chore` | Tareas de mantenimiento (dependencias, build) |
| `perf` | Mejora de rendimiento |
| `security` | Cambio relativo a seguridad |

Ejemplos:
```
feat(tramites): permitir subsanación de trámites observados
fix(auth): corregir expiración de refresh token
security(datos): enmascarar DNI en los logs de aplicación
```

## 8. Pruebas — nomenclatura

Patrón `Metodo_Escenario_ResultadoEsperado`:

```csharp
[Fact]
public void Aprobar_TramiteEnRevision_CambiaEstadoAAprobado() { ... }

[Fact]
public void Aprobar_TramiteEnBorrador_LanzaTransicionInvalidaException() { ... }
```

- **AAA**: *Arrange – Act – Assert*, separado visualmente.
- Aserciones con **FluentAssertions** (`resultado.Should().Be(...)`).
- Objetivo de cobertura: **≥ 80 %** en `Domain` y `Application`.

## 9. Formato automático

- **`.editorconfig`** en la raíz define estilo (sangría de 4 espacios, `using` ordenados, etc.).
- **`dotnet format`** se ejecuta antes de cada commit (hook) y en CI.
- El build falla si hay advertencias de análisis (`TreatWarningsAsErrors` en Release).
