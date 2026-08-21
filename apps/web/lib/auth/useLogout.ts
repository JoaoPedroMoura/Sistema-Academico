"use client";

import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { authApi } from "./authApi";
import { useSession } from "./SessionProvider";

export function useLogout() {
  const { setSession } = useSession();
  const router = useRouter();

  return useMutation({
    mutationFn: () => authApi.logout(),
    onSettled: () => {
      // Limpa a sessão local mesmo se a chamada falhar (ex. API fora do ar) — não faz sentido
      // manter alguém "logado" na UI se o logout não pôde ser confirmado.
      setSession(null, null);
      router.push("/login");
    },
  });
}
