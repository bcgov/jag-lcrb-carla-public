/** Initial bundle budget: sums the files index.html loads up front (module scripts, preloads, stylesheets) against a baseline; the angular.json budgets still apply during the build. */
import { readFileSync, statSync } from 'node:fs';
import path from 'node:path';

export interface InitialBundle {
  files: string[];
  bytes: number;
}

/** Files referenced from index.html that load before the first render. */
export function initialFiles(indexHtml: string): string[] {
  const files = new Set<string>();
  for (const m of indexHtml.matchAll(/<script\b[^>]*\bsrc="([^"]+)"/g)) files.add(m[1]);
  for (const m of indexHtml.matchAll(/<link\b[^>]*\brel="(?:modulepreload|stylesheet)"[^>]*\bhref="([^"]+)"/g)) {
    files.add(m[1]);
  }
  return [...files].filter((f) => !/^(https?:)?\/\//.test(f)).sort();
}

export function measureInitialBundle(browserDir: string): InitialBundle {
  const files = initialFiles(readFileSync(path.join(browserDir, 'index.html'), 'utf8'));
  const bytes = files.reduce((sum, f) => sum + statSync(path.join(browserDir, f.replace(/^\//, ''))).size, 0);
  return { files, bytes };
}

/** True when `bytes` is within `tolerance` (a fraction, 0.05 = 5%) above the baseline. */
export function withinBudget(bytes: number, baselineBytes: number, tolerance: number): boolean {
  return bytes <= baselineBytes * (1 + tolerance);
}
