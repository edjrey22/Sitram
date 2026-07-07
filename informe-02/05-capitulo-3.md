# Capítulo III

## Material y Métodos

### 3.1 Tipo de investigación

La presente investigación es de tipo **aplicada – tecnológica**. Es aplicada porque su
finalidad no es la generación de conocimiento teórico puro, sino la **resolución de un problema
concreto y práctico**: la gestión presencial, opaca e insegura de los trámites en las
municipalidades. Es tecnológica porque el resultado principal es un **artefacto de software**
—una plataforma web— construido mediante la aplicación sistemática de conocimientos de
ingeniería. Este tipo de investigación se orienta a transformar la realidad, aportando una
solución tangible y verificable.

### 3.2 Nivel de investigación

El nivel de la investigación es **descriptivo**. El estudio busca **describir y caracterizar**
los resultados obtenidos en cada fase del desarrollo (análisis, diseño e implementación) y las
propiedades del producto resultante (funcionamiento, cumplimiento de seguridad y usabilidad),
sin manipular deliberadamente las variables ni establecer relaciones de causa-efecto entre
ellas. La investigación describe "cómo es y cómo se manifiesta" el fenómeno estudiado —en este
caso, el desarrollo y las características de la plataforma—.

### 3.3 Diseño de investigación

El diseño es **no experimental**, de corte **transversal**, con **enfoque cuantitativo**.

- Es **no experimental** porque no se manipulan intencionalmente las variables independientes;
  se observa el fenómeno tal como se presenta.
- Es **transversal** (o transeccional) porque la medición de las variables se realiza en un
  **único momento** en el tiempo, sobre el producto ya desarrollado y la muestra piloto.
- Es de **enfoque cuantitativo** porque los indicadores se expresan y analizan numéricamente
  (porcentajes de cumplimiento, puntaje SUS, coeficientes de validación y confiabilidad).

El esquema del diseño puede representarse como:

```
        M  ──────────►  O
```

Donde **M** representa la muestra (la plataforma desarrollada y los usuarios piloto) y **O**
representa la observación o medición de las variables (funcionamiento, seguridad y usabilidad)
realizada en un solo momento.

### 3.4 Población y muestra

#### 3.4.1 Población

La población está conformada por los **trámites y los usuarios del ámbito municipal**
—ciudadanos que solicitan servicios y funcionarios que los gestionan—, tomando como caso de
referencia la **Municipalidad Distrital de San Juan Bautista** (provincia de Huamanga, región
de Ayacucho).

#### 3.4.2 Muestra

Se emplea un **muestreo no probabilístico por conveniencia**, técnica en la que los elementos se
seleccionan según su accesibilidad y disponibilidad para el investigador, adecuada para las
fases iniciales de validación de software. Para la evaluación de usabilidad se define una
**muestra piloto de 20 usuarios**, tamaño coherente con la práctica reportada en la literatura
de usabilidad, donde se reconoce que muestras reducidas permiten detectar la mayoría de los
problemas de usabilidad.

**Criterios de inclusión:**
- Personas mayores de edad, residentes en el ámbito de referencia.
- Usuarios con acceso a un dispositivo con conexión a internet.
- Participación voluntaria con consentimiento informado.

**Criterios de exclusión:**
- Personas que no completen la totalidad de las tareas de la evaluación.
- Participantes que hayan intervenido en el desarrollo de la plataforma.

> La muestra se incorpora como **diseño metodológico propuesto** que refuerza la validez del
> estudio; sus resultados son indicativos para esta fase inicial y no pretenden ser
> generalizables estadísticamente al universo de usuarios.

### 3.5 Variables e indicadores

#### 3.5.1 Definición conceptual de las variables

- **X1. Desarrollo del software (proceso).** Conjunto de fases de ingeniería (análisis, diseño
  e implementación) ejecutadas de manera sistemática para construir la plataforma.
- **X2. Funcionamiento (producto).** Grado en que los módulos críticos del sistema operan
  correctamente y sin excepciones frente a los casos de prueba definidos.
- **X3. Seguridad y protección de datos.** Grado de cumplimiento de los controles técnicos de
  seguridad de la información y de las obligaciones legales de protección de datos personales.
- **X4. Usabilidad.** Facilidad con la que los usuarios logran sus objetivos al interactuar con
  la plataforma, en términos de eficacia, eficiencia y satisfacción.

#### 3.5.2 Definición operacional de las variables

La operacionalización traduce cada variable en dimensiones e indicadores medibles mediante
instrumentos concretos:

| Variable | Dimensión | Indicadores | Escala | Instrumento |
|----------|-----------|-------------|--------|-------------|
| X1 Desarrollo | Análisis | N.° de requisitos (RF/RNF); N.° de historias de usuario; N.° de actores | Razón | Ficha de análisis documental |
| | Diseño | N.° de diagramas (arquitectura, ER); N.° de ADR; controles de seguridad diseñados | Razón | Ficha de análisis documental |
| | Implementación | N.° de módulos implementados; cobertura de pruebas (%) | Razón | Ficha de análisis documental |
| X2 Funcionamiento | Operatividad de módulos críticos | % de módulos que operan sin excepción | Razón | Ficha / pruebas |
| X3 Seguridad y datos | Controles técnicos | % de controles OWASP/ASVS implementados | Razón | Checklist de seguridad |
| | Cumplimiento legal | % de obligaciones de la Ley 29733 cubiertas | Razón | Checklist de seguridad |
| X4 Usabilidad | Percepción del usuario | Puntaje promedio en la Escala SUS (0–100) | Intervalo | Cuestionario SUS |

### 3.6 Técnicas e instrumentos para el tratamiento de datos e información

#### 3.6.1 Técnicas para recolectar información

- **Análisis documental:** revisión sistemática de los entregables de ingeniería (documento de
  requisitos, diagramas, ADR, código fuente y reportes de pruebas) para medir las variables X1 y
  X2.
- **Observación estructurada:** verificación directa y guiada del cumplimiento de los controles
  de seguridad y protección de datos, para medir la variable X3.
- **Encuesta:** aplicación de un cuestionario a la muestra piloto de usuarios para medir la
  percepción de usabilidad, variable X4.

#### 3.6.2 Instrumentos para recolectar información

1. **Ficha de análisis documental.** Instrumento estructurado que registra la existencia y la
   operatividad de los entregables de ingeniería y de los módulos del sistema. Mide X1 y X2.
2. **Checklist de seguridad y protección de datos.** Lista de verificación dicotómica
   (Cumple / No cumple / No aplica) construida sobre los controles de OWASP ASVS y las
   obligaciones de la Ley N.° 29733 y su reglamento. Mide X3.
3. **Cuestionario de usabilidad (Escala SUS).** Instrumento estandarizado de 10 ítems en escala
   Likert de cinco puntos. Mide X4.

#### 3.6.3 Validación y confiabilidad de los instrumentos

La credibilidad de los resultados exige que los instrumentos sean **válidos** y **confiables**.

**a) Validez de contenido — Juicio de expertos.** Todos los instrumentos se someten a la
evaluación de un panel de **mínimo tres expertos** (con grado de magíster o doctor en Ingeniería
de Software, Seguridad de la Información o Protección de Datos), quienes califican cada ítem en
tres criterios: **claridad**, **coherencia** y **relevancia**. La concordancia entre los jueces
se cuantifica mediante el **Coeficiente V de Aiken**, cuyo criterio de aceptación es **V ≥ 0,80**
por ítem y global. Su fórmula es:

```
          S
V = ───────────────
     n · (c − 1)
```

Donde **S** es la sumatoria de las valoraciones de los jueces, **n** el número de jueces y **c**
el número de valores de la escala. Los ítems con V < 0,80 se corrigen o eliminan.

**b) Confiabilidad — Alfa de Cronbach.** Para el cuestionario SUS (escala de percepción), se
calcula el coeficiente **Alfa de Cronbach** sobre una prueba piloto, con criterio de aceptación
**α ≥ 0,70**. Su fórmula es:

```
        k        Σ Vi
α = ───────── · (1 − ─────)
      k − 1          Vt
```

Donde **k** es el número de ítems, **Vi** la varianza de cada ítem y **Vt** la varianza total.
Se adjuntan como anexos las constancias de validación firmadas por los expertos.

#### 3.6.4 Herramientas para el tratamiento de datos e información

- **Desarrollo:** Visual Studio / Visual Studio Code, .NET 10, C# 14, SQL Server 2022, Entity
  Framework Core 10 y el sistema de control de versiones Git.
- **Pruebas:** xUnit (framework de pruebas), Moq (simulación de dependencias), FluentAssertions
  (aserciones) y herramientas de reporte de cobertura.
- **Gestión SDD:** GitHub Spec Kit, que produce los artefactos `spec.md`, `plan.md` y
  `tasks.md`.
- **Análisis de datos:** hojas de cálculo y software estadístico para el cómputo del Coeficiente
  V de Aiken, el Alfa de Cronbach y el puntaje SUS.

#### 3.6.5 Diseño estadístico

Se emplea **estadística descriptiva**. Los indicadores de cumplimiento de las variables X1, X2 y
X3 se expresan mediante **frecuencias absolutas y relativas (porcentajes)**. La variable X4
(usabilidad) se resume mediante la **media aritmética** y la **desviación estándar** del puntaje
SUS. La validación de los instrumentos utiliza el Coeficiente V de Aiken y el Alfa de Cronbach.

#### 3.6.6 Análisis e interpretación de datos

- Los **porcentajes de cumplimiento** (X1, X2, X3) se interpretan frente a las metas definidas
  en los requisitos no funcionales; por ejemplo, se espera una cobertura de pruebas ≥ 80 % y un
  cumplimiento de módulos críticos del 100 %.
- El **puntaje SUS** (X4) se interpreta según la escala de referencia: un puntaje **superior a
  80,3** corresponde a una usabilidad "excelente"; entre **68 y 80,3**, "buena"; alrededor de
  **68**, "promedio"; e **inferior a 68**, "por debajo del promedio".
- La **V de Aiken** y el **Alfa de Cronbach** se contrastan con sus umbrales de aceptación
  (0,80 y 0,70, respectivamente).

#### 3.6.7 Aspectos éticos

Dado que el proyecto trata datos personales y aplica instrumentos a personas, se observan los
siguientes principios éticos: la **participación voluntaria** con **consentimiento informado**
de los usuarios de la muestra; la **confidencialidad** y el **anonimato** de sus respuestas; el
uso de los datos **exclusivamente** para los fines de la investigación; y el cumplimiento de la
Ley N.° 29733 en todo el tratamiento de datos, incluyendo el uso de datos anonimizados o
sintéticos en los entornos de prueba.

### 3.7 Técnicas para aplicar el marco de trabajo (SDD + Scrum + XP)

El desarrollo integra tres metodologías complementarias que se articulan en un flujo coherente:
**SDD define y traza el *qué*, Scrum gestiona el *cuándo* y el ritmo, y XP asegura el *cómo*
técnico.**

#### 3.7.1 Spec-Driven Development (SDD) — núcleo metodológico

Cada funcionalidad recorre el ciclo de cuatro etapas de SDD antes de considerarse terminada:

1. **`/specify` (Especificar):** se describe la necesidad en lenguaje de negocio —actores,
   criterios de aceptación y reglas— sin decisiones técnicas. Se genera el artefacto `spec.md`.
2. **`/plan` (Planificar):** se traduce la especificación a un diseño técnico —entidades
   afectadas, endpoints, esquema de base de datos e impacto en seguridad—. Se genera `plan.md`.
3. **`/tasks` (Descomponer):** se divide el plan en tareas pequeñas, atómicas y verificables. Se
   genera `tasks.md`.
4. **`/implement` (Implementar):** se codifica tarea por tarea, cada una acompañada de sus
   pruebas.

De este modo, **ninguna línea de código se escribe sin una especificación previa**, lo que
garantiza la trazabilidad requisito → diseño → código y produce documentación viva que sirve de
evidencia del proceso.

**Figura 2**

*Ciclo de cuatro etapas del Spec-Driven Development (GitHub Spec Kit)*

```
  /specify         /plan            /tasks           /implement
+----------+     +----------+     +----------+     +---------------+
| spec.md  | --> | plan.md  | --> | tasks.md | --> | codigo + test |
| QUE y    |     | COMO     |     | pasos    |     | verificado    |
| POR QUE  |     | (diseno) |     | atomicos |     |               |
+----------+     +----------+     +----------+     +---------------+
      |                |               |                   |
      +----------------+--- revision --+-------------------+
```

*Nota.* Elaboración propia con base en el flujo de GitHub Spec Kit.

#### 3.7.2 Scrum — gestión iterativa

El trabajo se organiza en **sprints** con la siguiente estructura:

- **Product Backlog:** lista priorizada de historias de usuario derivadas de los requisitos
  funcionales (ver Anexo de historias de usuario).
- **Sprint Planning:** al inicio de cada sprint se selecciona el conjunto de historias a
  desarrollar, conformando el Sprint Backlog.
- **Daily Scrum:** seguimiento breve y frecuente del avance y los impedimentos.
- **Sprint Review:** al final de cada sprint se presenta el incremento funcional.
- **Sprint Retrospective:** se reflexiona sobre el proceso para mejorar en el siguiente ciclo.

**Planificación referencial de sprints:**

| Sprint | Objetivo | Módulos / historias |
|--------|----------|---------------------|
| Sprint 1 | Cimientos y seguridad | Gestión de identidad y acceso (registro, login JWT, RBAC) |
| Sprint 2 | Configuración del TUPA | Tipos de trámite, requisitos y flujos de aprobación |
| Sprint 3 | Ciclo del trámite | Inicio, documentos, máquina de estados, subsanación |
| Sprint 4 | Pagos y notificaciones | Cálculo de tasa, pago, seguimiento y correo |
| Sprint 5 | Protección de datos y auditoría | Consentimiento, derechos ARCO, incidentes, auditoría |

#### 3.7.3 Extreme Programming (XP) — calidad de código

Durante la implementación se aplican las prácticas de ingeniería de XP:

- **Desarrollo guiado por pruebas (TDD):** se escriben primero las pruebas y luego el código que
  las satisface.
- **Integración continua:** cada cambio se integra y verifica automáticamente mediante el
  pipeline de CI, que ejecuta la compilación, las pruebas y el análisis estático.
- **Diseño simple y refactorización:** se mantiene el código lo más simple posible y se mejora
  continuamente sin alterar su comportamiento.
- **Estándares de codificación:** se respetan las convenciones definidas para el proyecto,
  asegurando un código homogéneo y legible.

La articulación de las tres metodologías se materializa en el flujo de trabajo del proyecto
(estrategia de ramas, pull requests con revisión e integración continua), documentado en los
artefactos de ingeniería.
