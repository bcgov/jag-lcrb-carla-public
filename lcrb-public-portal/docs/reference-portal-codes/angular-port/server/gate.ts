/** Access gate: request check plus the access-code form handler. */
import express, { type NextFunction, type Request, type Response, type Router } from 'express';
import {
  GATE_COOKIE,
  GATE_MAX_AGE_SECONDS,
  computeGateToken,
  expectedGateToken,
  getAccessCode,
  isGateEnabled,
  safeEqual,
} from '@server-lib/gate';

/** Gate runs on every path except framework internals and static asset files, so the lock screen always loads. */
const MATCHER =
  /^\/(?!_next\/static|_next\/image|favicon.ico|robots.txt|sitemap.xml|manifest.webmanifest|.*\.(?:png|jpg|jpeg|gif|svg|ico|webp|woff|woff2|ttf|otf|css|js|map)$).*$/;

/** Wrong-code delay that blunts brute force. */
export const WRONG_CODE_DELAY_MS = 600;

/** Allow, 401 for `/api/*`, or redirect to `/gate?next=`. */
export async function gateMiddleware(req: Request, res: Response, next: NextFunction): Promise<void> {
  if (!MATCHER.test(req.path)) return next();
  // Gate disabled → behave like a normal app.
  if (!isGateEnabled()) return next();

  const pathname = req.path;
  // Always allow the gate page itself and its assets, so users can unlock.
  if (pathname === '/gate' || pathname.startsWith('/gate/')) return next();

  const expected = await expectedGateToken();
  const provided = readCookie(req, GATE_COOKIE) ?? '';
  if (expected && safeEqual(provided, expected)) return next();

  // API/data requests should fail as JSON rather than an HTML redirect.
  if (pathname.startsWith('/api/')) {
    res.status(401).json({ error: 'Access code required.' });
    return;
  }

  // Everything else → send to the gate, preserving where they were headed.
  const search = req.originalUrl.includes('?') ? req.originalUrl.slice(req.originalUrl.indexOf('?')) : '';
  const url = new URL('/gate', `${req.protocol}://${req.get('host') ?? 'localhost'}`);
  const nextPath = pathname + search;
  if (nextPath && nextPath !== '/') url.searchParams.set('next', nextPath);
  // 307 redirect, as the prototype does.
  res.redirect(307, url.toString());
}

/** Only allow same-origin, absolute-path redirects (prevents open redirects). */
function safeNext(next: unknown): string {
  const value = typeof next === 'string' ? next : '';
  if (value.startsWith('/') && !value.startsWith('//')) return value;
  return '/login';
}

/** The gate page posts a plain HTML form here; the redirect after the POST is a 303 See Other. */
export async function verifyAccessCode(req: Request, res: Response): Promise<void> {
  const body = (req.body ?? {}) as Record<string, unknown>;
  const next = safeNext(body['next']);
  const submitted = String(body['code'] ?? '');
  const accessCode = getAccessCode();

  // Gate somehow disabled between render and submit → just continue.
  if (!accessCode) {
    res.redirect(303, next);
    return;
  }

  const submittedToken = await computeGateToken(submitted);
  const expectedToken = await computeGateToken(accessCode);

  if (!safeEqual(submittedToken, expectedToken)) {
    await new Promise((r) => setTimeout(r, WRONG_CODE_DELAY_MS));
    const params = new URLSearchParams({ error: '1' });
    if (next && next !== '/login') params.set('next', next);
    res.redirect(303, `/gate?${params.toString()}`);
    return;
  }

  res.setHeader('Set-Cookie', gateCookie(expectedToken));
  res.redirect(303, next);
}

/** Cookie header formatted as Next.js writes it (Expires alongside Max-Age, SameSite=lax). */
function gateCookie(token: string): string {
  const expires = new Date(Date.now() + GATE_MAX_AGE_SECONDS * 1000).toUTCString();
  return [
    `${GATE_COOKIE}=${token}`,
    'Path=/',
    `Expires=${expires}`,
    `Max-Age=${GATE_MAX_AGE_SECONDS}`,
    ...(process.env['NODE_ENV'] === 'production' ? ['Secure'] : []),
    'HttpOnly',
    'SameSite=lax',
  ].join('; ');
}

function readCookie(req: Request, name: string): string | undefined {
  for (const part of (req.headers.cookie ?? '').split(';')) {
    const eq = part.indexOf('=');
    if (eq > 0 && part.slice(0, eq).trim() === name) return decodeURIComponent(part.slice(eq + 1).trim());
  }
  return undefined;
}

/** Gate middleware plus the `POST /gate` form handler, mounted before everything else. */
export function gateRouter(): Router {
  const router = express.Router();
  router.use(gateMiddleware);
  router.post('/gate', express.urlencoded({ extended: false }), verifyAccessCode);
  return router;
}
