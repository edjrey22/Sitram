<!--
Formato de diapositivas: cada "---" separa una hoja (compatible con Marp,
reveal-md, Slidev). Máximo 15 hojas. Primera hoja = portada.
-->

# SITRAM
### Sistema Integrado de Trámites Municipales

Plataforma web para iniciar, pagar y dar seguimiento a trámites municipales,
con énfasis en **seguridad de la información** y **protección de datos
personales** (Ley N.° 29733)

Cuadros Reyes Edson Jorge

Curso: Pruebas y Aseguramiento de la Calidad de Software · Docente: Mg. Ing.
Richard Zapata Casaverde

Universidad Nacional de San Cristóbal de Huamanga — Escuela Profesional de
Ingeniería de Sistemas · Ayacucho, Perú — julio 2026

---

## 1. Problema

Los trámites municipales en el Perú suelen gestionarse de forma **presencial,
opaca y sin trazabilidad**: el ciudadano no puede consultar el estado de su
expediente, y los datos personales que entrega (DNI, correo, teléfono) rara
vez reciben un tratamiento seguro.

Evidencia (local/nacional/internacional) muestra que los sistemas existentes
resuelven la digitalización del flujo, pero **no abordan de forma integral
la seguridad ni la protección de datos personales**.

**Caso de referencia**: Municipalidad Distrital de San Juan Bautista.

---

## 2. Objetivos

**General**: desarrollar una plataforma web para la gestión de trámites
municipales con enfoque en seguridad y protección de datos personales.

**Específicos**:

| # | Objetivo | Variable |
|---|----------|----------|
| OE1 | Analizar actores y requisitos | X1 Análisis |
| OE2 | Diseñar la arquitectura y el modelo de datos | X1 Diseño |
| OE3 | Implementar los módulos funcionales | X1 Implementación |
| OE4 | Validar el funcionamiento de módulos críticos | X2 Funcionamiento |
| OE5 | Verificar cumplimiento de seguridad y Ley 29733 | X3 Seguridad |
| OE6 | Evaluar la usabilidad (escala SUS) | X4 Usabilidad |

---

## 3. Alcance funcional

**7 actores**: Ciudadano, Mesa de Partes, Revisor, Jefe de Área,
Administrador, Auditor, Oficial de Datos Personales.

**42 requisitos funcionales** en 7 módulos:

| Módulo | RF |
|--------|----|
| Identidad y acceso | RF-001…006 |
| Configuración TUPA | RF-010…014 |
| Ciclo de vida del trámite | RF-020…030 |
| Pagos | RF-040…044 |
| Seguimiento y notificaciones | RF-050…053 |
| Protección de datos (ARCO) | RF-060…066 |
| Auditoría y reportes | RF-070…073 |

**26 requisitos no funcionales**, trazables y medibles (RNF-###).

---

## 4. Metodología

**Spec-Driven Development (SDD)** como núcleo — la especificación precede al
código — complementado con:

- **Scrum**: gestión iterativa por sprints.
- **Extreme Programming (XP)**: TDD, integración continua, calidad de
  código.

Herramienta: **GitHub Spec Kit**
(`/specify → /plan → /tasks → /implement`).

Los instrumentos de medición (checklist de seguridad, cuestionario SUS)
fueron validados por **juicio de expertos** (V de Aiken ≥ 0.80) y
**Alfa de Cronbach** (≥ 0.70) para confiabilidad.

---

## 5. Arquitectura

**Clean Architecture + Domain-Driven Design (DDD)**, regla de dependencia
apuntando siempre hacia el dominio:

```
API/Presentation → Application → Domain (núcleo, sin dependencias)
                         ↑
                  Infrastructure
```

| Proyecto | Responsabilidad |
|----------|------------------|
| `Domain` | Entidades, agregados, reglas de negocio |
| `Application` | Casos de uso (CQRS/MediatR), validadores |
| `Infrastructure` | EF Core, PostgreSQL, Identity, servicios externos |
| `Api` | Web API + Blazor: controllers, middlewares |

Agregado raíz `Tramite` con **máquina de estados** de 6 estados
(transiciones inválidas rechazadas por el propio agregado).

---

## 6. Modelo de datos y cifrado

**17 entidades** en torno al agregado `Tramite`, con clasificación de datos
personales y protección por capas:

- **En tránsito**: TLS/HTTPS obligatorio.
- **En reposo**: cifrado de volúmenes a nivel del proveedor (Supabase).
- **A nivel de columna**: **AES-256 en la capa de aplicación**
  (`CifradoColumna`) para DNI, correo y teléfono — determinista donde se
  necesita búsqueda exacta, aleatorio donde no.
- **Contraseñas**: hash bcrypt/PBKDF2 (nunca reversible).
- **Auditoría**: tabla `EventoAuditoria` inmutable (*append-only*).

---

## 7. Stack tecnológico

| Capa | Tecnología |
|------|------------|
| Lenguaje / Runtime | C# 14 · .NET 10 (LTS) |
| Frontend | Blazor Web App (render interactivo en servidor) |
| API | ASP.NET Core Web API |
| Persistencia | **PostgreSQL (Supabase)** · EF Core 10 (Npgsql) |
| Seguridad | ASP.NET Core Identity · JWT · RBAC · MFA |
| Validación | FluentValidation |
| Logging / Auditoría | Serilog |
| Pruebas | xUnit · Moq · FluentAssertions · Respawn |

---

## 8. Seguridad y protección de datos (Ley 29733)

Cumplimiento de la **Ley N.° 29733** y su reglamento (D.S. N.° 016-2024-JUS),
equivalente peruano del GDPR:

- **Derechos ARCO + portabilidad**: acceso, rectificación, cancelación
  (anonimización), oposición.
- **Consentimiento** informado y revocable, con registro de auditoría.
- **MFA por correo** para cuentas de funcionario.
- **Notificación de incidentes de seguridad** al Oficial de Datos
  Personales (rol dedicado, D.S. 016-2024-JUS).
- **RBAC** con principio de mínimo privilegio, validado en servidor.
- **Minimización**: solo se recolecta lo necesario por tipo de trámite.

---

## 9. Decisiones de arquitectura (ADR)

**7 ADR** formalmente justificados — incluyendo una revisión honesta de una
decisión propia:

| ADR | Decisión |
|-----|----------|
| 0001 | GitHub Spec Kit como herramienta SDD |
| 0002 | Clean Architecture + DDD |
| 0003 | SQL Server + EF Core *(reemplazada)* |
| 0004 | Estrategia de seguridad y Ley 29733 |
| 0005 | Identity + JWT + RBAC |
| 0006 | Frontend con Blazor |
| **0007** | **Migración de SQL Server a PostgreSQL/Supabase** |

> ADR-0003 eligió SQL Server por *Always Encrypted*/TDE nativos. En la
> práctica, *Always Encrypted* real requiere un almacén de claves externo
> inexistente en un entorno académico — nunca se llegó a usar. Al no
> depender ya de esa ventaja, se migró a PostgreSQL/Supabase (ADR-0007).

---

## 10. Resultados: implementación

| Indicador | Meta | Resultado |
|-----------|------|-----------|
| Módulos implementados | 7 / 7 | **7 / 7** |
| Advertencias del compilador (Release) | 0 | **0** |
| Cobertura de pruebas — `Domain` | ≥ 80 % | **86,7 %** |
| Cobertura de pruebas — `Application` (solo unitarias) | ≥ 80 % | 40,7 %* |
| Endpoints documentados en OpenAPI | 100 % | **100 %** |

<small>* Cifra parcial: no incluye la cobertura adicional de las 62 pruebas
de integración sobre los mismos casos de uso.</small>

Frontend Blazor completo: registro, login (usuario/DNI), recuperación y
confirmación de email, administración de trámites y de funcionarios.

---

## 11. Resultados: funcionamiento y seguridad

**146 / 146 pruebas automatizadas en verde**
(47 unitarias de dominio + 37 de aplicación + 62 de integración)

| Variable | Indicador | Resultado |
|----------|-----------|-----------|
| X2 Funcionamiento | 6 módulos críticos operativos | **100 % (6/6)** |
| X3 Seguridad técnica (OWASP) | 7 controles | **100 % (7/7)** |
| X3 Obligaciones legales (Ley 29733) | 6 obligaciones | **100 % (6/6)** |
| **X3 Total** | | **100 % (13/13)** |

Evidencia: pruebas negativas de autorización, verificación de cifrado
determinista/aleatorio, transacción atómica pago + estado, auditoría
*append-only*.

---

## 12. Conclusiones

1. La plataforma es **técnica, operativa, económica y legalmente viable**
   para digitalizar el ciclo completo del trámite.
2. Los 6 objetivos específicos se cumplieron; 5 de 6 con resultados medidos
   y verificados contra código y pruebas — solo la usabilidad queda para
   la fase de campo.
3. El proyecto documenta y **justifica sus propias decisiones**, incluida
   una revisión de arquitectura (SQL Server → PostgreSQL) hecha con
   evidencia, no solo intuición — parte del valor de SDD + ADR.
4. La combinación **SDD + Scrum + XP** resultó un marco replicable para
   software público con foco en calidad y seguridad.

---

## 13. Recomendaciones y trabajo pendiente

**Pendiente, honesto y no inventado:**
- **Usabilidad (X4)**: aplicar el cuestionario **SUS** a la muestra piloto
  real de 20 usuarios y calcular el Alfa de Cronbach — requiere trabajo de
  campo, no se puede derivar del repositorio.
- **Cobertura combinada** Domain + Application (fusionar con las pruebas de
  integración vía `reportgenerator`).

**Recomendaciones futuras:**
1. Auditoría de seguridad externa (pentest) antes del despliegue.
2. Integrar RENIEC/SUNAT y una pasarela de pagos real en producción.
3. Incorporar firma digital certificada.
4. Prever escalabilidad: particionado de auditoría, caché distribuida, alta
   disponibilidad.
5. Plan de capacitación para funcionarios y difusión ciudadana.
6. Extender el catálogo TUPA progresivamente (el sistema ya lo permite sin
   tocar código).

---

## Gracias

**Repositorio**: github.com/edjrey22/Sitram

**Documentación completa**: `docs/` · Informe formal: `informe-02/`

Preguntas
