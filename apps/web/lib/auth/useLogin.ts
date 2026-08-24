"use client";

import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import { authApi, type LoginPayload, type LoginResponse } from "./authApi";
import { useSession } from "./SessionProvider";
import { roleHomePath } from "./roleRouting";

/**
 * Mutation de login. Em caso de sucesso completo (não precisa escolher tenant), já guarda a
 * sessão em memória e redireciona para a área do papel. Quando `requiresTenantSelection` vem
 * true, quem chamou o hook decide o que fazer (mostrar a lista de unidades) — não há redirect
 * automático nesse caso.
 */
export function useLogin() {
  const { setSession } = useSession();
  const router = useRouter();

  return useMutation<LoginResponse, Error, LoginPayload>({
    mutationFn: (payload) => authApi.login(payload),
    onSuccess: (data) => {
      if (data.requiresTenantSelection) {
        return;
      }
      if (!data.accessToken || !data.role || !data.tenantSlug || !data.accountId || !data.email || !data.nome || !data.tenantNome) {
        return;
      }

      const precisaTrocarSenha = data.precisaTrocarSenha ?? false;

      setSession(
        {
          accountId: data.accountId,
          name: data.nome,
          email: data.email,
          tenantSlug: data.tenantSlug,
          tenantName: data.tenantNome,
          role: data.role,
          precisaTrocarSenha,
        },
        data.accessToken,
      );

      router.push(precisaTrocarSenha ? "/trocar-senha" : roleHomePath(data.role));
    },
  });
}
