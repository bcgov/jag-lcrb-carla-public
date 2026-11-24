/** The maxDuration request timeout (server/routes/index.ts). */
import type { AddressInfo } from 'node:net';
import express from 'express';
import { describe, expect, it } from 'vitest';
import { timeout } from '../../server/routes';

describe('maxDuration timeout', () => {
  async function serve(handler: express.RequestHandler) {
    const app = express();
    app.get('/slow', timeout(0.05), handler);
    const server = app.listen(0);
    await new Promise((r) => server.once('listening', r));
    return { server, url: `http://127.0.0.1:${(server.address() as AddressInfo).port}/slow` };
  }

  it('answers 504 when no response started in time', async () => {
    const { server, url } = await serve(() => undefined);
    const res = await fetch(url);
    server.close();
    expect(res.status).toBe(504);
    expect(await res.text()).toBe('Function timed out');
  });

  it('ends a started stream at the limit', async () => {
    const { server, url } = await serve((_req, res) => {
      res.write('partial');
    });
    const res = await fetch(url);
    expect(res.status).toBe(200);
    expect(await res.text()).toBe('partial');
    server.close();
  });
});
