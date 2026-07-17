// Pruebas end-to-end de SITRAM contra el frontend real (localhost:5049) y la API real
// (localhost:5299), corriendo en un navegador de verdad via Playwright. Captura evidencia
// en capturas/ para el informe de pruebas E2E.
const { chromium } = require("playwright");
const fs = require("fs");
const path = require("path");
const { execSync } = require("child_process");

const BASE = "http://localhost:5049";
const CAPS = path.join(__dirname, "capturas");
const WEB_LOG = "C:\\Users\\edjor\\AppData\\Local\\Temp\\web.log";
const sufijo = Date.now().toString().slice(-6);
const ciudadano = {
  nombres: "Elena",
  apellidos: "Quispe Huamán",
  dni: "70" + sufijo,
  correo: `elena.quispe.${sufijo}@example.com`,
  telefono: "987654321",
  direccion: "Jr. Los Álamos 123, Ayacucho",
  usuario: `equispe${sufijo}`,
  password: "Ciudadano#2026",
};
const ADMIN = { usuario: "administrador", password: "Admin#Sitram2026" };
const funcionario = {
  usuario: `rpalomino${sufijo}`,
  correo: `rosa.palomino.${sufijo}@example.com`,
  password: "Funcionario#2026",
};

const resultados = [];
function registrar(id, nombre, rf, resultado, notas, captura) {
  resultados.push({ id, nombre, rf, resultado, notas, captura });
  console.log(`[${resultado}] ${id} ${nombre} — ${notas}`);
}

function buscarEnlaceConfirmacion(correo) {
  const log = fs.readFileSync(WEB_LOG, "utf8");
  const idx = log.lastIndexOf(correo.replace(/(.{2}).*(@.*)/, "$1"));
  const m = log.match(/https:\/\/sitram\.local\/confirmar-email\?usuarioId=([\w-]+)&token=([^\s]+)/g);
  if (!m || m.length === 0) return null;
  return m[m.length - 1]; // el mas reciente
}

async function hayError(page) {
  return await page.locator(".alert-danger").count() > 0;
}

(async () => {
  const browser = await chromium.launch();
  const context = await browser.newContext({ viewport: { width: 1280, height: 900 } });
  const page = await context.newPage();

  try {
    // TC-01: Registro de ciudadano (RF-001)
    await page.goto(`${BASE}/registro`, { waitUntil: "networkidle" });
    await page.getByLabel("Nombres").fill(ciudadano.nombres);
    await page.getByLabel("Apellidos").fill(ciudadano.apellidos);
    await page.getByLabel("DNI (8 dígitos)").fill(ciudadano.dni);
    await page.getByLabel("Correo electrónico").fill(ciudadano.correo);
    await page.getByLabel("Teléfono").fill(ciudadano.telefono);
    await page.getByLabel("Dirección").fill(ciudadano.direccion);
    await page.getByLabel("Nombre de usuario").fill(ciudadano.usuario);
    await page.getByLabel(/Contraseña/).fill(ciudadano.password);
    await page.screenshot({ path: path.join(CAPS, "tc01-registro-formulario.png") });
    await page.getByRole("button", { name: "Crear cuenta" }).click();
    await page.waitForURL(/\/login/, { timeout: 8000 }).catch(() => {});
    await page.waitForTimeout(500);
    await page.screenshot({ path: path.join(CAPS, "tc01-registro-resultado.png") });
    const okRegistro = page.url().includes("/login") && page.url().includes("exito");
    registrar("TC-01", "Registro de ciudadano", "RF-001", okRegistro ? "OK" : "FALLO",
      okRegistro ? `Usuario ${ciudadano.usuario}, DNI ${ciudadano.dni}, redirige a login con mensaje de éxito` : "No redirigió a /login con éxito",
      "tc01-registro-resultado.png");

    // TC-02: Confirmación de correo (RF-001) — el enlace real llega por correo; en desarrollo
    // (SMTP no configurado) se registra en el log del servidor (ver docs/errores-conocidos.md)
    await page.waitForTimeout(300);
    const enlace = buscarEnlaceConfirmacion(ciudadano.correo);
    if (!enlace) {
      registrar("TC-02", "Confirmación de correo", "RF-001", "FALLO", "No se encontró el enlace en el log del servidor", null);
    } else {
      const url = enlace.replace("https://sitram.local", BASE);
      await page.goto(url, { waitUntil: "networkidle" });
      await page.waitForTimeout(500);
      await page.screenshot({ path: path.join(CAPS, "tc02-confirmar-email.png") });
      // fijar contraseña definitiva
      const inputs = page.locator('input[type="password"]');
      await inputs.nth(0).fill(ciudadano.password);
      await inputs.nth(1).fill(ciudadano.password);
      await page.getByRole("button", { name: /Guardar contraseña/ }).click();
      await page.waitForTimeout(800);
      await page.screenshot({ path: path.join(CAPS, "tc02-confirmar-email-resultado.png") });
      const texto = await page.locator("main").innerText();
      const ok = texto.includes("Contraseña actualizada");
      registrar("TC-02", "Confirmación de correo y fijar contraseña", "RF-001", ok ? "OK" : "FALLO",
        ok ? "Correo confirmado, contraseña definitiva establecida" : "No se confirmó correctamente", "tc02-confirmar-email-resultado.png");
    }

    // TC-03: Login del ciudadano recién registrado (RF-002)
    await page.goto(`${BASE}/login`, { waitUntil: "networkidle" });
    await page.getByRole("textbox").first().fill(ciudadano.usuario);
    await page.locator('input[type="password"]').fill(ciudadano.password);
    await page.getByRole("button", { name: "Iniciar sesión" }).click();
    await page.waitForTimeout(1200);
    await page.screenshot({ path: path.join(CAPS, "tc03-login-ciudadano.png") });
    const loginOk = !(await hayError(page)) && page.url() === `${BASE}/`;
    registrar("TC-03", "Login de ciudadano", "RF-002", loginOk ? "OK" : "FALLO",
      loginOk ? `Sesión iniciada como ${ciudadano.usuario}, redirige al dashboard` : "No pudo iniciar sesión", "tc03-login-ciudadano.png");

    // TC-04: Iniciar un trámite (RF-020)
    await page.goto(`${BASE}/tramites/nuevo`, { waitUntil: "networkidle" });
    await page.waitForTimeout(500);
    await page.screenshot({ path: path.join(CAPS, "tc04-catalogo-tramites.png") });
    const tarjetas = page.locator(".card[style*='cursor:pointer']");
    const nCatalogo = await tarjetas.count();
    if (nCatalogo === 0) {
      registrar("TC-04", "Iniciar trámite", "RF-020", "FALLO", "El catálogo de trámites está vacío", "tc04-catalogo-tramites.png");
    } else {
      await tarjetas.first().click();
      await page.waitForTimeout(1200);
      await page.screenshot({ path: path.join(CAPS, "tc04-tramite-iniciado.png") });
      const urlOk = /\/tramites\/[0-9a-f-]{36}/.test(page.url());
      registrar("TC-04", "Iniciar trámite", "RF-020", urlOk ? "OK" : "FALLO",
        urlOk ? `Trámite creado, redirige a ${page.url()}` : "No redirigió al detalle del trámite", "tc04-tramite-iniciado.png");

      // TC-05: Seguimiento del trámite (RF-050)
      await page.waitForTimeout(1500);
      await page.screenshot({ path: path.join(CAPS, "tc05-seguimiento-tramite.png") });
      const textoDetalle = await page.locator("main").innerText();
      const tieneEstado = /Recibido|Borrador|EnRevision|En revisión/i.test(textoDetalle);
      registrar("TC-05", "Consultar estado del trámite", "RF-050", tieneEstado ? "OK" : "FALLO",
        tieneEstado ? "El detalle muestra el estado actual del expediente" : "No se encontró el estado en la página", "tc05-seguimiento-tramite.png");
    }

    // Logout (limpiar cookie) antes de entrar como administrador
    await context.clearCookies();

    // TC-06: Login como administrador
    await page.goto(`${BASE}/login`, { waitUntil: "networkidle" });
    await page.getByRole("textbox").first().fill(ADMIN.usuario);
    await page.locator('input[type="password"]').fill(ADMIN.password);
    await page.getByRole("button", { name: "Iniciar sesión" }).click();
    await page.waitForTimeout(1200);
    await page.screenshot({ path: path.join(CAPS, "tc06-login-admin.png") });
    const adminOk = !(await hayError(page));
    registrar("TC-06", "Login de administrador", "RF-002", adminOk ? "OK" : "FALLO",
      adminOk ? "Sesión iniciada como administrador" : "No pudo iniciar sesión", "tc06-login-admin.png");

    // TC-07: Administrar catálogo de trámites (RF-010)
    await page.goto(`${BASE}/administracion/tipos-tramite`, { waitUntil: "networkidle" });
    await page.waitForTimeout(500);
    await page.screenshot({ path: path.join(CAPS, "tc07-administrar-tramites.png") });
    const accesoOk7 = !page.url().includes("acceso-denegado");
    registrar("TC-07", "Administrar catálogo de trámites (TUPA)", "RF-010", accesoOk7 ? "OK" : "FALLO",
      accesoOk7 ? "El administrador accede al catálogo de tipos de trámite" : "Acceso denegado inesperado", "tc07-administrar-tramites.png");

    // TC-08: Crear cuenta de funcionario (RF-006)
    await page.goto(`${BASE}/administracion/funcionarios`, { waitUntil: "networkidle" });
    await page.waitForTimeout(300);
    await page.locator("#f-user").fill(funcionario.usuario);
    await page.locator("#f-email").fill(funcionario.correo);
    await page.locator("#f-pass").fill(funcionario.password);
    await page.locator("#f-rol").selectOption("MesaDePartes");
    await page.screenshot({ path: path.join(CAPS, "tc08-crear-funcionario-formulario.png") });
    await page.getByRole("button", { name: "Crear cuenta de funcionario" }).click();
    await page.waitForTimeout(1200);
    await page.screenshot({ path: path.join(CAPS, "tc08-crear-funcionario-resultado.png") });
    const errorFunc = await hayError(page);
    registrar("TC-08", "Crear cuenta de funcionario", "RF-006", errorFunc ? "FALLO" : "OK",
      errorFunc ? "El formulario reportó un error" : `Funcionario ${funcionario.usuario} creado con rol MesaDePartes`, "tc08-crear-funcionario-resultado.png");

    // TC-09: Control de acceso negativo — ciudadano intenta acceder a administración (RNF-005)
    await context.clearCookies();
    await page.goto(`${BASE}/login`, { waitUntil: "networkidle" });
    await page.getByRole("textbox").first().fill(ciudadano.usuario);
    await page.locator('input[type="password"]').fill(ciudadano.password);
    await page.getByRole("button", { name: "Iniciar sesión" }).click();
    await page.waitForTimeout(1000);
    await page.goto(`${BASE}/administracion/funcionarios`, { waitUntil: "networkidle" });
    await page.waitForTimeout(500);
    await page.screenshot({ path: path.join(CAPS, "tc09-acceso-denegado.png") });
    const textoNeg = await page.locator("body").innerText();
    const denegado = page.url().includes("acceso-denegado") || /denegad|no autorizad|403/i.test(textoNeg);
    registrar("TC-09", "Control de acceso: ciudadano sin permiso de administración", "RNF-005", denegado ? "OK" : "FALLO",
      denegado ? "El acceso se deniega correctamente (RBAC en servidor)" : "¡El ciudadano pudo acceder a administración!", "tc09-acceso-denegado.png");

    await browser.close();
  } catch (err) {
    console.error("ERROR:", err.message);
    await page.screenshot({ path: path.join(CAPS, "error-inesperado.png") }).catch(() => {});
    await browser.close();
  }

  fs.writeFileSync(path.join(__dirname, "_resultados.json"), JSON.stringify(resultados, null, 2));
  console.log("\n--- RESUMEN ---");
  for (const r of resultados) console.log(`${r.id} [${r.resultado}] ${r.nombre} (${r.rf})`);
})();
