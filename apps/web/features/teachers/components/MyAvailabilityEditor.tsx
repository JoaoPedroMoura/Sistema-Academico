"use client";

import { useMeuPerfil, useAdicionarMinhaDisponibilidade, useRemoverMinhaDisponibilidade } from "../hooks/useTeachers";
import { AvailabilityGrid } from "./AvailabilityGrid";

/** Tela self-service do professor (ANALISE-TCC.md §6 — evolução em relação ao TCC original). */
export function MyAvailabilityEditor() {
  const { data: professor } = useMeuPerfil();
  const adicionar = useAdicionarMinhaDisponibilidade();
  const remover = useRemoverMinhaDisponibilidade();
  const erro = adicionar.error ?? remover.error;

  if (!professor) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>;
  }

  return (
    <div className="space-y-3 rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)] p-4">
      <h2 className="text-sm font-medium">Minha disponibilidade</h2>
      <p className="text-sm text-[var(--color-muted-foreground)]">Clique nos horários em que você pode dar aula.</p>
      <AvailabilityGrid
        disponibilidades={professor.disponibilidades}
        onAdicionar={(input) => adicionar.mutate(input)}
        onRemover={(id) => remover.mutate(id)}
        pendente={adicionar.isPending || remover.isPending}
      />
      {erro && <p className="text-sm text-[var(--color-destructive)]">{erro.message}</p>}
    </div>
  );
}
