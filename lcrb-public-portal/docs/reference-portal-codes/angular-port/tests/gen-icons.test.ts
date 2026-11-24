/** tools/gen-icons.mjs runs from this project's own lucide package. */
import { execFileSync } from 'node:child_process';
import { readFileSync, writeFileSync } from 'node:fs';
import path from 'node:path';
import { afterEach, describe, expect, it } from 'vitest';

const root = path.resolve(import.meta.dirname, '..');
const registry = path.join(root, 'src/app/shared/ui/primitives/icons.ts');
const original = readFileSync(registry, 'utf8');
const run = (...names: string[]) =>
  execFileSync(process.execPath, ['tools/gen-icons.mjs', ...names], { cwd: root, stdio: 'pipe' });

describe('gen-icons', () => {
  afterEach(() => writeFileSync(registry, original));

  it('regenerates the current registry unchanged', () => {
    run();
    expect(readFileSync(registry, 'utf8')).toBe(original);
  });

  it('adds an icon by name, alias or Icon/Lucide form', () => {
    run('Rocket', 'AlertCircle', 'AnchorIcon', 'LucideBike');
    const out = readFileSync(registry, 'utf8');
    expect(out).toContain("  Rocket: ['rocket', Rocket],");
    expect(out).toContain("  AlertCircle: ['circle-alert', CircleAlert],");
    expect(out).toContain("  AnchorIcon: ['anchor', Anchor],");
    expect(out).toContain("  LucideBike: ['bike', Bike],");
  });

  it('rejects an unknown name without writing', () => {
    expect(() => run('NotAnIcon')).toThrow(/Unknown lucide icon: NotAnIcon/);
    expect(readFileSync(registry, 'utf8')).toBe(original);
  });
});
