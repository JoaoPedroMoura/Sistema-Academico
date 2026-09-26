"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { documentsApi } from "../api/documentsApi";
import type { TipoDocumento } from "../types";

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

export function useDocumentosAnexos(pessoaId: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: ["documentos", pessoaId],
    queryFn: () => documentsApi.listar(auth, pessoaId),
    enabled: Boolean(auth.accessToken),
  });
}

export function useEnviarDocumento(pessoaId: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ tipo, arquivo }: { tipo: TipoDocumento; arquivo: File }) =>
      documentsApi.enviar(auth, pessoaId, tipo, arquivo),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["documentos", pessoaId] }),
  });
}

export function useRemoverDocumento(pessoaId: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => documentsApi.remover(auth, id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["documentos", pessoaId] }),
  });
}

/** Baixa o arquivo pela API (precisa do token) e abre em nova aba. */
export function useBaixarDocumento() {
  const auth = useAuth();
  return async (id: string) => {
    const blob = await documentsApi.baixar(auth, id);
    const url = URL.createObjectURL(blob);
    window.open(url, "_blank", "noopener");
    setTimeout(() => URL.revokeObjectURL(url), 60_000);
  };
}
