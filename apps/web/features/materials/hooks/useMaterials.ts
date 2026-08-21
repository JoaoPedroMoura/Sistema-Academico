"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { materialsApi } from "../api/materialsApi";
import type { EnviarMaterialInput } from "../types";

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

function key(turmaId: string) {
  return ["materiais", turmaId];
}

export function useMateriaisPorTurma(turmaId: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: key(turmaId),
    queryFn: () => materialsApi.listarPorTurma(auth, turmaId),
    enabled: Boolean(auth.accessToken) && Boolean(turmaId),
  });
}

export function useEnviarMaterial(turmaId: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: EnviarMaterialInput) => materialsApi.enviar(auth, input),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: key(turmaId) }),
  });
}

export function useMeusMateriais() {
  const auth = useAuth();
  return useQuery({
    queryKey: ["materiais", "meus"],
    queryFn: () => materialsApi.listarMeus(auth),
    enabled: Boolean(auth.accessToken),
  });
}
