"use client";

import Link from "next/link";
import { useSession } from "@/lib/auth/SessionProvider";
import { LogoutButton } from "@/shared/components/LogoutButton";

const LINKS = [
  { href: "/secretaria/solicitacoes", label: "Solicitações", description: "Triagem: aprovar, rejeitar, em análise" },
  { href: "/secretaria/alunos", label: "Alunos", description: "Matrícula e avanço de período" },
  { href: "/secretaria/materias", label: "Matérias", description: "Grade curricular e vínculo com professores" },
  { href: "/secretaria/grade", label: "Grade de Horário", description: "Gerar e visualizar" },
];

export default function SecretariaHomePage() {
  const { session } = useSession();

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">Área da Secretaria</h1>
          {session && (
            <p className="mt-1 text-xs text-[var(--color-muted-foreground)]">
              {session.name} · {session.tenantName} · {session.role}
            </p>
          )}
        </div>
        <LogoutButton />
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {LINKS.map((link) => (
          <Link
            key={link.href}
            href={link.href}
            className="rounded-lg border border-[var(--color-border)] p-4 hover:bg-[var(--color-muted)]"
          >
            <div className="font-medium">{link.label}</div>
            <div className="mt-1 text-sm text-[var(--color-muted-foreground)]">{link.description}</div>
          </Link>
        ))}
      </div>
    </div>
  );
}
