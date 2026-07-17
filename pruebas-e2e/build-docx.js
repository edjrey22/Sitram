// Genera INFORME-PRUEBAS-E2E.docx a partir de INFORME-PRUEBAS-E2E.md (markdown ligero:
// encabezados, parrafos, tablas, imagenes, negrita/cursiva/codigo). Formato simple, sin el
// aparato APA del informe academico principal.
const fs = require("fs");
const path = require("path");
const {
  Document, Packer, Paragraph, TextRun, Table, TableRow, TableCell,
  Header, Footer, AlignmentType, TabStopType, HeadingLevel, BorderStyle,
  WidthType, ShadingType, PageNumber, ImageRun,
} = require("docx");

const DIR = __dirname;
const CONTENT_WIDTH = 9026;
const LINE = 300;

function pngDims(buf) {
  return { w: buf.readUInt32BE(16), h: buf.readUInt32BE(20) };
}

function parseInline(text, baseOpts = {}) {
  const runs = [];
  let rem = text;
  const pattern = /(\*\*([^*]+)\*\*|`([^`]+)`|\*([^*]+)\*)/;
  while (rem.length) {
    const m = rem.match(pattern);
    if (!m) { runs.push(new TextRun({ text: rem, ...baseOpts })); break; }
    if (m.index > 0) runs.push(new TextRun({ text: rem.slice(0, m.index), ...baseOpts }));
    const tok = m[0];
    if (tok.startsWith("**")) runs.push(new TextRun({ text: m[2], bold: true, ...baseOpts }));
    else if (tok.startsWith("`")) runs.push(new TextRun({ text: m[3], font: "Courier New", ...baseOpts }));
    else if (tok.startsWith("*")) runs.push(new TextRun({ text: m[4], italics: true, ...baseOpts }));
    rem = rem.slice(m.index + tok.length);
  }
  return runs.length ? runs : [new TextRun({ text: "", ...baseOpts })];
}

function startsBlock(t) {
  return t === "" || t === "---" || /^#{1,4}\s+/.test(t) || t.startsWith("|") ||
    t.startsWith(">") || t.startsWith("![") || /^[-*]\s+/.test(t) || /^\d+\.\s+/.test(t);
}

function splitRow(line) {
  return line.trim().replace(/^\|/, "").replace(/\|$/, "").split("|").map(c => c.trim());
}

function buildTable(lines) {
  const rows = lines.filter(l => l.trim().startsWith("|"));
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
    borders, width: { size: widths[i], type: WidthType.DXA },
    shading: isHeader ? { fill: "D5E8F0", type: ShadingType.CLEAR, color: "auto" } : undefined,
    margins: { top: 60, bottom: 60, left: 100, right: 100 },
    children: [new Paragraph({ spacing: { after: 0, line: 240 }, children: parseInline(txt, isHeader ? { bold: true } : {}) })],
  });
  const trs = [new TableRow({ tableHeader: true, children: header.map((c, i) => mkCell(c, i, true)) })];
  for (const r of body) trs.push(new TableRow({ children: header.map((_, i) => mkCell(r[i] || "", i, false)) }));
  return new Table({ width: { size: CONTENT_WIDTH, type: WidthType.DXA }, columnWidths: widths, rows: trs });
}

function parseMarkdown(file) {
  const lines = fs.readFileSync(path.join(DIR, file), "utf8").split(/\r?\n/);
  const out = [];
  let i = 0;
  while (i < lines.length) {
    const line = lines[i];
    const trimmed = line.trim();

    if (trimmed === "---" || trimmed === "") { i++; continue; }

    const hm = trimmed.match(/^(#{1,4})\s+(.*)$/);
    if (hm) {
      const level = hm[1].length;
      out.push(new Paragraph({
        heading: [HeadingLevel.HEADING_1, HeadingLevel.HEADING_2, HeadingLevel.HEADING_3, HeadingLevel.HEADING_4][level - 1],
        pageBreakBefore: level === 2,
        spacing: { before: 200, after: 120, line: LINE },
        children: parseInline(hm[2].replace(/\*\*/g, "")),
      }));
      i++; continue;
    }

    const im = trimmed.match(/^!\[[^\]]*\]\(([^)]+)\)$/);
    if (im) {
      const imgPath = path.join(DIR, im[1]);
      if (fs.existsSync(imgPath)) {
        const data = fs.readFileSync(imgPath);
        const { w, h } = pngDims(data);
        const width = Math.min(w, 620);
        const height = Math.round(h * (width / w));
        out.push(new Paragraph({
          alignment: AlignmentType.CENTER,
          spacing: { after: 160, line: 240 },
          children: [new ImageRun({ type: "png", data, transformation: { width, height } })],
        }));
      }
      i++; continue;
    }

    if (trimmed.startsWith("|")) {
      const tbl = [];
      while (i < lines.length && lines[i].trim().startsWith("|")) { tbl.push(lines[i]); i++; }
      const t = buildTable(tbl);
      if (t) { out.push(t); out.push(new Paragraph({ spacing: { after: 160 }, children: [] })); }
      continue;
    }

    if (trimmed.startsWith(">")) {
      const buf = [];
      while (i < lines.length && lines[i].trim().startsWith(">")) { buf.push(lines[i].trim().replace(/^>\s?/, "")); i++; }
      out.push(new Paragraph({
        spacing: { after: 160, line: LINE },
        indent: { left: 720 },
        border: { left: { style: BorderStyle.SINGLE, size: 12, color: "9CC3E0", space: 8 } },
        children: parseInline(buf.join(" "), { italics: true }),
      }));
      continue;
    }

    let lm = trimmed.match(/^\d+\.\s+(.*)$/);
    if (lm) {
      out.push(new Paragraph({ spacing: { after: 60, line: LINE }, children: parseInline("• " + lm[1]) }));
      i++; continue;
    }
    lm = trimmed.match(/^[-*]\s+(.*)$/);
    if (lm) {
      out.push(new Paragraph({ spacing: { after: 60, line: LINE }, children: parseInline("• " + lm[1]) }));
      i++; continue;
    }

    const buf = [trimmed];
    i++;
    while (i < lines.length && !startsBlock(lines[i].trim())) { buf.push(lines[i].trim()); i++; }
    out.push(new Paragraph({ alignment: AlignmentType.JUSTIFIED, spacing: { after: 160, line: LINE }, children: parseInline(buf.join(" ")) }));
  }
  return out;
}

function mainHeader() {
  const tabRight = [{ type: TabStopType.RIGHT, position: CONTENT_WIDTH }];
  return new Header({
    children: [new Paragraph({
      tabStops: tabRight, spacing: { after: 0, line: 240 },
      border: { bottom: { style: BorderStyle.SINGLE, size: 6, color: "999999", space: 4 } },
      children: [
        new TextRun({ text: "SITRAM — Pruebas End-to-End", bold: true, size: 18 }),
        new TextRun({ text: "\t" }),
        new TextRun({ text: "2026-07-16", size: 18, color: "555555" }),
      ],
    })],
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

const children = parseMarkdown("INFORME-PRUEBAS-E2E.md");

const doc = new Document({
  styles: {
    default: { document: { run: { font: "Calibri", size: 22 } } },
    paragraphStyles: [
      { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 32, bold: true, font: "Calibri", color: "0B5C4A" },
        paragraph: { spacing: { before: 0, after: 160, line: LINE }, outlineLevel: 0 } },
      { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 26, bold: true, font: "Calibri", color: "0B5C4A" },
        paragraph: { spacing: { before: 240, after: 120, line: LINE }, outlineLevel: 1 } },
      { id: "Heading3", name: "Heading 3", basedOn: "Normal", next: "Normal", quickFormat: true,
        run: { size: 23, bold: true, font: "Calibri", color: "1B2430" },
        paragraph: { spacing: { before: 200, after: 100, line: LINE }, outlineLevel: 2 } },
    ],
  },
  sections: [{
    properties: { page: { size: { width: 11906, height: 16838 }, margin: { top: 1440, right: 1440, bottom: 1440, left: 1440 } } },
    headers: { default: mainHeader() },
    footers: { default: mainFooter() },
    children,
  }],
});

Packer.toBuffer(doc).then(buf => {
  const target = path.join(DIR, "INFORME-PRUEBAS-E2E.docx");
  fs.writeFileSync(target, buf);
  console.log("OK -> INFORME-PRUEBAS-E2E.docx (" + buf.length + " bytes)");
});
