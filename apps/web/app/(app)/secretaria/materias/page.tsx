"use client";

import { useMaterias, useExcluirMateria } from "@/features/subjects/hooks/useSubjects";
import { SubjectTable } from "@/features/subjects/components/SubjectTable";
import { SubjectForm } from "@/features/subjects/components/SubjectForm";
import { VinculoManager } from "@/features/subjects/components/VinculoManager";
import { BackLink } from "@/shared/components/BackLink";

export default function SecretariaMateriasPage() {
  const { data: materias, isLoading } = useMaterias();
  const excluir = useExcluirMateria();

  return (
    <div className="space-y-6 p-8">
      <BackLink href="/secretaria" label="Área da Secretaria" />
      <h1 className="text-xl font-semibold">Matérias</h1>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="space-y-4">
          {isLoading ? (
            <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
          ) : (
            <SubjectTable materias={materias ?? []} onExcluir={(id) => excluir.mutate(id)} excluindo={excluir.isPending} />
          )}
          {excluir.isError && <p className="text-sm text-[var(--color-destructive)]">{excluir.error.message}</p>}
        </div>

        <div className="space-y-4">
          <SubjectForm />
          <VinculoManager />
        </div>
      </div>
    </div>
  );
}
