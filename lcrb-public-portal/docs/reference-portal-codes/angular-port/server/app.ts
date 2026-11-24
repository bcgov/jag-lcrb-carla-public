/** Express app factory: API routes, the access gate and static hosting. */
import express from 'express';
import { existsSync } from 'node:fs';
import path from 'node:path';
import { gateRouter } from './gate';
import { apiRouter } from './routes';

export interface AppOptions {
  /** Folder with the Angular `browser` build output. Static hosting is skipped if it does not exist. */
  staticDir?: string;
}

export const DEFAULT_STATIC_DIR = path.resolve(import.meta.dirname, '../dist/portal-frontend-angular-v2/browser');

export function createApp(options: AppOptions = {}) {
  const staticDir = options.staticDir ?? DEFAULT_STATIC_DIR;
  const app = express();
  app.disable('x-powered-by');

  // Container health check, registered before the gate so it works with ACCESS_CODE set.
  app.get('/api/health', (_req, res) => {
    res.json({ status: 'ok' });
  });

  // Access gate in front of everything else, then the API routes.
  app.use(gateRouter());
  app.use('/api', apiRouter());

  // Unknown API paths answer as JSON, never with the SPA's index.html.
  app.use('/api', (_req, res) => {
    res.status(404).json({ error: 'Not found' });
  });

  if (existsSync(staticDir)) {
    app.use(express.static(staticDir, { index: false }));
    // A missing file (last segment has an extension) is a 404, not index.html, so the browser does not run HTML as a script.
    app.get(/\/[^/]+\.[a-z0-9]+$/i, (_req, res) => {
      res.status(404).type('text/plain').send('Not found');
    });
    // SPA fallback: Angular routing (and its not-found page) handles every other GET.
    app.get('/{*path}', (_req, res) => {
      res.sendFile(path.join(staticDir, 'index.html'));
    });
  }

  return app;
}
