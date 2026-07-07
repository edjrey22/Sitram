namespace Sitram.Integration.Tests;

/// <summary>
/// Colección compartida para todas las pruebas de integración: garantiza que
/// <see cref="SitramWebFactory"/> (y su recreación de esquema) se instancie **una sola vez**
/// por ejecución, evitando que dos clases de prueba recreen la BD en paralelo
/// (errores-conocidos 5.1: pruebas de integración que se contaminan entre sí).
/// </summary>
[CollectionDefinition("Integración")]
public sealed class IntegrationTestCollection : ICollectionFixture<SitramWebFactory>;
