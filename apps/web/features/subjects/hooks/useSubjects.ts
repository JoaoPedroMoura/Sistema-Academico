"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { subjectsApi } from "../api/subjectsApi";
import type { AtualizarMateriaInput, CriarMateriaInput } from "../types";

const MATERIAS_KEY = ["materias"];
const VINCULOS_KEY = ["vinculos"];

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

export function useMaterias(pesquisa?: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: [...MATERIAS_KEY, pesquisa ?? ""],
    queryFn: () => subjectsApi.listar(auth, pesquisa),
    enabled: Boolean(auth.accessToken),
  });
}

export function useCriarMateria() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: CriarMateriaInput) => subjectsApi.criar(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: MATERIAS_KEY }),
  });
}

export function useAtualizarMateria(id: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: AtualizarMateriaInput) => subjectsApi.atualizar(auth, id, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: MATERIAS_KEY }),
  });
}

export function useExcluirMateria() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => subjectsApi.excluir(auth, id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: MATERIAS_KEY }),
  });
}

export function useVinculos() {
  const auth = useAuth();
  return useQuery({
    queryKey: VINCULOS_KEY,
    queryFn: () => subjectsApi.listarVinculos(auth),
    enabled: Boolean(auth.accessToken),
  });
}

export function useAdicionarVinculo() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ materiaId, professorId }: { materiaId: string; professorId: string }) =>
      subjectsApi.adicionarVinculo(auth, materiaId, professorId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: VINCULOS_KEY }),
  });
}

export function useRemoverVinculo() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ materiaId, professorId }: { materiaId: string; professorId: string }) =>
      subjectsApi.removerVinculo(auth, materiaId, professorId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: VINCULOS_KEY }),
  });
}
