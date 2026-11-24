# portal-frontend-angular-ported

Angular 22 app ported from a Next.js prototype, plus a small Express server that hosts the API routes, the access gate and the built app.

## Table of contents

- [Rules](#rules)
- [Layout](#layout)

## Rules

- **Port as is.** Reproduce the prototype exactly, known bugs included. Never fix, improve or redesign. Any unavoidable difference needs the user's approval.
- **Team layout.** Files go in the layers of the team layout: `core/`, `domain/`, `data-access/`, `shared/{ui,forms,util}` and `features/<feature>/{pages,components,state,facades}`, with `<feature>.routes.ts`. A component used by more than one feature goes in `shared/ui/<area>/`, never in a feature.
- **Boundary rules.** `tests/boundaries.test.ts` checks seven rules: the layer imports, repository-only access to data-access, and no literal route paths. The allowlist (`tools/boundaries-allowlist.json`) is empty; a new entry needs a reason agreed in review.
- **Data through repositories.** Pages and components inject a repository token (`inject(LICENCES_REPOSITORY)`); they never import seed data or mock services. Types, labels and pure helpers come from `domain/`. The four feature stores and `core/navigation/link-context.ts` are the only exceptions.
- **Route keys.** Every internal link uses `path('key', params)` from `src/app/route-registry.ts`. A new page needs a key, a route in its feature's routes file and an entry in the visual matrix.
- **No secrets in the SPA.** Only `server/` reads environment variables. The bundle secrets test must pass.
- **Pinned versions.** Libraries kept from the prototype stay at the prototype's lockfile versions. Do not add packages without the user's approval.
- **pnpm 12** only (`packageManager` field). Build scripts are denied by default in `pnpm-workspace.yaml` (`allowBuilds`); approve one only with a reason.
- **Tests.** Every change has tests: `make v2-test` (unit, server, build output, env, secrets, boundaries, budget), `make v2-visual` and `make v2-e2e`.
- **Server routes are copies.** `server/routes/*.ts` stay identical to the prototype's route files apart from the edits that swap the Next.js request and response types for the standard `Request`/`Response`, and the moved import paths. The automated copy check needed the prototype checkout and is not in this copy.
- **Keep docs current.** Update the developer guide for any change to setup, env vars, ports, Make targets or layout. Update the Postman collection for any endpoint change.

## Layout

| Path | What it holds |
|---|---|
| `src/app/` | `app.ts` (shell), `app.config.ts`, `app.routes.ts` (shell routes, redirects, one lazy entry per feature, wildcard) and `route-registry.ts` (53 stable route keys) |
| `src/app/core/` | Session, a11y, the assistant, layout (header, footer, search, bell), navigation (title, pathname, link context, route helpers and feature matchers), errors, permissions (role guards), shell pages (gate, not found) |
| `src/app/domain/` | Framework-free models (`models/`), labels, rules and pure helpers per module |
| `src/app/data-access/` | `repositories/<module>/`: the repository interface and tree-shakable token, the mock implementation, signals, mock services and `mock/` seed data; `api/` and `mappers/` are placeholders until a backend contract exists |
| `src/app/shared/` | `ui/` (primitives, charts, BC Gov pieces, cross-feature components), `forms/` (the application framework and validators), `util/` (helpers, PDF builders) |
| `src/app/features/` | The ten features (dashboard, licences, applications, special-events, account, legal-entity, partner-lg, partner-police, staff, public-tools) |
| `src/environments/` | README only: the app has no client build constants yet |
| `server/` | Express 5: `main.ts`, `app.ts` (app factory), `web-adapter.ts`, `gate.ts`, `routes/` (the 21 prototype API routes, copied) and `lib/` (server-only code) |
| `e2e/` | Playwright suites in the Playwright container: `journeys/` (flows and interaction checks by user journey), `visual/` (screenshots and the matrix test), `smoke/`, `support/` |
| `tests/` | Repo and server tests (Vitest, node), the API fixtures and helpers shared with the unit specs (`fixtures/`, `support/`) |
| `tools/` | `check-boundaries.ts` with `boundaries-allowlist.json`, `check-budget.ts` with `budget-baseline.json`, `generate-client.mjs` (stub until an OpenAPI spec exists), `gen-icons.mjs` (`pnpm icons <Name…>`) |
| `docs/` | The developer guide and the Postman collection (`make v2-postman`) |
