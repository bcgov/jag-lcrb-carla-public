/** Checks the production build output (run after `ng build`) and that the Tailwind pipeline runs with the pinned version. */
import { existsSync, readdirSync, readFileSync } from 'node:fs';
import path from 'node:path';
import { describe, expect, it } from 'vitest';

const root = path.resolve(import.meta.dirname, '..');
const browserDir = path.join(root, 'dist/portal-frontend-angular-v2/browser');

describe('build output', () => {
  it('exists (run `pnpm build` first)', () => {
    expect(existsSync(path.join(browserDir, 'index.html'))).toBe(true);
  });

  it('contains Tailwind 4 layers and preflight in the global CSS', () => {
    const css = readdirSync(browserDir).filter((f) => /^styles-.*\.css$/.test(f));
    expect(css).toHaveLength(1);
    const text = readFileSync(path.join(browserDir, css[0]), 'utf8');
    expect(text).toContain('@layer theme,base,components,utilities');
    expect(text).toContain('box-sizing:border-box');
  });

  it('uses the Tailwind version pinned to the prototype (4.2.0)', () => {
    const pkg = JSON.parse(readFileSync(path.join(root, 'node_modules/tailwindcss/package.json'), 'utf8'));
    expect(pkg.version).toBe('4.2.0');
  });
});
