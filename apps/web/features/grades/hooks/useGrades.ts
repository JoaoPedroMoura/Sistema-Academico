"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { gradesApi } from "../api/gradesApi";
import type { LancarNotaInput } from "../types";

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

function key(turmaId: string) {
  return ["notas", turmaId];
}

export function useNotasPorTurma(turmaId: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: key(turmaId),
    queryFn: () => gradesApi.listarPorTurma(auth, turmaId),
    enabled: Boolean(auth.accessToken) && Boolean(turmaId),
  });
}

export function useLancarNota(turmaId: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: LancarNotaInput) => gradesApi.lancar(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: key(turmaId) }),
  });
}

export function useRevisarNota(turmaId: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, novoValor }: { id: string; novoValor: number }) => gradesApi.revisar(auth, id, novoValor),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: key(turmaId) }),
  });
}

export function useMinhasNotas() {
  const auth = useAuth();
  return useQuery({
    queryKey: ["notas", "minhas"],
    queryFn: () => gradesApi.listarMinhas(auth),
    enabled: Boolean(auth.accessToken),
  });
}
