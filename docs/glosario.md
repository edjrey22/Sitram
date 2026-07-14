# Glosario — SITRAM

> Lenguaje ubicuo (DDD): los términos de este glosario se usan **igual** en el código, la
> documentación y las conversaciones con usuarios. Si algo se llama `Trámite` en el negocio,
> se llama `Tramite` en el código — nunca `Request`, `Proceso` u otro sinónimo.

## Términos del dominio municipal

| Término | Definición |
|---------|-----------|
| **Trámite** | Procedimiento administrativo que un ciudadano realiza ante la municipalidad (p. ej. licencia de funcionamiento, certificado de zonificación). Es el **agregado raíz** del sistema. |
| **Tipo de Trámite** | Plantilla que define los requisitos, documentos, tasa y flujo de aprobación de una clase de trámite. Configurable por el `Administrador`. |
| **Expediente** | Conjunto de datos, documentos y actuaciones asociados a un trámite concreto a lo largo de su ciclo de vida. |
| **Solicitud** | Acto por el cual un ciudadano inicia un trámite. Genera un expediente en estado `Recibido`. |
| **Estado del Trámite** | Punto del ciclo de vida del expediente: `Borrador`, `Recibido`, `EnRevision`, `Observado`, `Aprobado`, `Rechazado`. |
| **Observación** | Requerimiento del revisor para que el ciudadano corrija o complete su expediente. Pasa el trámite a estado `Observado`. |
| **Subsanación** | Acción del ciudadano de corregir una observación, devolviendo el trámite a `EnRevision`. |
| **Flujo de Aprobación** | Secuencia de pasos y responsables por los que pasa un trámite hasta resolverse. Puede ser multinivel. |
| **Mesa de Partes** | Unidad (y rol) que recepciona y valida la admisibilidad de los trámites entrantes. |
| **Tasa / Arancel** | Monto que el ciudadano paga por el trámite, definido en el TUPA. |
| **TUPA** | Texto Único de Procedimientos Administrativos. Documento oficial que lista los trámites de la municipalidad, sus requisitos y costos. |
| **Resolución** | Acto administrativo final que aprueba o rechaza el trámite. |
| **Constancia / Certificado** | Documento emitido como resultado de un trámite aprobado. |
| **Ciudadano** | Persona natural o jurídica que solicita trámites. Titular de datos personales. |
| **Funcionario** | Servidor municipal que interviene en el flujo (mesa de partes, revisor, jefe de área). |

## Términos técnicos y de arquitectura

| Término | Definición |
|---------|-----------|
| **SDD** | *Spec-Driven Development*. Metodología donde la especificación precede y guía al código. |
| **Clean Architecture** | Estilo arquitectónico en capas concéntricas donde las dependencias apuntan hacia el dominio. |
| **DDD** | *Domain-Driven Design*. Enfoque de diseño centrado en el modelo del dominio y su lenguaje. |
| **Agregado** | Grupo de entidades y value objects tratados como una unidad de consistencia. Tiene una **raíz** que controla el acceso. |
| **Entidad** | Objeto del dominio con identidad propia y ciclo de vida (p. ej. `Documento`). |
| **Value Object** | Objeto sin identidad, definido por sus atributos e inmutable (p. ej. `Dni`, `Dinero`). |
| **Evento de Dominio** | Hecho relevante del negocio ya ocurrido (p. ej. `TramiteAprobado`). |
| **CQRS** | *Command Query Responsibility Segregation*. Separa operaciones de escritura (commands) de lectura (queries). |
| **Command** | Operación que **cambia** el estado del sistema. |
| **Query** | Operación que **lee** el estado sin modificarlo. |
| **Handler** | Clase que ejecuta un command o query concreto. |
| **DTO** | *Data Transfer Object*. Objeto plano para transportar datos entre capas o hacia el cliente. |
| **Puerto / Adaptador** | Puerto = interfaz definida por la aplicación; Adaptador = su implementación en Infrastructure. |
| **Repositorio** | Abstracción para persistir y recuperar agregados. |
| **Migración** | Cambio versionado del esquema de base de datos (EF Core). |
| **Middleware** | Componente del pipeline HTTP que procesa cada petición (auth, auditoría, errores). |

## Términos de seguridad y protección de datos

| Término | Definición |
|---------|-----------|
| **Ley 29733** | Ley de Protección de Datos Personales del Perú. Marco legal que rige el tratamiento de datos personales. |
| **Datos personales** | Toda información sobre una persona natural que la identifica o la hace identificable. |
| **Datos sensibles** | Subcategoría con protección reforzada (salud, biometría, etc.). Se minimiza su uso. |
| **Derechos ARCO** | Acceso, Rectificación, Cancelación y Oposición: derechos del titular sobre sus datos. |
| **Consentimiento** | Autorización informada, previa y expresa del titular para tratar sus datos. |
| **Minimización** | Principio de recolectar solo los datos estrictamente necesarios para la finalidad. |
| **RBAC** | *Role-Based Access Control*. Control de acceso basado en roles y permisos. |
| **Mínimo privilegio** | Cada actor tiene solo los permisos imprescindibles para su función. |
| **JWT** | *JSON Web Token*. Token firmado que transporta la identidad y permisos del usuario. |
| **TDE** | *Transparent Data Encryption*. Cifrado en reposo de toda la base de datos a nivel de motor (capacidad de SQL Server, evaluada en [ADR-0003](decisiones/ADR-0003-sql-server-ef-core.md); SITRAM usa en su lugar el cifrado de volúmenes del proveedor gestionado, ver [ADR-0007](decisiones/ADR-0007-migracion-postgresql-supabase.md)). |
| **Always Encrypted** | Cifrado a nivel de columna para datos sensibles, nativo de SQL Server. SITRAM implementa el equivalente funcional a nivel de aplicación (`CifradoColumna`, AES-256) — ver ADR-0007. |
| **Supabase** | Plataforma gestionada sobre PostgreSQL (base de datos, autenticación, almacenamiento) usada como motor de persistencia de SITRAM desde ADR-0007. |
| **Auditoría** | Registro inmutable de quién hizo qué, cuándo y desde dónde. |
| **Anonimización** | Transformación de datos personales para que dejen de identificar al titular (usada en el derecho al olvido). |
| **OWASP** | Organización de referencia en seguridad de aplicaciones; su *Top 10* guía nuestros controles. |
