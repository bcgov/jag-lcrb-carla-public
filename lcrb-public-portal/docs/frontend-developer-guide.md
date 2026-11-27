# Public portal frontend: developer guide

## Contents

- [Overview](#overview)
- [Current state](#current-state)
- [Prerequisites](#prerequisites)
- [Environment](#environment)
- [Commands](#commands)
- [Make targets](#make-targets)
- [Layout](#layout)
- [Adding a feature](#adding-a-feature)
- [Common issues](#common-issues)

## Overview

Angular frontend for the new LCRB public portal, where applicants and licensees sign in, apply for and manage licences. It is organised into core, domain, data-access, shared and feature layers, with one folder per feature under `src/app/features/`.

The code is in `lcrb-public-portal/frontend/`. The production image is a static build served by unprivileged nginx on port 8080, with an SPA fallback and `/healthz`.

Keep this guide up to date as part of each change: setup, commands, Make targets, ports, layout and the current state below.

## Current state

Update this section as features are built or mocks are replaced.

| Area | State |
|---|---|
| Login | Mock role picker (5 roles, demo identities, session in localStorage) |
| Dashboard | Built; stats, activity and renewals are page data |
| Notifications | Mock repository backed by localStorage (header bell and dashboard) |
| AI prompt box, assistant button, header search, drafts panel, smart reminders | Render but do nothing yet |
| Other features | Folder and empty `<feature>.routes.ts` only; their URLs land on Not Found |

## Prerequisites

- Node 24 and pnpm 12 (`corepack enable`; the version is pinned by `packageManager` in `package.json`).
- Docker with compose, for the container and the e2e tests.

## Environment

| Variable | Default | Used by |
|---|---|---|
| `PUBLIC_FE_PORT` | `8100` | Host port of the nginx container |
| `BASE_URL` | `http://web:8080` | Playwright target inside the compose network |

See `frontend/.env.example`. No other configuration is needed.

## Commands

Run from `lcrb-public-portal/frontend/`.

| Command | What it does |
|---|---|
| `pnpm install --frozen-lockfile` | Install dependencies |
| `pnpm dev` | ng serve on http://localhost:4220 |
| `pnpm lint` | ESLint: TypeScript, Angular, template accessibility and layer rules (`eslint.config.js`) |
| `pnpm test` | Unit tests (Vitest via `ng test`) |
| `pnpm build` | Production build to `dist/lcrb-public-portal-frontend/browser` |
| `pnpm test:tools` | Import-boundary check (`tools/check-boundaries.ts`, allowlist `tools/boundaries-allowlist.json`) |
| `pnpm format` | Prettier |

## Make targets

Run from root directory, where the new portal's `Makefile` is.

| Target | What it does |
|---|---|
| `make public-fe-install` | Install dependencies |
| `make public-fe-dev` | ng serve on 4220 |
| `make public-fe-lint` | ESLint only |
| `make public-fe-test` | Lint, unit tests, production build, boundary check |
| `make public-fe-up` | Build and start the nginx container on `PUBLIC_FE_PORT` (default 8100) |
| `make public-fe-e2e` | Playwright smoke tests (`e2e/`) in a container against the nginx container |
| `make public-fe-down` | Stop the containers |

## Layout

- `src/app/core`: shell services and layout (session, header, footer, navigation, errors, overlay, progress).
- `src/app/domain`: pure types and rules, no Angular and no browser globals.
- `src/app/data-access`: repositories; mocks live under `repositories/<module>/mock/`.
- `src/app/shared`: UI primitives, BC Gov components and utilities.
- `src/app/features/<feature>`: `pages`, `components`, `state`, `facades` and `<feature>.routes.ts`.
- `src/app/route-registry.ts`: stable route keys; links use `path('key')`.
- `nginx/nginx.conf`: SPA fallback, asset caching and `/healthz` for the container.
- `e2e/`, `tests/`, `tools/`: Playwright smoke tests, the import-boundary test and its checker.
- `eslint.config.js`: lint config. `boundaries/dependencies` enforces the layer imports in the editor; `tools/eslint-alias-resolver.cjs` resolves the tsconfig path aliases for it. Domain-to-Angular imports, literal route paths, browser globals in domain and import cycles are checked only by `pnpm test:tools`.

## Adding a feature

1. Add pages under `features/<feature>/pages` and routes in `<feature>.routes.ts`, using registry keys.
2. Wire the feature in `app.routes.ts` with `featureMatcher('<feature>')` and a lazy `loadChildren`.
3. Run `make public-fe-test`.

## Common issues

- **Port 8100 in use:** set `PUBLIC_FE_PORT` before `make public-fe-up`.
- **`pnpm lint` reports `boundaries/dependencies`:** the import crosses a layer; move the code or import through the module's `*.repository.ts`.
- **A new boundary violation fails `pnpm test:tools`:** fix the import. Only add an entry to `tools/boundaries-allowlist.json` when the reason has been agreed in review.
