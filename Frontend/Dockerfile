# Stage 1: Build frontend with Vite
FROM node:20 AS build

WORKDIR /app

COPY . .

RUN npm install
RUN npm run build

# Stage 2: Serve static files with nginx
FROM nginx:alpine

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx.conf /etc/nginx/conf.d

COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]