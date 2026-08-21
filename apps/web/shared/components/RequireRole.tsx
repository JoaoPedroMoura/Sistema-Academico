"use client";

import { useEffect, type ReactNode } from "react";
import { useRouter } from "next/navigation";
import { useSessionBootstrap } from "@/lib/auth/useSessionBootstrap";
import { roleHomePath } from "@/lib/auth/roleRouting";
import type { Role } from "@/lib/auth/types";

interface RequireRoleProps {
  role: Role;
  children: ReactNode;
}

/**
 * Guarda de papel do lado do client, usada no layout de cada área
 * (`app/(app)/{admin,secretaria,professor,aluno}/layout.tsx`). O proxy.ts (edge) já bloqueia
 * quem não tem sessão nenhuma antes de chegar aqui — isto cobre o caso mais fino: sessão válida,
 * mas de outro papel (ex. Professor tentando abrir /admin), redirecionando para a área certa
 * dele em vez de mostrar erro.
 */
export function RequireRole({ role, children }: RequireRoleProps) {
  const { session, status } = useSessionBootstrap();
  const router = useRouter();

  useEffect(() => {
    if (status === "ready" && session && session.role !== role) {
      router.replace(roleHomePath(session.role));
    }
  }, [status, session, role, router]);

  if (status === "checking") {
    return (
      <div className="flex flex-1 items-center justify-center p-8 text-sm text-[var(--color-muted-foreground)]">
        Carregando sessão…
      </div>
    );
  }

  if (!session || session.role !== role) {
    return null; // redirecionando (useEffect acima) ou useSessionBootstrap já mandou pro /login
  }

  return <>{children}</>;
}
