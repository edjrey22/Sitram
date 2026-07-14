# Errores conocidos y soluciones — SITRAM

> Catálogo de problemas frecuentes (bugs, trampas y anti-patrones) detectados o previstos en
> el stack .NET + EF Core + PostgreSQL/Supabase, con su causa y solución probada. Se
> actualiza cada vez que se resuelve un incidente relevante.

## Índice

- [1. Dominio y concurrencia](#1-dominio-y-concurrencia)
- [2. Entity Framework Core](#2-entity-framework-core)
- [3. Seguridad y protección de datos](#3-seguridad-y-protección-de-datos)
- [4. API y async](#4-api-y-async)
- [5. Pruebas](#5-pruebas)

---

## 1. Dominio y concurrencia

### 1.1 Doble aprobación / transición de estado en condición de carrera
- **Síntoma**: dos funcionarios aprueban el mismo trámite casi a la vez; el expediente
  queda en estado inconsistente o se duplica la resolución.
- **Causa**: no hay control de concurrencia; ambas transacciones leen el mismo estado
  `EnRevision` antes de escribir.
- **Solución**: **concurrencia optimista** con columna `rowversion`/`xmin` en el agregado
  `Tramite`. EF Core lanza `DbUpdateConcurrencyException` si el registro cambió; se responde
  409 y se pide reintentar. La máquina de estados del agregado rechaza además transiciones
  inválidas.

### 1.2 Transición de estado inválida permitida
- **Síntoma**: un trámite pasa de `Borrador` a `Aprobado` saltándose la revisión.
- **Causa**: cambiar el estado desde fuera del agregado (setter público) sin validar.
- **Solución**: `EstadoTramite` con setter privado; solo métodos del agregado (`Aprobar()`,
  `Observar()`) cambian el estado y validan la transición, lanzando
  `TransicionInvalidaException` si no procede.

---

## 2. Entity Framework Core

### 2.1 Problema N+1 en consultas
- **Síntoma**: listar trámites dispara cientos de consultas SQL; la página tarda segundos.
- **Causa**: acceso perezoso (*lazy loading*) a navegaciones dentro de un bucle.
- **Solución**: carga explícita con `Include`/`ThenInclude`, o mejor, **proyección** a un DTO
  con `Select` para traer solo las columnas necesarias. Revisar el SQL generado con logging
  de EF en desarrollo.

### 2.2 `DbContext` es de vida corta pero se inyecta como Singleton
- **Síntoma**: excepciones `Cannot access a disposed object` o datos corruptos entre peticiones.
- **Causa**: `DbContext` no es *thread-safe* y se registró con el ciclo de vida equivocado.
- **Solución**: registrar `DbContext` como **Scoped** (una instancia por petición). Nunca
  Singleton. Para tareas en segundo plano, crear un *scope* propio.

### 2.3 Migraciones que se pierden o chocan
- **Síntoma**: "pending model changes" o migraciones duplicadas al trabajar en paralelo.
- **Causa**: dos ramas generan migraciones sobre el mismo modelo.
- **Solución**: generar migraciones solo desde `develop` actualizado; una migración por PR;
  nombrarlas con verbo + fecha (`AddAuditoriaTramite`). Revisar el `Migrations/` en el PR.

### 2.4 Fechas con zona horaria inconsistente
- **Síntoma**: la hora de un evento de auditoría difiere entre servidor y cliente.
- **Causa**: mezcla de `DateTime.Now` (local) y UTC.
- **Solución**: **todo en UTC** en el servidor y la BD (`DateTime.UtcNow` o
  `DateTimeOffset`); la conversión a hora local se hace solo en la presentación.

---

## 3. Seguridad y protección de datos

### 3.1 Datos personales filtrados en logs
- **Síntoma**: aparecen DNI, correo o teléfono en los archivos de log o en trazas de error.
- **Causa**: logging del objeto/petición completa sin enmascarar.
- **Solución**: política de logging con **Serilog** que enmascara campos sensibles (`***`);
  prohibido loguear cuerpos de petición con datos personales. Revisar en el checklist de PR.
  (Ver [ADR-0004](decisiones/ADR-0004-seguridad-proteccion-datos.md)).

### 3.2 Búsqueda imposible sobre columnas cifradas a nivel de aplicación
- **Síntoma**: la consulta por DNI cifrado con `LIKE` o rango falla o no devuelve resultados.
- **Causa**: el cifrado **aleatorio** (`CifradoColumna.CifrarAleatorio`, IV aleatorio por
  fila) no permite comparaciones ni siquiera de igualdad — el mismo texto plano produce
  cifrados distintos cada vez. Es el mismo problema que tendría *Always Encrypted* aleatorio
  en SQL Server (la alternativa original evaluada en
  [ADR-0003](decisiones/ADR-0003-sql-server-ef-core.md)/[ADR-0007](decisiones/ADR-0007-migracion-postgresql-supabase.md)).
- **Solución**: usar cifrado **determinista** (`CifrarDeterministico`, IV derivado por HMAC
  del propio texto plano) en columnas que requieran igualdad exacta (búsqueda por DNI); no
  cifrar con IV aleatorio columnas que necesiten `LIKE`/ordenación, o rediseñar la búsqueda
  (índice hash separado). Decidir por campo según su clasificación de datos.

### 3.6 Desajuste de `Cifrado:Clave` entre entornos deja registros ilegibles para siempre
- **Síntoma**: `CryptographicException: Padding is invalid` al leer un registro específico;
  en Blazor Server esto revienta el circuito SignalR y la página queda "congelada" hasta
  recargar.
- **Causa**: `CifradoColumna` es Singleton y deriva sus claves de `Cifrado:Clave` (User
  Secrets) al arrancar. Si un registro se escribió con un proceso que tenía una clave
  distinta a la actual (p. ej. tras cambiar de máquina/entorno sin migrar los User Secrets),
  ese registro queda huérfano — no hay forma de recuperar el texto plano sin la clave
  original.
- **Solución**: `Descifrar` ahora valida la longitud mínima y envuelve la excepción con un
  mensaje explícito que señala el desajuste de clave (`src/Sitram.Infrastructure/Persistence/Cifrado/CifradoColumna.cs`),
  en vez de un `Padding is invalid` opaco. Si ocurre, verificar con un script standalone
  contra la BD antes de asumir un bug de código; los registros afectados deben limpiarse
  manualmente (no hay recuperación posible).

### 3.3 Autorización solo en el frontend
- **Síntoma**: un usuario manipula la petición y ejecuta acciones sin permiso.
- **Causa**: ocultar botones en la UI pero no validar en la API (control de acceso roto,
  OWASP A01).
- **Solución**: **toda** operación protegida valida la política RBAC en el servidor
  (`[Authorize(Policy=...)]`). El frontend es solo conveniencia, nunca la barrera de seguridad.

### 3.4 JWT sin expiración o con secreto débil/hardcodeado
- **Síntoma**: tokens que nunca caducan o clave firmante en el repositorio.
- **Causa**: mala configuración de emisión de tokens.
- **Solución**: token de acceso de **vida corta** + refresh token rotativo; clave firmante
  larga y aleatoria en gestor de secretos, **nunca** en `appsettings` versionado.

### 3.5 Aviso de vulnerabilidad en dependencia transitiva sin fix compatible
- **Síntoma**: `dotnet build` reporta `NU1903` — `Microsoft.OpenApi` tiene una vulnerabilidad
  conocida (`GHSA-v5pm-xwqc-g5wc`).
- **Causa**: el aviso afecta a **toda la línea 2.x**; el paquete llega de forma **transitiva**
  desde `Microsoft.AspNetCore.OpenApi` (.NET 10), que depende de la 2.x. La corrección está en
  la 3.x, incompatible con la integración OpenAPI del framework actual.
- **Solución**: se fija el **último parche 2.x** (`Microsoft.OpenApi 2.4.2`) y se **acepta y
  rastrea** el aviso con el mecanismo oficial de auditoría de NuGet
  (`<NuGetAuditSuppress>` en `Directory.Build.props`), en lugar de silenciar toda la auditoría.
  **Revisar** cuando `Microsoft.AspNetCore.OpenApi` referencie una versión sana y retirar la
  supresión (OWASP A06 — componentes vulnerables).

---

## 4. API y async

### 4.1 *Deadlock* por bloqueo síncrono de código async
- **Síntoma**: la petición se cuelga indefinidamente bajo carga.
- **Causa**: `.Result` o `.Wait()` sobre una tarea async.
- **Solución**: **async de extremo a extremo**; `await` siempre. Prohibido `.Result`/`.Wait()`
  en el flujo de petición.

### 4.2 Respuestas de error que exponen detalles internos
- **Síntoma**: el cliente recibe *stack traces* o mensajes de SQL.
- **Causa**: excepciones sin manejar que llegan al cliente.
- **Solución**: **middleware global** de excepciones que devuelve `Problem Details`
  (RFC 7807) genérico al cliente y registra el detalle completo (sin datos personales) del
  lado servidor.

### 4.3 Sobre-publicación / *over-posting*
- **Síntoma**: un cliente envía campos extra (p. ej. `Estado`) y modifica datos que no debería.
- **Causa**: vincular directamente la entidad de dominio al cuerpo de la petición.
- **Solución**: recibir siempre **DTOs de entrada** específicos, nunca la entidad. Mapear
  explícitamente solo los campos permitidos.

---

## 5. Pruebas

### 5.1 Pruebas de integración que se contaminan entre sí
- **Síntoma**: tests que pasan solos pero fallan en conjunto (dependen del orden).
- **Causa**: comparten estado en la base de datos de prueba.
- **Solución**: **Respawn** para limpiar la BD entre pruebas; cada test prepara y verifica su
  propio estado (aislamiento).

### 5.2 Pruebas frágiles atadas a la implementación
- **Síntoma**: cualquier refactor rompe decenas de tests aunque el comportamiento no cambie.
- **Causa**: verificar detalles internos en vez de comportamiento observable.
- **Solución**: probar **comportamiento** (entradas → salidas y efectos), no la estructura
  interna; usar mocks solo para las dependencias externas (puertos), no para el dominio.

### 5.3 `WebApplicationFactory` recreando el esquema en paralelo entre clases de prueba
- **Síntoma**: pruebas de integración que pasan solas o en su propia clase, pero fallan
  (errores de conexión/esquema) al ejecutar la suite completa; no siempre el mismo test falla.
- **Causa**: cada clase de prueba con `IClassFixture<WebApplicationFactory<T>>` crea **su propia
  instancia** de la factory. Como xUnit ejecuta **clases de prueba en paralelo** por defecto,
  dos factories llaman `EnsureDeleted()`/`Migrate()` **a la vez** sobre la misma base de datos
  de prueba, y una borra el esquema mientras la otra lo usa.
- **Solución**: agrupar todas las clases que comparten la BD de prueba en una **única
  `ICollectionFixture`** (`[CollectionDefinition]` + `[Collection("...")]` en cada clase), de
  modo que la factory —y la recreación del esquema— se instancie **una sola vez** por ejecución.
  Complementa a 5.1 (Respawn): aquí el problema es la *recreación* del esquema, no solo el
  estado de los datos.

---

## Plantilla para nuevas entradas

```markdown
### X.Y Título corto del problema
- **Síntoma**: qué se observa.
- **Causa**: por qué ocurre.
- **Solución**: cómo se resuelve (probada).
```
