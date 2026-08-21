"use client";

import Link from "next/link";
import { useSession } from "@/lib/auth/SessionProvider";
import { LogoutButton } from "@/shared/components/LogoutButton";
import { useMinhasTurmas } from "@/features/teachers/hooks/useTeachers";

export default function ProfessorHomePage() {
  const { session } = useSession();
  const { data: turmas, isLoading } = useMinhasTurmas();

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">Área do Professor</h1>
          {session && (
            <p className="mt-1 text-xs text-[var(--color-muted-foreground)]">
              {session.name} · {session.tenantName} · {session.role}
            </p>
          )}
        </div>
        <LogoutButton />
      </div>

      <div className="mb-4 flex items-center justify-between">
        <h2 className="text-sm font-medium text-[var(--color-muted-foreground)]">Minhas turmas</h2>
        <Link href="/professor/disponibilidade" className="text-sm text-[var(--color-primary)] hover:underline">
          Minha disponibilidade
        </Link>
      </div>

      {isLoading ? (
        <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
      ) : turmas && turmas.length > 0 ? (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {turmas.map((t) => (
            <Link
              key={t.id}
              href={`/professor/turmas/${t.id}`}
              className="rounded-lg border border-[var(--color-border)] p-4 hover:bg-[var(--color-muted)]"
            >
              <div className="font-medium">{t.materiaNome}</div>
              <div className="mt-1 text-sm text-[var(--color-muted-foreground)]">
                {t.dia} · {t.horaInicio}–{t.horaFim} · {t.periodoCurricular}º período
              </div>
            </Link>
          ))}
        </div>
      ) : (
        <p className="text-sm text-[var(--color-muted-foreground)]">
          Nenhuma turma na grade ativa. Confirme sua disponibilidade — sem ela o motor não consegue te alocar.
        </p>
      )}
    </div>
  );
}
