# Angular port. Run from this folder.

V2_COMPOSE ?= docker compose -f docker-compose.yml
V2_PLAYWRIGHT = $(V2_COMPOSE) --profile test run --rm --build playwright
V2_DEV_PORT ?= 8090

.PHONY: v2-install v2-dev v2-test v2-e2e v2-visual v2-visual-baseline v2-postman v2-image-secrets v2-up v2-down

## install dependencies (pnpm 12, pinned by packageManager)
v2-install:
	pnpm install --frozen-lockfile

## dev mode: Express on V2_DEV_PORT (default 8090) + ng serve on 4210 with /api and /gate proxied
v2-dev:
	{ PORT=$(V2_DEV_PORT) pnpm dev:server & SERVER=$$!; trap "kill $$SERVER" EXIT INT TERM; pnpm dev:web; }

## unit tests, production build, then server, build-output, env, secrets, boundary and budget tests
v2-test:
	pnpm test && pnpm build && pnpm test:server

## user-journey flows and interaction checks (e2e/journeys) and the smoke test in the Playwright container
v2-e2e:
	$(V2_PLAYWRIGHT) e2e/smoke e2e/journeys --pass-with-no-tests

## visual suite in the Playwright container, compared against the app's own baselines (1%)
v2-visual:
	$(V2_PLAYWRIGHT) e2e/visual --pass-with-no-tests

## capture the visual baselines from the running app (git-ignored, regenerated); run make v2-up first
v2-visual-baseline:
	$(V2_PLAYWRIGHT) e2e/visual --update-snapshots=all

## Postman collection (gate + 21 API routes) with newman on the compose network; run make v2-up first
v2-postman:
	docker run --rm --network portal-frontend-angular-v2_default -v "$(CURDIR)/docs/postman:/etc/newman" -w /etc/newman \
		node:24-slim npx -y newman@6.2.1 run portal-frontend-angular.postman_collection.json \
		--env-var baseUrl=http://angular:8080 --env-var gatedUrl=http://angular-gated:8080

## bundle secrets test on the running image's own build output, with the .env values it runs with; run make v2-up first
v2-image-secrets:
	rm -rf dist/image && mkdir -p dist/image
	$(V2_COMPOSE) cp angular:/app/dist/portal-frontend-angular-v2/browser dist/image/browser
	docker run --rm --env-file .env -v "$(CURDIR):/app" -w /app \
		-v "$(CURDIR)/dist/image/browser:/app/dist/portal-frontend-angular-v2/browser:ro" \
		node:24-slim node node_modules/vitest/vitest.mjs run --config vitest.server.config.ts tests/bundle-secrets.test.ts

## build and start the app's container (V2_ANGULAR_PORT, default 8090) and its gated twin (V2_ANGULAR_GATED_PORT, default 8091)
v2-up:
	$(V2_COMPOSE) up -d --build --wait angular angular-gated

## stop all containers (angular, angular-gated, playwright)
v2-down:
	$(V2_COMPOSE) --profile test down
