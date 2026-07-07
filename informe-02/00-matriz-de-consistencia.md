# Matriz de Consistencia — SITRAM

> Instrumento que articula, en una sola vista, la coherencia lógica entre problema,
> objetivos, hipótesis, variables e indicadores y metodología. Es la base transversal del
> informe (se incluye como **Anexo 1**) y garantiza que los cinco capítulos sean coherentes.

**Título:** Desarrollo de una plataforma web para la gestión de trámites municipales con
enfoque en seguridad y protección de datos personales, 2026.

---

## 1. Problemas, objetivos e hipótesis

| PROBLEMAS | OBJETIVOS | HIPÓTESIS |
|-----------|-----------|-----------|
| **General** | **General** | **General** |
| ¿Cuáles son los resultados del desarrollo de una plataforma web para la gestión de trámites municipales con enfoque en seguridad y protección de datos personales, 2026? | Desarrollar una plataforma web para la gestión de trámites municipales con enfoque en seguridad y protección de datos personales, que digitalice el ciclo completo del trámite y garantice la trazabilidad y confidencialidad de la información. | Por tratarse de una investigación **aplicada de nivel descriptivo**, no se formula hipótesis contrastable (Hernández-Sampieri et al., 2014). Se plantea el supuesto de que la plataforma cumplirá técnicamente sus módulos críticos y alcanzará un nivel de usabilidad aceptable. |
| **Específicos** | **Específicos** | |
| PE1. ¿Cuáles son los resultados de la fase de **análisis** del desarrollo de la plataforma? | OE1. Determinar los resultados de la fase de análisis (requisitos funcionales y no funcionales). | — |
| PE2. ¿Cuáles son los resultados de la fase de **diseño** (arquitectura, seguridad y datos)? | OE2. Determinar los resultados de la fase de diseño (arquitectura, modelo de datos y controles de seguridad). | — |
| PE3. ¿Cuáles son los resultados de la fase de **implementación** de los módulos? | OE3. Determinar los resultados de la fase de implementación de los módulos del sistema. | — |
| PE4. ¿Cuál es el nivel de cumplimiento del **funcionamiento** de los módulos críticos? | OE4. Validar el correcto funcionamiento de los módulos críticos (autenticación, autorización, cifrado, flujo de trámite y auditoría). | — |
| PE5. ¿Cuál es el nivel de cumplimiento de los controles de **seguridad y protección de datos** (Ley 29733)? | OE5. Evaluar el cumplimiento de los controles de seguridad y protección de datos personales conforme a la Ley N.° 29733 y su reglamento. | — |
| PE6. ¿Cuál es el nivel de **usabilidad** de la plataforma? | OE6. Evaluar la usabilidad de la plataforma mediante la participación de usuarios piloto. | — |

---

## 2. Variables, dimensiones e indicadores

| VARIABLE | DIMENSIÓN | INDICADORES | INSTRUMENTO |
|----------|-----------|-------------|-------------|
| **X1. Desarrollo del software** (proceso) | Análisis | N.° de requisitos funcionales y no funcionales identificados; historias de usuario definidas | Ficha de análisis documental |
| | Diseño | Diagramas de arquitectura y ER elaborados; ADR registrados; controles de seguridad diseñados | Ficha de análisis documental |
| | Implementación | N.° de módulos implementados; cobertura de pruebas (%) | Ficha de análisis documental |
| **X2. Funcionamiento** (producto) | Operatividad de módulos críticos | % de módulos que operan sin excepción (autenticación, RBAC, cifrado, flujo, auditoría, pagos) | Ficha de análisis documental / pruebas |
| **X3. Seguridad y protección de datos** | Cumplimiento de controles | % de controles OWASP/ASVS implementados; % de requisitos de la Ley 29733 cubiertos (cifrado, ARCO, auditoría, notificación de incidentes) | Lista de verificación (checklist) |
| **X4. Usabilidad** | Percepción del usuario | Puntaje promedio en la Escala de Usabilidad del Sistema (SUS) | Cuestionario SUS |

> **Definición operacional.** X1 mide el *proceso* de ingeniería (fases y entregables); X2 y
> X3 miden el *producto* (qué tan bien funciona y qué tan seguro es); X4 mide la *experiencia*
> del usuario. Este esquema replica y amplía el del modelo del grupo de calidad, añadiendo la
> variable de **seguridad y protección de datos** por su relevancia en el dominio municipal.

---

## 3. Metodología

| ASPECTO | DESCRIPCIÓN |
|---------|-------------|
| **Tipo de investigación** | Aplicada – tecnológica |
| **Nivel** | Descriptivo |
| **Enfoque** | Cuantitativo |
| **Diseño** | No experimental, de corte transversal |
| **Metodología de desarrollo** | **Spec-Driven Development (SDD)** con GitHub Spec Kit como **núcleo obligatorio**, complementado con las prácticas más óptimas: **Scrum** (gestión iterativa de las fases) y **XP / Extreme Programming** (calidad del código) |
| **Ámbito / caso de referencia** | Planteamiento **general** para la gestión de trámites municipales; se toma como **caso de aplicación de referencia** la **Municipalidad Distrital de San Juan Bautista (Ayacucho)** para contextualizar y validar el diseño |
| **Población** | Trámites y usuarios del ámbito municipal (ciudadanos y funcionarios) |
| **Muestra** | Muestra piloto no probabilística por conveniencia (20 usuarios) para la evaluación de usabilidad. Se incluye como **diseño metodológico propuesto** que refuerza la validez del estudio |
| **Técnicas** | Análisis documental, observación, encuesta |
| **Instrumentos** | Ficha de análisis documental, checklist de seguridad (OWASP/Ley 29733), cuestionario SUS |
| **Validación de instrumentos** | Validez de contenido por **juicio de expertos** (V de Aiken ≥ 0.80) y confiabilidad por **Alfa de Cronbach** (≥ 0.70). Ver [validación de instrumentos](00b-validacion-instrumentos.md) |
| **Herramientas** | .NET 10, C# 14, Blazor, SQL Server 2022, EF Core 10, xUnit, Serilog |

---

## 4. Coherencia (lectura horizontal)

Cada problema específico (PE) tiene **un** objetivo específico (OE) que lo responde, medido
por **una** variable con indicadores concretos:

```
PE1 → OE1 → X1 (Análisis)          medido por: N.º requisitos, historias de usuario
PE2 → OE2 → X1 (Diseño)            medido por: diagramas, ADR, controles diseñados
PE3 → OE3 → X1 (Implementación)    medido por: módulos, cobertura de pruebas
PE4 → OE4 → X2 (Funcionamiento)    medido por: % módulos operativos
PE5 → OE5 → X3 (Seguridad/datos)   medido por: % controles OWASP/Ley 29733
PE6 → OE6 → X4 (Usabilidad)        medido por: puntaje SUS
```

Esta trazabilidad se mantiene en todo el informe: los indicadores se reportan como
**resultados** en el Capítulo IV.
