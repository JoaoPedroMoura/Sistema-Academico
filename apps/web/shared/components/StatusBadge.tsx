const STATUS = {
  Aberta: { label: "Aberta", className: "bg-[var(--color-warning-soft)] text-[var(--color-warning)]" },
  EmAnalise: { label: "Em análise", className: "bg-[var(--color-secondary)] text-[var(--color-primary)]" },
  Aprovada: { label: "Aprovada", className: "bg-[var(--color-success-soft)] text-[var(--color-success)]" },
  Rejeitada: { label: "Rejeitada", className: "bg-[var(--color-destructive-soft)] text-[var(--color-destructive)]" },
};

/** Pílula de status de solicitação: fundo *-soft da cor, texto e ponto na cor cheia. */
export function StatusBadge({ status }: { status: keyof typeof STATUS }) {
  const { label, className } = STATUS[status];
  return (
    <span
      className={`inline-flex h-[22px] items-center gap-1.5 rounded-[var(--radius-full)] px-2 text-xs font-medium ${className}`}
    >
      <span className="size-1.5 rounded-[var(--radius-full)] bg-current" aria-hidden />
      {label}
    </span>
  );
}
