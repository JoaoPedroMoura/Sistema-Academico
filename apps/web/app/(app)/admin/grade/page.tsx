"use client";

import { useGradeAtiva, useGerarGrade, useExcluirGrade } from "@/features/schedule/hooks/useSchedule";
import { GradeView } from "@/features/schedule/components/GradeView";
import { BackLink } from "@/shared/components/BackLink";

export default function GradePage() {
  const { data: grade, isLoading } = useGradeAtiva();
  const gerar = useGerarGrade();
  const excluir = useExcluirGrade();

  return (
    <div className="space-y-6 p-8">
      <BackLink href="/admin" label="Área do Admin" />

      <div className="flex items-center justify-between">
        <h1 className="text-xl font-semibold">Grade de Horário</h1>
        <div className="flex gap-2">
          {grade && (
            <button
              type="button"
              onClick={() => {
                if (window.confirm("Excluir esta grade? Isso remove todas as turmas alocadas nela.")) {
                  excluir.mutate(grade.id);
                }
              }}
              disabled={excluir.isPending}
              className="rounded-md border border-[var(--color-destructive)] px-4 py-2 text-sm font-medium text-[var(--color-destructive)] hover:bg-[var(--color-muted)] disabled:opacity-50"
            >
              {excluir.isPending ? "Excluindo…" : "Excluir grade"}
            </button>
          )}
          <button
            type="button"
            onClick={() => gerar.mutate()}
            disabled={gerar.isPending}
            className="rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
          >
            {gerar.isPending ? "Gerando…" : "Gerar nova grade"}
          </button>
        </div>
      </div>

      {gerar.isError && <p className="text-sm text-[var(--color-destructive)]">{gerar.error.message}</p>}
      {excluir.isError && <p className="text-sm text-[var(--color-destructive)]">{excluir.error.message}</p>}

      {gerar.data && !gerar.data.completa && (
        <div className="rounded-md border border-[var(--color-warning)] bg-[var(--color-muted)] p-3 text-sm">
          Grade gerada como rascunho — matérias não alocadas: {gerar.data.materiasNaoAlocadas.join(", ")}.
          Ajuste disponibilidade/vínculos e gere de novo.
        </div>
      )}

      {isLoading ? (
        <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
      ) : grade ? (
        <GradeView grade={grade} />
      ) : (
        <p className="text-sm text-[var(--color-muted-foreground)]">
          Nenhuma grade publicada ainda. Cadastre professores, matérias, disponibilidade e vínculos, depois gere a grade.
        </p>
      )}
    </div>
  );
}
