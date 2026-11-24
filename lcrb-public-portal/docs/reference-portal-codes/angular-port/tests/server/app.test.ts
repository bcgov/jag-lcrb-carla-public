import { mkdtempSync, rmSync, writeFileSync } from 'node:fs';
import type { Server } from 'node:http';
import type { AddressInfo } from 'node:net';
import { tmpdir } from 'node:os';
import path from 'node:path';
import { afterAll, beforeAll, describe, expect, it } from 'vitest';
import { createApp } from '../../server/app';

describe('Express app', () => {
  let server: Server;
  let base: string;
  let staticDir: string;

  beforeAll(async () => {
    staticDir = mkdtempSync(path.join(tmpdir(), 'pfa-static-'));
    writeFileSync(path.join(staticDir, 'index.html'), '<!doctype html><app-root></app-root>');
    writeFileSync(path.join(staticDir, 'main.js'), 'console.log("main")');
    server = createApp({ staticDir }).listen(0);
    await new Promise((r) => server.once('listening', r));
    base = `http://127.0.0.1:${(server.address() as AddressInfo).port}`;
  });

  afterAll(() => {
    server.close();
    rmSync(staticDir, { recursive: true, force: true });
  });

  it('answers the health check', async () => {
    const res = await fetch(`${base}/api/health`);
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({ status: 'ok' });
  });

  it('serves built static files', async () => {
    const res = await fetch(`${base}/main.js`);
    expect(res.status).toBe(200);
    expect(await res.text()).toContain('main');
  });

  it('falls back to index.html for app routes', async () => {
    for (const route of ['/', '/login', '/police/review/123']) {
      const res = await fetch(`${base}${route}`);
      expect(res.status).toBe(200);
      expect(await res.text()).toContain('<app-root>');
    }
  });

  it('answers missing files with 404, not the SPA', async () => {
    for (const file of ['/_vercel/insights/script.js', '/chunk-missing.js', '/nested/logo.png']) {
      const res = await fetch(`${base}${file}`);
      expect(res.status).toBe(404);
      expect(await res.text()).not.toContain('<app-root>');
    }
  });

  it('answers unknown API paths with JSON 404, not the SPA', async () => {
    const res = await fetch(`${base}/api/does-not-exist`);
    expect(res.status).toBe(404);
    expect(await res.json()).toEqual({ error: 'Not found' });
  });

  it('skips static hosting when there is no build', async () => {
    const bare = createApp({ staticDir: path.join(staticDir, 'missing') }).listen(0);
    await new Promise((r) => bare.once('listening', r));
    const port = (bare.address() as AddressInfo).port;
    const res = await fetch(`http://127.0.0.1:${port}/login`);
    expect(res.status).toBe(404);
    bare.close();
  });
});
