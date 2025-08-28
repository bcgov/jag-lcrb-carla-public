# Modern Angular + nginx build (replaces cllc-public-angular + cllc-public-app-app)
FROM node:18-bullseye AS build
WORKDIR /app

# Copy package files for better Docker layer caching
COPY ./cllc-public-app/ClientApp/package*.json ./
RUN npm ci --production=false

# Copy source and build
COPY ./cllc-public-app/ClientApp/ ./
# Use environment-specific base-href - can be overridden via build args
ARG BASE_HREF=/lcrb/
RUN npm run build -- --configuration production --base-href ${BASE_HREF} --output-path=dist/lcrb

FROM nginx:alpine
# Install envsubst for runtime config if needed
RUN apk add --no-cache gettext

# Set proper permissions for OpenShift random UID
RUN chmod g+rwx /var/cache/nginx /var/run /var/log/nginx && \
    chmod g+rw /etc/nginx/conf.d

# Copy built Angular app
COPY --from=build /app/dist/lcrb /usr/share/nginx/html/

# Copy nginx configuration
COPY cicd/docker/cllc-public-app/app/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 8080
# OpenShift will inject random UID, so don't set USER
CMD ["nginx", "-g", "daemon off;"]
