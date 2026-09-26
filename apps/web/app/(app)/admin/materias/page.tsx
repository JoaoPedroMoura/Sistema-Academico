"use client";

import { useState } from "react";
import { useMaterias, useExcluirMateria } from "@/features/subjects/hooks/useSubjects";
import { SubjectTable } from "@/features/subjects/components/SubjectTable";
import { SubjectForm } from "@/features/subjects/components/SubjectForm";
import { VinculoManager } from "@/features/subjects/components/VinculoManager";
import { BackLink } from "@/shared/components/BackLink";
import { primaryButtonClass } from "@/shared/components/FormDialog";
import type { Materia } from "@/features/subjects/types";

export default function MateriasPage() {
  const { data: materias, isLoading } = useMaterias();
  const excluir = useExcluirMateria();
  // undefined = modal fechado; null = criando; Materia = editando
  const [emEdicao, setEmEdicao] = useState<Materia | null | undefined>(undefined);

  return (
    <div className="space-y-6 p-8">
      <BackLink href="/admin" label="Área do Admin" />
      <div className="flex items-center justify-between">
        <h1 className="font-[family-name:var(--font-display)] text-2xl font-semibold">Matérias</h1>
        <button type="button" onClick={() => setEmEdicao(null)} className={primaryButtonClass}>
          Adicionar matéria
        </button>
      </div>
      {emEdicao !== undefined && (
        <SubjectForm key={emEdicao?.id ?? "nova"} materia={emEdicao} onClose={() => setEmEdicao(undefined)} />
      )}

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="space-y-4">
          {isLoading ? (
            <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
          ) : (
            <SubjectTable materias={materias ?? []} onEditar={setEmEdicao} onExcluir={(id) => excluir.mutate(id)} excluindo={excluir.isPending} />
          )}
          {excluir.isError && <p className="text-sm text-[var(--color-destructive)]">{excluir.error.message}</p>}
        </div>

        <div className="space-y-4">
          <VinculoManager />
        </div>
      </div>
    </div>
  );
}
