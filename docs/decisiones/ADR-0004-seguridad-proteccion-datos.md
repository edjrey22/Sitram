# ADR-0004: Estrategia de seguridad y protección de datos

- **Estado**: Aceptada
- **Fecha**: 2026-07-01
- **Decisores**: Equipo de proyecto

## Contexto

SITRAM procesa **datos personales** de la ciudadanía (nombres, DNI, dirección, teléfono,
correo) e incluso **datos que pueden considerarse sensibles** según el contexto del trámite.
Esto activa obligaciones legales bajo la **Ley N.° 29733 – Ley de Protección de Datos
Personales (Perú)** y su **nuevo reglamento D.S. N.° 016-2024-JUS** (vigente desde el
29-03-2024, que reemplaza al D.S. 003-2013-JUS), cuyos principios son análogos al GDPR
europeo:

- **Consentimiento** informado para el tratamiento de datos.
- **Finalidad**: los datos solo se usan para el trámite declarado.
- **Proporcionalidad / minimización**: se recolecta solo lo necesario.
- **Seguridad**: medidas técnicas adecuadas para evitar accesos no autorizados.
- **Derechos ARCO** + **portabilidad**: Acceso, Rectificación, Cancelación, Oposición y
  derecho a portar los datos en formato interoperable.
- **Oficial de Datos Personales**: rol responsable obligatorio.
- **Notificación de incidentes de seguridad** (brechas) a la autoridad y al titular.

> El reglamento de 2024 endureció el régimen sancionador: en 2024 se impusieron
> **S/ 13,4 millones** en multas (133 procedimientos, 454 entidades fiscalizadas), con
> topes de hasta **S/ 550 000**. Las entidades públicas **no están exentas**.

## Decisión

Se establece una estrategia de **defensa en profundidad** con los siguientes controles:

### 1. Cifrado
- **En tránsito**: HTTPS obligatorio con **TLS 1.3**; redirección forzada y HSTS.
- **En reposo**: cifrado de los volúmenes de almacenamiento a nivel de proveedor (Supabase,
  desde la migración documentada en [ADR-0007](ADR-0007-migracion-postgresql-supabase.md)).
- **A nivel columna**: cifrado **AES-256 a nivel de aplicación** (`CifradoColumna`) para
  campos sensibles (DNI, teléfono, correo) — funcionalmente equivalente a *Always
  Encrypted*, adoptado porque Always Encrypted real requiere un almacén de claves externo
  (Azure Key Vault/HSM) inexistente en el entorno académico (detalle en ADR-0007).
- **Contraseñas**: nunca en texto plano; *hashing* con **bcrypt/PBKDF2** (vía Identity).

### 2. Minimización y clasificación de datos
- Cada campo se clasifica: `Público`, `Interno`, `Personal`, `Sensible`.
- No se registran datos personales en los logs de aplicación (ver más abajo).

### 3. Auditoría
- Toda operación sobre un trámite genera un `EventoAuditoria` **inmutable** (quién, qué,
  cuándo, desde qué IP).
- Los logs de auditoría se separan de los logs técnicos y se retienen según política.

### 4. Registro seguro (logging)
- **Serilog** con *destructuring* que **enmascara** campos sensibles (`***`).
- Prohibido registrar el cuerpo completo de peticiones que contengan datos personales.

### 5. Derechos del titular (ARCO)
- **Acceso**: endpoint para que el ciudadano exporte sus datos.
- **Rectificación**: edición de datos personales con registro de auditoría.
- **Cancelación / derecho al olvido**: **borrado lógico** + anonimización de datos
  personales, conservando el expediente por obligación legal de archivo municipal.
- **Oposición**: gestión de consentimientos revocables.

### 6. Control de acceso
- **RBAC** con principio de **mínimo privilegio** (detalle en
  [ADR-0005](ADR-0005-autenticacion-autorizacion.md)).

## Consecuencias

**Positivas**
- Cumplimiento demostrable de la Ley 29733 → argumento fuerte para el informe.
- Reducción del impacto de una eventual brecha (datos cifrados e ilegibles).

**Negativas / mitigaciones**
- El cifrado determinista (usado para permitir búsqueda por igualdad en Dni/Correo) no
  permite `LIKE`/rangos sobre esas columnas, igual que *Always Encrypted*. → Se cifran solo
  los campos que no requieren búsqueda con cifrado aleatorio; se usa cifrado determinista
  donde se necesite igualdad exacta.
- Overhead de auditoría. → Escritura asíncrona y particionado de la tabla de auditoría.
- Complejidad adicional. → Documentada y encapsulada en `Infrastructure`.

## Referencias
- Ley N.° 29733, Ley de Protección de Datos Personales (Perú).
- **D.S. N.° 016-2024-JUS**, nuevo Reglamento de la Ley 29733 (vigente desde 2024).
- OWASP Top 10 y OWASP ASVS como marco de controles técnicos.
- Diagnóstico y evidencia en [analisis-problematica.md](../analisis-problematica.md).
