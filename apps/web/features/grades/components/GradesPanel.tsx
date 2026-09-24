"use client";

import { useState, type FormEvent } from "react";
import { useAlunosDaTurma } from "@/features/teachers/hooks/useTeachers";
import { useNotasPorTurma, useLancarNota } from "../hooks/useGrades";

export function GradesPanel({ turmaId }: { turmaId: string }) {
  const { data: alunos } = useAlunosDaTurma(turmaId);
  const { data: notas } = useNotasPorTurma(turmaId);
  const lancar = useLancarNota(turmaId);

  const [alunoId, setAlunoId] = useState("");
  const [tipo, setTipo] = useState("");
  const [valor, setValor] = useState(10);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!alunoId || !tipo) return;
    lancar.mutate(
      { turmaId, alunoId, tipo, valor },
      { onSuccess: () => setTipo("") },
    );
  }

  return (
    <div className="space-y-4">
      <h3 className="text-sm font-medium">Notas</h3>

      {notas && notas.length > 0 ? (
        <ul className="space-y-1 text-sm">
          {notas.map((n) => (
            <li key={n.id} className="flex items-center justify-between rounded-md bg-[var(--color-muted)] px-3 py-1.5">
              <span>
                {n.alunoNome} · {n.tipo}
              </span>
              <strong className="font-mono tabular-nums">{n.valor}</strong>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-sm text-[var(--color-muted-foreground)]">Nenhuma nota lançada ainda.</p>
      )}

      <form onSubmit={handleSubmit} className="flex flex-wrap items-end gap-2">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Aluno</label>
          <select
            required
            value={alunoId}
            onChange={(e) => setAlunoId(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-2 py-1.5 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
          >
            <option value="">Selecione…</option>
            {alunos?.map((a) => (
              <option key={a.id} value={a.id}>
                {a.nome} (<span className="font-mono tabular-nums">{a.matricula}</span>)
              </option>
            ))}
          </select>
        </div>
        <div className="space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Tipo</label>
          <input
            type="text"
            placeholder="Prova 1"
            required
            value={tipo}
            onChange={(e) => setTipo(e.target.value)}
            className="rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-2 py-1.5 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
          />
        </div>
        <div className="space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Valor</label>
          <input
            type="number"
            min={0}
            max={10}
            step={0.1}
            required
            value={valor}
            onChange={(e) => setValor(Number(e.target.value))}
            className="w-20 rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-2 py-1.5 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
          />
        </div>
        <button
          type="submit"
          disabled={lancar.isPending}
          className="rounded-md bg-[var(--color-primary)] px-3 py-1.5 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
        >
          Lançar
        </button>
      </form>
      {lancar.isError && <p className="text-sm text-[var(--color-destructive)]">{lancar.error.message}</p>}
    </div>
  );
}
