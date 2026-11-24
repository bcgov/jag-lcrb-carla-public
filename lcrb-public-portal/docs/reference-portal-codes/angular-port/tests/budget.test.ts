/** The initial bundle must stay within 5% of tools/budget-baseline.json. Run after `pnpm build`. */
import { readFileSync } from 'node:fs';
import path from 'node:path';
import { describe, expect, it } from 'vitest';
import { initialFiles, measureInitialBundle, withinBudget } from '../tools/check-budget';

const root = path.resolve(import.meta.dirname, '..');
const browserDir = path.join(root, 'dist/portal-frontend-angular-v2/browser');
const baseline = JSON.parse(readFileSync(path.join(root, 'tools/budget-baseline.json'), 'utf8')) as {
  initialBytes: number;
  tolerance: number;
};

describe('initial bundle budget', () => {
  it('reads scripts, module preloads and stylesheets from index.html, once each', () => {
    const html =
      '<link rel="icon" href="/icon.svg"><link rel="stylesheet" href="s.css" media="print">' +
      '<link rel="stylesheet" href="s.css"><link rel="modulepreload" href="c.js"><script src="main.js" type="module">';
    expect(initialFiles(html)).toEqual(['c.js', 'main.js', 's.css']);
  });

  it('applies the tolerance', () => {
    expect(withinBudget(105, 100, 0.05)).toBe(true);
    expect(withinBudget(106, 100, 0.05)).toBe(false);
  });

  it('stays within the baseline plus tolerance', () => {
    const { bytes } = measureInitialBundle(browserDir);
    expect(bytes, `initial bundle ${bytes} bytes, baseline ${baseline.initialBytes}`).toBeGreaterThan(0);
    expect(withinBudget(bytes, baseline.initialBytes, baseline.tolerance)).toBe(true);
  });
});
