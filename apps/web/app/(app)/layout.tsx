"use client";

import Link from "next/link";
import { useSession } from "@/lib/auth/SessionProvider";
import { roleHomePath } from "@/lib/auth/roleRouting";
import { LogoutButton } from "@/shared/components/LogoutButton";
import { ThemeToggle } from "@/shared/components/ThemeToggle";

/** Cabeçalho comum a todas as áreas logadas: marca, quem está logado, tema e sair. */
export default function AppLayout({ children }: { children: React.ReactNode }) {
  const { session } = useSession();

  return (
    <>
      <header className="flex items-center justify-between gap-4 border-b border-[var(--color-border)] bg-[var(--color-surface)] px-8 py-3">
        <Link
          href={session ? roleHomePath(session.role) : "/login"}
          className="font-[family-name:var(--font-display)] font-semibold focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
        >
          Sistema Acadêmico Faeterj
        </Link>
        <div className="flex items-center gap-4">
          {session && (
            <span className="hidden text-xs text-[var(--color-muted-foreground)] sm:inline">
              {session.name} · {session.tenantName} · {session.role}
            </span>
          )}
          <ThemeToggle />
          {session && <LogoutButton />}
        </div>
      </header>
      {children}
    </>
  );
}
