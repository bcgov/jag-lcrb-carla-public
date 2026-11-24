/** Layer boundaries, checked by tools/check-boundaries.ts; the allowlist may only shrink. */
import { mkdirSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import path from 'node:path';
import { afterAll, describe, expect, it } from 'vitest';
import { checkBoundaries, importsOf, layerOf, ruleFor, typeOnlyImportsOf, usesBrowserApi } from '../tools/check-boundaries';

const root = path.resolve(import.meta.dirname, '..');
const readJson = <T>(file: string): T => JSON.parse(readFileSync(path.join(root, file), 'utf8')) as T;
const allowlist = readJson<{ violations: string[] }>('tools/boundaries-allowlist.json').violations;

describe('boundary rules', () => {
  it('classifies layers by path', () => {
    expect(layerOf('src/app/domain/roles.ts')).toBe('domain');
    expect(layerOf('src/app/data-access/repositories/licences/mock/licences.ts')).toBe('seed');
    expect(layerOf('src/app/data-access/repositories/licences/licence-cap-engine.ts')).toBe('data-access');
    expect(layerOf('src/app/shared/ui/map/map-view.ts')).toBe('shared:ui');
    expect(layerOf('src/app/features/licences/pages/licences.page.ts')).toBe('feature:licences');
    expect(layerOf('src/app/app.routes.ts')).toBe('app');
  });

  it('applies each rule', () => {
    const seed = 'src/app/data-access/repositories/seps/mock/seps.ts';
    expect(ruleFor('src/app/domain/a.ts', 'src/app/core/session/s.ts')).toBe('R1');
    expect(ruleFor('src/app/data-access/repositories/x/x.ts', 'src/app/shared/forms/f.ts')).toBe('R2');
    expect(ruleFor('src/app/data-access/repositories/x/x.ts', 'src/app/shared/util/u.ts')).toBeNull();
    expect(ruleFor('src/app/shared/ui/map/m.ts', 'src/app/features/staff/pages/p.ts')).toBe('R3');
    expect(ruleFor('src/app/features/police/pages/p.ts', 'src/app/features/staff/pages/p.ts')).toBe('R4');
    expect(ruleFor('src/app/features/police/pages/p.ts', seed)).toBe('R5');
    expect(ruleFor('src/app/data-access/repositories/seps/seps.repository.mock.ts', seed)).toBeNull();
    expect(ruleFor('src/app/app.routes.ts', 'src/app/features/staff/staff.routes.ts')).toBeNull();
  });

  it('reads static, dynamic, side-effect and re-export imports', () => {
    const src = "import { a } from './a';\nimport type { B } from \"@domain/b\";\nimport './c';\nexport { d } from '../d';\nconst e = () => import('./e');";
    expect(importsOf(src).sort()).toEqual(['../d', './a', './c', './e', '@domain/b']);
  });

  it('knows which imports are type-only (they do not count for cycles)', () => {
    const src = 'import type { A } from "./a"\nimport { b } from "./b"\nimport type { B } from "./b"\nexport type { C } from "./c"\nimport { type D, d } from "./d"\nimport { type E } from "./e"';
    expect([...typeOnlyImportsOf(src)].sort()).toEqual(['./a', './c', './e']);
  });

  it('finds browser globals in code, not in comments or strings', () => {
    expect(usesBrowserApi('const a = document.createElement("a")')).toBe(true);
    expect(usesBrowserApi('window.sessionStorage.setItem(k, v)')).toBe(true);
    expect(usesBrowserApi('/** a short window. */\n// see document.body\nconst s = "upload the document. then window.x"')).toBe(false);
    expect(usesBrowserApi('const url = "https://x.test/a" // ok\nconst n = 1')).toBe(false);
  });

  describe('on a planted project', () => {
    const dir = mkdtempSync(path.join(tmpdir(), 'boundaries-'));
    afterAll(() => rmSync(dir, { recursive: true, force: true }));
    const write = (file: string, text: string) => {
      mkdirSync(path.dirname(path.join(dir, file)), { recursive: true });
      writeFileSync(path.join(dir, file), text);
    };
    write('src/app/features/police/pages/p.page.ts', "import { x } from '@features/staff/components/x';\nexport const p = x;");
    write('src/app/features/staff/components/x.ts', "export const x = 1;");
    write('src/app/domain/a.ts', "import { b } from './b';\nexport const a = b;");
    write('src/app/domain/b.ts', "import { a } from './a';\nexport const b = a;");
    write('src/app/shared/ui/w.ts', "export const w = () => fetch('/api/foo');");
    write('src/app/domain/c.ts', "export const c = () => localStorage.getItem('k');");
    write('src/lib/old.ts', "import { x } from '@/app/features/staff/components/x';\nexport const o = x;");

    it('reports a cross-feature import, a feature import from outside features, a cycle, a browser global in domain and a fetch outside data-access', () => {
      expect(checkBoundaries(dir)).toEqual([
        'CYCLE|domain/a.ts,domain/b.ts|',
        'R1|domain/c.ts|browser-api',
        'R3|src/lib/old.ts|features/staff/components/x.ts',
        'R4|features/police/pages/p.page.ts|features/staff/components/x.ts',
        'R5|shared/ui/w.ts|fetch:/api',
      ]);
    });

    it('judges an unmoved file by its layer path', () => {
      const found = checkBoundaries(dir, { 'src/lib/old.ts': 'src/app/domain/old.ts' });
      expect(found).toContain('R1|domain/old.ts|features/staff/components/x.ts');
    });
  });
});

describe('the project', () => {
  const found = checkBoundaries(root);

  it('has no violation outside the allowlist', () => {
    expect(found.filter((v) => !allowlist.includes(v))).toEqual([]);
  });

  it('has no stale allowlist entry (remove fixed violations from tools/boundaries-allowlist.json)', () => {
    expect(allowlist.filter((v) => !found.includes(v))).toEqual([]);
  });
});
