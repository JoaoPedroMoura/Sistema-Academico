"use client";

import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { useSession } from "@/lib/auth/SessionProvider";
import { httpClient } from "@/shared/api/httpClient";

const EMAIL_VALIDO = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

/**
 * Pré-checagem de email já cadastrado enquanto o usuário digita (espera 400ms de pausa).
 * Só um aviso: a API revalida na criação. `null` = ainda não sabemos (vazio, inválido, carregando).
 */
export function useEmailDisponivel(email: string, ativo: boolean): boolean | null {
  const { session, accessToken } = useSession();
  const [debounced, setDebounced] = useState(email);

  useEffect(() => {
    const timer = setTimeout(() => setDebounced(email.trim().toLowerCase()), 400);
    return () => clearTimeout(timer);
  }, [email]);

  const { data } = useQuery({
    queryKey: ["email-disponivel", debounced],
    queryFn: () =>
      httpClient.get<{ emailDisponivel: boolean }>(
        `/api/cadastro/disponibilidade?email=${encodeURIComponent(debounced)}`,
        { accessToken, tenantSlug: session?.tenantSlug ?? null },
      ),
    enabled: ativo && Boolean(accessToken) && EMAIL_VALIDO.test(debounced),
    staleTime: 30_000,
  });

  if (!ativo || debounced !== email.trim().toLowerCase()) return null;
  return data?.emailDisponivel ?? null;
}
