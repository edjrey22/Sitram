# ADR-0003: SQL Server + EF Core como capa de persistencia

- **Estado**: Aceptada
- **Fecha**: 2026-07-01
- **Decisores**: Equipo de proyecto

## Contexto

Los trámites municipales son **datos altamente relacionales y transaccionales**: un
expediente enlaza ciudadano, tipo de trámite, documentos, pagos y un historial de
auditoría. Se requieren garantías **ACID** (un pago y el cambio de estado deben confirmarse
juntos) y **consultas relacionales** complejas para reportes administrativos.

Además, el requisito de protección de datos (Ley 29733) exige capacidades de **cifrado en
reposo** y **auditoría** a nivel de motor.

## Alternativas consideradas

| Opción | Por qué se descartó |
|--------|---------------------|
| **MongoDB / NoSQL documental** | El modelo es fuertemente relacional; perderíamos integridad referencial y transacciones multi-documento naturales |
| **PostgreSQL** | Válido técnicamente, pero SQL Server ofrece integración directa con el stack .NET, TDE (Transparent Data Encryption) y *Always Encrypted* para datos sensibles, además de ser el estándar en el curso |
| **ADO.NET / Dapper puro** | Mayor control pero mucho código repetitivo; perdemos migraciones y el modelado del dominio con EF |

## Decisión

Se adopta **SQL Server 2022** como motor y **Entity Framework Core 10** como ORM.

- **Migraciones EF Core** para versionar el esquema (code-first).
- **Always Encrypted / cifrado a nivel columna** para datos personales sensibles (DNI,
  teléfono, correo) — ver [ADR-0004](ADR-0004-seguridad-proteccion-datos.md).
- **Transparent Data Encryption (TDE)** para cifrado en reposo de toda la base.
- Repositorios en `Infrastructure` que implementan los puertos definidos en `Application`.

## Consecuencias

**Positivas**
- Transacciones ACID nativas para operaciones críticas (pago + cambio de estado).
- Migraciones reproducibles y versionadas en Git.
- Capacidades de cifrado y auditoría a nivel de motor que apoyan el cumplimiento legal.

**Negativas / mitigaciones**
- EF Core puede generar consultas ineficientes (problema N+1). → Se controla con `Include`
  explícito, proyecciones y revisión de SQL generado (ver [errores conocidos](../errores-conocidos.md)).
- Acoplamiento al ecosistema Microsoft. → Aislado en `Infrastructure`; el dominio no
  depende de EF.
