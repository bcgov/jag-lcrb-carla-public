# Dockerfile.web (Angular 13+ build then nginx serve)
FROM node:18-bullseye AS build
WORKDIR /app
COPY ./cllc-public-app/ClientApp/package*.json ./
RUN npm ci
COPY ./cllc-public-app/ClientApp/ ./
# Keep your base-href (/lcrb/) as needed
RUN npm run build -- --configuration production --base-href /lcrb/ --output-path=dist/lcrb

FROM nginx:alpine
# Nginx will run as non-root just fine under OpenShift random UID if we adjust perms
# Copy app
COPY --from=build /app/dist/lcrb /usr/share/nginx/html

# Copy nginx configuration
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 8080
# Don't set USER; OpenShift will inject a random non-root UID.
CMD ["nginx","-g","daemon off;"]
