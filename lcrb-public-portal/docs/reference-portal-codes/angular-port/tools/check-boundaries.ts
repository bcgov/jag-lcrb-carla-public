/** Layer boundary check: imports breaking rules R1-R7 or forming a cycle are reported unless in tools/boundaries-allowlist.json (the list can only shrink). */
import { existsSync, readdirSync, readFileSync, statSync } from 'node:fs';
import path from 'node:path';
import ts from 'typescript';
import { ROUTES } from '../src/app/route-registry';

type Layer =
  | 'domain'
  | 'data-access'
  | 'seed'
  | 'core'
  | 'shared:ui'
  | 'shared:forms'
  | 'shared:util'
  | `feature:${string}`
  | 'app'
  | 'server'
  | 'other';

/** Import specifier prefix to folder (relative to the project root), most specific first. */
const ALIASES: [string, string][] = [
  ['@/components/', 'src/app/components/'],
  ['@/hooks/', 'src/app/hooks/'],
  ['@core/', 'src/app/core/'],
  ['@domain/', 'src/app/domain/'],
  ['@data-access/', 'src/app/data-access/'],
  ['@shared/', 'src/app/shared/'],
  ['@features/', 'src/app/features/'],
  ['@server-lib/', 'server/lib/'],
  ['@/', 'src/'],
];

const ROOT_WIRING = new Set(['src/app/app.routes.ts', 'src/app/route-registry.ts', 'src/app/app.config.ts', 'src/app/app.ts']);

export function layerOf(v2Path: string): Layer {
  if (v2Path.startsWith('server/')) return 'server';
  if (ROOT_WIRING.has(v2Path)) return 'app';
  const m = v2Path.match(/^src\/app\/(domain|data-access|core|shared|features)\/([^/]*)/);
  if (!m) return v2Path.startsWith('src/app/') ? 'app' : 'other';
  const [, top, sub] = m;
  if (top === 'features') return `feature:${sub}`;
  if (top === 'data-access') return /\/mock\//.test(v2Path) ? 'seed' : 'data-access';
  if (top === 'shared') return sub === 'ui' ? 'shared:ui' : sub === 'forms' ? 'shared:forms' : 'shared:util';
  return top as Layer;
}

/** The rule an import from `from` to `to` breaks, or null. */
export function ruleFor(from: string, to: string): string | null {
  const a = layerOf(from);
  const b = layerOf(to);
  if (a === 'domain' && b !== 'domain') return 'R1';
  if ((a === 'data-access' || a === 'seed') && !['domain', 'data-access', 'seed', 'shared:util'].includes(b)) return 'R2';
  if (b.startsWith('feature:') && !a.startsWith('feature:') && a !== 'app') return 'R3';
  if (b.startsWith('feature:') && a.startsWith('feature:') && a !== b) return 'R4';
  if (b === 'seed' && a !== 'data-access' && a !== 'seed' && a !== 'app') return 'R5';
  return null;
}

function listSources(root: string): string[] {
  const dir = path.join(root, 'src');
  return readdirSync(dir, { recursive: true, encoding: 'utf8' })
    .map((f) => `src/${f.split(path.sep).join('/')}`)
    .filter((f) => f.endsWith('.ts') && !f.endsWith('.spec.ts') && !f.endsWith('.d.ts'));
}

function resolve(root: string, from: string, spec: string): string | null {
  let base: string | null = null;
  if (spec.startsWith('.')) base = path.posix.normalize(path.posix.join(path.posix.dirname(from), spec));
  else {
    const alias = ALIASES.find(([prefix]) => spec.startsWith(prefix));
    if (alias) base = alias[1] + spec.slice(alias[0].length);
  }
  if (!base) return null;
  for (const candidate of [`${base}.ts`, `${base}/index.ts`, base]) {
    const full = path.join(root, candidate);
    if (existsSync(full) && statSync(full).isFile()) return candidate;
  }
  return null;
}

export function importsOf(source: string): string[] {
  const specs = new Set<string>();
  for (const m of source.matchAll(/(?:\bfrom|\bimport\s*\(|^\s*import)\s*['"]([^'"]+)['"]/gm)) specs.add(m[1]);
  return [...specs];
}

/** True when code (comments and string literals removed) uses a browser global. */
export function usesBrowserApi(source: string): boolean {
  const code = source
    .replace(/\/\*[\s\S]*?\*\//g, '')
    .replace(/(^|[^:])\/\/.*$/gm, '$1')
    .replace(/(["'`])(?:\\.|(?!\1)[^\\])*\1/g, '""');
  return /\b(?:window|document|localStorage|sessionStorage)\s*\./.test(code);
}

/** Specifiers used only for types (`import type`, `export type`, or `import { type A, type B }`); erased at compile time. */
export function typeOnlyImportsOf(source: string): Set<string> {
  const sf = ts.createSourceFile('x.ts', source, ts.ScriptTarget.Latest, true);
  const typeOnly = new Set<string>();
  const value = new Set<string>();
  for (const st of sf.statements) {
    if ((ts.isImportDeclaration(st) || ts.isExportDeclaration(st)) && st.moduleSpecifier && ts.isStringLiteral(st.moduleSpecifier)) {
      const spec = st.moduleSpecifier.text;
      let onlyTypes = false;
      if (ts.isImportDeclaration(st)) {
        const clause = st.importClause;
        const nb = clause?.namedBindings;
        onlyTypes = !!clause && (clause.isTypeOnly || (!clause.name && !!nb && ts.isNamedImports(nb) && nb.elements.length > 0 && nb.elements.every((e) => e.isTypeOnly)));
      } else {
        const ec = st.exportClause;
        onlyTypes = st.isTypeOnly || (!!ec && ts.isNamedExports(ec) && ec.elements.length > 0 && ec.elements.every((e) => e.isTypeOnly));
      }
      (onlyTypes ? typeOnly : value).add(spec);
    }
  }
  for (const m of source.matchAll(/\bimport\s*\(\s*['"]([^'"]+)['"]/g)) value.add(m[1]);
  for (const spec of value) typeOnly.delete(spec);
  return typeOnly;
}

/** Strongly connected components with more than one file (Tarjan). */
function cycles(graph: Map<string, string[]>): string[][] {
  let index = 0;
  const idx = new Map<string, number>();
  const low = new Map<string, number>();
  const stack: string[] = [];
  const onStack = new Set<string>();
  const out: string[][] = [];
  const visit = (v: string) => {
    idx.set(v, index);
    low.set(v, index++);
    stack.push(v);
    onStack.add(v);
    for (const w of graph.get(v) ?? []) {
      if (!idx.has(w)) {
        visit(w);
        low.set(v, Math.min(low.get(v)!, low.get(w)!));
      } else if (onStack.has(w)) low.set(v, Math.min(low.get(v)!, idx.get(w)!));
    }
    if (low.get(v) === idx.get(v)) {
      const scc: string[] = [];
      let w: string;
      do {
        w = stack.pop()!;
        onStack.delete(w);
        scc.push(w);
      } while (w !== v);
      if (scc.length > 1) out.push(scc.sort());
    }
  };
  for (const v of graph.keys()) if (!idx.has(v)) visit(v);
  return out;
}

const short = (p: string) => p.replace(/^src\/app\//, '');

/** Module-function stores that call data-access services directly (rule 6 exceptions). */
export const STORE_EXCEPTIONS = new Set([
  'src/app/features/legal-entity/state/legal-entity-store.ts',
  'src/app/features/legal-entity/state/tied-house-store.ts',
  'src/app/features/legal-entity/state/screening-store.ts',
  'src/app/features/public-tools/state/establishment-plan-store.ts',
  'src/app/core/navigation/link-context.ts',
]);

/** Rule 6 for one import: `to` is a non-seed data-access file that is not a repository. */
export function breaksEntryRule(from: string, to: string, valueImport: boolean): boolean {
  const a = layerOf(from);
  if (!valueImport || a === 'data-access' || a === 'seed' || a === 'app' || a === 'server' || a === 'other') return false;
  if (STORE_EXCEPTIONS.has(from)) return false;
  return layerOf(to) === 'data-access' && !to.endsWith('.repository.ts');
}

/** Registry paths as regexes; `:param` matches one segment. The home path ('') is left out. */
const ROUTE_PATTERNS = Object.values(ROUTES)
  .filter((p) => p !== '')
  .map((p) => new RegExp(`^/${p.split('/').map((seg) => (seg.startsWith(':') ? '[^/]+' : seg.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'))).join('/')}$`));

const isRoutePath = (candidate: string) => {
  const bare = candidate.split(/[?#]/)[0];
  // A literal ending in '/' is the prefix of a concatenated link ('/police/review/' + id).
  const probe = bare.endsWith('/') && bare.length > 1 ? `${bare}x` : bare;
  return probe.length > 1 && ROUTE_PATTERNS.some((re) => re.test(probe));
};

/** Literal route paths in a source file (rule 7): string and template literals, and link attributes in templates. */
export function literalRoutePaths(source: string): string[] {
  const sf = ts.createSourceFile('x.ts', source, ts.ScriptTarget.Latest, true);
  const out = new Set<string>();
  const check = (text: string) => {
    if (text.startsWith('/') && isRoutePath(text)) out.add(text);
    for (const m of text.matchAll(/(?:routerLink|href)\s*=\s*"(\/[^"]*)"|'(\/[^']*)'/g)) {
      const value = m[1] ?? m[2];
      if (isRoutePath(value)) out.add(value);
    }
  };
  const visit = (node: ts.Node) => {
    if (ts.isStringLiteral(node) || ts.isNoSubstitutionTemplateLiteral(node)) check(node.text);
    else if (ts.isTemplateExpression(node)) {
      check(node.head.text + node.templateSpans.map((sp) => `\${x}${sp.literal.text}`).join(''));
      node.templateSpans.forEach((sp) => visit(sp.expression));
      return;
    }
    ts.forEachChild(node, visit);
  };
  visit(sf);
  return [...out].sort();
}

/** All violations as sorted keys `RULE|from|to` (paths without `src/app/`); `structureMap` optionally remaps a file to the path it is judged by. */
export function checkBoundaries(root: string, structureMap: Record<string, string> = {}): string[] {
  const v2 = (p: string) => structureMap[p] ?? p;
  const found = new Set<string>();
  const graph = new Map<string, string[]>();
  for (const file of listSources(root)) {
    const source = readFileSync(path.join(root, file), 'utf8');
    const from = v2(file);
    const edges: string[] = [];
    if (layerOf(from) === 'domain' && /from\s+['"]@angular\//.test(source)) found.add(`R1|${short(from)}|@angular`);
    if (layerOf(from) === 'domain' && usesBrowserApi(source)) found.add(`R1|${short(from)}|browser-api`);
    if (!['data-access', 'seed'].includes(layerOf(from)) && /fetch\(\s*[`'"]\/api/.test(source)) {
      found.add(`R5|${short(from)}|fetch:/api`);
    }
    const typeOnly = typeOnlyImportsOf(source);
    for (const spec of importsOf(source)) {
      const target = resolve(root, file, spec);
      if (!target || !target.endsWith('.ts')) continue;
      if (!typeOnly.has(spec)) edges.push(target);
      const rule = ruleFor(from, v2(target));
      // The module-function stores keep their direct data-access and seed imports (rules 5 and 6).
      if (rule && !(rule === 'R5' && STORE_EXCEPTIONS.has(from))) found.add(`${rule}|${short(from)}|${short(v2(target))}`);
      if (breaksEntryRule(from, v2(target), !typeOnly.has(spec))) found.add(`R6|${short(from)}|${short(v2(target))}`);
    }
    if (/^(feature:|core$|shared:)/.test(layerOf(from))) {
      for (const literal of literalRoutePaths(source)) found.add(`R7|${short(from)}|${literal}`);
    }
    graph.set(file, edges);
  }
  for (const scc of cycles(graph)) found.add(`CYCLE|${scc.map((f) => short(v2(f))).sort().join(',')}|`);
  return [...found].sort();
}
