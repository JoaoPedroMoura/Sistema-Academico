"use client";

import { useMinhasPresencas } from "../hooks/useAttendance";

export function MinhasPresencasList() {
  const { data: presencas, isLoading } = useMinhasPresencas();

  if (isLoading) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>;
  }

  if (!presencas || presencas.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum registro de presença ainda.</p>;
  }

  const ordenadas = [...presencas].sort((a, b) => b.dataAula.localeCompare(a.dataAula));

  return (
    <ul className="space-y-2">
      {ordenadas.map((p) => (
        <li
          key={p.id}
          className="flex items-center justify-between rounded-md border border-[var(--color-border)] p-3 text-sm"
        >
          <div>
            <span className="font-medium">{p.materiaNome}</span>
            <span className="ml-2 text-[var(--color-muted-foreground)]">{p.dataAula}</span>
          </div>
          <div className="flex items-center gap-2">
            {p.justificativa && (
              <span className="text-xs text-[var(--color-muted-foreground)]">{p.justificativa}</span>
            )}
            <span
              className={
                p.presente
                  ? "font-medium text-[var(--color-success)]"
                  : "font-medium text-[var(--color-destructive)]"
              }
            >
              {p.presente ? "Presente" : "Falta"}
            </span>
          </div>
        </li>
      ))}
    </ul>
  );
}
