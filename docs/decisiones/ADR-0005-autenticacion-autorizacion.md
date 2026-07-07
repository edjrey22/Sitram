# ADR-0005: Autenticación con Identity/JWT y autorización RBAC

- **Estado**: Aceptada
- **Fecha**: 2026-07-01
- **Decisores**: Equipo de proyecto

## Contexto

SITRAM tiene **actores con privilegios muy distintos**: ciudadanos (inician y siguen sus
trámites), funcionarios de mesa de partes (reciben), revisores (evalúan), jefes de área
(aprueban/rechazan) y administradores (configuran). El acceso a cada operación debe
controlarse de forma estricta y auditable.

## Decisión

### Autenticación
- **ASP.NET Core Identity** para gestión de usuarios, contraseñas y bloqueo por intentos.
- **JWT (JSON Web Tokens)** para autenticar peticiones a la API sin estado de sesión en
  servidor.
- **Refresh tokens** rotativos y de vida corta para el token de acceso (mitiga robo de token).
- **MFA opcional** para cuentas de funcionario (segundo factor por correo/OTP).

### Autorización — RBAC (Role-Based Access Control)
Modelo de **roles → permisos**, con principio de **mínimo privilegio**:

| Rol | Permisos principales |
|-----|----------------------|
| `Ciudadano` | Crear trámite propio, subir documentos, pagar, consultar estado propio |
| `MesaDePartes` | Recepcionar trámites, validar documentos de admisibilidad |
| `Revisor` | Evaluar expediente, observar, solicitar subsanación |
| `JefeDeArea` | Aprobar / rechazar trámites de su área |
| `Administrador` | Gestionar tipos de trámite, roles, tasas y parámetros |
| `Auditor` | Solo lectura del registro de auditoría |
| `OficialDatosPersonales` | Gestionar solicitudes ARCO e incidentes de seguridad (D.S. 016-2024-JUS) |

La autorización se implementa con **políticas** (`policy-based authorization`) basadas en
*claims* de permiso, no en el nombre del rol directamente. Esto permite cambiar qué permisos
tiene un rol sin tocar el código de los controllers.

```csharp
// Ejemplo de política
options.AddPolicy("PuedeAprobarTramite", policy =>
    policy.RequireClaim("permiso", "tramite:aprobar"));

// En el controller
[Authorize(Policy = "PuedeAprobarTramite")]
public async Task<IActionResult> Aprobar(...) { ... }
```

## Alternativas consideradas

| Opción | Por qué se descartó |
|--------|---------------------|
| **Autorización solo por rol** (`[Authorize(Roles="...")]`) | Rígido: cambiar permisos exige recompilar; difícil de auditar |
| **Sesiones con cookies en servidor** | Estado en servidor complica el escalado; JWT es stateless |
| **Proveedor externo (Auth0/Azure AD B2C)** | Añade costo y dependencia externa innecesaria para el alcance del curso |

## Consecuencias

**Positivas**
- Control de acceso granular, auditable y modificable sin recompilar.
- API sin estado → más simple de escalar y probar.
- Alineado con OWASP (control de acceso roto es el riesgo #1 del OWASP Top 10).

**Negativas / mitigaciones**
- Revocación de JWT no es inmediata. → Tokens de acceso de vida corta + lista de revocación
  para refresh tokens.
- Complejidad de gestionar claims de permisos. → Se centraliza la asignación rol→permiso en
  una tabla de configuración administrable.
