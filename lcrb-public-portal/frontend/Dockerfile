# Builds the Angular SPA and serves it as static files from unprivileged nginx on 8080.

FROM node:24-slim AS build
WORKDIR /app
RUN corepack enable
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
RUN pnpm install --frozen-lockfile
COPY . .
RUN pnpm build

FROM nginxinc/nginx-unprivileged:1.29-alpine AS runtime
COPY nginx/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/lcrb-public-portal-frontend/browser /usr/share/nginx/html
EXPOSE 8080
HEALTHCHECK --interval=10s --timeout=3s --start-period=5s --retries=3 \
  CMD wget -q -O /dev/null http://127.0.0.1:8080/healthz || exit 1
