/**
 * Helpers compartilhados pelas rotas BFF de auth (login/refresh/logout). Não é uma rota — Next.js
 * só trata `route.ts` como endpoint, então este arquivo fica invisível ao roteador.
 *
 * Por quê um BFF aqui: o cookie httpOnly do refresh token precisa viver na origem do Next.js
 * (para o proxy.ts do edge conseguir lê-lo e barrar navegação antes da página renderizar). Se a
 * API .NET setasse o cookie diretamente, ele ficaria preso à origem dela — o Next nunca o veria
 * (decisão registrada em ARCHITECTURE.md §5.2). Essas rotas re-emitem o cookie na origem certa.
 */
const API_URL = process.env.API_INTERNAL_URL ?? process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5100";

export const REFRESH_COOKIE_NAME = "faeterj_refresh";

export function backendUrl(path: string): string {
  return `${API_URL}${path}`;
}

export function extractCookieValue(setCookieHeader: string, cookieName: string): string | null {
  const match = setCookieHeader.match(new RegExp(`${cookieName}=([^;]+)`));
  return match ? decodeURIComponent(match[1]) : null;
}

/**
 * `Response.headers.get('set-cookie')` só devolve 1 valor mesmo com múltiplos headers — usa
 * `getSetCookie()` (undici, runtime de fetch do Next.js) quando disponível.
 */
export function getSetCookieValues(response: Response): string[] {
  const headers = response.headers as Headers & { getSetCookie?: () => string[] };
  if (typeof headers.getSetCookie === "function") {
    return headers.getSetCookie();
  }
  const single = response.headers.get("set-cookie");
  return single ? [single] : [];
}

export const REFRESH_COOKIE_MAX_AGE_SECONDS = 60 * 60 * 24 * 7; // espelha Jwt:RefreshTokenDays da Api
