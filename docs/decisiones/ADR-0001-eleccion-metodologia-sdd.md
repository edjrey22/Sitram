# ADR-0001: Elección de metodología y herramienta SDD

- **Estado**: Aceptada
- **Fecha**: 2026-07-01
- **Decisores**: Equipo de proyecto

## Contexto

El curso exige desarrollar el software mediante **Spec-Driven Development (SDD)**: las
especificaciones son la fuente de verdad y preceden al código. Se debe elegir **una** de
tres herramientas: **GitHub Spec Kit**, **OpenSpec** o **Kiro**.

Criterios de evaluación:

1. Compatibilidad con el flujo de trabajo con asistentes de IA (Claude Code).
2. Independencia del IDE (no atarnos a un editor concreto).
3. Trazabilidad especificación → plan → tareas → código (útil para el informe académico).
4. Madurez y documentación disponible.

## Alternativas consideradas

| Herramienta | Ventajas | Desventajas |
|-------------|----------|-------------|
| **GitHub Spec Kit** | CLI abierta, agnóstica de IDE, integra con Claude Code; genera `spec.md`, `plan.md`, `tasks.md`; flujo `/specify → /plan → /tasks → /implement` fácil de mapear a los capítulos del informe | Proyecto joven; requiere disciplina manual |
| **OpenSpec** | Modelo claro de `changes/` vs `specs/`; buen control de cambios incrementales | Menor integración directa con el flujo de Claude Code; curva de adopción |
| **Kiro** | Formato `requirements/design/tasks` con notación **EARS**, muy académico | Atado a su propio IDE (AWS); menos portable para un trabajo universitario |

## Decisión

Se adopta **GitHub Spec Kit**.

Razones:

- Su ciclo `/specify → /plan → /tasks → /implement` **mapea casi 1:1** con la estructura del
  informe exigido (Análisis → Diseño → Implementación → Resultados).
- Es **agnóstica de IDE** y funciona nativamente con Claude Code, nuestra herramienta de
  desarrollo asistido.
- Los artefactos que genera (`spec.md`, `plan.md`, `tasks.md`) son **texto plano versionable**
  que se adjunta como evidencia y anexos del trabajo.

## Consecuencias

**Positivas**
- Trazabilidad completa: cada línea de código se remonta a una especificación.
- Los entregables del curso salen casi directamente de los artefactos de Spec Kit.
- Facilita la revisión y la evaluación por parte del docente.

**Negativas / mitigaciones**
- Requiere disciplina: *no se escribe código sin spec previa*. → Se refuerza en el
  [flujo de trabajo](../flujo-de-trabajo.md).
- Herramienta joven: posibles cambios de API. → Se fija la versión en la documentación del
  entorno.

## Nota de implementación (2026-07-16)

En la práctica, no se generó un `spec.md`/`plan.md`/`tasks.md` separado por cada
funcionalidad como archivo suelto versionado. El ciclo `/specify → /plan → /tasks →
/implement` se siguió como disciplina de trabajo, pero su contenido se consolidó en la
documentación de ingeniería única del proyecto para evitar duplicar información: los
requisitos ([requisitos.md](../requisitos.md)) hacen de especificación global, la
arquitectura y los ADR hacen de plan técnico, y la trazabilidad a tareas queda en los
nombres de los `Command`/`Handler` y en el historial de commits. Ver el detalle en
[flujo-de-trabajo.md § 1](../flujo-de-trabajo.md#1-ciclo-spec-driven-development-spec-kit).
