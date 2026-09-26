"use client";

/** Alterna claro/escuro via data-theme no <html>; a escolha fica no localStorage (lida no layout antes de pintar). */
export function ThemeToggle() {
  function alternar() {
    const root = document.documentElement;
    const atual = root.dataset.theme ?? (matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
    const novo = atual === "dark" ? "light" : "dark";
    root.dataset.theme = novo;
    try {
      localStorage.setItem("theme", novo);
    } catch {}
  }

  return (
    <button
      type="button"
      onClick={alternar}
      aria-label="Alternar tema claro/escuro"
      title="Alternar tema"
      className="rounded-md p-2 text-[var(--color-muted-foreground)] hover:bg-[var(--color-muted)] hover:text-[var(--color-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
    >
      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
        {/* Lua no claro, sol no escuro — troca via CSS, sem estado. */}
        <path className="dark:hidden" d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
        <g className="hidden dark:inline">
          <circle cx="12" cy="12" r="4" />
          <path d="M12 2v2M12 20v2M4.93 4.93l1.41 1.41M17.66 17.66l1.41 1.41M2 12h2M20 12h2M6.34 17.66l-1.41 1.41M19.07 4.93l-1.41 1.41" />
        </g>
      </svg>
    </button>
  );
}
