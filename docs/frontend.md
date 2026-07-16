# Frontend — SITRAM

> Mapa de páginas del cliente web. Blazor Web App con **render interactivo en servidor**
> (justificación completa en [ADR-0006](decisiones/ADR-0006-frontend-blazor.md)): la lógica
> de la interfaz corre en el servidor, el navegador solo recibe las diferencias del DOM por
> una conexión en tiempo real — los datos personales nunca se descargan al cliente.

## 1. Páginas y rutas

| Página | Ruta | Autorización | Propósito |
|--------|------|---------------|-----------|
| `Login.razor` | `/login` | Pública | Inicio de sesión (usuario o DNI) |
| `Registro.razor` | `/registro` | Pública | Alta de ciudadano (RF-001) |
| `ConfirmarEmail.razor` | `/confirmar-email` | Pública (token) | Verifica el correo y fija la contraseña |
| `RecuperarContrasena.razor` | `/recuperar-contrasena` | Pública | Solicita enlace de recuperación (RF-004) |
| `RestablecerContrasena.razor` | `/restablecer-contrasena` | Pública (token) | Fija nueva contraseña |
| `VerificarMfa.razor` | `/verificar-mfa` | Pública (flujo de login) | Segundo factor por correo (RF-005) |
| `Dashboard.razor` | `/` | `TramiteConsultar` | Lista de trámites del usuario autenticado |
| `Detalle.razor` | `/tramites/{Id:guid}` | `TramiteConsultar` | Detalle y seguimiento de un trámite |
| `NuevoTramite.razor` | `/tramites/nuevo` | `TramiteIniciar` | Iniciar un trámite (RF-020) |
| `Notificaciones.razor` | `/notificaciones` | Autenticado | Notificaciones del usuario |
| `Perfil.razor` | `/perfil` | Autenticado | Datos del perfil (ciudadano o funcionario) |
| `CrearFuncionario.razor` | `/administracion/funcionarios` | `AdministracionGestionar` | Alta de cuentas de funcionario con rol (RF-006) |
| `AdministrarTramites.razor` | `/administracion/tipos-tramite` | `AdministracionGestionar` | CRUD del catálogo TUPA (RF-010…013) |
| `AccesoDenegado.razor` | `/acceso-denegado` | Pública | Página de error 403 |
| `NotFound.razor` | `/not-found` | Pública | Página de error 404 |
| `Error.razor` | `/Error` | Pública | Página de error genérica no controlada |

## 2. Patrón de página autorizada

Todas las páginas protegidas siguen la misma plantilla (ver `CrearFuncionario.razor` o
`AdministrarTramites.razor` como referencia):

```razor
@page "/ruta"
@rendermode InteractiveServer
@attribute [Authorize(Policy = "NombreDePolitica")]
@inject ISender Sender
```

- El acceso se valida **en el servidor** vía la política (nunca solo ocultando UI).
- La página envía `Command`/`Query` al mismo `Sender` (MediatR) que usa la API — comparte la
  capa `Application` con `Sitram.Api`, sin duplicar lógica de negocio.
- Estados de carga (`_cargando`), error (`_error`) y éxito (`_mensajeExito`) siguen la misma
  convención de nombres en todo el proyecto.

## 3. Navegación condicionada por rol

`Components/Layout/MainLayout.razor` construye el menú (escritorio y móvil) con bloques
`<AuthorizeView Policy="...">`: un enlace solo aparece si el usuario tiene la política
correspondiente. Políticas usadas hoy en la navegación: `TramiteConsultar`, `TramiteIniciar`,
`AdministracionGestionar`.

> **Hueco conocido**: no hay todavía una página para Mesa de Partes (política
> `TramiteRecepcionar`, RF-024) ni su entrada de navegación — el endpoint de API existe
> (`POST /api/tramites/{id}/revision`) pero el frontend no se construyó. Ver
> [errores-conocidos.md](errores-conocidos.md) para el patrón general de este tipo de huecos.

## 4. Autenticación en el cliente

- Cookie de sesión emitida por Blazor Server tras el login (no se expone el JWT al navegador).
- `IIdentityService`/`ICurrentUserService` exponen el usuario autenticado a las páginas.
- `returnUrl` se valida como ruta local antes de redirigir (anti *open redirect*), ver
  `Login.razor`.
