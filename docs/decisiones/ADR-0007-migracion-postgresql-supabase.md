# ADR-0007: Migración de SQL Server a PostgreSQL/Supabase

- **Estado**: Aceptada
- **Fecha**: 2026-07-08
- **Decisores**: Equipo de proyecto
- **Reemplaza a**: [ADR-0003](ADR-0003-sql-server-ef-core.md)

## Contexto

[ADR-0003](ADR-0003-sql-server-ef-core.md) adoptó SQL Server 2022 principalmente por su
integración nativa con el stack .NET y por ofrecer **TDE** y **Always Encrypted** como
capacidades de cifrado a nivel de motor, alineadas con la Ley 29733.

En la práctica del entorno académico surgieron dos problemas:

1. **Always Encrypted real requiere infraestructura externa** (un almacén de claves como
   Azure Key Vault o un HSM) que no existe en un entorno de desarrollo local ni es viable de
   aprovisionar para un proyecto académico sin presupuesto. Esto ya había obligado a
   implementar una alternativa funcional equivalente —cifrado AES-256 a nivel de aplicación
   en `CifradoColumna` (`Sitram.Infrastructure.Persistence.Cifrado`)— documentada como brecha
   aceptada de RNF-003. Es decir: el proyecto **nunca llegó a usar** la capacidad nativa que
   había motivado elegir SQL Server en primer lugar.
2. **Costo y portabilidad del entorno de desarrollo**: mantener una instancia de SQL
   Server/LocalDB por máquina de desarrollo (el equipo migró de Visual Studio a VS Code y
   cambió de entorno más de una vez durante el proyecto) es más frágil que apuntar a una
   base gestionada accesible desde cualquier máquina.

Dado que la ventaja diferencial de SQL Server (Always Encrypted/TDE nativos) ya no aplicaba
en la práctica, se reevaluó la alternativa descartada en ADR-0003.

## Alternativas consideradas

| Opción | Por qué se descartó |
|--------|---------------------|
| **Mantener SQL Server/LocalDB** | La ventaja que justificó la elección original (Always Encrypted/TDE nativos) nunca se pudo usar realmente; persiste el problema de portabilidad entre máquinas de desarrollo |
| **Postgres autoalojado (Docker)** | Añade una dependencia de infraestructura (contenedor corriendo) que cada desarrollador debe mantener; no resuelve el acceso multi-máquina |
| **PostgreSQL gestionado en Supabase** | Gratuito para el tier académico, accesible por red desde cualquier entorno, sin licenciamiento ni infraestructura propia que mantener |

## Decisión

Se migra la persistencia de **SQL Server 2022** a **PostgreSQL (Supabase, gestionado)**,
usando **Npgsql** como proveedor de EF Core 10.

- `UseSqlServer` → `UseNpgsql` con `EnableRetryOnFailure` (reintentos ante fallas
  transitorias de red, dado que la base ya no es local).
- Las 10 migraciones de SQL Server se descartan y se genera una única migración
  `InicialPostgres` sobre el esquema equivalente.
- Tipos de columna: `varbinary` → `bytea`, `nvarchar(max)` → `text`.
- **Concurrencia optimista**: `RowVersion` (específico de SQL Server) se reemplaza por la
  columna de sistema `xmin` de Postgres (patrón documentado por Npgsql).
- El cifrado de columna sigue siendo el mismo (`CifradoColumna`, AES-256 a nivel de
  aplicación) — este ADR no cambia la estrategia de cifrado, solo formaliza que **nunca
  dependió de una capacidad nativa de SQL Server** (ver [ADR-0004](ADR-0004-seguridad-proteccion-datos.md)).
- Cadenas de conexión reales (Supabase) se resuelven vía **User Secrets**, nunca desde
  `appsettings.json` (que conserva un valor local de ejemplo, `Host=localhost;...`, solo
  como placeholder no funcional).

## Consecuencias

**Positivas**
- Elimina la dependencia de una instancia local de SQL Server; cualquier máquina con
  conectividad de red puede desarrollar contra la misma base.
- Sin costo de licenciamiento; tier gratuito de Supabase suficiente para el alcance
  académico.
- No se pierde ninguna capacidad que realmente estuviera en uso (Always Encrypted/TDE
  nativos nunca se implementaron; ver Contexto).

**Negativas / mitigaciones**
- **Cifrado en reposo**: ya no hay "TDE" en el sentido de SQL Server. Supabase cifra los
  volúmenes de almacenamiento a nivel de infraestructura (en reposo) por defecto, lo cual
  cubre RNF-004 a nivel de proveedor, pero es una garantía distinta a la de un motor
  autoalojado con TDE explícito — se documenta esta diferencia en
  [requisitos.md § RNF-004](../requisitos.md).
- **Latencia de red**: la base ya no es local, por lo que las pruebas de integración (62
  pruebas contra Supabase real) tardan más (~5 min) que contra una LocalDB en el mismo
  equipo. Se acepta como costo del entorno gestionado.
- Requiere gestionar bien los **User Secrets** por máquina/entorno — un desajuste (secrets
  faltantes o con clave de cifrado distinta) es una fuente de errores documentada en
  [errores-conocidos.md](../errores-conocidos.md).

## Referencias
- [ADR-0003](ADR-0003-sql-server-ef-core.md) (decisión original, reemplazada por este ADR).
- [ADR-0004](ADR-0004-seguridad-proteccion-datos.md) (estrategia de cifrado, sin cambios).
- [requisitos.md](../requisitos.md) — RNF-003, RNF-004.
