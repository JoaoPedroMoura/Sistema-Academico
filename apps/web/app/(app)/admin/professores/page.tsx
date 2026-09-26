"use client";

import { useState } from "react";
import { useProfessores, useExcluirProfessor } from "@/features/teachers/hooks/useTeachers";
import { TeacherTable } from "@/features/teachers/components/TeacherTable";
import { TeacherForm } from "@/features/teachers/components/TeacherForm";
import { BackLink } from "@/shared/components/BackLink";
import { primaryButtonClass } from "@/shared/components/FormDialog";
import type { Professor } from "@/features/teachers/types";

export default function ProfessoresPage() {
  const { data: professores, isLoading } = useProfessores();
  const excluir = useExcluirProfessor();
  // undefined = modal fechado; null = criando; Professor = editando
  const [emEdicao, setEmEdicao] = useState<Professor | null | undefined>(undefined);

  return (
    <div className="space-y-6 p-8">
      <BackLink href="/admin" label="Área do Admin" />
      <div className="flex items-center justify-between">
        <h1 className="font-[family-name:var(--font-display)] text-2xl font-semibold">Professores</h1>
        <button type="button" onClick={() => setEmEdicao(null)} className={primaryButtonClass}>
          Adicionar professor
        </button>
      </div>
      {emEdicao !== undefined && (
        <TeacherForm key={emEdicao?.id ?? "novo"} professor={emEdicao} onClose={() => setEmEdicao(undefined)} />
      )}

      <div className="space-y-4">
          {isLoading ? (
            <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
          ) : (
            <TeacherTable
              professores={professores ?? []}
              onEditar={setEmEdicao}
              onExcluir={(id) => excluir.mutate(id)}
              excluindo={excluir.isPending}
            />
          )}
          {excluir.isError && <p className="text-sm text-[var(--color-destructive)]">{excluir.error.message}</p>}
      </div>
    </div>
  );
}
