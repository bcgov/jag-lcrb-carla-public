# src/environments

Client-side build constants, per build configuration.

The app has none yet: the only build-dependent behaviour is the production-only analytics in `app.ts`, which uses Angular's `isDevMode()`. When a real constant is needed (for example a public feature flag), add `environment.ts` and `environment.development.ts` here and wire them with `fileReplacements` in `angular.json`.

Never put secrets or runtime configuration here. Everything in this folder is compiled into the browser bundle. Only `server/` reads environment variables, and `tests/bundle-secrets.test.ts` fails if a secret reaches the bundle.
