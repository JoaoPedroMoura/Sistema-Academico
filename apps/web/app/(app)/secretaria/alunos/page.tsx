"use client";

import { useState } from "react";
import { useAlunos } from "@/features/students/hooks/useStudents";
import { StudentTable } from "@/features/students/components/StudentTable";
import { StudentForm } from "@/features/students/components/StudentForm";
import { BackLink } from "@/shared/components/BackLink";
import { primaryButtonClass } from "@/shared/components/FormDialog";
import type { Aluno } from "@/features/students/types";

export default function AlunosPage() {
  const { data: alunos, isLoading } = useAlunos();
  // undefined = modal fechado; null = matriculando; Aluno = editando
  const [emEdicao, setEmEdicao] = useState<Aluno | null | undefined>(undefined);

  return (
    <div className="space-y-6 p-8">
      <BackLink href="/secretaria" label="Área da Secretaria" />
      <div className="flex items-center justify-between">
        <h1 className="font-[family-name:var(--font-display)] text-2xl font-semibold">Alunos</h1>
        <button type="button" onClick={() => setEmEdicao(null)} className={primaryButtonClass}>
          Matricular aluno
        </button>
      </div>
      {emEdicao !== undefined && (
        <StudentForm key={emEdicao?.id ?? "novo"} aluno={emEdicao} onClose={() => setEmEdicao(undefined)} />
      )}

      {isLoading ? (
        <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
      ) : (
        <StudentTable alunos={alunos ?? []} onEditar={setEmEdicao} />
      )}
    </div>
  );
}
