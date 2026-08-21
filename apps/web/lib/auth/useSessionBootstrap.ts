"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter, usePathname } from "next/navigation";
import { authApi } from "./authApi";
import { useSession } from "./SessionProvider";

/**
 * Restaura a sessão em memória a partir do cookie httpOnly (via POST /api/auth/refresh) quando a
 * página é recarregada — o access token vive só em memória (SessionProvider), então some a cada
 * reload; isso o repõe usando o refresh token que o browser já manda sozinho.
 */
export function useSessionBootstrap() {
  const { session, setSession } = useSession();
  const [refreshStatus, setRefreshStatus] = useState<"checking" | "ready">("checking");
  const tentativaFeita = useRef(false);
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    if (session || tentativaFeita.current) {
      return;
    }
    tentativaFeita.current = true;

    authApi
      .refresh()
      .then((data) => {
        if (!data.accessToken || !data.role || !data.tenantSlug || !data.accountId || !data.email || !data.nome || !data.tenantNome) {
          throw new Error("Sessão inválida.");
        }

        setSession(
          {
            accountId: data.accountId,
            name: data.nome,
            email: data.email,
            tenantSlug: data.tenantSlug,
            tenantName: data.tenantNome,
            role: data.role,
          },
          data.accessToken,
        );
        setRefreshStatus("ready");
      })
      .catch(() => {
        router.replace(`/login?redirectTo=${encodeURIComponent(pathname)}`);
      });
  }, [session, setSession, router, pathname]);

  return { session, status: session ? "ready" : refreshStatus } as const;
}
