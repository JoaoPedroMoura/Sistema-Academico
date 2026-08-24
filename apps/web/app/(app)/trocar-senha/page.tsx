"use client";

import { useSessionBootstrap } from "@/lib/auth/useSessionBootstrap";
import { TrocarSenhaForm } from "@/lib/auth/TrocarSenhaForm";

/**
 * Página de troca de senha obrigatória (ARCHITECTURE.md §7.5) — qualquer papel autenticado pode
 * acessar, por isso não usa `RequireRole` (que redireciona para cá quando `precisaTrocarSenha` é
 * true, então redirecionar de novo aqui seria um loop). Só garante que existe uma sessão.
 */
export default function TrocarSenhaPage() {
  const { session, status } = useSessionBootstrap();

  if (status === "checking") {
    return (
      <div className="flex flex-1 items-center justify-center p-8 text-sm text-[var(--color-muted-foreground)]">
        Carregando sessão…
      </div>
    );
  }

  if (!session) {
    return null; // useSessionBootstrap já mandou pro /login
  }

  return (
    <main className="flex flex-1 items-center justify-center p-8">
      <div className="w-full max-w-sm rounded-lg border border-[var(--color-border)] p-8">
        <TrocarSenhaForm />
      </div>
    </main>
  );
}
