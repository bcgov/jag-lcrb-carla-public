/** Access gate, one block per row of the behaviour table. ACCESS_CODE is a dummy test value. */
import { mkdtempSync, rmSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import path from 'node:path';
import { afterAll, afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computeGateToken } from '@server-lib/gate';
import { useServer } from './support/server';

const CODE = 'unit-test-gate-code';
const staticDir = mkdtempSync(path.join(tmpdir(), 'pfa-gate-'));
writeFileSync(path.join(staticDir, 'index.html'), '<!doctype html><app-root></app-root>');
writeFileSync(path.join(staticDir, 'main.js'), 'console.log("main")');
afterAll(() => rmSync(staticDir, { recursive: true, force: true }));

const srv = useServer({ staticDir });
const get = (p: string, cookie?: string) =>
  fetch(`${srv.base()}${p}`, { redirect: 'manual', headers: cookie ? { cookie } : {} });
const submit = (fields: Record<string, string>) =>
  fetch(`${srv.base()}/gate`, {
    method: 'POST',
    redirect: 'manual',
    headers: { 'content-type': 'application/x-www-form-urlencoded' },
    body: new URLSearchParams(fields).toString(),
  });

describe('gate disabled (ACCESS_CODE unset)', () => {
  beforeEach(() => vi.stubEnv('ACCESS_CODE', ''));
  afterEach(() => vi.unstubAllEnvs());

  it('passes every request through', async () => {
    expect((await get('/dashboard')).status).toBe(200);
    expect((await get('/api/entity-directory?q=zz')).status).toBe(200);
  });

  it('POST /gate just continues to next', async () => {
    const res = await submit({ next: '/dashboard', code: 'x' });
    expect(res.status).toBe(303);
    expect(res.headers.get('location')).toBe('/dashboard');
    expect(res.headers.get('set-cookie')).toBeNull();
  });
});

describe('gate enabled', () => {
  let token: string;
  beforeEach(async () => {
    vi.stubEnv('ACCESS_CODE', CODE);
    token = await computeGateToken(CODE);
  });
  afterEach(() => vi.unstubAllEnvs());

  it('allows /gate and its sub-paths without a cookie', async () => {
    expect((await get('/gate')).status).toBe(200);
    expect((await get('/gate?error=1')).status).toBe(200);
    expect((await get('/gate/anything')).status).toBe(200);
  });

  it('skips static assets by the proxy.ts matcher', async () => {
    expect((await get('/main.js')).status).toBe(200);
    for (const p of ['/bc-logo.png', '/x.woff2', '/x.css', '/favicon.ico', '/robots.txt', '/_next/static/x']) {
      expect((await get(p)).status, p).not.toBe(307);
    }
  });

  it('answers /api/* without a valid cookie with 401 JSON', async () => {
    for (const cookie of [undefined, 'lcrb_access=wrong', `lcrb_access=${token}x`]) {
      const res = await get('/api/entity-directory', cookie);
      expect(res.status).toBe(401);
      expect(await res.json()).toEqual({ error: 'Access code required.' });
    }
  });

  it('redirects other paths to /gate with next', async () => {
    const res = await get('/police/queue?tab=2');
    expect(res.status).toBe(307);
    const loc = new URL(res.headers.get('location') ?? '');
    expect(loc.pathname).toBe('/gate');
    expect(loc.searchParams.get('next')).toBe('/police/queue?tab=2');
  });

  it('redirects / to /gate without next', async () => {
    const res = await get('/');
    expect(res.status).toBe(307);
    expect(new URL(res.headers.get('location') ?? '').search).toBe('');
  });

  it('lets a valid cookie through', async () => {
    expect((await get('/dashboard', `other=1; lcrb_access=${token}`)).status).toBe(200);
    expect((await get('/api/entity-directory', `lcrb_access=${token}`)).status).toBe(200);
  });

  it('POST /gate with a wrong code waits 600 ms and re-prompts', async () => {
    const start = Date.now();
    const res = await submit({ next: '/licences', code: 'nope' });
    expect(Date.now() - start).toBeGreaterThanOrEqual(590);
    expect(res.status).toBe(303);
    expect(res.headers.get('location')).toBe('/gate?error=1&next=%2Flicences');
    expect(res.headers.get('set-cookie')).toBeNull();
  });

  it('POST /gate with a wrong code drops next when it is /login', async () => {
    const res = await submit({ next: '/login', code: 'nope' });
    expect(res.headers.get('location')).toBe('/gate?error=1');
  });

  it('POST /gate with the right code sets the 5 h HttpOnly cookie and redirects', async () => {
    const res = await submit({ next: '/licences', code: CODE });
    expect(res.status).toBe(303);
    expect(res.headers.get('location')).toBe('/licences');
    const cookie = res.headers.get('set-cookie') ?? '';
    expect(cookie).toMatch(new RegExp(`^lcrb_access=${token}; Path=/; Expires=[^;]+; Max-Age=18000; HttpOnly; SameSite=lax$`));
  });

  it('marks the cookie Secure in production', async () => {
    vi.stubEnv('NODE_ENV', 'production');
    const res = await submit({ next: '/', code: CODE });
    expect(res.headers.get('set-cookie')).toMatch(/; Secure; HttpOnly;/);
  });

  it('only redirects to a safe same-origin next', async () => {
    for (const next of ['//evil.example', 'https://evil.example', 'evil', '']) {
      const res = await submit({ next, code: CODE });
      expect(res.headers.get('location'), next).toBe('/login');
    }
  });
});
