"use client";

import Link from "next/link";
import { useSession } from "@/lib/auth/SessionProvider";
import { LogoutButton } from "@/shared/components/LogoutButton";

const LINKS = [
  { href: "/admin/professores", label: "Professores", description: "Cadastro e disponibilidade" },
  { href: "/admin/materias", label: "Matérias", description: "Cadastro e vínculo com professores" },
  { href: "/admin/grade", label: "Grade de Horário", description: "Gerar e visualizar (motor GRASP)" },
];

export default function AdminHomePage() {
  const { session } = useSession();

  return (
    <div className="p-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold">Área do Admin</h1>
          {session && (
            <p className="mt-1 text-xs text-[var(--color-muted-foreground)]">
              {session.name} · {session.tenantName} · {session.role}
            </p>
          )}
        </div>
        <LogoutButton />
      </div>

      <div className="grid gap-4 sm:grid-cols-3">
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

      <p className="mt-6 text-sm text-[var(--color-muted-foreground)]">
        Funcionários, usuários e vínculo matéria-professor detalhado: em construção.
      </p>
    </div>
  );
}
