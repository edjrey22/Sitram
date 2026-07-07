# Análisis de la problemática — SITRAM

> Diagnóstico basado en evidencia (local, nacional e internacional) que **justifica** el
> proyecto y fundamenta los [requisitos](requisitos.md). Alimenta el Capítulo I
> (Planteamiento del Problema) y el Capítulo II (Marco Teórico) del informe.

## 1. El problema en tres niveles

### 1.1 Nivel local — Ayacucho / Huamanga

- La **Municipalidad Provincial de Huamanga** fue la **entidad más quejada** ante la
  Defensoría del Pueblo en Ayacucho: **75 casos en 2021** y **77 casos en 2022**,
  liderando el ranking regional dos años consecutivos.
- La atención es **mayoritariamente presencial**, en horario partido (08:00–13:00 y
  15:00–17:30), lo que obliga al ciudadano a acudir físicamente y en horarios de oficina.
- Aunque existen algunos servicios en línea (gestión documentaria), **no cubren el ciclo
  completo del trámite** (inicio, pago, seguimiento y resolución de forma remota).
- Como **caso de aplicación de referencia** se toma la **Municipalidad Distrital de San Juan
  Bautista** (provincia de Huamanga), uno de los distritos más poblados de la provincia, donde
  la gestión de trámites mantiene el mismo carácter presencial. El planteamiento y el diseño de
  SITRAM son **generales** (aplicables a cualquier municipalidad); San Juan Bautista solo
  contextualiza y valida la solución.

### 1.2 Nivel nacional — Perú

**Brecha de digitalización municipal (la más crítica):**
- Solo el **31 %** de municipalidades dispone de **Mesa de Partes Digital**.
- Apenas el **2 %** ha implementado un Modelo de Gestión Documental.
- Solo **199 de 1 891 municipios** (≈ 10,5 %) tenían el **Sistema Único de Trámites (SUT)**
  operativo en 2024.
- El **93,81 %** migró su web al dominio `gob.pe`, pero **tener web no equivale a tener
  procesos digitales** ni interoperables.

**Burocracia y barreras:**
- **3 de cada 4 barreras burocráticas** eliminadas por Indecopi en los últimos 5 años fueron
  creadas por municipalidades (exigencias innecesarias para permisos y trámites).

**Corrupción asociada al trámite presencial:**
- Las **municipalidades encabezan** los casos de corrupción en trámite (**> 37 %** del total
  en la 2.ª mitad de 2024, según la Defensoría del Pueblo); a nivel país se registran
  **> 40 000 casos** de corrupción en trámite.
- Perú se ubicó en el puesto **127 de 180** en el Índice de Percepción de la Corrupción 2025,
  atribuido en parte al **exceso de burocracia**.

**Protección de datos (marco legal reforzado en 2024):**
- El **29 de marzo de 2024** entró en vigor el nuevo **Reglamento de la Ley N.° 29733**
  (**D.S. N.° 016-2024-JUS**), que añade obligaciones: **Oficial de Datos Personales**,
  **notificación de incidentes de seguridad** y derecho de **portabilidad**.
- En 2024 se impusieron **S/ 13,4 millones en multas** (casi el doble que 2023), con **133
  procedimientos sancionadores** y **454 entidades fiscalizadas**; las multas llegan hasta
  **S/ 550 000**. Las entidades públicas no están exentas.

### 1.3 Nivel internacional — países en desarrollo

- La evidencia académica confirma que el **e-government reduce la corrupción y mejora la
  transparencia y la efectividad** del Estado, al eliminar la discrecionalidad del trato
  presencial ("petty corruption" en la ventanilla).
- Principales **retos de implementación**: infraestructura TI y **escasez de habilidades
  digitales**, baja colaboración interinstitucional, **desconfianza ciudadana** y
  desigualdad de acceso a internet.
- Riesgo emergente: el mal uso de los datos recolectados → refuerza la necesidad de
  **protección de datos y auditoría** desde el diseño.
- La corrupción y flujos ilícitos cuestan a los países en desarrollo **US$ 1,26 billones**
  anuales, lo que dimensiona la importancia de digitalizar y transparentar.

## 2. Síntesis del problema

> En la región de Ayacucho, y en la mayoría de municipalidades del Perú, la gestión de
> trámites es predominantemente **presencial, lenta, poco transparente y expuesta a
> corrupción**, con una **brecha de digitalización** que deja fuera el ciclo completo del
> trámite. A la vez, un **marco legal de protección de datos reforzado (2024)** exige medidas
> técnicas que la mayoría de municipios no implementa. Esto genera **pérdida de tiempo,
> sobrecostos, riesgo de fraude y vulneración de datos personales** para la ciudadanía.

## 3. Causas raíz (árbol de problemas resumido)

```
                        Gestión municipal de trámites deficiente
                                        │
        ┌───────────────────┬───────────┴──────────┬──────────────────────┐
   Procesos manuales   Baja digitalización   Discrecionalidad      Debilidad en
   y presenciales      (falta de sistemas)   en ventanilla         protección de datos
        │                    │                    │                       │
   colas, horarios      31% MPD, 2% GDD      corrupción/coimas       multas, brechas,
   pérdida de tiempo    sin interoperar      trato desigual          desconfianza
```

## 4. Cómo SITRAM ataca cada causa

| Causa raíz | Respuesta de SITRAM | Requisitos asociados |
|------------|---------------------|----------------------|
| Procesos manuales / presenciales | Trámite 100 % en línea: inicio, pago y seguimiento remotos | RF-020…RF-053 |
| Baja digitalización / sin trazabilidad | Ciclo completo digital + estado en tiempo real + reportes | RF-050, RF-052, RF-072 |
| Discrecionalidad y corrupción en ventanilla | Flujo reglado, máquina de estados, **auditoría inmutable** de cada acción | RF-029, RF-070, RF-073 |
| Barreras burocráticas | Catálogo TUPA claro con requisitos y costos explícitos | RF-010, RF-011, RF-014 |
| Debilidad en protección de datos | Cifrado, RBAC, consentimiento, derechos ARCO, **notificación de incidentes** | RF-060…RF-064, RNF-001…RNF-013 |

## 5. Impacto de la investigación en los requisitos

La investigación **añade o refuerza** los siguientes requisitos (ver
[requisitos.md](requisitos.md), sección de cambios):

1. **Notificación de incidentes de seguridad** (nueva obligación del D.S. 016-2024-JUS) →
   nuevo **RF-065**.
2. **Registro/rol de Oficial de Datos Personales** → refuerza el actor y **RF-066**.
3. **Portabilidad de datos** (nuevo derecho) → refuerza **RF-060** (exportación en formato
   interoperable).
4. **Transparencia del TUPA** (contra barreras burocráticas) → refuerza **RF-014**.
5. **Trazabilidad/auditoría como control anticorrupción** → eleva la prioridad de
   **RF-070/RF-073** a crítica.

## 6. Fuentes

- [Defensoría del Pueblo — 6152 casos de corrupción en trámite (2.ª mitad 2024)](https://www.defensoria.gob.pe/defensoria-del-pueblo-6152-son-los-casos-de-corrupcion-en-tramite-registrados-en-la-segunda-mitad-del-2024/)
- [Defensoría del Pueblo — más de 40 000 casos de corrupción en trámite en el país](https://www.defensoria.gob.pe/defensoria-del-pueblo-se-registran-mas-de-40-000-casos-de-corrupcion-en-tramite-en-todo-el-pais/)
- [Defensoría del Pueblo — municipios, entidades más quejadas en Ayacucho (2022)](https://www.defensoria.gob.pe/defensoria-del-pueblo-municipios-fueron-entidades-mas-quejada-en-2022-en-ayacucho/)
- [Defensoría del Pueblo — municipalidades más quejadas en Ayacucho (2021)](https://www.defensoria.gob.pe/defensoria-del-pueblo-municipalidades-son-las-entidades-mas-quejadas-del-ano-2021-en-ayacucho/)
- [Gestión — Municipalidades generan 3 de cada 4 barreras burocráticas](https://gestion.pe/economia/municipalidades-generan-3-de-cada-4-barreras-burocraticas-las-cifras-por-desregular-shock-regulatorio-indecopi-mef-noticia/)
- [Conexión ESAN — Desafíos del gobierno digital en las municipalidades](https://www.esan.edu.pe/conexion-esan/desafios-de-la-implementacion-del-gobierno-digital-en-las-municipalidades)
- [La Prensa Regional — Municipalidades no consolidan gobierno digital](https://prensaregional.pe/municipalidades-no-consolidan-gobierno-digital-para-mejorar-servicios/)
- [LP Derecho — Reglamento de la Ley 29733 (D.S. 016-2024-JUS)](https://lpderecho.pe/reglamento-ley-proteccion-datos-personales-decreto-supremo-016-2024-jus/)
- [LexLatin — Cambios del reglamento de protección de datos (Ley 29733)](https://lexlatin.com/entrevistas/ley-29733-cambios-reglamento-proteccion-datos--peru)
- [Tandfonline — Issues and challenges of implementing e-governance in developing countries (2024)](https://www.tandfonline.com/doi/full/10.1080/23311975.2024.2340579)
- [Tandfonline — E-government and petty corruption in public sector service delivery](https://www.tandfonline.com/doi/full/10.1080/09537325.2022.2067037)
- [Wiley — Public sector digitalization, corruption, and sustainability in the developing world (2024)](https://onlinelibrary.wiley.com/doi/10.1002/sd.2900)
