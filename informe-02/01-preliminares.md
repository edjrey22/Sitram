<!-- ================= PORTADA ================= -->

<div align="center">

**UNIVERSIDAD NACIONAL DE SAN CRISTÓBAL DE HUAMANGA**

**FACULTAD DE INGENIERÍA DE MINAS, GEOLOGÍA Y CIVIL**

**ESCUELA PROFESIONAL DE INGENIERÍA DE SISTEMAS**

<br>

*Curso: Pruebas y Aseguramiento de la Calidad de Software*

<br><br>

**PROYECTO**

## "Desarrollo de una plataforma web para la gestión de trámites municipales con enfoque en seguridad y protección de datos personales, 2026"

<br><br>

**PRESENTADO POR:**

Cuadros Reyes Edson Jorge

<br>

**DOCENTE:**

Mg. Ing. Richard Zapata Casaverde

<br><br>

Ayacucho – Perú

2026

</div>

<div style="page-break-after: always;"></div>

<!-- ================= DEDICATORIA ================= -->

## Dedicatoria

Dedico el presente trabajo a mis padres, quienes han sido mi pilar fundamental y me han
brindado su apoyo constante a lo largo de mis estudios universitarios; este logro es fruto
de su amor y sacrificio. Asimismo, expreso mi gratitud a mis docentes, por su generosidad al
compartir sus enseñanzas y fortalecer mis conocimientos durante mi formación profesional.
Finalmente, agradezco a mis amigos, por su compañía leal y su apoyo incondicional, quienes
hicieron posible que hoy alcance esta meta.

<div style="page-break-after: always;"></div>

<!-- ================= AGRADECIMIENTO ================= -->

## Agradecimiento

En primer lugar, agradezco a Dios, por haberme bendecido con la vida, guiar mi camino y darme
la fortaleza necesaria para perseverar y superar cada obstáculo a lo largo de mi formación
profesional. A mi alma máter, la Universidad Nacional de San Cristóbal de Huamanga, y a todos
los docentes de la Escuela Profesional de Ingeniería de Sistemas, por su compromiso académico
y por haberme impartido los conocimientos que hoy constituyen los cimientos de mi carrera. De
manera especial, al docente del curso, por su guía en el desarrollo de este proyecto.
Finalmente, mi gratitud eterna a mi familia, por su ejemplo de esfuerzo, su amor incondicional
y su apoyo constante, motor que me impulsó a cumplir esta meta.

<div style="page-break-after: always;"></div>

<!-- ================= RESUMEN ================= -->

## Resumen

El presente proyecto, de tipo **aplicada – tecnológica** y nivel **descriptivo**, tuvo como
objetivo general **desarrollar una plataforma web para la gestión de trámites municipales con
enfoque en seguridad y protección de datos personales**, materializada en un Producto Mínimo
Viable (MVP). Metodológicamente, se empleó **Spec-Driven Development (SDD)** como marco de
trabajo principal —en el que la especificación precede y guía al código—, complementado con
**Scrum** para la gestión iterativa de las fases de análisis, diseño e implementación, y con
prácticas de **Extreme Programming (XP)** para asegurar la calidad del código. A nivel
arquitectónico, la plataforma se estructuró bajo **Clean Architecture** y **Domain-Driven
Design (DDD)**, con **C# 14 sobre .NET 10** en el backend, una interfaz de usuario en **Blazor**
—con render interactivo en servidor, que mantiene la lógica y los datos personales fuera del
navegador— y **SQL Server 2022** como base de
datos relacional, incorporando controles de seguridad transversales: autenticación con JWT,
autorización basada en roles (RBAC), cifrado en tránsito (TLS) y en reposo (TDE/Always
Encrypted) y un registro de auditoría inmutable. La evaluación se diseñó sobre cuatro
variables: el proceso de desarrollo, el funcionamiento de los módulos críticos, el
cumplimiento de los controles de **seguridad y protección de datos** conforme a la **Ley
N.° 29733** y su reglamento (D.S. N.° 016-2024-JUS), y la usabilidad, esta última evaluada
mediante la **Escala de Usabilidad del Sistema (SUS)** sobre una muestra piloto. Los
instrumentos fueron sometidos a **validación por juicio de expertos** (V de Aiken) para
garantizar su credibilidad. Se concluye que la plataforma diseñada constituye una solución
tecnológica viable y segura que digitaliza el ciclo completo del trámite municipal,
promoviendo la transparencia, la trazabilidad y la protección de los datos de la ciudadanía.

**Palabras clave:** trámites municipales, gobierno digital, seguridad de la información,
protección de datos personales, Spec-Driven Development, Clean Architecture.

<div style="page-break-after: always;"></div>

<!-- ================= ABSTRACT ================= -->

## Abstract

This **applied–technological** and **descriptive** project aimed to **develop a web platform
for the management of municipal procedures with a focus on security and personal data
protection**, materialized as a Minimum Viable Product (MVP). Methodologically,
**Spec-Driven Development (SDD)** was used as the main framework —where the specification
precedes and guides the code—, complemented with **Scrum** for the iterative management of the
analysis, design, and implementation phases, and with **Extreme Programming (XP)** practices to
ensure code quality. Architecturally, the platform was structured using **Clean Architecture**
and **Domain-Driven Design (DDD)**, with **C# 14 on .NET 10** in the backend, a **Blazor**
front-end —using server-side interactive rendering, which keeps logic and personal data off the
browser— and **SQL Server 2022** as the relational database, incorporating cross-cutting security controls: JWT
authentication, role-based access control (RBAC), encryption in transit (TLS) and at rest
(TDE/Always Encrypted), and an immutable audit log. The evaluation was designed around four
variables: the development process, the operation of critical modules, compliance with
**security and personal data protection** controls under **Law No. 29733** and its regulation
(D.S. No. 016-2024-JUS), and usability, the latter assessed through the **System Usability
Scale (SUS)** on a pilot sample. The instruments were subjected to **expert judgment
validation** (Aiken's V) to ensure their credibility. It is concluded that the designed
platform constitutes a viable and secure technological solution that digitizes the complete
municipal procedure cycle, promoting transparency, traceability, and the protection of
citizens' data.

**Keywords:** municipal procedures, digital government, information security, personal data
protection, Spec-Driven Development, Clean Architecture.
