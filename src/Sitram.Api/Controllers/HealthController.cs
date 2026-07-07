using Microsoft.AspNetCore.Mvc;

namespace Sitram.Api.Controllers;

/// <summary>Endpoint de verificación de estado del servicio.</summary>
[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(new { estado = "ok", servicio = "SITRAM", utc = DateTime.UtcNow });
}
