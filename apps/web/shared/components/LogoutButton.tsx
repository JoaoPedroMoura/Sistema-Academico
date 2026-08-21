"use client";

import { useLogout } from "@/lib/auth/useLogout";

export function LogoutButton() {
  const logout = useLogout();

  return (
    <button
      type="button"
      onClick={() => logout.mutate()}
      disabled={logout.isPending}
      className="text-sm text-[var(--color-muted-foreground)] hover:text-[var(--color-foreground)] disabled:opacity-50"
    >
      {logout.isPending ? "Saindo…" : "Sair"}
    </button>
  );
}
