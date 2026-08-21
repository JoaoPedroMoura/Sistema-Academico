"use client";

import Link from "next/link";
import { useSession } from "@/lib/auth/SessionProvider";
import { LogoutButton } from "@/shared/components/LogoutButton";
import { useMeuPerfilAluno } from "@/features/students/hooks/useStudents";

const AREAS = [
  { href: "/aluno/notas", titulo: "Notas", descricao: "Consulte suas notas por matéria." },
  { href: "/aluno/presencas", titulo: "Presença", descricao: "Acompanhe suas faltas e presenças." },
  { href: "/aluno/materiais", titulo: "Materiais", descricao: "Baixe materiais complementares das suas turmas." },
  { href: "/aluno/solicitacoes", titulo: "Solicitações", descricao: "Abra e acompanhe solicitações à secretaria." },
];

export default function AlunoHomePage() {
  const { session } = useSession();
  const { data: perfil } = useMeuPerfilAluno();

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">Área do Aluno</h1>
          {session && (
            <p className="mt-1 text-xs text-[var(--color-muted-foreground)]">
              {session.name} · {session.tenantName} · {session.role}
            </p>
          )}
          {perfil && (
            <p className="mt-1 text-xs text-[var(--color-muted-foreground)]">
              Matrícula {perfil.matricula} · {perfil.periodoAtual}º período
            </p>
          )}
        </div>
        <LogoutButton />
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {AREAS.map((a) => (
          <Link
            key={a.href}
            href={a.href}
            className="rounded-lg border border-[var(--color-border)] p-4 hover:bg-[var(--color-muted)]"
          >
            <div className="font-medium">{a.titulo}</div>
            <div className="mt-1 text-sm text-[var(--color-muted-foreground)]">{a.descricao}</div>
          </Link>
        ))}
      </div>
    </div>
  );
}
