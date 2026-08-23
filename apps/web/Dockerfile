# Build de teste/dev via docker-compose — não é a imagem de produção do TCC (isso é trabalho
# futuro, ver ARCHITECTURE.md §8: sem pipeline de CI/CD nem deploy configurados ainda).
FROM node:24-alpine AS build
WORKDIR /app

COPY apps/web/package.json apps/web/package-lock.json ./
RUN npm ci

COPY apps/web/ .

# NEXT_PUBLIC_* é embutido no bundle do browser em build time — precisa ser a URL que o
# navegador do host enxerga (porta publicada da Api), não o nome do serviço na rede interna
# do Docker.
ARG NEXT_PUBLIC_API_URL=http://localhost:5100
ENV NEXT_PUBLIC_API_URL=${NEXT_PUBLIC_API_URL}
RUN npm run build

FROM node:24-alpine AS runtime
WORKDIR /app
ENV NODE_ENV=production

COPY --from=build /app/public ./public
COPY --from=build /app/.next/standalone ./
COPY --from=build /app/.next/static ./.next/static

EXPOSE 3000
ENV PORT=3000
ENV HOSTNAME=0.0.0.0

CMD ["node", "server.js"]
