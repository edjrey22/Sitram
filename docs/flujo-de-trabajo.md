# Flujo de trabajo — SITRAM

> Cómo se trabaja el día a día: del *spec* al código, con control de versiones e integración
> continua. Regla de oro (SDD): **no se escribe código sin una especificación aprobada**.

## 1. Ciclo Spec-Driven Development (Spec Kit)

Cada funcionalidad recorre cuatro etapas antes de darse por terminada:

```
   /specify            /plan               /tasks              /implement
 ┌───────────┐      ┌───────────┐       ┌───────────┐       ┌──────────────┐
 │ spec.md   │ ───► │ plan.md   │ ────► │ tasks.md  │ ────► │ código + test│
 │ QUÉ y POR │      │ CÓMO      │       │ pasos     │       │ verificado   │
 │  QUÉ      │      │ (diseño)  │       │ atómicos  │       │              │
 └───────────┘      └───────────┘       └───────────┘       └──────────────┘
      │                   │                   │                     │
      └───────────────────┴─── revisión ──────┴─────────────────────┘
```

1. **`/specify`** — Se describe la necesidad en lenguaje de negocio: actores, criterios de
   aceptación, reglas. **Sin decisiones técnicas.**
2. **`/plan`** — Se traduce a diseño técnico: entidades afectadas, endpoints, esquema de BD,
   impacto en seguridad.
3. **`/tasks`** — Se descompone en tareas pequeñas y verificables.
4. **`/implement`** — Se codifica tarea por tarea, cada una con sus pruebas.

Los artefactos (`spec.md`, `plan.md`, `tasks.md`) se versionan en Git y sirven como
**evidencia y anexos** del informe académico.

## 2. Estrategia de ramas (Git Flow simplificado)

```
main        ──●───────────────●───────────────●──   (producción, siempre estable)
               \             / \             /
develop     ────●───●───●───●───●───●───●───●────   (integración)
                 \     /         \     /
feature/*         ●───●           ●───●              (una rama por funcionalidad)
```

| Rama | Propósito | Origen | Destino |
|------|-----------|--------|---------|
| `main` | Código en producción, estable y etiquetado | — | — |
| `develop` | Rama de integración | `main` | `main` (release) |
| `feature/<nombre>` | Nueva funcionalidad | `develop` | `develop` |
| `fix/<nombre>` | Corrección de error | `develop` | `develop` |
| `hotfix/<nombre>` | Corrección urgente en producción | `main` | `main` + `develop` |

Nombre de rama: `feature/SITRAM-123-iniciar-tramite`.

## 3. Ciclo de un cambio (Pull Request)

```
1. Crear rama desde develop        git switch -c feature/SITRAM-123-...
2. Trabajar con commits pequeños    (Conventional Commits)
3. dotnet format + dotnet test      (local, antes de subir)
4. Push y abrir Pull Request        hacia develop
5. CI ejecuta build + tests + análisis
6. Revisión de código (al menos 1)  checklist más abajo
7. Merge (squash) si CI verde       y aprobación
8. Borrar la rama de feature
```

### Checklist de revisión (Definition of Done)

- [ ] Existe *spec* aprobada que respalda el cambio.
- [ ] La build pasa sin advertencias.
- [ ] Pruebas nuevas/actualizadas y cobertura ≥ 80 % en el código tocado.
- [ ] No hay datos personales en logs ni en mensajes de error.
- [ ] Autorización correcta en endpoints nuevos (política RBAC).
- [ ] Migración de BD incluida si cambió el esquema.
- [ ] Documentación actualizada (glosario/ADR si aplica).
- [ ] Commits siguen Conventional Commits.

## 4. Integración continua (CI)

Pipeline (GitHub Actions) que se dispara en cada push y PR:

```
┌──────────┐   ┌──────────┐   ┌──────────┐   ┌──────────────┐   ┌──────────────┐
│ restore  │──►│  build   │──►│  test    │──►│ análisis     │──►│ cobertura    │
│          │   │ (warn=err)│  │ unit+integ│  │ estático     │   │ (≥80% gate)  │
└──────────┘   └──────────┘   └──────────┘   └──────────────┘   └──────────────┘
```

- **build**: `dotnet build -c Release` con advertencias como errores.
- **test**: unitarias + integración (BD de prueba efímera).
- **análisis estático**: analizadores de .NET + reglas de seguridad; opcional SonarQube.
- **gate de cobertura**: el PR se bloquea si baja del umbral.

## 5. Entornos

| Entorno | Uso | Base de datos |
|---------|-----|---------------|
| `Development` | Máquina del desarrollador | PostgreSQL (Supabase, compartido) |
| `Testing` | CI y pruebas de integración | BD efímera por corrida (`sitram_test_<guid>`), se limpia con Respawn |
| `Staging` | Validación previa (opcional) | Copia con datos anonimizados |
| `Production` | Uso real | PostgreSQL (Supabase) con backups |

**Nunca** se usan datos personales reales fuera de `Production`. Los entornos inferiores
usan **datos sintéticos o anonimizados** (obligación de la Ley 29733).

## 6. Gestión de secretos

- **Nada de secretos en el repositorio** (cadenas de conexión, claves JWT, llaves de cifrado).
- Desarrollo: **User Secrets** de .NET (`dotnet user-secrets`).
- CI/Producción: variables de entorno / gestor de secretos.
- `appsettings.json` solo contiene valores no sensibles; los sensibles se sobrescriben por entorno.

## 7. Versionado

- **SemVer** (`MAJOR.MINOR.PATCH`) para releases, etiquetados en `main` (`v1.0.0`).
- Cada release genera un `CHANGELOG.md` derivado de los Conventional Commits.

## 8. Marco metodológico combinado (SDD + Scrum + XP)

El proyecto integra tres metodologías complementarias que se reparten responsabilidades:
**SDD define y traza el *qué*, Scrum gestiona el *cuándo* y el ritmo, y XP asegura el *cómo*
técnico.** El **SDD (GitHub Spec Kit) es el núcleo obligatorio**; Scrum y XP son las prácticas
complementarias que se aplican sobre él.

| Metodología | Rol en el proyecto | Cómo se materializa aquí |
|-------------|--------------------|--------------------------|
| **SDD** (núcleo) | Trazabilidad especificación → código | Ciclo `/specify → /plan → /tasks → /implement` (sección 1) |
| **Scrum** (gestión) | Organización iterativa del trabajo | *Sprints* por módulo; el *Product Backlog* son las historias de usuario derivadas de los RF; el *Sprint Backlog* alimenta las ramas `feature/*` (sección 2) |
| **XP** (calidad) | Excelencia técnica del código | **TDD**, integración continua (sección 4), *diseño simple*, refactorización y estándares de código ([convenciones](convenciones.md)) |

**Planificación referencial de sprints** (una tanda de historias por sprint):

| Sprint | Objetivo | Módulos |
|--------|----------|---------|
| 1 | Cimientos y seguridad | Identidad y acceso (registro, login JWT, RBAC) |
| 2 | Configuración del TUPA | Tipos de trámite, requisitos, flujos |
| 3 | Ciclo del trámite | Máquina de estados, documentos, subsanación |
| 4 | Pagos y notificaciones | Cálculo de tasa, pago, seguimiento, correo |
| 5 | Protección de datos y auditoría | Consentimiento, ARCO, incidentes, auditoría |

> Este apartado mantiene alineados los artefactos de ingeniería con el Capítulo III del informe
> (Material y Métodos), evitando que el informe declare metodologías no documentadas aquí.
