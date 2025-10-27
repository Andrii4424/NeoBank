FROM node:20 AS build
WORKDIR /app
COPY src/Client/package*.json ./
RUN npm install

COPY src/Client/ .
RUN npm run build -- --configuration production

FROM nginx:alpine
COPY --from=build /app/dist/Client /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
