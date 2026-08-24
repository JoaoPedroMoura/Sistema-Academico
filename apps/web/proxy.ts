import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

/**
 * Proxy (substitui "middleware" a partir do Next.js 16 — ver AGENTS.md deste projeto).
 * Protege as rotas por papel antes da renderização, evitando flash de conteúdo não autorizado.
 *
 * Checa só a *presença* do cookie de sessão — barato, roda em toda requisição. A checagem fina
 * (o papel da sessão bate com a área acessada?) fica por conta de `RequireRole`, no layout de
 * cada área, que já tem a sessão completa via POST /api/auth/refresh.
 *
 * O cookie httpOnly vive na origem do próprio Next.js (não da API .NET) — ver
 * `app/api/auth/_lib.ts` para o porquê (padrão BFF, decisão registrada em ARCHITECTURE.md §5.2).
 */
// "trocar-senha" não é uma área de papel — é a página obrigatória de troca de senha temporária
// (ARCHITECTURE.md §7.5), acessível a qualquer sessão autenticada independente do papel — mas
// precisa do mesmo bloqueio de "sem cookie = sem acesso" que as áreas de papel têm.
const PROTECTED_AREAS = ["admin", "secretaria", "professor", "aluno", "trocar-senha"] as const;
const REFRESH_COOKIE_NAME = "faeterj_refresh";

export function proxy(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const isProtectedArea = PROTECTED_AREAS.some((area) => pathname.startsWith(`/${area}`));

  if (!isProtectedArea) {
    return NextResponse.next();
  }

  if (!request.cookies.has(REFRESH_COOKIE_NAME)) {
    const loginUrl = new URL("/login", request.url);
    loginUrl.searchParams.set("redirectTo", pathname);
    return NextResponse.redirect(loginUrl);
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    "/admin/:path*",
    "/secretaria/:path*",
    "/professor/:path*",
    "/aluno/:path*",
    "/trocar-senha/:path*",
  ],
};
