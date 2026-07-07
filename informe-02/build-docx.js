const fs = require("fs");
const path = require("path");
const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  Header, Footer, AlignmentType, LevelFormat, TabStopType,
  TableOfContents, HeadingLevel, BorderStyle, WidthType, ShadingType,
  PageNumber, ExternalHyperlink, ImageRun,
} = require("docx");

// ancho/alto (px) desde el chunk IHDR de un PNG (big-endian, offsets 16 y 20)
function pngDims(buf) {
  return { w: buf.readUInt32BE(16), h: buf.readUInt32BE(20) };
}

const DIR = __dirname;
const CONTENT_WIDTH = 9026; // A4 (11906) - 2*1440 margenes

// ---------- configuracion de encabezado / pie / portada ----------
const CFG = {
  university: "UNIVERSIDAD NACIONAL DE SAN CRISTÓBAL DE HUAMANGA",
  faculty: "FACULTAD DE INGENIERÍA DE MINAS, GEOLOGÍA Y CIVIL",
  school: "ESCUELA PROFESIONAL DE INGENIERÍA DE SISTEMAS",
  kind: "INFORME",
  title: "Desarrollo de una plataforma web para la gestión de trámites municipales con enfoque en seguridad y protección de datos personales, 2026",
  author: "Cuadros Reyes Edson Jorge",
  teacher: "Mg. Ing. Richard Zapata Casaverde",
  course: "Pruebas y Aseguramiento de la Calidad de Software",
  place: "Ayacucho – Perú",
  date: "Julio, 2026",
  // encabezado
  hdrLeft1: "SITRAM — Sistema Integrado de Trámites Municipales",
  hdrLeft2: "Plataforma web · .NET 10 + Blazor",
  hdrRight1: "Documentación Técnica",
  hdrRight2: "Versión 1.0.0",
};

// APA: interlineado 1.5 = 360; sencillo = 240; sangria de primera linea = 720 (0.5")
const LINE = 360;
const INDENT = 720;

// ---------- inline markdown -> TextRun[] ----------
function parseInline(text, baseOpts = {}) {
  const runs = [];
  let rem = text;
  const pattern = /(\*\*([^*]+)\*\*|`([^`]+)`|\[([^\]]+)\]\(([^)]+)\)|\*([^*]+)\*)/;
  while (rem.length) {
    const m = rem.match(pattern);
    if (!m) { runs.push(new TextRun({ text: rem, ...baseOpts })); break; }
    if (m.index > 0) runs.push(new TextRun({ text: rem.slice(0, m.index), ...baseOpts }));
    const tok = m[0];
    if (tok.startsWith("**")) runs.push(new TextRun({ text: m[2], bold: true, ...baseOpts }));
    else if (tok.startsWith("`")) runs.push(new TextRun({ text: m[3], font: "Courier New", ...baseOpts }));
    else if (tok.startsWith("[")) runs.push(new ExternalHyperlink({ children: [new TextRun({ text: m[4], style: "Hyperlink" })], link: m[5] }));
    else if (tok.startsWith("*")) runs.push(new TextRun({ text: m[6], italics: true, ...baseOpts }));
    rem = rem.slice(m.index + tok.length);
  }
  return runs.length ? runs : [new TextRun({ text: "", ...baseOpts })];
}

// una linea es "especial" (figura/tabla/nota) => sin sangria de primera linea
function isCaption(t) {
  return /^(\*\*)?\s*(Figura|Tabla)\s+\d+/i.test(t) || /^\*Nota\./i.test(t);
}

// ¿la linea inicia un bloque nuevo? (para saber cuando cortar la union de un parrafo)
function startsBlock(tr) {
  return tr === "" || tr === "---"
    || /^(#{1,4})\s+/.test(tr) || tr.startsWith("|") || tr.startsWith(">")
    || tr.startsWith("```") || tr.startsWith("<") || tr.startsWith("&")
    || tr.startsWith("![")
    || /^[-*]\s+/.test(tr) || /^\d+\.\s+/.test(tr);
}

function bodyPara(text, { hangingRefs = false } = {}) {
  const caption = isCaption(text);
  let indent;
  if (hangingRefs) indent = { left: INDENT, hanging: INDENT };   // sangria francesa (referencias)
  else if (!caption) indent = { firstLine: INDENT };             // APA: sangria de primera linea
  return new Paragraph({
    alignment: AlignmentType.LEFT,                               // APA: alineado a la izquierda
    spacing: { after: 0, line: LINE },
    indent,
    keepNext: caption || undefined,
    children: parseInline(text),
  });
}

const HEADINGS = {
  1: HeadingLevel.HEADING_1, 2: HeadingLevel.HEADING_2,
  3: HeadingLevel.HEADING_3, 4: HeadingLevel.HEADING_4,
};

function splitRow(line) {
  return line.trim().replace(/^\|/, "").replace(/\|$/, "").split("|").map(c => c.trim());
}

function buildTable(tableLines) {
  const rows = tableLines.filter(l => l.trim().startsWith("|"));
  if (rows.length < 2) return null;
  const header = splitRow(rows[0]);
  const body = rows.slice(2).map(splitRow);
  const n = header.length;
  const base = Math.floor(CONTENT_WIDTH / n);
  const widths = Array(n).fill(base);
  widths[n - 1] = CONTENT_WIDTH - base * (n - 1);
  const border = { style: BorderStyle.SINGLE, size: 1, color: "BBBBBB" };
  const borders = { top: border, bottom: border, left: border, right: border };
  const mkCell = (txt, i, isHeader) => new TableCell({
    borders,
    width: { size: widths[i], type: WidthType.DXA },
    shading: isHeader ? { fill: "D5E8F0", type: ShadingType.CLEAR, color: "auto" } : undefined,
    margins: { top: 60, bottom: 60, left: 100, right: 100 },
    children: [new Paragraph({
      spacing: { after: 0, line: 240 },   // tablas: interlineado sencillo (permitido por APA)
      children: parseInline(txt, isHeader ? { bold: true } : {}),
    })],
  });
  const trs = [];
  trs.push(new TableRow({ tableHeader: true, children: header.map((c, i) => mkCell(c, i, true)) }));
  for (const r of body) {
    const cells = [];
    for (let i = 0; i < n; i++) cells.push(mkCell(r[i] || "", i, false));
    trs.push(new TableRow({ children: cells }));
  }
  return new Table({ width: { size: CONTENT_WIDTH, type: WidthType.DXA }, columnWidths: widths, rows: trs });
}

// ---------- parse de un archivo markdown a elementos docx ----------
function parseMarkdown(file, { startMarker = null, breakOnH2 = false, hangingRefs = false } = {}) {
  let lines = fs.readFileSync(path.join(DIR, file), "utf8").split(/\r?\n/);
  if (startMarker) {
    const idx = lines.findIndex(l => l.startsWith(startMarker));
    if (idx >= 0) lines = lines.slice(idx);
  }
  const out = [];
  let i = 0;
  let inFence = false, fenceBuf = [];
  let firstHeading = true;
  while (i < lines.length) {
    const line = lines[i];
    const trimmed = line.trim();

    // bloque de codigo / figura (monoespaciado, sencillo)
    if (trimmed.startsWith("```")) {
      if (!inFence) { inFence = true; fenceBuf = []; }
      else {
        inFence = false;
        for (const fl of fenceBuf) {
          out.push(new Paragraph({
            spacing: { after: 0, line: 240 },
            children: [new TextRun({ text: fl || " ", font: "Courier New", size: 16 })],
          }));
        }
        out.push(new Paragraph({ spacing: { after: 80 }, children: [] }));
      }
      i++; continue;
    }
    if (inFence) { fenceBuf.push(line); i++; continue; }

    // saltar html/comentarios/hr
    if (trimmed.startsWith("<") || trimmed.startsWith("&") || trimmed === "---" || trimmed === "") { i++; continue; }

    // encabezado
    const hm = trimmed.match(/^(#{1,4})\s+(.*)$/);
    if (hm) {
      const level = hm[1].length;
      const brk = (breakOnH2 && level === 2 && !firstHeading);
      out.push(new Paragraph({
        heading: HEADINGS[level],
        pageBreakBefore: brk || undefined,
        spacing: { before: 200, after: 120, line: LINE },
        children: parseInline(hm[2].replace(/\*\*/g, "")),
      }));
      firstHeading = false;
      i++; continue;
    }

    // imagen embebida: ![alt](ruta)
    const im = trimmed.match(/^!\[[^\]]*\]\(([^)]+)\)$/);
    if (im) {
      const imgPath = path.join(DIR, im[1]);
      if (fs.existsSync(imgPath)) {
        const data = fs.readFileSync(imgPath);
        const { w, h } = pngDims(data);
        const width = Math.min(w, 560);
        const height = Math.round(h * (width / w));
        out.push(new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 80, line: 240 },
          children: [new ImageRun({ type: "png", data, transformation: { width, height } })],
        }));
      }
      i++; continue;
    }

    // tabla
    if (trimmed.startsWith("|")) {
      const tbl = [];
      while (i < lines.length && lines[i].trim().startsWith("|")) { tbl.push(lines[i]); i++; }
      const t = buildTable(tbl);
      if (t) { out.push(t); out.push(new Paragraph({ spacing: { after: 120 }, children: [] })); }
      continue;
    }

    // cita / nota
    if (trimmed.startsWith(">")) {
      const buf = [];
      while (i < lines.length && lines[i].trim().startsWith(">")) {
        buf.push(lines[i].trim().replace(/^>\s?/, "")); i++;
      }
      out.push(new Paragraph({
        alignment: AlignmentType.LEFT,
        spacing: { after: 120, line: LINE },
        indent: { left: INDENT },
        border: { left: { style: BorderStyle.SINGLE, size: 12, color: "9CC3E0", space: 8 } },
        children: parseInline(buf.join(" "), { italics: true }),
      }));
      continue;
    }

    // lista con vinetas (une lineas de continuacion del mismo item)
    let lm = trimmed.match(/^[-*]\s+(.*)$/);
    if (lm) {
      const buf = [lm[1]];
      i++;
      while (i < lines.length && !startsBlock(lines[i].trim())) { buf.push(lines[i].trim()); i++; }
      out.push(new Paragraph({
        numbering: { reference: "bullets", level: 0 },
        spacing: { after: 40, line: LINE },
        children: parseInline(buf.join(" ")),
      }));
      continue;
    }
    // lista numerada (une lineas de continuacion del mismo item)
    lm = trimmed.match(/^\d+\.\s+(.*)$/);
    if (lm) {
      const buf = [lm[1]];
      i++;
      while (i < lines.length && !startsBlock(lines[i].trim())) { buf.push(lines[i].trim()); i++; }
      out.push(new Paragraph({
        numbering: { reference: "nums", level: 0 },
        spacing: { after: 40, line: LINE },
        children: parseInline(buf.join(" ")),
      }));
      continue;
    }

    // parrafo normal: une las lineas envueltas del mismo parrafo en uno solo
    {
      const buf = [trimmed];
      i++;
      while (i < lines.length && !startsBlock(lines[i].trim())) { buf.push(lines[i].trim()); i++; }
      out.push(bodyPara(buf.join(" "), { hangingRefs }));
    }
    continue;
  }
  return out;
}

// ---------- portada ----------
function centered(text, opts = {}) {
  return new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 120, line: LINE }, children: [new TextRun({ text, ...opts })] });
}
function rightLine(label, value) {
  return new Paragraph({
    alignment: AlignmentType.RIGHT, spacing: { after: 40, line: LINE },
    children: [new TextRun({ text: label + "  ", bold: true, size: 24 }), new TextRun({ text: value, size: 24 })],
  });
}
function gap(after) { return new Paragraph({ spacing: { after }, children: [] }); }

function portada() {
  return [
    centered(CFG.university, { bold: true, size: 30 }),
    centered(CFG.faculty, { bold: true, size: 24 }),
    centered(CFG.school, { bold: true, size: 22 }),
    gap(240),
    centered("[ Escudo institucional UNSCH ]", { size: 20, color: "AAAAAA", italics: true }),
    gap(240),
    centered(CFG.kind, { bold: true, size: 26 }),
    gap(80),
    centered("“" + CFG.title + "”", { bold: true, size: 26 }),
    gap(360),
    rightLine("Presentado por:", CFG.author),
    gap(60),
    rightLine("Docente:", CFG.teacher),
    gap(60),
    rightLine("Curso:", CFG.course),
    gap(400),
    centered(CFG.place, { size: 24 }),
    centered(CFG.date, { size: 24 }),
    new Paragraph({ pageBreakBefore: true, children: [] }),
  ];
}

// ---------- encabezado y pie ----------
function mainHeader() {
  const tabRight = [{ type: TabStopType.RIGHT, position: CONTENT_WIDTH }];
  return new Header({
    children: [
      new Paragraph({
        tabStops: tabRight, spacing: { after: 0, line: 240 },
        children: [
          new TextRun({ text: CFG.hdrLeft1, bold: true, size: 16 }),
          new TextRun({ text: "\t" }),
          new TextRun({ text: CFG.hdrRight1, bold: true, size: 16 }),
        ],
      }),
      new Paragraph({
        tabStops: tabRight, spacing: { after: 0, line: 240 },
        border: { bottom: { style: BorderStyle.SINGLE, size: 6, color: "999999", space: 4 } },
        children: [
          new TextRun({ text: CFG.hdrLeft2, size: 16, color: "555555" }),
          new TextRun({ text: "\t" }),
          new TextRun({ text: CFG.hdrRight2, size: 16, color: "555555" }),
        ],
      }),
    ],
  });
}
function mainFooter() {
  return new Footer({
    children: [new Paragraph({
      alignment: AlignmentType.CENTER, spacing: { after: 0, line: 240 },
      children: [new TextRun({ text: "Página ", size: 18 }), new TextRun({ children: [PageNumber.CURRENT], size: 18 })],
    })],
  });
}
const emptyHeader = new Header({ children: [new Paragraph({ children: [] })] });
const emptyFooter = new Footer({ children: [new Paragraph({ children: [] })] });

// ---------- ensamblaje ----------
const children = [];
children.push(...portada());

// Tabla de contenido
children.push(new Paragraph({ heading: HeadingLevel.HEADING_1, alignment: AlignmentType.CENTER, children: [new TextRun("Contenido")] }));
children.push(new TableOfContents("Tabla de contenido", { hyperlink: true, headingStyleRange: "1-3" }));
children.push(new Paragraph({ pageBreakBefore: true, children: [] }));

// preliminares desde Dedicatoria
children.push(...parseMarkdown("01-preliminares.md", { startMarker: "## Dedicatoria", breakOnH2: true }));

// cuerpo, cada archivo en pagina nueva
const body = [
  ["02-introduccion.md", {}], ["03-capitulo-1.md", {}], ["04-capitulo-2.md", {}],
  ["05-capitulo-3.md", {}], ["06-capitulo-4.md", {}], ["07-capitulo-5.md", {}],
  ["08-referencias.md", { hangingRefs: true }], ["09-anexos.md", {}],
  ["00-matriz-de-consistencia.md", {}], ["00b-validacion-instrumentos.md", {}],
];
for (const [f, opts] of body) {
  children.push(new Paragraph({ pageBreakBefore: true, children: [] }));
  children.push(...parseMarkdown(f, opts));
}

const doc = new Document({
  styles: {
    default: { document: { run: { font: "Times New Roman", size: 24 } } },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 28, bold: true, font: "Times New Roman", color: "000000" },
        paragraph: { alignment: AlignmentType.CENTER, spacing: { before: 240, after: 120, line: LINE }, outlineLevel: 0 } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 26, bold: true, font: "Times New Roman", color: "000000" },
        paragraph: { spacing: { before: 200, after: 100, line: LINE }, outlineLevel: 1 } },
      { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 24, bold: true, italics: true, font: "Times New Roman", color: "000000" },
        paragraph: { spacing: { before: 160, after: 80, line: LINE }, outlineLevel: 2 } },
      { id: "Heading4", name: "Heading 4", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 24, bold: true, font: "Times New Roman", color: "000000" },
        paragraph: { spacing: { before: 120, after: 80, line: LINE }, outlineLevel: 3 } },
    ],
  },
  numbering: {
    config: [
      { reference: "bullets", levels: [{ level: 0, format: LevelFormat.BULLET, text: "•", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
      { reference: "nums", levels: [{ level: 0, format: LevelFormat.DECIMAL, text: "%1.", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 720, hanging: 360 } } } }] },
    ],
  },
  sections: [{
    properties: {
      titlePage: true,   // portada sin encabezado/pie
      page: { size: { width: 11906, height: 16838 }, margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 } },
    },
    headers: { default: mainHeader(), first: emptyHeader },
    footers: { default: mainFooter(), first: emptyFooter },
    children,
  }],
});

Packer.toBuffer(doc).then(buf => {
  const target = path.join(DIR, "SITRAM-Informe-02.docx");
  try {
    fs.writeFileSync(target, buf);
    console.log("OK -> SITRAM-Informe-02.docx (" + buf.length + " bytes)");
  } catch (e) {
    if (e.code === "EBUSY" || e.code === "EPERM") {
      const alt = path.join(DIR, "SITRAM-Informe-02.NUEVO.docx");
      fs.writeFileSync(alt, buf);
      console.log("AVISO: el .docx estaba abierto/bloqueado. Escrito en SITRAM-Informe-02.NUEVO.docx (" + buf.length + " bytes)");
    } else throw e;
  }
});
