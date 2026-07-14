# Registro de Decisiones de Arquitectura (ADR)

Un **ADR** (Architecture Decision Record) documenta una decisión técnica significativa,
su contexto y sus consecuencias. Cada decisión importante del proyecto queda aquí para
que cualquier persona (o el ingeniero evaluador) entienda **por qué** se hizo algo, no
solo qué se hizo.

## Formato

Cada ADR sigue la plantilla:

- **Estado**: Propuesta · Aceptada · Rechazada · Reemplazada por ADR-XXXX
- **Contexto**: la situación y fuerzas en juego
- **Decisión**: lo que se decidió
- **Consecuencias**: efectos positivos y negativos (trade-offs)
- **Alternativas consideradas**: qué más se evaluó y por qué se descartó

## Índice

| ADR | Título | Estado |
|-----|--------|--------|
| [0001](ADR-0001-eleccion-metodologia-sdd.md) | Elección de metodología y herramienta SDD | Aceptada |
| [0002](ADR-0002-clean-architecture.md) | Clean Architecture + DDD | Aceptada |
| [0003](ADR-0003-sql-server-ef-core.md) | SQL Server + EF Core como capa de persistencia | Reemplazada por 0007 |
| [0004](ADR-0004-seguridad-proteccion-datos.md) | Estrategia de seguridad y protección de datos | Aceptada |
| [0005](ADR-0005-autenticacion-autorizacion.md) | Autenticación con Identity/JWT y autorización RBAC | Aceptada |
| [0006](ADR-0006-frontend-blazor.md) | Frontend con Blazor (Web App, render interactivo en servidor) | Aceptada |
| [0007](ADR-0007-migracion-postgresql-supabase.md) | Migración de SQL Server a PostgreSQL/Supabase | Aceptada |
