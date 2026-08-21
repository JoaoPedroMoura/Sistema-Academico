import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import {
  backendUrl,
  extractCookieValue,
  getSetCookieValues,
  REFRESH_COOKIE_MAX_AGE_SECONDS,
  REFRESH_COOKIE_NAME,
} from "../_lib";

export async function POST(request: NextRequest) {
  const cookieValue = request.cookies.get(REFRESH_COOKIE_NAME)?.value;

  if (!cookieValue) {
    return NextResponse.json({ message: "Sessão não encontrada." }, { status: 401 });
  }

  const backendResponse = await fetch(backendUrl("/api/auth/refresh"), {
    method: "POST",
    headers: { Cookie: `${REFRESH_COOKIE_NAME}=${encodeURIComponent(cookieValue)}` },
  });

  const responseBody = await backendResponse.text();
  const response = new NextResponse(responseBody, {
    status: backendResponse.status,
    headers: { "Content-Type": "application/json" },
  });

  if (backendResponse.ok) {
    const refreshCookie = getSetCookieValues(backendResponse).find((c) => c.startsWith(`${REFRESH_COOKIE_NAME}=`));
    const value = refreshCookie ? extractCookieValue(refreshCookie, REFRESH_COOKIE_NAME) : null;
    if (value) {
      response.cookies.set(REFRESH_COOKIE_NAME, value, {
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        sameSite: "lax",
        path: "/",
        maxAge: REFRESH_COOKIE_MAX_AGE_SECONDS,
      });
    }
  } else {
    // Sessão expirada/revogada na API — limpa o cookie local também, senão o proxy.ts continua
    // achando que há sessão (só checa presença do cookie, não validade).
    response.cookies.delete(REFRESH_COOKIE_NAME);
  }

  return response;
}
