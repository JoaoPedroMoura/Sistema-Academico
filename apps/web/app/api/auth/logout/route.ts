import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import { backendUrl, REFRESH_COOKIE_NAME } from "../_lib";

export async function POST(request: NextRequest) {
  const cookieValue = request.cookies.get(REFRESH_COOKIE_NAME)?.value;

  if (cookieValue) {
    // Best-effort: mesmo se a API estiver fora, o cookie local é limpo de qualquer forma.
    await fetch(backendUrl("/api/auth/logout"), {
      method: "POST",
      headers: { Cookie: `${REFRESH_COOKIE_NAME}=${encodeURIComponent(cookieValue)}` },
    }).catch(() => undefined);
  }

  const response = new NextResponse(null, { status: 204 });
  response.cookies.delete(REFRESH_COOKIE_NAME);
  return response;
}
