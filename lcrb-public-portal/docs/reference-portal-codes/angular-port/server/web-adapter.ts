/** Bridges Express to Web-standard Request/Response so the prototype's route handlers can be reused with minimal edits. */
import { Readable } from 'node:stream';
import type { ReadableStream as NodeWebReadableStream } from 'node:stream/web';
import type { Request as ExpressRequest, Response as ExpressResponse } from 'express';

export type WebHandler = (req: Request) => Response | Promise<Response>;

/** Build a Web Request from an Express request. The body is streamed, not buffered. */
export function toWebRequest(req: ExpressRequest): Request {
  const url = new URL(req.originalUrl, `${req.protocol}://${req.get('host') ?? 'localhost'}`);
  const headers = new Headers();
  for (const [key, value] of Object.entries(req.headers)) {
    if (value === undefined) continue;
    if (Array.isArray(value)) value.forEach((v) => headers.append(key, v));
    else headers.set(key, value);
  }
  const hasBody = req.method !== 'GET' && req.method !== 'HEAD';
  return new Request(url, {
    method: req.method,
    headers,
    body: hasBody ? (Readable.toWeb(req) as unknown as BodyInit) : undefined,
    // Required by Node's fetch implementation when the body is a stream.
    ...(hasBody ? { duplex: 'half' } : {}),
  } as RequestInit);
}

/** Write a Web Response to an Express response, piping streamed bodies. */
export async function sendWebResponse(webRes: Response, res: ExpressResponse): Promise<void> {
  res.status(webRes.status);
  webRes.headers.forEach((value, key) => {
    // Set-Cookie may appear more than once; Headers joins it, so use getSetCookie.
    if (key.toLowerCase() === 'set-cookie') return;
    res.setHeader(key, value);
  });
  const cookies = webRes.headers.getSetCookie();
  if (cookies.length > 0) res.setHeader('set-cookie', cookies);

  if (!webRes.body) {
    res.end();
    return;
  }
  await new Promise<void>((resolve, reject) => {
    Readable.fromWeb(webRes.body as unknown as NodeWebReadableStream)
      .on('error', reject)
      .pipe(res)
      .on('finish', resolve)
      .on('error', reject);
  });
}

/** Wrap a Web handler as Express middleware. */
export function adapt(handler: WebHandler) {
  return async (req: ExpressRequest, res: ExpressResponse, next: (err?: unknown) => void) => {
    try {
      await sendWebResponse(await handler(toWebRequest(req)), res);
    } catch (err) {
      next(err);
    }
  };
}
