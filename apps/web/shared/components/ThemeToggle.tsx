"use client";

import { useEffect, useState } from "react";

type Theme = "light" | "dark";

/** Alterna claro/escuro via data-theme no <html>; a escolha fica no localStorage (lida no layout antes de pintar). */
export function ThemeToggle() {
  const [theme, setTheme] = useState<Theme | null>(null);

  useEffect(() => {
    const salvo = document.documentElement.dataset.theme as Theme | undefined;
    setTheme(salvo ?? (matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light"));
  }, []);

  function alternar() {
    const novo: Theme = theme === "dark" ? "light" : "dark";
    document.documentElement.dataset.theme = novo;
    try {
      localStorage.setItem("theme", novo);
    } catch {}
    setTheme(novo);
  }

  const label = theme === "dark" ? "Usar tema claro" : "Usar tema escuro";

  return (
    <button
      type="button"
      onClick={alternar}
      aria-label={label}
      title={label}
      className="rounded-md p-2 text-[var(--color-muted-foreground)] hover:bg-[var(--color-muted)] hover:text-[var(--color-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
    >
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
        {theme === "dark" ? (
          <>
            <circle cx="12" cy="12" r="4" />
            <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41" />
          </>
        ) : (
          <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
        )}
      </svg>
    </button>
  );
}
