# Guion de exposición — SITRAM

> Monólogo completo para la sustentación oral, hoja por hoja, siguiendo
> [`docs/presentacion.md`](presentacion.md) (15 diapositivas). Incluye los momentos en
> los que conviene mostrar código real (con ruta exacta de archivo) y el recorrido por
> `docs/`. Tiempo estimado total: 12-15 minutos a ritmo normal; ajusta cortando los
> párrafos entre corchetes `[opcional]` si te piden menos tiempo.

---

## Antes de empezar

Ten abiertas dos ventanas además de la presentación:
1. **VS Code** con el proyecto `C:\proyecto_software` (para los momentos de código).
2. **El navegador** en `github.com/edjrey22/Sitram`, por si te piden ver el historial de commits o los ADR directamente en GitHub.

No necesitas mostrar código en cada diapositiva — está marcado solo donde suma. El resto del tiempo, la diapositiva sola sostiene el discurso.

---

## Hoja 1 — Portada

*(Preséntate mientras se ve la portada con el escudo de la UNSCH.)*

> Buenos días/tardes. Mi nombre es Edson Cuadros Reyes, y voy a presentar SITRAM, el
> Sistema Integrado de Trámites Municipales, desarrollado para el curso de Pruebas y
> Aseguramiento de la Calidad de Software, bajo la asesoría del Mg. Ing. Richard Zapata
> Casaverde.
>
> SITRAM es una plataforma web para que un ciudadano inicie, pague y siga el estado de
> un trámite municipal sin ir presencialmente a la municipalidad — y el eje central del
> proyecto no es solo digitalizar ese flujo, sino hacerlo con un enfoque serio de
> seguridad de la información y protección de datos personales, conforme a la Ley
> N.° 29733.

---

## Hoja 2 — Problema

> El punto de partida es un diagnóstico real: en la mayoría de municipalidades
> peruanas, el trámite documentario sigue siendo presencial, opaco y sin trazabilidad.
> El ciudadano entrega sus documentos y después no tiene forma de saber en qué estado
> está su expediente. Y cuando revisamos los antecedentes —tanto locales como
> nacionales e internacionales— encontramos un patrón: los sistemas existentes
> resuelven la parte de "digitalizar el flujo", pero casi ninguno aborda de forma
> integral la seguridad ni la protección de los datos personales que se recogen en el
> proceso.
>
> Tomamos como caso de referencia la Municipalidad Distrital de San Juan Bautista,
> aquí en Ayacucho.

---

## Hoja 3 — Objetivos

> De ese problema se desprende el objetivo general: desarrollar una plataforma web
> para la gestión de trámites municipales con enfoque en seguridad y protección de
> datos personales.
>
> Ese objetivo se descompone en seis objetivos específicos, cada uno atado a una
> variable medible — esto es importante porque en el Capítulo IV voy a volver
> exactamente a esta tabla para mostrar el resultado de cada uno: análisis, diseño,
> implementación, funcionamiento, seguridad y usabilidad.

---

## Hoja 4 — Alcance funcional

> Para dimensionar el proyecto: identificamos siete actores del sistema, desde el
> ciudadano hasta un rol que la ley exige — el Oficial de Datos Personales — y
> especificamos 42 requisitos funcionales agrupados en 7 módulos, más 26 requisitos no
> funcionales trazables y medibles.
>
> Esto no quedó en un documento aislado: cada requisito tiene un identificador (RF-XXX,
> RNF-XXX) que se usa consistentemente en el código, en las pruebas y en el informe —
> así cualquier persona puede rastrear de un requisito a su implementación y de vuelta.

**[Código, opcional]** Si quieren ver la fuente de esta tabla: `docs/requisitos.md` — ahí está cada RF y RNF con su actor y prioridad MoSCoW.

---

## Hoja 5 — Metodología

> El desarrollo se organizó con Spec-Driven Development como núcleo — es decir, la
> especificación se escribe antes que el código, no después como documentación de
> relleno — complementado con Scrum para la gestión iterativa y Extreme Programming
> para la disciplina técnica: pruebas primero, integración continua.
>
> La herramienta que usamos para materializar el SDD fue GitHub Spec Kit, con su ciclo
> `/specify → /plan → /tasks → /implement`.
>
> Y algo que quiero resaltar: los instrumentos que usamos para medir —el checklist de
> seguridad y el cuestionario SUS— no los inventamos y los usamos así nomás. Pasaron
> por juicio de expertos con el coeficiente V de Aiken, y por el Alfa de Cronbach para
> confiabilidad. Esto está detallado en `docs/00b-validacion-instrumentos.md` del
> informe, con su propio diagrama de flujo de aceptación.

---

## Hoja 6 — Arquitectura

> A nivel de arquitectura, adoptamos Clean Architecture combinada con Domain-Driven
> Design. La regla que gobierna todo es la regla de dependencia: el código siempre
> apunta hacia adentro, hacia el dominio. El dominio no sabe que existe una base de
> datos, ni que existe una API — eso es lo que nos permitió, más adelante, cambiar el
> motor de base de datos sin tocar una sola línea de las reglas de negocio.
>
> El corazón del dominio es el agregado `Tramite`, que implementa una máquina de
> estados de seis estados. Ninguna transición inválida puede ocurrir porque el propio
> agregado la rechaza — no es una validación en el controlador, es una invariante del
> dominio.

**[Código — mostrar aquí]** Abre `src/Sitram.Domain/Tramites/Tramite.cs`. Señala los métodos como `IniciarRevision()`, `Aprobar()`, `Rechazar()` y cómo cada uno llama internamente a un método privado que valida el estado origen antes de cambiar — por ejemplo:
> "Miren, aquí el método `Aprobar()` no simplemente asigna un nuevo estado: internamente valida que el estado actual sea `EnRevision`, y si no lo es, lanza una excepción de dominio. Esto es lo que en DDD se llama proteger las invariantes dentro del agregado."

---

## Hoja 7 — Modelo de datos y cifrado

> El modelo de datos tiene 17 entidades alrededor del agregado `Tramite`, y aquí es
> donde entra la protección de datos personales en serio. Trabajamos con tres capas:
>
> En tránsito, todo va por TLS. En reposo, el proveedor de base de datos cifra los
> volúmenes de almacenamiento. Y a nivel de columna —esto es lo más particular—
> ciframos con AES-256 en la propia capa de aplicación los campos sensibles: DNI,
> correo y teléfono. Usamos cifrado determinista para los campos donde necesitamos
> buscar por igualdad, como el DNI, y cifrado aleatorio donde no hace falta.
>
> Las contraseñas nunca se cifran de forma reversible — se hashean con bcrypt o
> PBKDF2. Y cada acción sobre un trámite queda en una tabla de auditoría que es
> append-only: no se puede editar ni borrar desde la aplicación.

**[Código — mostrar aquí]** Abre `src/Sitram.Infrastructure/Persistence/Cifrado/CifradoColumna.cs`. Señala `CifrarDeterministico` vs `CifrarAleatorio`, y el comentario sobre cómo se deriva el IV determinista con HMAC-SHA256:
> "Este archivo es exactamente donde vive la implementación real del cifrado que mencioné. El texto del mismo DNI siempre produce el mismo cifrado —por eso podemos buscar por igualdad sin descifrar toda la tabla."

---

## Hoja 8 — Stack tecnológico

> El stack: C# 14 sobre .NET 10, que es la versión LTS actual. El frontend es Blazor
> Web App con render interactivo en servidor — esto lo elegimos por una razón de
> seguridad, no solo de productividad: los datos personales y la lógica nunca se
> descargan al navegador del cliente.
>
> La API es ASP.NET Core Web API. La persistencia es PostgreSQL gestionado en
> Supabase, con Entity Framework Core 10 vía Npgsql. Y para pruebas: xUnit, Moq,
> FluentAssertions y Respawn para aislar el estado entre pruebas de integración.

---

## Hoja 9 — Seguridad y protección de datos (Ley 29733)

> Este es, junto con la arquitectura, el eje central de la sustentación. Cumplimos con
> la Ley N.° 29733 y su reglamento vigente, el Decreto Supremo 016-2024-JUS, que es el
> equivalente peruano del GDPR europeo.
>
> Implementamos los derechos ARCO completos —acceso, rectificación, cancelación y
> oposición— más el derecho de portabilidad. El consentimiento del ciudadano queda
> registrado y es revocable. Las cuentas de funcionario tienen segundo factor de
> autenticación por correo. Y si ocurre un incidente de seguridad, el sistema lo
> notifica a un rol dedicado que exige la norma: el Oficial de Datos Personales.
>
> Todo el control de acceso es RBAC, con mínimo privilegio, y se valida siempre en el
> servidor — nunca confiamos en lo que oculta o muestra la interfaz.

**[Código, opcional]** Si preguntan cómo se valida en servidor: `src/Sitram.Api/Program.cs`, sección de políticas (`RequireClaim("permiso", ...)`), y cualquier controlador con `[Authorize(Policy = "...")]`, por ejemplo `src/Sitram.Api/Controllers/TramitesController.cs`.

---

## Hoja 10 — Decisiones de arquitectura (ADR)

> Documentamos siete decisiones de arquitectura formalmente, siguiendo el formato ADR:
> contexto, decisión, consecuencias y alternativas consideradas.
>
> Y quiero detenerme en una en particular, porque creo que es la que mejor demuestra
> el proceso de ingeniería del proyecto, no solo el resultado. La decisión ADR-0003
> original eligió SQL Server como motor de base de datos, justamente por dos
> capacidades nativas: Always Encrypted para cifrado de columna, y TDE para cifrado en
> reposo. En la práctica, durante la implementación, nos dimos cuenta de que Always
> Encrypted real necesita un almacén de claves externo —Azure Key Vault o un HSM— que
> no existe en un entorno académico sin presupuesto de infraestructura. Por eso ya
> habíamos implementado el cifrado a nivel de aplicación que mostré hace un momento.
>
> Entonces, si la ventaja que justificó elegir SQL Server nunca se llegó a usar,
> ¿para qué mantener esa dependencia? Migramos a PostgreSQL gestionado en Supabase, y
> documentamos esa migración como ADR-0007, que reemplaza formalmente a la 0003 —no la
> borramos, queda como registro histórico de por qué se cambió de opinión.

**[Código — mostrar aquí]** Abre `docs/decisiones/ADR-0007-migracion-postgresql-supabase.md` y muestra la sección "Contexto". Si hay tiempo, abre también `docs/decisiones/ADR-0003-sql-server-ef-core.md` y señala la nota que dice "Reemplazada por ADR-0007".

---

## Hoja 11 — Resultados: implementación

> Pasando a resultados. Los siete módulos funcionales están implementados al 100%.
> El compilador no genera ninguna advertencia en configuración Release. La cobertura
> de pruebas unitarias en la capa de dominio es 86,7%, por encima de la meta del 80%
> que nos habíamos fijado como RNF-050.
>
> En la capa de aplicación, la cifra medida solo con pruebas unitarias es 40,7% — y
> quiero ser transparente con esto: es una cifra parcial, porque no incluye la
> cobertura adicional que aportan las 62 pruebas de integración que ejercitan esos
> mismos casos de uso a través de la API real. Medir eso de forma combinada con
> `reportgenerator` es un pendiente que dejamos documentado, no una cifra que
> inventamos para que se vea mejor.
>
> Y el frontend en Blazor está completo: registro, login por usuario o DNI,
> recuperación y confirmación de correo, administración de trámites y de funcionarios.

---

## Hoja 12 — Resultados: funcionamiento y seguridad

> El indicador más contundente: 146 de 146 pruebas automatizadas pasan en verde. 47
> son unitarias de dominio, 37 de aplicación, y 62 de integración —estas últimas
> corren contra una base de datos PostgreSQL real en Supabase, no contra un mock.
>
> Con esa evidencia, los seis módulos críticos —autenticación, autorización, cifrado,
> flujo del trámite, pagos y auditoría— operan al 100%. Y el checklist de seguridad y
> cumplimiento legal, que cruza controles técnicos OWASP con obligaciones de la Ley
> 29733, da 13 de 13, o sea 100% de cumplimiento.

**[Código, opcional]** Si quieren ver una prueba real: `tests/Sitram.Domain.Tests/` para las reglas de la máquina de estados, o `tests/Sitram.Integration.Tests/` para un flujo completo contra la API.

---

## Hoja 13 — Conclusiones

> En conclusión: la plataforma es técnica, operativa, económica y legalmente viable
> para digitalizar el ciclo completo del trámite municipal.
>
> De los seis objetivos específicos, cinco tienen resultado medido y verificado contra
> código y pruebas reales — solo la usabilidad queda pendiente de la fase de campo,
> porque necesita usuarios reales respondiendo un cuestionario, algo que no se puede
> sacar del repositorio.
>
> Y quiero cerrar con algo que para mí es el valor más importante del proyecto más
> allá del código: documentamos y justificamos nuestras propias decisiones, incluyendo
> una revisión de arquitectura hecha con evidencia y no solo con intuición. Eso es,
> en el fondo, lo que Spec-Driven Development y los ADR permiten hacer bien.

---

## Hoja 14 — Recomendaciones y trabajo pendiente

> Quedan dos cosas honestamente pendientes, no ocultas: aplicar el cuestionario SUS a
> la muestra piloto real, y fusionar la medición de cobertura de dominio y aplicación
> con las pruebas de integración.
>
> Como recomendaciones para una siguiente etapa: una auditoría de seguridad externa
> antes de producción, integración real con RENIEC y SUNAT, firma digital certificada,
> previsión de escalabilidad para un despliegue regional, un plan de capacitación para
> los funcionarios, y extender el catálogo de trámites —algo que el sistema ya permite
> sin tocar código, porque el catálogo es configurable desde la administración.

---

## Hoja 15 — Cierre

> Con esto cierro la presentación. El código completo, la documentación de ingeniería
> y el informe formal están públicos en el repositorio
> `github.com/edjrey22/Sitram`. Quedo atento a sus preguntas.

---

## Recorrido por `docs/` (si te lo piden o hay tiempo extra)

Si el jurado pide "muéstranos dónde está la documentación", ábrela en este orden:

1. **`docs/README.md`** — es el índice. Muestra la tabla de contenidos y la tabla de
   "Estado" al final (todo en verde: documentación completa, 146/146 tests).
2. **`docs/requisitos.md`** — los 42 RF y 26 RNF con su actor y prioridad MoSCoW; la
   sección 4 muestra la trazabilidad de requisito a diseño.
3. **`docs/arquitectura.md`** — el diagrama de capas en ASCII y la tabla resumen de
   los 7 ADR al final.
4. **`docs/modelo-datos.md`** — el diagrama entidad-relación en Mermaid y la tabla de
   clasificación/cifrado de columnas (sección 3).
5. **`docs/decisiones/`** — la carpeta con los 7 archivos ADR individuales, más
   `docs/decisiones/README.md` como índice con el estado de cada uno.
6. **`docs/errores-conocidos.md`** — si preguntan "¿tuvieron problemas durante el
   desarrollo?", este es el archivo que responde con evidencia: cada entrada tiene
   síntoma, causa raíz y solución probada (por ejemplo, la sección 3.6 documenta el
   incidente real de una clave de cifrado desincronizada entre entornos).
7. **`docs/convenciones.md`** y **`docs/flujo-de-trabajo.md`** — si preguntan sobre
   normas de código o el flujo de Git/CI, sin necesidad de entrar al detalle a menos
   que insistan.

## Recorrido por el código (si piden "muéstranos el código")

Si te piden abrir Visual Studio Code y navegar en vivo, este es el orden que mejor
cuenta la historia (de afuera hacia adentro, como manda Clean Architecture):

1. `Sitram.slnx` — la solución completa: 4 proyectos de código, 3 de pruebas.
2. `src/Sitram.Domain/Tramites/Tramite.cs` — el agregado raíz y la máquina de estados.
3. `src/Sitram.Application/Tramites/Commands/Transiciones/` — los *commands* que
   orquestan cada transición (uno por caso de uso, patrón CQRS con MediatR).
4. `src/Sitram.Infrastructure/Persistence/Cifrado/CifradoColumna.cs` — el cifrado a
   nivel de columna.
5. `src/Sitram.Infrastructure/DependencyInjection.cs` — dónde se conecta EF Core con
   Npgsql (PostgreSQL), para mostrar que la migración de motor solo tocó este archivo.
6. `src/Sitram.Api/Program.cs` — las políticas de autorización basadas en claims.
7. `src/Sitram.Api/Controllers/TramitesController.cs` — cómo un endpoint HTTP se
   conecta a un *command* vía `[Authorize(Policy = "...")]` + `Sender.Send(...)`.
8. `src/Sitram.Web/Components/Pages/` — cualquier página Blazor (por ejemplo
   `AdministrarTramites.razor`) para mostrar el frontend real funcionando.
9. `tests/Sitram.Domain.Tests/` — una prueba de la máquina de estados, para cerrar
   demostrando que las reglas del paso 2 están verificadas automáticamente.

**Regla de oro para esta parte**: no navegues más de 9 archivos ni más de 3-4 minutos
en código — el jurado evalúa el discurso y el criterio, no una demo exhaustiva.
