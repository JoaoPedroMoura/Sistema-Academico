"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { teachersApi } from "../api/teachersApi";
import type {
  AdicionarDisponibilidadeInput,
  AtualizarProfessorInput,
  CriarProfessorInput,
} from "../types";

const QUERY_KEY = ["professores"];

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

export function useProfessores(pesquisa?: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: [...QUERY_KEY, pesquisa ?? ""],
    queryFn: () => teachersApi.listar(auth, pesquisa),
    enabled: Boolean(auth.accessToken),
  });
}

export function useProfessor(id: string | null) {
  const auth = useAuth();
  return useQuery({
    queryKey: [...QUERY_KEY, id],
    queryFn: () => teachersApi.obter(auth, id!),
    enabled: Boolean(auth.accessToken) && Boolean(id),
  });
}

export function useCriarProfessor() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: CriarProfessorInput) => teachersApi.criar(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  });
}

export function useAtualizarProfessor(id: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: AtualizarProfessorInput) => teachersApi.atualizar(auth, id, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  });
}

export function useExcluirProfessor() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => teachersApi.excluir(auth, id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  });
}

export function useAdicionarDisponibilidade(id: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: AdicionarDisponibilidadeInput) => teachersApi.adicionarDisponibilidade(auth, id, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  });
}

export function useRemoverDisponibilidade(id: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (disponibilidadeId: string) => teachersApi.removerDisponibilidade(auth, id, disponibilidadeId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: QUERY_KEY }),
  });
}

// --- Self-service do professor autenticado (área Professor) ---

const MEU_PERFIL_KEY = ["meu-perfil-professor"];
const MINHAS_TURMAS_KEY = ["minhas-turmas"];

export function useMeuPerfil() {
  const auth = useAuth();
  return useQuery({
    queryKey: MEU_PERFIL_KEY,
    queryFn: () => teachersApi.meuPerfil(auth),
    enabled: Boolean(auth.accessToken),
  });
}

export function useMinhasTurmas() {
  const auth = useAuth();
  return useQuery({
    queryKey: MINHAS_TURMAS_KEY,
    queryFn: () => teachersApi.minhasTurmas(auth),
    enabled: Boolean(auth.accessToken),
  });
}

export function useAlunosDaTurma(turmaId: string | null) {
  const auth = useAuth();
  return useQuery({
    queryKey: ["alunos-da-turma", turmaId],
    queryFn: () => teachersApi.alunosDaTurma(auth, turmaId!),
    enabled: Boolean(auth.accessToken) && Boolean(turmaId),
  });
}

export function useAdicionarMinhaDisponibilidade() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: AdicionarDisponibilidadeInput) => teachersApi.adicionarMinhaDisponibilidade(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: MEU_PERFIL_KEY }),
  });
}

export function useRemoverMinhaDisponibilidade() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (disponibilidadeId: string) => teachersApi.removerMinhaDisponibilidade(auth, disponibilidadeId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: MEU_PERFIL_KEY }),
  });
}
