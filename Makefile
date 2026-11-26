# New portal: run and test its services locally. Run from this folder.

PUBLIC_FE_DIR ?= lcrb-public-portal/frontend
PUBLIC_FE_COMPOSE ?= docker compose -f $(PUBLIC_FE_DIR)/docker-compose.yml

.PHONY: public-fe-install public-fe-dev public-fe-lint public-fe-test public-fe-e2e public-fe-up public-fe-down

## public frontend: install dependencies (pnpm 12, pinned by packageManager)
public-fe-install:
	cd $(PUBLIC_FE_DIR) && pnpm install --frozen-lockfile

## public frontend dev mode: ng serve on 4220
public-fe-dev:
	cd $(PUBLIC_FE_DIR) && pnpm dev

## public frontend: ESLint, including the layer rules
public-fe-lint:
	cd $(PUBLIC_FE_DIR) && pnpm lint

## public frontend: lint, unit tests, production build, then the import-boundary check
public-fe-test:
	cd $(PUBLIC_FE_DIR) && pnpm lint && pnpm test && pnpm build && pnpm test:tools

## public frontend: Playwright smoke tests in a container against the nginx container
public-fe-e2e:
	$(PUBLIC_FE_COMPOSE) --profile test run --rm --build playwright

## public frontend: build and start the nginx container (PUBLIC_FE_PORT, default 8100)
public-fe-up:
	$(PUBLIC_FE_COMPOSE) up -d --build --wait web

## public frontend: stop its containers
public-fe-down:
	$(PUBLIC_FE_COMPOSE) --profile test down
