"use client";

import { useProfessor, useAdicionarDisponibilidade, useRemoverDisponibilidade } from "../hooks/useTeachers";
import { AvailabilityGrid } from "./AvailabilityGrid";

/** Disponibilidade de um professor editada pelo Admin (dentro do modal de professor). */
export function TeacherAvailabilityEditor({ professorId }: { professorId: string }) {
  const { data: professor } = useProfessor(professorId);
  const adicionar = useAdicionarDisponibilidade(professorId);
  const remover = useRemoverDisponibilidade(professorId);
  const erro = adicionar.error ?? remover.error;

  if (!professor) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>;
  }

  return (
    <div className="space-y-2">
      <h3 className="text-sm font-medium">Disponibilidade</h3>
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
