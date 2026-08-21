"use client";

import { useState } from "react";
import { useAlunosDaTurma } from "@/features/teachers/hooks/useTeachers";
import { usePresencas, useRegistrarPresenca } from "../hooks/useAttendance";

function hoje(): string {
  return new Date().toISOString().slice(0, 10);
}

export function AttendancePanel({ turmaId }: { turmaId: string }) {
  const [data, setData] = useState(hoje());
  const { data: alunos } = useAlunosDaTurma(turmaId);
  const { data: presencas } = usePresencas(turmaId, data);
  const registrar = useRegistrarPresenca(turmaId, data);

  // Só guarda o que o usuário mexeu nesta sessão — o valor "de base" (o que já veio do
  // servidor) é calculado direto no render, sem efeito algum: nada para sincronizar.
  const [overrides, setOverrides] = useState<Record<string, boolean>>({});

  function presenteDe(alunoId: string): boolean {
    if (alunoId in overrides) {
      return overrides[alunoId];
    }
    return presencas?.find((p) => p.alunoId === alunoId)?.presente ?? true;
  }

  function handleTrocarData(novaData: string) {
    setData(novaData);
    setOverrides({}); // outra data, outro contexto — não faz sentido herdar os toggles daqui
  }

  function handleSalvar() {
    if (!alunos) return;
    registrar.mutate(alunos.map((a) => ({ alunoId: a.id, presente: presenteDe(a.id) })));
  }

  return (
    <div className="space-y-4">
      <h3 className="text-sm font-medium">Presença</h3>

      <div className="space-y-1">
        <label className="text-xs text-[var(--color-muted-foreground)]">Data da aula</label>
        <input
          type="date"
          value={data}
          onChange={(e) => handleTrocarData(e.target.value)}
          className="rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
        />
      </div>

      {alunos && alunos.length > 0 ? (
        <ul className="space-y-1">
          {alunos.map((a) => (
            <li key={a.id} className="flex items-center justify-between rounded-md bg-[var(--color-muted)] px-3 py-1.5 text-sm">
              <span>
                {a.nome} ({a.matricula})
              </span>
              <label className="flex items-center gap-2">
                <input
                  type="checkbox"
                  checked={presenteDe(a.id)}
                  onChange={(e) => setOverrides((prev) => ({ ...prev, [a.id]: e.target.checked }))}
                />
                Presente
              </label>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum aluno nesta turma.</p>
      )}

      <button
        type="button"
        onClick={handleSalvar}
        disabled={registrar.isPending || !alunos?.length}
        className="rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
      >
        {registrar.isPending ? "Salvando…" : "Salvar presença"}
      </button>
      {registrar.isError && <p className="text-sm text-[var(--color-destructive)]">{registrar.error.message}</p>}
    </div>
  );
}
