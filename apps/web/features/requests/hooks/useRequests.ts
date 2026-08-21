"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { requestsApi } from "../api/requestsApi";
import type { AbrirSolicitacaoInput, StatusSolicitacao } from "../types";

const SOLICITACOES_KEY = ["solicitacoes"];
const MINHAS_SOLICITACOES_KEY = ["solicitacoes", "minhas"];

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

export function useSolicitacoes(status?: StatusSolicitacao) {
  const auth = useAuth();
  return useQuery({
    queryKey: [...SOLICITACOES_KEY, status ?? "todas"],
    queryFn: () => requestsApi.listar(auth, status),
    enabled: Boolean(auth.accessToken),
  });
}

export function useMarcarEmAnalise() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => requestsApi.marcarEmAnalise(auth, id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: SOLICITACOES_KEY }),
  });
}

export function useAprovarSolicitacao() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, resposta }: { id: string; resposta?: string }) => requestsApi.aprovar(auth, id, resposta),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: SOLICITACOES_KEY }),
  });
}

export function useRejeitarSolicitacao() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, resposta }: { id: string; resposta: string }) => requestsApi.rejeitar(auth, id, resposta),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: SOLICITACOES_KEY }),
  });
}

export function useMinhasSolicitacoes() {
  const auth = useAuth();
  return useQuery({
    queryKey: MINHAS_SOLICITACOES_KEY,
    queryFn: () => requestsApi.listarMinhas(auth),
    enabled: Boolean(auth.accessToken),
  });
}

export function useAbrirSolicitacao() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: AbrirSolicitacaoInput) => requestsApi.abrir(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: MINHAS_SOLICITACOES_KEY }),
  });
}
