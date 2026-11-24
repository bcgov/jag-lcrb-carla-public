/** Starts the Express app on a free port for a server test file. */
import type { Server } from 'node:http';
import type { AddressInfo } from 'node:net';
import { afterAll, beforeAll } from 'vitest';
import { type AppOptions, createApp } from '../../../server/app';

/** Registers beforeAll/afterAll hooks; `base()` is the server URL once started. */
export function useServer(options: AppOptions = { staticDir: '/nonexistent' }) {
  let server: Server;
  let url = '';
  beforeAll(async () => {
    server = createApp(options).listen(0);
    await new Promise((r) => server.once('listening', r));
    url = `http://127.0.0.1:${(server.address() as AddressInfo).port}`;
  });
  afterAll(() => {
    server.close();
  });
  return {
    base: () => url,
    /** POST a JSON body. */
    post: (path: string, body: unknown, init: RequestInit = {}) =>
      fetch(`${url}${path}`, {
        method: 'POST',
        headers: { 'content-type': 'application/json', ...(init.headers ?? {}) },
        body: typeof body === 'string' ? body : JSON.stringify(body),
        ...init,
      }),
  };
}
