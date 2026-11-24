# portal-frontend-angular-v2: Developer guide

How to set up, run and test the Angular app locally.

## Table of contents

- [1. Current state](#1-current-state)
- [2. Prerequisites](#2-prerequisites)
- [3. Environment setup](#3-environment-setup)
- [4. Make targets](#4-make-targets)
- [5. Ports](#5-ports)
- [6. Dev mode](#6-dev-mode)
- [7. Containers](#7-containers)
- [8. Tests](#8-tests)
- [8a. Code layout and conventions](#8a-code-layout-and-conventions)
- [9. API collection](#9-api-collection)
- [10. Common issues](#10-common-issues)

## 1. Current state

The app has all 53 routes of the prototype. It includes:
- the Express server with the access gate, the 21 API routes and a health check, and the containers
- the prototype's CSS, assets and `lib/` code, with the React hooks converted to `injectX()` functions
- the UI primitives, the app shell, the six provider services and the AI layer
- the ten features under `src/app/features/`

The visual, smoke and journey suites pass against the prototype's behaviour, and the container image passes the bundle secrets test.

## 2. Prerequisites

| Tool | Version | Notes |
|---|---|---|
| Node.js | 24 LTS | Angular 22 supports `^22.22.3 \|\| ^24.15.0 \|\| >=26` |
| pnpm | 12 | Pinned by `packageManager` in `package.json` |
| Docker with Compose | Docker 29, Compose 5 tested | Needed for the containers and the Playwright suites |
| make | any | Targets live in the root `Makefile` |

No browser install is needed locally: Playwright runs in its own container.

## 3. Environment setup

```
cp .env.example .env      # optional; everything works with defaults
```

- `.env` is read by the Express server and by Docker Compose. It is never committed.
- All variables are listed with comments in `.env.example`. Without any values, the access gate is off and the AI and Blob routes return the prototype's friendly errors (AI JSON routes answer with an error status, streaming routes stream an error chunk, Blob routes answer 500).
- Only `server/` reads environment variables. The Angular bundle never does, and the bundle secrets test enforces it.

## 4. Make targets

Run from this folder (the `Makefile` here).

| Target | What it does |
|---|---|
| `make v2-install` | `pnpm install --frozen-lockfile` |
| `make v2-dev` | Express server (`tsx watch`, port 8090) and `ng serve` (port 4210, bound to `0.0.0.0`) together. `/api` and `/gate` are proxied to Express. Ctrl+C stops both. |
| `make v2-test` | Angular unit tests, production build, then the server, build-output, `.env.example` and bundle secrets tests |
| `make v2-up` | Builds and starts the port's container and its gated twin (`angular-gated`), and waits for their health checks |
| `make v2-postman` | Runs the Postman collection with newman (in a `node:24-slim` container on the compose network) against `angular` and `angular-gated`. Needs `make v2-up` first. |
| `make v2-image-secrets` | Copies the browser bundle out of the running `angular` container into `dist/image/browser` and runs the bundle secrets test on it (mounted over the default build path) in a `node:24-slim` container, with the `.env` values passed by Docker (`--env-file`). Needs `make v2-up` and a `.env` file. |
| `make v2-visual-baseline` | Captures the screenshots from the running app into `e2e/visual/__screenshots__/` (git-ignored). Needs `make v2-up` first. |
| `make v2-e2e` | Smoke test and functional flows in the Playwright container |
| `make v2-visual` | Visual suite: every screenshot must match its baseline within 1% |
| `make v2-down` | Stops all of the above containers |

## 5. Ports

Inside the containers the ports stay 8080 and 3000.

| Port | Service | Change it with |
|---|---|---|
| 4210 | `ng serve` (dev mode) | `--port` in the `dev:web` script; the proxy target is in `proxy.conf.json` |
| 8090 | Express server in dev mode, and the host port of the `angular` container | `V2_DEV_PORT` (Make variable, dev mode), `V2_ANGULAR_PORT` (container) |
| 8091 | Gated Angular container (`angular-gated`, ACCESS_CODE = `GATE_TEST_CODE`) | `V2_ANGULAR_GATED_PORT` |

The `V2_*` port names are read by Docker Compose only (from the shell or `.env`).

## 6. Dev mode

```
make v2-install
make v2-dev
```

Open http://localhost:4210. Health check through the proxy: http://localhost:4210/api/health returns `{"status":"ok"}`.

Package scripts, if you prefer to run the processes separately: `pnpm dev:server`, `pnpm dev:web`.

## 7. Containers

`docker-compose.yml` (project name `portal-frontend-angular-v2`) defines:

| Service | Image | Purpose |
|---|---|---|
| `angular` | built from `Dockerfile` | Multi-stage: builds the SPA, then runs the Express server, which serves the SPA and `/api`. Health check on `/api/health`. Runs as user `node`. |
| `angular-gated` | same image as `angular` | The app with the access gate on (`ACCESS_CODE` from `GATE_TEST_CODE`, a dummy test value), for the gate flow and the gate error screenshot. Host port `V2_ANGULAR_GATED_PORT` (default 8091). |
| `playwright` (profile `test`) | `mcr.microsoft.com/playwright:v1.65.0-noble` | Runs the Playwright suites against `angular` and `angular-gated` by service name, so rendering is identical on every machine |

```
make v2-up
curl http://localhost:8090/api/health
make v2-down
```

## 8. Tests

| Suite | Tool | Location | Run |
|---|---|---|---|
| Angular unit and component tests (primitives, converted hooks, providers, route resolution) | Vitest via `ng test` (jsdom) | `src/**/*.spec.ts` | `pnpm test` |
| Server tests: the gate, the 21 API routes (AI and Blob stubbed) and timeouts | Vitest (node) | `tests/server/*.test.ts` | `pnpm test:server` |
| Build output, `.env.example`, bundle secrets (including no server-only `lib/` file in the bundle), layer boundaries and the initial bundle budget | Vitest (node), after `pnpm build` | `tests/*.test.ts` | `pnpm test:server` |
| Smoke, visual and user-journey suites | Playwright 1.65 in a container | `e2e/smoke`, `e2e/visual`, `e2e/journeys/<journey>` (entry, licensee, special-events, police, local-government, shell) | `make v2-e2e`, `make v2-visual` |

Playwright projects: `angular-desktop`, `angular-mobile` (1280x800 and 390x844).

**Playwright suites**

| Suite | Files | What it checks |
|---|---|---|
| Visual routes | `e2e/visual/routes.ts` (matrix), `routes.spec.ts` | Full-page screenshot of every prototype route (53, some for several roles: 62 entries, 145 screenshots with the states). `matrix.test.ts` (Vitest) fails on duplicate screenshot ids. |
| Visual states | `e2e/visual/states.ts`, `states.spec.ts` | 12 interaction states: assistant panel, menus, dialogs, validation toast, Guided Mode, reviewer brief, gate error |
| Visual shell | `e2e/visual/shell.ts`, `shell.spec.ts` | Element screenshots of the header (each role and signed out), footer, user menu, mobile menu sheet and notifications popover |
| Flows | `flow-01` to `flow-10` in `e2e/journeys/<journey>/` | The 10 user flows, asserting the visible outcome and the stored data |
| Interaction checks | `interactions-*.spec.ts` in `e2e/journeys/<journey>/` | 27 click-through checks (menus, filters, tabs, saving dialogs, downloads, toasts, role settings), desktop only (`desktopOnly` in `e2e/support/flow-helpers.ts`); downloads are checked by file name (`downloadName`) |

- **Baselines** live in `e2e/visual/__screenshots__/` and are **not committed**. Capture them from a known-good build with `make v2-up && make v2-visual-baseline`; later runs of `make v2-visual` compare against them. The page setup is deterministic (frozen clock, seeded random), so an unchanged app matches.
- **Ported flags.** `e2e/support/ported.ts` lists the routes, states and flows switched on for Angular. Angular projects skip everything else. Every id is listed, so the only skips are the single-viewport states.
- **Layer boundaries.** `tests/boundaries.test.ts` runs `tools/check-boundaries.ts`: the seven rules (the five layer rules, data-access only through repository tokens, no literal route paths) and import cycles (type-only imports do not count). The four feature stores and `link-context` are permanent exceptions to rules 5 and 6 (`STORE_EXCEPTIONS`). Known violations would go in `tools/boundaries-allowlist.json`, which is empty; a new violation fails, and so does a listed one that no longer occurs.
- **Bundle budget.** `tests/budget.test.ts` (`tools/check-budget.ts`) sums the files `index.html` loads up front and fails if they grow more than 5% over `tools/budget-baseline.json` (the baseline taken from the first build).
- **Bundle secrets on the image.** `make v2-image-secrets` copies the image's browser bundle to `dist/image/browser` and mounts it read-only over `dist/portal-frontend-angular-v2/browser` in the scan container, so `tests/bundle-secrets.test.ts` runs unchanged on the image's own output. Your local build is not touched.
- **API fixtures** in `tests/fixtures/api/`: JSON answers for the AI routes, AI SDK v6 UI message streams (`uiStream`), and an `/api/upload` stub. `/api/entity-directory` passes through. Any other `/api` call fails the test.

**Helpers** in `e2e/support/` (Playwright only) and `tests/support/` (shared with the unit specs: `api-fixtures.ts`, `fetch-stub.ts`, `flatten-routes.ts`, `secret-scan.ts`):
- `preparePage` (`parity.ts`): signs in a role (the prototype's demo identities), seeds storage, freezes the clock, seeds `Math.random`, stubs map tiles and routes `/api` to the fixtures
- `seedStorage`, `freezeClock`, `seedRandom` (mulberry32, restarted on every page load; the Angular target first discards the draws React makes and Angular does not, `FRAMEWORK_DRAWS` in `parity.ts`), `stubMapTiles`
- `settle`: network idle, web fonts loaded, two animation frames
- `waitForCharts`, `fastForwardAnimations`: chart pages, where Recharts animates in JavaScript
- `routeApi`: fixture router with call counts and unmatched-call detection

Reports are written to `playwright-report/` and `test-results/` (both git-ignored).

**Stubbing AI and Blob in tests**
- **Server tests** call the real `generateText` and `streamText`, with the model swapped for a `MockLanguageModelV3` from `ai/test` (part of `ai`, no extra package). See `tests/server/support/ai-stub.ts`: `jsonModel`, `textModel`, `failingModel` and `streamModel`. Each test file activates it with `vi.mock('ai', …)`. `@vercel/blob` is stubbed with `vi.mock`. No test reaches the AI Gateway or Blob.
- **Angular unit specs** use `stubFetch` (`tests/support/fetch-stub.ts`). It serves the same `tests/fixtures/api/` fixtures as the Playwright suites and records the request bodies.

## 8a. Code layout and conventions

- **Layout.** The team layout:
  - `domain/` holds framework-free models, labels, rules and pure helpers. It imports nothing outside domain and uses no browser globals.
  - `data-access/repositories/<module>/` holds the repository interface and token, the mock implementation, signals, mock services and the `mock/` seed data.
  - `core/` holds the app-wide services, the layout, navigation, permissions and shell pages.
  - `shared/ui`, `shared/forms` and `shared/util` hold the design system, the form engine and the helpers. A component used by more than one feature goes in `shared/ui/<area>/`.
  - `features/<feature>/` holds `pages/`, `components/`, `state/` (stores used by one feature), `facades/` (multi-step orchestration) and `<feature>.routes.ts`. No feature imports another.
- **Repositories.** Each backend module has `data-access/repositories/<module>/<module>.repository.ts` (interface and `InjectionToken`) and `<module>.repository.mock.ts`. Tokens are tree-shakable and default to the mock, so nothing is registered in `app.config.ts` and unit tests need no providers. Inject a token in a component or service (`inject(LICENCES_REPOSITORY).licences()`); collections are signals, and hook members (`drafts`, `receipts`, `store`, …) must be called in an injection context. Outside data-access, values come from data-access only through these tokens (boundary rule 6); the four feature stores and `link-context` are the exceptions. To use a real backend later, provide a real implementation for the token in `app.config.ts`.
- **Route keys.** `src/app/route-registry.ts` holds a stable key per route. Every internal link uses `path('licences.detail', { id })` (in a template through `protected readonly path = path;`); the boundary test fails on a literal route path. `path()` inserts parameter values as given.
- **Path aliases.** `@core/*`, `@domain/*`, `@data-access/*`, `@shared/*`, `@features/*` (all under `src/app/`), `@server-lib/*` (`server/lib/`) and `@/*` (`src/`, used for `@/app/route-registry`). They are set in `tsconfig.json`, and `vitest.server.config.ts` uses the same mappings.
- **Server-only files** (gate, knowledge base, model config, registry services and so on) live in `server/lib/`, outside the browser build. The bundle test takes its list from that folder and fails if any of them reaches the browser bundle.
- **Icons.** Use `<svg lucide="Name" class="…"></svg>` with a lucide icon name (aliases such as `AlertCircle` and the `XIcon` / `LucideX` forms also work). To add icons to the registry, run `pnpm icons Name1 Name2 …`. This regenerates `src/app/shared/ui/primitives/icons.ts` from the installed lucide 0.564.0 package; do not edit that file by hand.
- **Server routes.** `server/routes/<name>.ts` is a copy of the prototype's `app/api/<name>/route.ts`. Only the swap of `NextResponse`/`NextRequest` for `Response`/`Request` is allowed, and `tests/server/routes.test.ts` fails on any other difference. `server/routes/index.ts` registers each file at `/api/<name>`, and turns its `maxDuration` export into a request timeout (504 before the response starts). `server/gate.ts` is mounted before the API routes; `/api/health` comes before the gate, so the container health check works with `ACCESS_CODE` set.
- **Pathname.** `injectPathname()` (`src/app/core/navigation/pathname.ts`) is the prototype's `usePathname()`.
- **Component hosts and child selectors.** Components use `host: { style: 'display: contents' }` so they add no box. When a parent styles its direct children (`divide-y`, `space-y-*`, `[&>*]`, `first:`/`last:`), those selectors hit the host, not the element inside it. A component rendered as such a child goes on the real element instead (an attribute selector such as `a[dashboardActivityItem]`).
- **Controlled primitives.** Checkbox, Switch, RadioGroup and Select follow Radix (`ui/controllable.ts`): a bound `[checked]`/`[value]` wins and a click only emits `checkedChange`/`valueChange`; unbound, they keep their own state from `defaultChecked`/`defaultValue`. After a click they render at once (`injectFlushSync`), as React does for clicks.
- **Select overlay.** `ui-select` has a block host (so parent `space-y-*` margins land on it), and its list is positioned like Radix's popper: natural width, then shifted to keep 10px from the viewport edges.
- **Assistant panel.** Loaded with `@defer` in the launcher, so the AI SDK stays out of the initial bundle.
- **Routes.** Each feature's pages are in `features/<feature>/<feature>.routes.ts` as `page(routePath(key), loader, extra)`. `app.routes.ts` keeps the shell routes, the redirects and the wildcard, and loads each feature with `{ matcher: featureMatcher('<key prefix>'), loadChildren }` (`core/navigation/route-page.ts`), so only the matching feature's route chunk is fetched. The route specs read the full table through `tests/support/flatten-routes.ts`. Each matrix id maps to its page in `PORTED_PAGES` in `app.routes.spec.ts`, and its ids are listed in `e2e/support/ported.ts`.
- **Adding a page.** Add a key to `route-registry.ts`, the route to its feature's `<feature>.routes.ts` (`page(routePath(key), loader)`), an entry to the visual matrix (`e2e/visual/routes.ts`) with a baseline (`make v2-visual-baseline`), and the page id to `e2e/support/ported.ts`. Link to it with `path(key)`.

## 9. API collection

`docs/postman/portal-frontend-angular.postman_collection.json` covers the access gate and all 21 API routes (29 requests), with test scripts. `sample-upload.txt` beside it is the upload request's file.

| Variable | Default | Meaning |
|---|---|---|
| `baseUrl` | `http://localhost:8090` | The port with the gate off (`make v2-up` or `make v2-dev`) |
| `gatedUrl` | `http://angular-gated:8080` | A server with `ACCESS_CODE` set. On the compose network (`make v2-postman`) use the default; for Postman on the host use `http://localhost:8091` (`V2_ANGULAR_GATED_PORT`). |
| `accessCode` | `parity-gate-code` | The dummy `GATE_TEST_CODE` |

Run it with `make v2-postman`. Without `AI_GATEWAY_API_KEY` and `BLOB_READ_WRITE_TOKEN`, the AI and Blob requests get their friendly errors, and the tests accept either outcome. **With `AI_GATEWAY_API_KEY` set, the 17 AI requests make live, billed model calls** (small models, a few cents a run). The `document-check` request sends a minimal one-page PDF, so it also passes against a live key. **With `BLOB_READ_WRITE_TOKEN` set, the `upload` request stores a real file** (`documents/postman/sample-upload-<suffix>.txt`) on every run, and there is no delete route. Remove these test files in the Vercel dashboard, or with `list` and `del` from `@vercel/blob` run inside the `angular` container, so the token stays there.

## 10. Common issues

| Problem | Cause | Fix |
|---|---|---|
| `pnpm install` fails with `ERR_PNPM_IGNORED_BUILDS` | pnpm 12 blocks dependency build scripts unless decided in `pnpm-workspace.yaml` | Add the package under `allowBuilds` with `false` (or `true` with a reason) |
| `make v2-test` fails at "build output exists" | The Node-side tests read `dist/` | `v2-test` builds first; when running `pnpm test:server` alone, run `pnpm build` first |
| Playwright cannot reach `http://angular:8080` | The container is not healthy yet, or the app failed to start | `make v2-up` and check `docker compose logs angular` |
| Postman: the unlock request sets no cookie in the jar | The cookie is `Secure` in production builds, so clients drop it over plain HTTP | The collection copies it into `gateCookie` and sends it as a header |
| Dev mode runs, but http://localhost:4210 does not load in the host browser | `ng serve` on its own listens only on `localhost` (`[::1]` in the container), so port forwarding from the host over IPv4 cannot connect | `dev:web` passes `--host 0.0.0.0`. If you run `ng serve` by hand, add `--host 0.0.0.0` |
| Port 8090, 8091 or 4210 already in use | Another process holds the port | Set `V2_ANGULAR_PORT` or `V2_ANGULAR_GATED_PORT` in `.env`, or `V2_DEV_PORT` for the dev server (then also change the target in `proxy.conf.json`) |
| `make v2-visual` fails with "snapshot doesn't exist" | Baselines are not committed | `make v2-up && make v2-visual-baseline` |
| A screenshot differs between runs of an unchanged app | Something on the page is not deterministic (dates, random values, animation) | Check the diff in `test-results/`; fix the cause in the test setup (seed, wait, stub), not by raising the threshold |
| A route shows "Something went wrong on this page" | An error reached the `ErrorHandler` (route error page) | Check the browser console for the logged error (`[v0] Route error boundary caught:`) |
| A new icon renders nothing, or the build says the name is not assignable to `IconName` | The icon is not in the registry | `pnpm icons <Name>` |
| The gate flow's cookie is missing | The gate cookie is `Secure` in production builds and browsers drop it over plain HTTP | Flow 1 checks the `Set-Cookie` header and re-stores the same cookie without `Secure` (test-only) |
