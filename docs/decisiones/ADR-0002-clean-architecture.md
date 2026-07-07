# ADR-0002: Clean Architecture + DDD

- **Estado**: Aceptada
- **Fecha**: 2026-07-01
- **Decisores**: Equipo de proyecto

## Contexto

El dominio de trámites municipales tiene **reglas de negocio complejas y críticas**:
máquinas de estado de expedientes, flujos de aprobación multinivel, validaciones legales
y obligaciones de auditoría. Estas reglas deben ser **independientes** de la base de datos
y del framework web para poder probarlas y mantenerlas a largo plazo.

Se necesitaba un estilo arquitectónico que:

1. Aísle las reglas de negocio de los detalles técnicos.
2. Permita pruebas unitarias sin infraestructura.
3. Escale a medida que crecen los tipos de trámite.

## Alternativas consideradas

| Opción | Por qué se descartó |
|--------|---------------------|
| **Arquitectura en 3 capas clásica** (UI → BLL → DAL) | La lógica termina acoplada al acceso a datos; difícil de probar de forma aislada |
| **Transaction Script / CRUD directo** | Insuficiente para reglas de estado y aprobación; degenera en "código espagueti" |
| **Microservicios** | Sobredimensionado para el alcance de un proyecto de curso; complejidad operativa injustificada |

## Decisión

Se adopta **Clean Architecture** con cuatro capas (`Domain`, `Application`,
`Infrastructure`, `Api`) y **DDD táctico** (agregados, entidades, value objects, eventos de
dominio). La **regla de dependencia** se aplica estrictamente: todo apunta hacia el `Domain`.

El agregado raíz `Tramite` encapsula la máquina de estados; las transiciones inválidas son
imposibles desde fuera del agregado.

## Consecuencias

**Positivas**
- El `Domain` y la capa `Application` se prueban **sin base de datos** → cobertura alta y rápida.
- Cambiar de motor de BD o de framework web no toca el núcleo de negocio.
- Modelo alineado con el lenguaje del dominio (ver [glosario](../glosario.md)).

**Negativas / mitigaciones**
- Más proyectos y "ceremonia" inicial (DTOs, mapeos, interfaces). → Justificado por la
  criticidad del dominio; se automatiza el mapeo con AutoMapper.
- Curva de aprendizaje de CQRS/MediatR. → Se documenta con ejemplos en
  [convenciones](../convenciones.md).
