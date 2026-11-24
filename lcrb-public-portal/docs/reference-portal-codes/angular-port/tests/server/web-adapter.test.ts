import express from 'express';
import type { Server } from 'node:http';
import type { AddressInfo } from 'node:net';
import { afterAll, beforeAll, describe, expect, it } from 'vitest';
import { adapt } from '../../server/web-adapter';

describe('web adapter', () => {
  let server: Server;
  let base: string;

  beforeAll(async () => {
    const app = express();
    // Echo: JSON body, method, query string and a header, like a Next.js POST handler.
    app.post(
      '/echo',
      adapt(async (req) => {
        const body = await req.json();
        const url = new URL(req.url);
        return Response.json({ method: req.method, q: url.searchParams.get('q'), body, h: req.headers.get('x-test') });
      }),
    );
    // Streamed body in chunks, like an AI SDK UI message stream.
    app.get(
      '/stream',
      adapt(() => {
        const enc = new TextEncoder();
        const stream = new ReadableStream({
          start(c) {
            ['data: one\n\n', 'data: two\n\n'].forEach((s) => c.enqueue(enc.encode(s)));
            c.close();
          },
        });
        return new Response(stream, { headers: { 'content-type': 'text/event-stream' } });
      }),
    );
    // Status, multiple Set-Cookie headers and an empty body.
    app.get(
      '/cookies',
      adapt(() => {
        const headers = new Headers();
        headers.append('set-cookie', 'a=1; Path=/');
        headers.append('set-cookie', 'b=2; Path=/');
        return new Response(null, { status: 204, headers });
      }),
    );
    server = app.listen(0);
    await new Promise((r) => server.once('listening', r));
    base = `http://127.0.0.1:${(server.address() as AddressInfo).port}`;
  });

  afterAll(() => server.close());

  it('passes method, query, headers and JSON body to the handler', async () => {
    const res = await fetch(`${base}/echo?q=find`, {
      method: 'POST',
      headers: { 'content-type': 'application/json', 'x-test': 'yes' },
      body: JSON.stringify({ hello: 'world' }),
    });
    expect(res.status).toBe(200);
    expect(await res.json()).toEqual({ method: 'POST', q: 'find', body: { hello: 'world' }, h: 'yes' });
  });

  it('pipes streamed bodies through unchanged', async () => {
    const res = await fetch(`${base}/stream`);
    expect(res.headers.get('content-type')).toBe('text/event-stream');
    expect(await res.text()).toBe('data: one\n\ndata: two\n\n');
  });

  it('keeps the status and every Set-Cookie header', async () => {
    const res = await fetch(`${base}/cookies`);
    expect(res.status).toBe(204);
    expect(res.headers.getSetCookie()).toEqual(['a=1; Path=/', 'b=2; Path=/']);
  });
});
