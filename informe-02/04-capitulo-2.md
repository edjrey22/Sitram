# Capítulo II

## Marco Teórico

### 2.1 Antecedentes de la investigación

> **Nota para el autor:** los antecedentes se describen a partir de estudios reales
> localizados en repositorios académicos (Alicia–Concytec, RENATI–Sunedu y repositorios
> institucionales). Antes de la entrega final, completa el **apellido del autor y el año
> exacto** en formato APA a partir del enlace indicado en cada caso (ver Referencias). Los
> datos de la publicación (título, institución, URL) ya están verificados.

Los antecedentes constituyen el conjunto de investigaciones previas que abordan problemáticas
o soluciones similares a la del presente proyecto. Su revisión permite conocer el estado del
arte, identificar metodologías empleadas y delimitar el aporte diferencial de este trabajo. Se
organizan en antecedentes nacionales e internacionales.

#### 2.1.1 Antecedentes nacionales

**a) Sistema web de trámite documentario – UGEL Tambopata (2020).** El objetivo de esta
investigación fue desarrollar un sistema web para la mejora de los servicios a los usuarios de
la Unidad de Gestión Educativa Local de Tambopata. Metodológicamente, empleó el Proceso
Unificado Racional (RUP), el patrón arquitectónico Modelo-Vista-Controlador (MVC), el lenguaje
JavaScript y el framework Angular. La calidad del software se evaluó con la norma ISO 9126,
obteniendo valores de satisfacción entre "regular" y "satisfactorio" (funcionalidad 3,20;
fiabilidad 3,26; portabilidad 3,59; eficiencia 3,61; promedio 3,41). Este antecedente respalda
la pertinencia de digitalizar la gestión documentaria pública y de evaluar la calidad del
software con estándares internacionales; se diferencia del presente proyecto en que no aborda
la seguridad ni la protección de datos personales de forma integral.

**b) Sistema web basado en Scrum para autorizaciones e infracciones – Dirección Regional de
Transportes y Comunicaciones de Ayacucho.** Este estudio, desarrollado en el ámbito regional de
Ayacucho, tuvo como objetivo mejorar el proceso de autorizaciones e infracciones mediante un
sistema web construido con la metodología ágil Scrum. Constituye un antecedente especialmente
relevante por su cercanía geográfica y por evidenciar la aplicabilidad de las metodologías
ágiles a los procesos administrativos del sector público en la misma región donde se
contextualiza el presente proyecto.

**c) Sistema informático de gestión de trámite documentario – Gobierno Regional de San Martín,
Moyobamba (2022).** El estudio tuvo como objetivo reducir el tiempo de la gestión de trámites
documentarios mediante un sistema web construido con la metodología ágil Scrum. Aporta
evidencia cuantitativa sobre la eficacia de las metodologías ágiles para la mejora de tiempos
en procesos administrativos del Estado, aspecto directamente relacionado con el objetivo de
eficiencia del presente proyecto.

**d) Sistema web para la emisión de licencias de funcionamiento – Municipalidad de Puente
Piedra.** Esta investigación desarrolló un sistema web para la emisión de licencias de
funcionamiento de negocios de riesgo bajo o medio. Es un antecedente de alta pertinencia porque
la licencia de funcionamiento es precisamente uno de los trámites municipales típicos que la
plataforma SITRAM digitaliza; confirma la viabilidad y la demanda de digitalizar este tipo de
procedimientos en el ámbito municipal peruano.

**e) Sistema web de gestión de trámite documentario – Municipalidad Distrital de
Bellavista-Sullana (2016).** Se implementó un sistema web para optimizar los procesos del área
de mesa de partes, dentro de la línea de investigación de aplicación de las TIC para la mejora
continua de la calidad en las organizaciones del Perú. Confirma que el área de mesa de partes
es un punto crítico de digitalización en las municipalidades, coincidente con el diseño de
SITRAM.

**f) Modelo de ciberseguridad para prevenir ciberataques en aplicaciones web (Perú).** Diversos
trabajos nacionales han abordado modelos de madurez en ciberseguridad basados en estándares
como ISO/IEC 27032 y el marco del NIST para proteger aplicaciones web y activos de información.
Estos estudios fundamentan la necesidad de incorporar la seguridad de la información como un
eje transversal del desarrollo —enfoque que el presente proyecto adopta desde el diseño—, y
evidencian que la seguridad suele tratarse de forma aislada y no integrada al ciclo de
desarrollo, brecha que SITRAM busca cubrir.

#### 2.1.2 Antecedentes internacionales

**a) "Por Mi Barrio", Montevideo, Uruguay (Aguerre et al., 2024).** Estudio de gobernanza
publicado en el *Information Systems Journal* sobre una plataforma web y móvil de código abierto
lanzada en 2014, que permite a los ciudadanos reportar y consultar el estado de reparación de
problemas urbanos. Demuestra el valor de las plataformas cívicas de gobierno abierto para la
trazabilidad y la participación ciudadana, principios que SITRAM incorpora al permitir el
seguimiento del estado del trámite en tiempo real.

**b) Confianza ciudadana en estrategias de e-government: municipio rural de Sudáfrica (2024).**
Esta investigación analizó las perspectivas de los ciudadanos sobre los servicios electrónicos
en un municipio rural, concluyendo que los niveles de confianza pública en el gobierno
electrónico están estrechamente vinculados a la percepción de las medidas de privacidad y
seguridad implementadas. Fundamenta la decisión de este proyecto de priorizar la seguridad y la
protección de datos como factores determinantes de la adopción.

**c) E-government y confianza pública en países en desarrollo: revisión sistemática
(International Journal of Public Administration, 2025).** Síntesis de la literatura que confirma
que el gobierno electrónico promueve la confianza mediante la mejora de la prestación de
servicios, la transparencia y la participación, aunque su impacto depende de la fiabilidad
institucional, la confianza tecnológica y las condiciones contextuales. Advierte que el bajo
desempeño, la escasa alfabetización digital y los conflictos de valores minan la confianza
sostenida.

**d) Interoperabilidad confiable de sistemas de identidad digital en países en desarrollo
(Springer, AI & Society, 2024).** Aborda las consideraciones para una interoperabilidad
transfronteriza confiable de los sistemas de identidad digital, destacando el papel de los
Sistemas de Identidad Fundacional para la identificación y autenticación de los ciudadanos.
Sustenta la importancia de un módulo de gestión de identidad robusto, componente central de
SITRAM.

**e) Evaluación de portales web de servicios al ciudadano en América Latina.** Revisión
sistemática que analizó la calidad metodológica de la evaluación de usabilidad centrada en el
usuario de aplicaciones digitales gubernamentales, a partir de estudios indexados en Web of
Science, Scopus e IEEE Xplore. Fundamenta la importancia de evaluar la usabilidad de las
plataformas públicas —variable considerada en este proyecto mediante la escala SUS.

**f) Impacto del gobierno digital en la modernización del Estado (revisión sistemática).**
Trabajo que sintetiza la relación entre gobierno digital, modernización del Estado y servicios
al ciudadano en gobiernos locales de América Latina, confirmando que la digitalización mejora
el acceso a la información pública y la eficiencia de los servicios.

### 2.2 Bases teóricas

#### 2.2.1 Gobierno digital (e-government)

El gobierno digital, o gobierno electrónico, es el uso de las tecnologías de la información y la
comunicación (TIC), en especial de internet, para mejorar la prestación de los servicios
públicos, la transparencia, la eficiencia y la participación ciudadana. La Organización de las
Naciones Unidas, a través de su Índice de Desarrollo de Gobierno Electrónico (EGDI), mide su
avance considerando tres dimensiones: los servicios en línea, la infraestructura de
telecomunicaciones y el capital humano.

La literatura distingue **niveles o fases de madurez** del gobierno electrónico, que van desde
la simple **presencia** informativa (sitios web que publican información), pasando por la
**interacción** (formularios descargables, comunicación bidireccional) y la **transacción**
(realización completa de trámites en línea, incluido el pago), hasta la **integración** o
transformación (interoperabilidad total entre entidades). El presente proyecto se sitúa en el
nivel **transaccional**, el más exigente y el que mayor valor aporta al ciudadano, ya que
permite completar el ciclo del trámite sin presencialidad.

En el Perú, el gobierno digital se rige por el **Decreto Legislativo N.° 1412, Ley de Gobierno
Digital**, que establece el marco de gobernanza, los principios de interoperabilidad y la
obligación de las entidades públicas de transformar digitalmente sus servicios, bajo la
rectoría de la Secretaría de Gobierno y Transformación Digital de la Presidencia del Consejo de
Ministros.

#### 2.2.2 Trámite administrativo y TUPA

Un **procedimiento administrativo** o **trámite** es el conjunto de actos y diligencias que un
administrado realiza ante una entidad pública para obtener un pronunciamiento o servicio
(una licencia, un permiso, un certificado, una constancia). En el ámbito municipal, estos
procedimientos están regulados por el **Texto Único de Procedimientos Administrativos (TUPA)**,
documento oficial de gestión que compendia todos los procedimientos de iniciativa de parte,
estableciendo para cada uno sus requisitos, plazos, calificación (automática o de evaluación
previa) y el costo o **tasa** correspondiente, expresado en función de la Unidad Impositiva
Tributaria (UIT). El TUPA constituye la fuente normativa que la plataforma SITRAM digitaliza y
parametriza en su módulo de configuración de tipos de trámite.

#### 2.2.3 Corrupción administrativa y transparencia

La corrupción administrativa se entiende como el abuso del poder público para beneficio
privado. En el nivel de la atención al ciudadano, adopta la forma de la denominada *petty
corruption* o pequeña corrupción: pagos indebidos para agilizar trámites, tratos preferenciales
y discrecionalidad en la aplicación de las normas. La **transparencia** —entendida como la
apertura y disponibilidad de la información sobre los actos públicos— y la **trazabilidad** —el
registro completo y verificable de cada actuación— son los mecanismos fundamentales para
combatirla. La digitalización de los trámites, al eliminar el contacto discrecional y generar
un rastro auditable, es una herramienta reconocida de lucha contra la corrupción.

#### 2.2.4 Aplicación web

Una **aplicación web** es un software basado en la arquitectura **cliente-servidor**, cuyo
cliente se ejecuta en un navegador y se comunica con el servidor mediante el protocolo **HTTP/
HTTPS**. Las arquitecturas modernas desacoplan el **frontend** (capa de presentación que se
ejecuta en el navegador) del **backend** (capa de lógica de negocio y acceso a datos que se
ejecuta en el servidor), comunicándose a través de una **API REST** (*Representational State
Transfer*), un estilo de arquitectura que utiliza los métodos HTTP (GET, POST, PUT, DELETE) para
operar sobre recursos identificados por URLs. Este desacoplamiento favorece la escalabilidad, el
mantenimiento y la posibilidad de servir a múltiples clientes (web, móvil) desde un mismo
backend. Las aplicaciones que cargan una única página y actualizan su contenido dinámicamente se
denominan **SPA** (*Single Page Application*).

#### 2.2.5 Ingeniería de software y metodologías de desarrollo

La ingeniería de software es la aplicación de un enfoque sistemático, disciplinado y
cuantificable al desarrollo, operación y mantenimiento del software. El presente proyecto
integra tres metodologías complementarias.

##### 2.2.5.1 Spec-Driven Development (SDD)

El Desarrollo Guiado por Especificaciones (SDD) es una metodología en la que la
**especificación es la fuente de verdad** y precede al código. Primero se define, en lenguaje de
negocio, *qué* debe hacer el sistema y *por qué*; solo después se determina *cómo*
implementarlo. Herramientas como **GitHub Spec Kit** estructuran este flujo en cuatro etapas
—`/specify` (especificar), `/plan` (planificar el diseño), `/tasks` (descomponer en tareas) e
`/implement` (implementar)—, garantizando la trazabilidad entre la necesidad, el diseño y el
código. El SDD reduce la ambigüedad, evita el desarrollo de funcionalidades no solicitadas y
produce una documentación viva que sirve de evidencia del proceso. Es el marco metodológico
**principal** de este proyecto.

##### 2.2.5.2 Scrum

Scrum es un marco de trabajo ágil para la gestión de proyectos complejos, basado en el
desarrollo **iterativo e incremental** a través de ciclos cortos denominados *sprints*
(típicamente de dos a cuatro semanas). Define:

- **Roles:** el *Product Owner* (responsable de maximizar el valor y gestionar el Product
  Backlog), el *Scrum Master* (facilitador que garantiza la correcta aplicación del marco) y el
  *equipo de desarrollo* (autoorganizado y multifuncional).
- **Eventos:** la *planificación del sprint* (Sprint Planning), la *reunión diaria* (Daily
  Scrum), la *revisión del sprint* (Sprint Review) y la *retrospectiva* (Sprint Retrospective).
- **Artefactos:** el *Product Backlog* (lista priorizada de requisitos como historias de
  usuario), el *Sprint Backlog* (trabajo comprometido para el sprint) y el *Incremento*
  (producto potencialmente entregable al final de cada sprint).

En este proyecto, Scrum gestiona iterativamente las fases de análisis, diseño e implementación.

##### 2.2.5.3 Extreme Programming (XP)

Extreme Programming (XP), formulada por Kent Beck, es una metodología ágil centrada en la
**excelencia técnica y la calidad del código**. Promueve prácticas de ingeniería como el
**desarrollo guiado por pruebas** (*Test-Driven Development*, TDD), la **integración continua**,
el **diseño simple**, la **refactorización** constante, los **estándares de codificación** y la
**programación en parejas**. XP complementa a Scrum: mientras este gestiona el *qué* y el
*cuándo*, XP aporta la disciplina técnica del *cómo*, asegurando un software mantenible y de
alta calidad.

#### 2.2.6 Arquitectura de software

La arquitectura de software es la estructura fundamental de un sistema, compuesta por sus
componentes, las relaciones entre ellos y los principios que gobiernan su diseño y evolución.

##### 2.2.6.1 Clean Architecture

Propuesta por Robert C. Martin (2017), la **Arquitectura Limpia** organiza el software en capas
concéntricas: en el centro, las entidades y reglas de negocio; alrededor, los casos de uso; y en
las capas externas, los detalles técnicos (frameworks, base de datos, interfaz de usuario). Su
principio rector es la **regla de dependencia**: las dependencias del código fuente solo pueden
apuntar hacia adentro, hacia las políticas de mayor nivel. Esto aísla las reglas de negocio de
los detalles de implementación, favoreciendo la testabilidad, la independencia tecnológica y la
mantenibilidad. Es el estilo arquitectónico base de SITRAM.

##### 2.2.6.2 Domain-Driven Design (DDD)

El Diseño Guiado por el Dominio (DDD), formulado por Eric Evans (2003), propone modelar el
software en torno al dominio del negocio y su **lenguaje ubicuo** (*ubiquitous language*): un
vocabulario común y riguroso compartido entre los expertos del dominio y los desarrolladores,
que se refleja directamente en el código. Sus **patrones tácticos** incluyen: la **entidad**
(objeto con identidad propia), el **objeto de valor** (definido por sus atributos e inmutable),
el **agregado** (grupo de objetos tratados como una unidad de consistencia, con una raíz que
controla el acceso), el **evento de dominio** (hecho relevante del negocio) y el **repositorio**
(abstracción para persistir agregados). En SITRAM, el agregado raíz `Tramite` encapsula la
máquina de estados del expediente y garantiza sus invariantes.

##### 2.2.6.3 Patrón CQRS

*Command Query Responsibility Segregation* (CQRS) es un patrón que separa las operaciones de
**escritura** (comandos, que modifican el estado del sistema) de las operaciones de **lectura**
(consultas, que no lo modifican). Esta separación simplifica cada operación, mejora la
mantenibilidad y permite optimizar de forma independiente las lecturas y las escrituras. Se
implementa habitualmente mediante el patrón **mediador** (con librerías como MediatR en .NET),
que desacopla al emisor de una petición de su manejador.

**Figura 1**

*Flujo de un comando bajo el patrón CQRS en SITRAM*

```
Ciudadano ──HTTP POST /tramites──> TramitesController
                                        |
                                        v
                        IniciarTramiteCommand ──> MediatR
                                                    |
                                    +---------------+
                                    v
                        IniciarTramiteCommandHandler
                                    |  (valida y crea el agregado Tramite)
                                    v
                        ITramiteRepository.AddAsync()
                                    |
                                    v
                                SQL Server
```

*Nota.* Elaboración propia a partir del diseño arquitectónico del proyecto.

##### 2.2.6.4 Patrones de diseño

Los patrones de diseño son soluciones reutilizables a problemas recurrentes del diseño de
software. El proyecto emplea, entre otros: el **Repositorio** (abstracción del acceso a datos),
la **Inyección de Dependencias** (*Dependency Injection*, que provee las dependencias de un
objeto desde el exterior, favoreciendo el desacoplamiento) y el **Mediador**. Estos patrones se
alinean con los principios **SOLID** (responsabilidad única, abierto/cerrado, sustitución de
Liskov, segregación de interfaces e inversión de dependencias).

#### 2.2.7 Tecnologías de desarrollo

##### 2.2.7.1 Plataforma .NET y lenguaje C#

**.NET** es una plataforma de desarrollo multiplataforma, de código abierto, mantenida por
Microsoft, que permite construir aplicaciones para web, escritorio, móvil y nube. **.NET 10** es
su versión de soporte a largo plazo (LTS), lo que garantiza estabilidad y mantenimiento
prolongado. **C#** es un lenguaje de programación orientado a objetos, fuertemente tipado,
moderno y expresivo; su versión **14** incorpora mejoras de productividad y de seguridad de
tipos.

##### 2.2.7.2 ASP.NET Core y API REST

**ASP.NET Core** es el framework de .NET para construir aplicaciones y APIs web de alto
rendimiento. Ofrece un modelo de *middleware* (componentes encadenados que procesan cada
petición HTTP), inyección de dependencias integrada, y facilidades para exponer APIs REST
documentadas mediante **OpenAPI/Swagger**.

##### 2.2.7.3 SQL Server

**SQL Server 2022** es un sistema gestor de bases de datos relacional (SGBDR) de Microsoft que
garantiza las propiedades **ACID** (atomicidad, consistencia, aislamiento y durabilidad) de las
transacciones. Ofrece capacidades avanzadas de seguridad como el **cifrado transparente de
datos** (TDE) para el cifrado en reposo y **Always Encrypted** para el cifrado a nivel de
columna de datos sensibles.

##### 2.2.7.4 Entity Framework Core

**Entity Framework Core 10** es el mapeador objeto-relacional (ORM) de .NET, que permite a los
desarrolladores trabajar con la base de datos mediante objetos del lenguaje en lugar de SQL
directo. Soporta el enfoque *code-first* y el versionado del esquema mediante **migraciones**,
lo que facilita la evolución controlada de la base de datos.

##### 2.2.7.5 Blazor (interfaz de usuario)

**Blazor** es el framework de .NET para construir interfaces de usuario web interactivas con C#,
prescindiendo de JavaScript. Ofrece distintos modos de render; el proyecto adopta el **render
interactivo en servidor** (*Interactive Server*), en el que la lógica de la interfaz se ejecuta
en el servidor y el navegador solo recibe las diferencias de la interfaz a través de una conexión
en tiempo real. Esta elección tiene una motivación de seguridad y protección de datos: la lógica
y los **datos personales no se descargan al navegador**, la sesión autenticada no se almacena en
el cliente (evitando el robo de token por *cross-site scripting*) y el motor de plantillas
codifica la salida por defecto, mitigando dicho riesgo. Al compartir el lenguaje (C#) con el
backend, se reutilizan los objetos de transferencia (DTO) y las validaciones, y se reduce la
dependencia de librerías de terceros. La justificación completa se documenta en el ADR de
frontend del proyecto.

#### 2.2.8 Seguridad de la información

##### 2.2.8.1 La tríada CID

La seguridad de la información se sustenta en tres propiedades fundamentales, conocidas como la
tríada **CID**: la **confidencialidad** (la información solo es accesible para quienes están
autorizados), la **integridad** (la información es exacta y no ha sido alterada indebidamente) y
la **disponibilidad** (la información y los servicios están accesibles cuando se necesitan).

##### 2.2.8.2 Autenticación y JWT

La **autenticación** es el proceso de verificar la identidad de un usuario. Un mecanismo moderno
y ampliamente adoptado es el **JWT** (*JSON Web Token*): un token compacto, firmado
digitalmente, que transporta información (*claims*) sobre la identidad y los permisos del
usuario. Al ser autocontenido y firmado, permite una autenticación **sin estado** (*stateless*)
en el servidor, lo que favorece la escalabilidad. Su seguridad se refuerza con tokens de vida
corta y *refresh tokens* rotativos.

##### 2.2.8.3 Autorización y RBAC

La **autorización** determina qué acciones puede realizar un usuario autenticado. El **control
de acceso basado en roles** (*Role-Based Access Control*, RBAC) es un modelo que asigna permisos
a **roles**, y roles a **usuarios**, bajo el principio de **mínimo privilegio**: cada actor
recibe únicamente los permisos imprescindibles para su función. Este modelo facilita la gestión,
la auditoría y la modificación de los permisos sin alterar el código.

##### 2.2.8.4 Criptografía

La **criptografía** protege la información transformándola de modo que solo sea legible por
quienes poseen la clave adecuada. Se distinguen: el **cifrado simétrico** (una misma clave cifra
y descifra, p. ej. AES), el **cifrado asimétrico** (par de claves pública y privada) y las
**funciones de hash** (transformación unidireccional, usada para almacenar contraseñas mediante
algoritmos como bcrypt o PBKDF2). En tránsito, los datos se protegen con **TLS** (*Transport
Layer Security*), que cifra la comunicación entre cliente y servidor (HTTPS). En reposo, se
emplean TDE y el cifrado a nivel de columna.

##### 2.2.8.5 OWASP

El *Open Worldwide Application Security Project* (**OWASP**) es una organización sin fines de
lucro de referencia en seguridad de aplicaciones. Su **Top 10** enumera los riesgos de seguridad
más críticos —encabezados por el **control de acceso roto** (*Broken Access Control*)—, y su
estándar **ASVS** (*Application Security Verification Standard*) proporciona una lista
verificable de controles de seguridad, que este proyecto adopta como base de su checklist.

#### 2.2.9 Protección de datos personales

##### 2.2.9.1 Concepto y principios

Los **datos personales** son toda información sobre una persona natural que la identifica o la
hace identificable. Su tratamiento se rige por principios fundamentales: **consentimiento**
(autorización informada del titular), **finalidad** (uso limitado al propósito declarado),
**proporcionalidad** o minimización (recolectar solo lo necesario), **calidad** (datos exactos y
actualizados) y **seguridad** (medidas técnicas y organizativas adecuadas).

##### 2.2.9.2 Ley N.° 29733 y su reglamento

En el Perú, la **Ley N.° 29733 – Ley de Protección de Datos Personales** y su reglamento
vigente, el **Decreto Supremo N.° 016-2024-JUS** (en vigor desde marzo de 2024), constituyen el
marco legal aplicable, análogo al Reglamento General de Protección de Datos (GDPR) europeo. El
reglamento de 2024 reforzó las obligaciones de las entidades, incorporando la designación de un
**Oficial de Datos Personales**, la **notificación de incidentes de seguridad** (brechas de
datos) y el derecho de **portabilidad**, bajo un régimen sancionador que en 2024 impuso multas
por más de 13,4 millones de soles.

##### 2.2.9.3 Derechos ARCO y portabilidad

Los titulares de datos personales gozan de los **derechos ARCO**: **Acceso** (conocer qué datos
suyos se tratan), **Rectificación** (corregir datos inexactos), **Cancelación** (eliminar sus
datos cuando proceda, el llamado "derecho al olvido") y **Oposición** (oponerse al tratamiento).
El reglamento de 2024 añade el derecho de **portabilidad** (obtener y transferir sus datos en un
formato interoperable). La plataforma SITRAM implementa estos derechos como funcionalidades
concretas.

#### 2.2.10 Calidad de software

##### 2.2.10.1 ISO/IEC 25010

La norma **ISO/IEC 25010** define un modelo de calidad del producto de software estructurado en
ocho características: adecuación funcional, eficiencia de desempeño, compatibilidad, **usabilidad**,
fiabilidad, **seguridad**, mantenibilidad y portabilidad. Este modelo orienta la definición de
los requisitos no funcionales del proyecto.

##### 2.2.10.2 Usabilidad y la escala SUS

La **usabilidad** es la medida en que un producto puede ser utilizado por usuarios específicos
para alcanzar objetivos con eficacia, eficiencia y satisfacción. Para su evaluación, este
proyecto emplea la **Escala de Usabilidad del Sistema** (*System Usability Scale*, SUS),
desarrollada por John Brooke (1996): un instrumento estandarizado de **10 ítems** en escala
Likert de cinco puntos que produce un puntaje global de 0 a 100. Es un instrumento robusto,
rápido de aplicar y con una confiabilidad reportada elevada (α ≈ 0,91), ampliamente utilizado en
la industria y la academia.

#### 2.2.11 Pruebas de software

Las **pruebas de software** (*testing*) son el conjunto de actividades orientadas a verificar
que el software cumple con sus requisitos y a detectar defectos. Se organizan en niveles: las
**pruebas unitarias** (verifican una unidad de código de forma aislada), las **pruebas de
integración** (verifican la interacción entre varios componentes, incluida la base de datos) y
las **pruebas de extremo a extremo** (*end-to-end*, que verifican el sistema completo desde la
perspectiva del usuario). En el ecosistema .NET se emplean frameworks como **xUnit**, junto con
librerías de simulación de dependencias (*mocking*, como Moq) y de aserciones expresivas (como
FluentAssertions). La **cobertura de pruebas** —el porcentaje de código ejercitado por las
pruebas— es un indicador de la calidad y la mantenibilidad del software.

### 2.3 Comparación y justificación del stack tecnológico

La selección de las tecnologías se realizó considerando los requisitos de seguridad,
rendimiento, mantenibilidad y las capacidades de cumplimiento normativo del proyecto.

| Necesidad | Alternativas evaluadas | Elección y justificación |
|-----------|------------------------|--------------------------|
| Lenguaje / plataforma | Java/Spring, Node.js, Python/Django | **C# 14 / .NET 10 (LTS)**: robustez, rendimiento, tipado fuerte, soporte prolongado e integración nativa con seguridad y SQL Server |
| Interfaz de usuario | React, Angular (SPA con token en el navegador) | **Blazor (render en servidor)**: mantiene lógica y datos personales fuera del navegador, sin token en el cliente y con un solo lenguaje (C#) |
| Base de datos | PostgreSQL, MySQL, MongoDB | **SQL Server 2022**: modelo relacional ACID y capacidades de cifrado (TDE, Always Encrypted) que apoyan el cumplimiento de la Ley 29733 |
| ORM | Dapper, ADO.NET | **Entity Framework Core 10**: productividad, migraciones versionadas y abstracción del acceso a datos |
| Arquitectura | 3 capas, CRUD, microservicios | **Clean Architecture + DDD**: criticidad de las reglas de negocio, testabilidad y auditoría |
| Metodología | Cascada, solo Scrum | **SDD** (núcleo) + Scrum + XP: trazabilidad especificación→código, gestión ágil y calidad técnica |
| Autenticación | Sesiones, OAuth externo | **Identity + JWT + RBAC**: control de acceso granular, sin estado y auditable |

La justificación detallada de cada decisión se documenta en los Registros de Decisiones de
Arquitectura (ADR) del proyecto.

### 2.4 Definición de términos básicos

- **API REST:** interfaz de comunicación entre sistemas basada en los métodos del protocolo HTTP.
- **Agregado:** grupo de objetos del dominio tratados como una unidad de consistencia.
- **Auditoría:** registro inmutable de las acciones realizadas en el sistema (quién, qué,
  cuándo, desde dónde).
- **Cifrado:** transformación de datos para que solo sean legibles con la clave adecuada.
- **JWT:** token firmado que transporta la identidad y los permisos de un usuario.
- **MVP:** Producto Mínimo Viable; versión con las funcionalidades esenciales para validar la
  solución.
- **RBAC:** control de acceso basado en roles y permisos.
- **SDD:** Desarrollo Guiado por Especificaciones.
- **TUPA:** Texto Único de Procedimientos Administrativos.
- **Trámite:** procedimiento administrativo que un ciudadano realiza ante la municipalidad.
- **Usabilidad:** facilidad con la que los usuarios logran sus objetivos al usar el sistema.
