"use client";

import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { scheduleApi } from "../api/scheduleApi";

const GRADE_KEY = ["grade-ativa"];

function useAuth() {
  const { session, accessToken } = useSession();
  return { accessToken, tenantSlug: session?.tenantSlug ?? null };
}

export function useGradeAtiva() {
  const auth = useAuth();
  return useQuery({
    queryKey: GRADE_KEY,
    queryFn: () => scheduleApi.obterAtiva(auth),
    enabled: Boolean(auth.accessToken),
  });
}

export function useGerarGrade() {
  const auth = useAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => scheduleApi.gerar(auth),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: GRADE_KEY }),
  });
}
