# Requisitos — SITRAM

> Base contractual del proyecto. Cada requisito es **atómico, verificable y trazable**.
> Nomenclatura: `RF-###` (funcional), `RNF-###` (no funcional). La columna *Prioridad* usa
> **MoSCoW**: **M** (Must), **S** (Should), **C** (Could), **W** (Won't, por ahora).
> Estos requisitos alimentan las especificaciones SDD (`/specify`) y el Capítulo I del informe.

## 1. Actores del sistema

| Actor | Descripción |
|-------|-------------|
| **Ciudadano** | Persona natural o jurídica que inicia y sigue sus trámites. Titular de datos. |
| **Mesa de Partes** | Funcionario que recepciona y verifica admisibilidad de los trámites. |
| **Revisor** | Funcionario que evalúa técnicamente el expediente. |
| **Jefe de Área** | Funcionario que aprueba o rechaza el trámite. |
| **Administrador** | Configura tipos de trámite, tasas, roles y parámetros. |
| **Auditor** | Consulta (solo lectura) el registro de auditoría. |
| **Oficial de Datos Personales** | Responsable legal (D.S. 016-2024-JUS) de atender solicitudes ARCO e incidentes de seguridad de datos. |
| **Sistema** | Actor automático: notificaciones, vencimientos, tareas programadas. |

> **Nota sobre el conteo.** Se distinguen **siete actores de negocio** (con rol de usuario y
> privilegios propios: Ciudadano, Mesa de Partes, Revisor, Jefe de Área, Administrador, Auditor
> y Oficial de Datos Personales) del actor **Sistema**, que es un agente **automático** y no un
> usuario. Por ello el informe reporta **siete actores** (los de negocio); *Sistema* se modela
> aparte por ser el disparador de acciones programadas.

---

## 2. Requisitos funcionales (RF)

### 2.1 Gestión de identidad y acceso

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-001 | El sistema debe permitir el **registro** de un ciudadano con verificación de correo. | Ciudadano | M |
| RF-002 | El sistema debe permitir **iniciar sesión** con credenciales y emitir un JWT. | Todos | M |
| RF-003 | El sistema debe **bloquear la cuenta** tras 5 intentos fallidos consecutivos. | Sistema | M |
| RF-004 | El sistema debe permitir **recuperar la contraseña** mediante enlace temporal al correo. | Ciudadano | M |
| RF-005 | El sistema debe exigir **segundo factor (MFA)** para cuentas de funcionario. | Funcionarios | S |
| RF-006 | El administrador debe poder **crear, editar y desactivar** cuentas de funcionario y asignarles rol. | Administrador | M |

### 2.2 Configuración de trámites (TUPA)

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-010 | El administrador debe poder **crear un tipo de trámite** con nombre, descripción, área responsable y tasa. | Administrador | M |
| RF-011 | El administrador debe poder definir los **documentos requeridos** por tipo de trámite. | Administrador | M |
| RF-012 | El administrador debe poder definir el **flujo de aprobación** (pasos y roles) por tipo de trámite. | Administrador | M |
| RF-013 | El administrador debe poder **activar/desactivar** un tipo de trámite sin borrarlo (borrado lógico). | Administrador | M |
| RF-014 | El sistema debe mostrar al ciudadano el **catálogo de trámites** disponibles con sus requisitos y costo. | Ciudadano | M |

### 2.3 Ciclo de vida del trámite

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-020 | El ciudadano debe poder **iniciar un trámite** seleccionando un tipo y completando el formulario. | Ciudadano | M |
| RF-021 | El ciudadano debe poder **adjuntar documentos** (PDF/imagen) al expediente. | Ciudadano | M |
| RF-022 | El sistema debe **guardar como borrador** un trámite no enviado y permitir retomarlo. | Ciudadano | S |
| RF-023 | El ciudadano debe poder **enviar** el trámite, pasándolo a estado `Recibido`. | Ciudadano | M |
| RF-024 | Mesa de Partes debe poder **verificar admisibilidad** y aceptar o rechazar la recepción. | Mesa de Partes | M |
| RF-025 | El revisor debe poder **evaluar** el expediente y registrar el resultado. | Revisor | M |
| RF-026 | El revisor debe poder **observar** el trámite indicando qué debe subsanar el ciudadano. | Revisor | M |
| RF-027 | El ciudadano debe poder **subsanar** una observación y reenviar el expediente. | Ciudadano | M |
| RF-028 | El jefe de área debe poder **aprobar o rechazar** el trámite, generando una resolución. | Jefe de Área | M |
| RF-029 | El sistema debe **impedir transiciones de estado inválidas** (máquina de estados). | Sistema | M |
| RF-030 | El sistema debe **generar el documento resultante** (constancia/certificado) al aprobar. | Sistema | S |

### 2.4 Pagos

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-040 | El sistema debe **calcular la tasa** correspondiente al tipo de trámite. | Sistema | M |
| RF-041 | El ciudadano debe poder **registrar el pago** de la tasa (integración con pasarela). | Ciudadano | M |
| RF-042 | El sistema debe **confirmar el pago** y el cambio de estado de forma atómica (transacción). | Sistema | M |
| RF-043 | El sistema debe **impedir el avance** de un trámite con tasa impaga. | Sistema | M |
| RF-044 | El ciudadano debe poder **descargar el comprobante** de pago. | Ciudadano | S |

### 2.5 Seguimiento y notificaciones

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-050 | El ciudadano debe poder **consultar el estado** de sus trámites en tiempo real. | Ciudadano | M |
| RF-051 | El sistema debe **notificar por correo** cada cambio de estado relevante. | Sistema | M |
| RF-052 | El ciudadano debe poder ver el **historial de actuaciones** de su expediente. | Ciudadano | S |
| RF-053 | El sistema debe **alertar** al ciudadano cuando un trámite observado esté por vencer. | Sistema | C |

### 2.6 Protección de datos (derechos ARCO — Ley 29733)

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-060 | El ciudadano debe poder **exportar sus datos personales** en un **formato interoperable** (derecho de acceso y **portabilidad**, D.S. 016-2024-JUS). | Ciudadano | M |
| RF-061 | El ciudadano debe poder **rectificar** sus datos personales, con registro de auditoría. | Ciudadano | M |
| RF-062 | El ciudadano debe poder solicitar la **cancelación/anonimización** de sus datos (derecho al olvido), conservando el expediente por obligación legal. | Ciudadano | M |
| RF-063 | El sistema debe **registrar el consentimiento** informado del ciudadano al tratar sus datos. | Sistema | M |
| RF-064 | El ciudadano debe poder **revocar el consentimiento** (derecho de oposición). | Ciudadano | S |
| RF-065 | El sistema debe **detectar y notificar incidentes de seguridad** (brechas de datos) al Oficial de Datos Personales y dejar constancia, conforme al D.S. 016-2024-JUS. | Sistema | M |
| RF-066 | El sistema debe permitir **designar y registrar un Oficial de Datos Personales** con acceso a incidentes y solicitudes ARCO. | Administrador | S |

### 2.7 Auditoría y reportes

| ID | Requisito | Actor | Prioridad |
|----|-----------|-------|-----------|
| RF-070 | El sistema debe **registrar en auditoría** toda acción sobre un trámite (quién, qué, cuándo, IP). | Sistema | M |
| RF-071 | El auditor debe poder **consultar y filtrar** el registro de auditoría (solo lectura). | Auditor | M |
| RF-072 | El jefe de área debe poder generar **reportes** de trámites por estado, tipo y periodo. | Jefe de Área | S |
| RF-073 | El registro de auditoría debe ser **inmutable** (no editable ni borrable desde la aplicación). | Sistema | M |

---

## 3. Requisitos no funcionales (RNF)

> Los RNF son **medibles**: cada uno indica cómo se verifica.

### 3.1 Seguridad

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-001 | Toda comunicación debe usar **HTTPS/TLS 1.3**; HTTP se redirige a HTTPS. | Escaneo TLS; sin endpoints en HTTP. | M |
| RNF-002 | Las contraseñas se almacenan con **hash bcrypt/PBKDF2**, nunca en texto plano. | Revisión de BD y código. | M |
| RNF-003 | Los datos personales sensibles (DNI, teléfono, correo) se **cifran a nivel columna** con AES-256 a nivel de aplicación (equivalente funcional a Always Encrypted; ver [ADR-0007](decisiones/ADR-0007-migracion-postgresql-supabase.md)). | Inspección de esquema y de `CifradoColumna`. | M |
| RNF-004 | La base de datos completa se cifra en reposo (cifrado de volúmenes gestionado por el proveedor, Supabase). | Configuración/documentación de Supabase. | M |
| RNF-005 | El control de acceso se valida **en el servidor** en cada operación protegida (RBAC). | Pruebas de autorización negativas. | M |
| RNF-006 | El sistema no debe exponer *stack traces* ni detalles internos al cliente. | Pruebas de error → Problem Details. | M |
| RNF-007 | El sistema debe cumplir los controles aplicables del **OWASP Top 10 / ASVS nivel 2**. | Checklist OWASP + revisión. | S |
| RNF-008 | Los tokens de acceso deben expirar en **≤ 15 min**; los refresh tokens rotan. | Configuración JWT. | M |

### 3.2 Protección de datos (Ley 29733)

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-010 | **Ningún dato personal** debe aparecer en logs de aplicación (enmascarado con `***`). | Inspección de logs. | M |
| RNF-011 | Se recolectan **solo los datos necesarios** para cada trámite (minimización). | Revisión de formularios. | M |
| RNF-012 | Los entornos distintos de producción usan **datos anonimizados o sintéticos**. | Política de datos por entorno. | M |
| RNF-013 | Las solicitudes de derechos ARCO se atienden en **≤ 10 días hábiles**. | Registro de solicitudes. | S |

### 3.3 Rendimiento

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-020 | Las consultas de estado de trámite responden en **< 500 ms** (p95). | Pruebas de carga. | S |
| RNF-021 | El sistema soporta **200 usuarios concurrentes** sin degradación notable. | Prueba de carga con JMeter/k6. | S |
| RNF-022 | La subida de un documento de hasta **10 MB** se procesa en **< 5 s**. | Prueba manual/automatizada. | C |

### 3.4 Disponibilidad y fiabilidad

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-030 | Disponibilidad objetivo del **99 %** en horario de atención. | Monitoreo de *uptime*. | S |
| RNF-031 | **Respaldos** automáticos diarios de la base de datos con retención de 30 días. | Verificación de backups. | M |
| RNF-032 | Las operaciones de pago + cambio de estado son **transaccionales** (todo o nada). | Pruebas de integración. | M |

### 3.5 Usabilidad

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-040 | La interfaz debe ser **responsiva** (móvil y escritorio), diseño *mobile-first*. | Prueba en dispositivos. | M |
| RNF-041 | La usabilidad se evalúa con la escala **SUS**, objetivo **≥ 70** puntos. | Encuesta a usuarios piloto. | S |
| RNF-042 | Los mensajes de error son **claros y accionables**, en español. | Revisión de UX. | S |

### 3.6 Mantenibilidad

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-050 | Cobertura de pruebas **≥ 80 %** en las capas `Domain` y `Application`. | Reporte de cobertura en CI. | M |
| RNF-051 | El código respeta las [convenciones](convenciones.md) y compila **sin advertencias** en Release. | Gate de CI. | M |
| RNF-052 | Toda decisión arquitectónica relevante queda documentada como **ADR**. | Revisión de `decisiones/`. | S |

### 3.7 Portabilidad y compatibilidad

| ID | Requisito | Métrica / Verificación | Prioridad |
|----|-----------|------------------------|-----------|
| RNF-060 | El backend corre sobre **.NET 10** en Windows y Linux (contenedores). | Build multiplataforma. | S |
| RNF-061 | La API expone documentación **OpenAPI**. | Endpoint `/openapi/v1.json` disponible en Development. | M |

---

## 4. Trazabilidad (requisito → diseño)

| Requisitos | Se materializan en |
|------------|--------------------|
| RF-020…RF-030 | Agregado `Tramite` y máquina de estados ([arquitectura](arquitectura.md)) |
| RF-060…RF-064, RNF-010…RNF-013 | [ADR-0004 Seguridad y protección de datos](decisiones/ADR-0004-seguridad-proteccion-datos.md) |
| RF-001…RF-006, RNF-005, RNF-008 | [ADR-0005 Autenticación y autorización](decisiones/ADR-0005-autenticacion-autorizacion.md) |
| RF-040…RF-044, RNF-032 | Transacciones ACID ([ADR-0003](decisiones/ADR-0003-sql-server-ef-core.md)) |
| RF-070…RF-073 | Tabla de auditoría inmutable (modelo de datos) |

---

## 5. Fuera de alcance (por ahora)

| ID | Excluido | Motivo |
|----|----------|--------|
| RF-W01 | Pasarela de pagos con integración bancaria real | Se simula/integra en modo prueba (alcance MVP) |
| RF-W02 | App móvil nativa | La web responsiva cubre el objetivo |
| RF-W03 | Firma digital certificada (con entidad certificadora) | Complejidad legal fuera del alcance académico |
| RF-W04 | Interoperabilidad con RENIEC/SUNAT en vivo | Se documenta como línea futura |
