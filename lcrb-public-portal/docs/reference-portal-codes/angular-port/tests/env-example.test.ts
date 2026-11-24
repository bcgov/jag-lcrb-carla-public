/** Every environment variable read by server code must be documented in .env.example. */
import { existsSync, readdirSync, readFileSync, statSync } from 'node:fs';
import path from 'node:path';
import { describe, expect, it } from 'vitest';

const root = path.resolve(import.meta.dirname, '..');
// The code the server imports from src/ lives in domain and data-access.
const SCANNED = ['server', 'tests', 'e2e', 'src/app/domain', 'src/app/data-access', 'playwright.config.ts', 'vitest.server.config.ts'];

function tsFiles(p: string): string[] {
  const full = path.join(root, p);
  if (!existsSync(full)) return [];
  if (statSync(full).isFile()) return [full];
  return readdirSync(full, { recursive: true, encoding: 'utf8' })
    .filter((f) => f.endsWith('.ts'))
    .map((f) => path.join(full, f));
}

export function envNamesRead(source: string): string[] {
  const names = new Set<string>();
  for (const m of source.matchAll(/process\.env(?:\.([A-Z][A-Z0-9_]*)|\[['"]([A-Z][A-Z0-9_]*)['"]\])/g)) {
    names.add(m[1] ?? m[2]);
  }
  return [...names];
}

describe('.env.example', () => {
  const documented = new Set(
    readFileSync(path.join(root, '.env.example'), 'utf8')
      .split('\n')
      .map((l) => l.match(/^([A-Z][A-Z0-9_]*)=/)?.[1])
      .filter((n): n is string => Boolean(n)),
  );

  it('finds variable reads in both access styles', () => {
    expect(envNamesRead("process.env.FOO + process.env['BAR']")).toEqual(['FOO', 'BAR']);
  });

  it('documents every variable read by server-side code', () => {
    const read = new Set(SCANNED.flatMap(tsFiles).flatMap((f) => envNamesRead(readFileSync(f, 'utf8'))));
    // This file's own fixture strings are not real reads.
    read.delete('FOO');
    read.delete('BAR');
    const missing = [...read].filter((n) => !documented.has(n));
    expect(missing).toEqual([]);
  });
});
