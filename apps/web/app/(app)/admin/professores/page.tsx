"use client";

import { useState } from "react";
import { useProfessores, useExcluirProfessor } from "@/features/teachers/hooks/useTeachers";
import { TeacherTable } from "@/features/teachers/components/TeacherTable";
import { TeacherForm } from "@/features/teachers/components/TeacherForm";
import { TeacherAvailabilityEditor } from "@/features/teachers/components/TeacherAvailabilityEditor";
import { BackLink } from "@/shared/components/BackLink";

export default function ProfessoresPage() {
  const [selecionadoId, setSelecionadoId] = useState<string | null>(null);
  const { data: professores, isLoading } = useProfessores();
  const excluir = useExcluirProfessor();

  return (
    <div className="space-y-6 p-8">
      <BackLink href="/admin" label="Área do Admin" />
      <h1 className="text-xl font-semibold">Professores</h1>

      <div className="grid gap-6 lg:grid-cols-2">
        <div className="space-y-4">
          {isLoading ? (
            <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
          ) : (
            <TeacherTable
              professores={professores ?? []}
              selecionadoId={selecionadoId}
              onSelecionar={setSelecionadoId}
              onExcluir={(id) => {
                excluir.mutate(id, {
                  onSuccess: () => {
                    if (selecionadoId === id) setSelecionadoId(null);
                  },
                });
              }}
              excluindo={excluir.isPending}
            />
          )}
          {excluir.isError && <p className="text-sm text-[var(--color-destructive)]">{excluir.error.message}</p>}
        </div>

        <div className="space-y-4">
          <TeacherForm />
          {selecionadoId && <TeacherAvailabilityEditor professorId={selecionadoId} />}
        </div>
      </div>
    </div>
  );
}
