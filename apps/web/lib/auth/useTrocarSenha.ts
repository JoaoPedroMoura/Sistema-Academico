"use client";

import { useMutation } from "@tanstack/react-query";
import { authApi } from "./authApi";
import { useSession } from "./SessionProvider";

interface TrocarSenhaInput {
  senhaAtual: string;
  novaSenha: string;
}

/**
 * Troca de senha self-service — vai direto na API .NET (Bearer token da sessão em memória), não
 * passa pelo BFF (não mexe no cookie de refresh). Em caso de sucesso, atualiza a sessão local
 * (`precisaTrocarSenha: false`) para o `RequireRole` parar de redirecionar pra cá.
 */
export function useTrocarSenha() {
  const { session, accessToken, setSession } = useSession();

  return useMutation<void, Error, TrocarSenhaInput>({
    mutationFn: ({ senhaAtual, novaSenha }) => {
      if (!accessToken || !session) {
        throw new Error("Sessão não encontrada.");
      }
      return authApi.trocarSenha(accessToken, session.tenantSlug, senhaAtual, novaSenha);
    },
    onSuccess: () => {
      if (session) {
        setSession({ ...session, precisaTrocarSenha: false }, accessToken);
      }
    },
  });
}
