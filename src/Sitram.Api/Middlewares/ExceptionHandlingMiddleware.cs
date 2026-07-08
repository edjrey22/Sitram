using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sitram.Application.Common.Exceptions;
using Sitram.Domain.Exceptions;

namespace Sitram.Api.Middlewares;

/// <summary>
/// Middleware global de excepciones. Traduce cualquier error a un
/// <see cref="ProblemDetails"/> (RFC 7807) genérico y registra el detalle del lado servidor,
/// sin filtrar datos personales ni <i>stack traces</i> al cliente (RNF-006, errores-conocidos 4.2).
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error no controlado al procesar {Method} {Path}", context.Request.Method, context.Request.Path);

            var (status, title) = ex switch
            {
                ValidationException => (HttpStatusCode.BadRequest, "Error de validación"),
                TipoTramiteNoDisponibleException => (HttpStatusCode.BadRequest, "Tipo de trámite no disponible"),
                PagoRequeridoException => (HttpStatusCode.PaymentRequired, "Pago requerido"),
                AutenticacionInvalidaException => (HttpStatusCode.Unauthorized, "Autenticación inválida"),
                NotFoundException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
                TransicionInvalidaException => (HttpStatusCode.Conflict, "Transición de estado inválida"),
                DbUpdateConcurrencyException => (HttpStatusCode.Conflict, "Conflicto de concurrencia; vuelva a intentarlo"),
                DomainException => (HttpStatusCode.BadRequest, "Regla de negocio no satisfecha"),
                _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado"),
            };

            var problem = new ProblemDetails
            {
                Status = (int)status,
                Title = title,
                // No se expone el mensaje interno en errores 500
                Detail = status == HttpStatusCode.InternalServerError ? null : ex.Message,
            };

            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
