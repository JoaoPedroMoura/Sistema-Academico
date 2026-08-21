"use client";

import { useAlunos } from "@/features/students/hooks/useStudents";
import { StudentTable } from "@/features/students/components/StudentTable";
import { StudentForm } from "@/features/students/components/StudentForm";

export default function AlunosPage() {
  const { data: alunos, isLoading } = useAlunos();

  return (
    <div className="space-y-6 p-8">
      <h1 className="text-xl font-semibold">Alunos</h1>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="space-y-4">
          {isLoading ? (
            <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
          ) : (
            <StudentTable alunos={alunos ?? []} />
          )}
        </div>
        <div>
          <StudentForm />
        </div>
      </div>
    </div>
  );
}
