import type { ReactNode } from "react";

/** Rótulo pequeno + controle, para os formulários de cadastro. */
export function Campo({ label, children, className = "" }: { label: string; children: ReactNode; className?: string }) {
  return (
    <label className={`block space-y-1 ${className}`}>
      <span className="text-xs text-[var(--color-muted-foreground)]">{label}</span>
      {children}
    </label>
  );
}

/** Seção recolhível (<details> nativo) dentro do modal. */
export function Secao({ titulo, children }: { titulo: string; children: ReactNode }) {
  return (
    <details className="rounded-md border border-[var(--color-border)] px-3 py-2 [&[open]>summary]:mb-3">
      <summary className="cursor-pointer text-sm font-medium">{titulo}</summary>
      <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">{children}</div>
    </details>
  );
}
