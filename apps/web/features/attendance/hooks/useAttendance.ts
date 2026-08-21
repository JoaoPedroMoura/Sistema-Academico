"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { attendanceApi } from "../api/attendanceApi";
import type { RegistroPresencaInput } from "../types";

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

function key(turmaId: string, data: string) {
  return ["presencas", turmaId, data];
}

export function usePresencas(turmaId: string, data: string) {
  const auth = useAuth();
  return useQuery({
    queryKey: key(turmaId, data),
    queryFn: () => attendanceApi.listar(auth, turmaId, data),
    enabled: Boolean(auth.accessToken) && Boolean(turmaId) && Boolean(data),
  });
}

export function useRegistrarPresenca(turmaId: string, data: string) {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (registros: RegistroPresencaInput[]) => attendanceApi.registrar(auth, turmaId, data, registros),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: key(turmaId, data) }),
  });
}

export function useMinhasPresencas() {
  const auth = useAuth();
  return useQuery({
    queryKey: ["presencas", "minhas"],
    queryFn: () => attendanceApi.listarMinhas(auth),
    enabled: Boolean(auth.accessToken),
  });
}
