# ADR-0006: Frontend con Blazor (Web App, render interactivo en servidor)

- **Estado**: Aceptada
- **Fecha**: 2026-07-06
- **Decisores**: Equipo de proyecto

## Contexto

SITRAM necesita una interfaz de usuario web, **responsiva** y *mobile-first* ([RNF-040](../requisitos.md)),
sobre la que se evalúa la usabilidad con la escala SUS (variable X4). La capa de presentación
es también una **superficie de ataque** y un punto de exposición de datos personales: dónde se
ejecuta la lógica, dónde se guarda el token de sesión y cuántas dependencias de terceros entran
al navegador son decisiones con impacto directo en la **seguridad** y en la **protección de
datos** (Ley N.° 29733), que son los ejes rectores del proyecto.

El backend ya está definido en .NET 10 con Clean Architecture ([ADR-0002](ADR-0002-clean-architecture.md)),
autenticación Identity/JWT y autorización RBAC ([ADR-0005](ADR-0005-autenticacion-autorizacion.md)).

## Alternativas consideradas

| Opción | Seguridad / protección de datos | Por qué se descartó (o no) |
|--------|---------------------------------|----------------------------|
| **React + TypeScript (SPA)** | Código y datos se ejecutan en el navegador; el token se almacena en el cliente (localStorage/memory), expuesto a robo por XSS; gran superficie de dependencias npm | Estándar del mercado, pero añade un segundo lenguaje y una cadena de suministro JS amplia que auditar; mayor exposición de PII y token |
| **Angular + TypeScript (SPA)** | Igual que React: cliente pesado, token en el navegador | Framework maduro (uno de los antecedentes lo usó), pero mismos inconvenientes de exposición y toolchain separado |
| **Blazor WebAssembly (SPA en C#)** | Un solo lenguaje, pero el código C# y los datos igualmente descienden y corren en el navegador; token en el cliente | Evita npm, pero no reduce la exposición de datos/lógica en el cliente |
| **Blazor Web App — render interactivo en servidor** ✅ | La lógica y los datos permanecen en el servidor; el navegador solo recibe *diffs* de UI; el token/sesión no vive en el cliente | **Elegida** (ver abajo) |

## Decisión

Se adopta **Blazor Web App sobre .NET 10** con **modo de render interactivo en servidor**
(*Interactive Server*) como opción por defecto. Se reserva el render **WebAssembly** solo para
componentes puntuales que exijan interactividad sin conexión **y** que **no manejen datos
personales**.

La **Web API REST** ([ADR-0002](ADR-0002-clean-architecture.md)) se mantiene como puerto de
entrada para **integraciones externas** y una eventual app móvil; la UI Blazor y la API consumen
la **misma capa `Application`**, sin duplicar reglas de negocio.

## Justificación (por qué es la mejor opción para seguridad y protección de datos)

1. **Minimización de exposición de datos.** El C# se ejecuta en el servidor; al cliente solo
   viajan diferencias de UI por SignalR. Los DTOs, las reglas y la **PII (DNI, correo, teléfono)
   no se descargan al navegador**, alineándose con la minimización de datos ([RNF-010, RNF-011](../requisitos.md))
   y con [ADR-0004](ADR-0004-seguridad-proteccion-datos.md).
2. **Sin token en el navegador.** La sesión autenticada vive en el circuito del servidor con
   **cookie `httpOnly`**, lo que neutraliza el robo de token por XSS —un vector que sí afecta a
   las SPA que guardan el JWT en el cliente— (OWASP A07; [errores-conocidos 3.4](../errores-conocidos.md)).
3. **Protección XSS por defecto.** El motor Razor **codifica la salida** automáticamente
   (OWASP A03: *Injection*).
4. **Menor cadena de suministro.** Un solo lenguaje y toolchain (C#), sin ecosistema npm →
   menos dependencias de terceros que auditar (OWASP A06: *Vulnerable and Outdated Components*).
5. **Reutilización de contratos y autorización.** Se comparten DTOs y validadores
   (`FluentValidation`) con `Application`, y las **mismas políticas RBAC** se aplican en la UI y
   en la API, sin lógica de autorización en el cliente ([errores-conocidos 3.3](../errores-conocidos.md)).

## Consecuencias

**Positivas**
- Superficie de exposición de datos y de token mínima → refuerza el cumplimiento de la Ley 29733.
- Un solo lenguaje/stack → menor complejidad para un equipo reducido y trazabilidad del lenguaje
  ubicuo también en la UI.

**Negativas / mitigaciones**
- Requiere **conexión persistente** (SignalR) y el estado de UI reside en el servidor. → Para el
  objetivo de disponibilidad del 99 % ([RNF-030](../requisitos.md)) es suficiente; ante escala
  mayor se añade un *backplane* (Redis) y balanceo con afinidad de sesión.
- Latencia de red en cada interacción. → Aceptable para formularios de trámite; los componentes
  muy interactivos y sin PII pueden pasar a WebAssembly.

## Referencias
- [ADR-0004 Seguridad y protección de datos](ADR-0004-seguridad-proteccion-datos.md) · OWASP Top 10 (A01, A03, A06, A07).
- [Requisitos RNF-040 (usabilidad), RNF-010/011 (datos en logs/minimización)](../requisitos.md).
