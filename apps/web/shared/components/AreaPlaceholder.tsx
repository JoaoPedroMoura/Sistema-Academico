"use client";

import { useSession } from "@/lib/auth/SessionProvider";
import { LogoutButton } from "./LogoutButton";

interface AreaPlaceholderProps {
  title: string;
  description: string;
}

/**
 * Placeholder genérico usado pelas páginas de área (admin/secretaria/professor/aluno) enquanto
 * as telas reais de cada feature não são implementadas (Fase 7). Substituído incrementalmente,
 * uma feature por vez. Mostra a sessão atual + logout — útil para validar o fluxo de auth
 * (Fase 6) visualmente antes das telas reais existirem.
 */
export function AreaPlaceholder({ title, description }: AreaPlaceholderProps) {
  const { session } = useSession();

  return (
    <div className="rounded-lg border border-[var(--color-border)] p-8">
      <div className="mb-4 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">{title}</h1>
          {session && (
            <p className="mt-1 text-xs text-[var(--color-muted-foreground)]">
              {session.name} · {session.tenantName} · {session.role}
            </p>
          )}
        </div>
        <LogoutButton />
      </div>
      <p className="text-sm text-[var(--color-muted-foreground)]">{description}</p>
    </div>
  );
}
