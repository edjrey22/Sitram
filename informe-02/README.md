# Informe formal — SITRAM (versión definitiva)

> Documento académico del proyecto, redactado según el **modelo del grupo de calidad**
> (ver `../EJEMPLO DOC/TrabajoEjemplo.pdf`). Se redacta por capítulos en Markdown y se compila
> a Word (.docx). **Esta es la versión vigente** (reemplaza a la antigua carpeta `informe/`).

## Título del proyecto

**"Desarrollo de una plataforma web para la gestión de trámites municipales con enfoque en
seguridad y protección de datos personales, 2026"**

## Estructura del informe

| Sección | Archivo |
|---------|---------|
| Preliminares (portada, dedicatoria, resumen, abstract) | `01-preliminares.md` |
| Introducción | `02-introduccion.md` |
| Capítulo I — Planteamiento del Problema | `03-capitulo-1.md` |
| Capítulo II — Marco Teórico | `04-capitulo-2.md` |
| Capítulo III — Material y Métodos | `05-capitulo-3.md` |
| Capítulo IV — Resultados y Discusión | `06-capitulo-4.md` |
| Capítulo V — Conclusiones y Recomendaciones | `07-capitulo-5.md` |
| Referencias bibliográficas | `08-referencias.md` |
| Anexos | `09-anexos.md` |
| Matriz de Consistencia (base transversal) | `00-matriz-de-consistencia.md` |
| Validación de instrumentos (metodología) | `00b-validacion-instrumentos.md` |

## Compilar a Word

```bash
npm install      # solo la primera vez (instala la librería docx)
npm run build    # genera SITRAM-Informe-02.docx
```

## Fuente de contenido

El informe **reutiliza** los artefactos de ingeniería de la carpeta [`../docs/`](../docs/):
análisis de problemática, requisitos, arquitectura, decisiones (ADR), modelo de datos, etc.
