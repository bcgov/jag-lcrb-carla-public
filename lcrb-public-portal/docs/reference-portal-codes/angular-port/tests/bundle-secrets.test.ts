/** No secret may reach the browser bundle. Run after a production build (`pnpm build`). */
import { existsSync, readdirSync, readFileSync } from 'node:fs';
import path from 'node:path';
import { describe, expect, it } from 'vitest';
import { findSecrets, isSecretName } from './support/secret-scan';

const root = path.resolve(import.meta.dirname, '..');
const browserDir = path.join(root, 'dist/portal-frontend-angular-v2/browser');

/** Server-only files: everything under server/lib/, outside the browser build. */
const serverOnly = readdirSync(path.join(root, 'server/lib'), { recursive: true, encoding: 'utf8' })
  .filter((f) => f.endsWith('.ts'))
  .map((f) => path.join('server/lib', f));

/** Long string literals of a server-only file that no browser source contains: markers for that file. */
function markersOf(file: string, browserSources: string[]): string[] {
  const text = readFileSync(path.join(root, file), 'utf8');
  const literals = [...text.matchAll(/(["'`])((?:(?!\1)[^\\\n$]){10,})\1/g)].map((m) => m[2]);
  return [...new Set(literals)].filter((l) => !browserSources.some((src) => src.includes(l))).slice(0, 5);
}

const secretNames = readFileSync(path.join(root, '.env.example'), 'utf8')
  .split('\n')
  .map((l) => l.match(/^([A-Z][A-Z0-9_]*)=/)?.[1])
  .filter((n): n is string => Boolean(n))
  .filter(isSecretName);

describe('secret scanner', () => {
  it('treats keys, tokens, the access code and private API URLs as secret, not plain config', () => {
    expect(secretNames).toEqual(
      expect.arrayContaining(['ACCESS_CODE', 'AI_GATEWAY_API_KEY', 'BLOB_READ_WRITE_TOKEN', 'BC_REGISTRIES_API_URL']),
    );
    expect(['PORT', 'NODE_ENV', 'ANGULAR_URL', 'ANGULAR_PORT'].some(isSecretName)).toBe(false);
  });

  it('catches a planted dummy key, variable name and environment value', () => {
    const planted = [
      'const k="vck_' + 'A1b2C3d4E5f6G7h8I9j0K1l2"',
      'read(process.env.AI_GATEWAY_API_KEY)',
      'fetch(url,{headers:{x:"planted-dummy-secret-value"}})',
    ].join(';');
    const hits = findSecrets({ text: planted, names: secretNames, values: ['planted-dummy-secret-value'] });
    expect(hits).toContain('variable name AI_GATEWAY_API_KEY');
    expect(hits).toContain('a secret value from the environment');
    expect(hits.some((h) => h.startsWith('key pattern'))).toBe(true);
  });

  it('does not flag ordinary code', () => {
    expect(findSecrets({ text: 'export const token = signal(""); const PORTAL = 1;', names: secretNames, values: [] })).toEqual([]);
  });
});

describe('browser bundle', () => {
  it('exists (run `pnpm build` first)', () => {
    expect(existsSync(browserDir)).toBe(true);
  });

  it('contains no secret names, values or key patterns', () => {
    const values = secretNames.map((n) => process.env[n] ?? '').filter(Boolean);
    const files = readdirSync(browserDir, { recursive: true, encoding: 'utf8' }).filter((f) =>
      /\.(js|css|html|json|txt|map)$/.test(f),
    );
    expect(files.length).toBeGreaterThan(0);
    const hits = files.flatMap((f) =>
      findSecrets({ text: readFileSync(path.join(browserDir, f), 'utf8'), names: secretNames, values }).map(
        (h) => `${f}: ${h}`,
      ),
    );
    expect(hits).toEqual([]);
  });

  it('contains none of the server-only lib files', () => {
    expect(serverOnly.length).toBeGreaterThanOrEqual(9);
    const browserSources = readdirSync(path.join(root, 'src'), { recursive: true, encoding: 'utf8' })
      .map((f) => path.join('src', f))
      .filter((f) => f.endsWith('.ts') && !f.endsWith('.spec.ts') && !serverOnly.includes(f))
      .map((f) => readFileSync(path.join(root, f), 'utf8'));
    const bundle = readdirSync(browserDir, { recursive: true, encoding: 'utf8' })
      .filter((f) => f.endsWith('.js'))
      .map((f) => readFileSync(path.join(browserDir, f), 'utf8'))
      .join('\n');
    for (const file of serverOnly) {
      const markers = markersOf(file, browserSources);
      expect(markers.length, `${file} has no unique markers`).toBeGreaterThan(0);
      expect(markers.filter((m) => bundle.includes(m)), file).toEqual([]);
    }
  });
});
