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
  const body = await request.text();

  const backendResponse = await fetch(backendUrl("/api/auth/login"), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body,
  });

  const responseBody = await backendResponse.text();
  const response = new NextResponse(responseBody, {
    status: backendResponse.status,
    headers: { "Content-Type": "application/json" },
  });

  const refreshCookie = getSetCookieValues(backendResponse).find((c) => c.startsWith(`${REFRESH_COOKIE_NAME}=`));
  const value = refreshCookie ? extractCookieValue(refreshCookie, REFRESH_COOKIE_NAME) : null;

  if (value) {
    response.cookies.set(REFRESH_COOKIE_NAME, value, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      path: "/", // precisa ser "/" (não só /api/auth) para o proxy.ts conseguir ler em qualquer rota
      maxAge: REFRESH_COOKIE_MAX_AGE_SECONDS,
    });
  }

  return response;
}
