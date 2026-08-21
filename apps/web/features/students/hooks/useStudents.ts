"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { studentsApi } from "../api/studentsApi";
import type { MatricularAlunoInput } from "../types";

const ALUNOS_KEY = ["alunos"];

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

export function useAlunos(pesquisa?: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: [...ALUNOS_KEY, pesquisa ?? ""],
    queryFn: () => studentsApi.listar(auth, pesquisa),
    enabled: Boolean(auth.accessToken),
  });
}

export function useMatricularAluno() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: MatricularAlunoInput) => studentsApi.matricular(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ALUNOS_KEY }),
  });
}

export function useAvancarPeriodo() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, novoPeriodo }: { id: string; novoPeriodo: number }) =>
      studentsApi.avancarPeriodo(auth, id, novoPeriodo),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ALUNOS_KEY }),
  });
}

export function useMeuPerfilAluno() {
  const auth = useAuth();
  return useQuery({
    queryKey: ["alunos", "me"],
    queryFn: () => studentsApi.meuPerfil(auth),
    enabled: Boolean(auth.accessToken),
  });
}
