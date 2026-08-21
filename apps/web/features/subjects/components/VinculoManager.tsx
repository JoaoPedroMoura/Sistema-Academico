"use client";

import { useState, type FormEvent } from "react";
import { useMaterias, useVinculos, useAdicionarVinculo, useRemoverVinculo } from "../hooks/useSubjects";
import { useProfessores } from "@/features/teachers/hooks/useTeachers";

/** Caso de uso "Manter Matérias do Professor" (ANALISE-TCC.md §4, UC3) — quem pode lecionar o quê. */
export function VinculoManager() {
  const { data: materias } = useMaterias();
  const { data: professores } = useProfessores();
  const { data: vinculos } = useVinculos();
  const [materiaId, setMateriaId] = useState("");
  const [professorId, setProfessorId] = useState("");
  const adicionar = useAdicionarVinculo();
  const remover = useRemoverVinculo();

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (!materiaId || !professorId) return;
    adicionar.mutate({ materiaId, professorId });
  }

  return (
    <div className="space-y-3 rounded-md border border-[var(--color-border)] p-4">
      <h2 className="text-sm font-medium">Vínculo Matéria ↔ Professor</h2>

      {vinculos && vinculos.length > 0 && (
        <ul className="space-y-1 text-sm">
          {vinculos.map((v) => (
            <li key={v.id} className="flex items-center justify-between rounded-md bg-[var(--color-muted)] px-3 py-1.5">
              <span>
                {v.materiaNome} ↔ {v.professorNome}
              </span>
              <button
                type="button"
                onClick={() => remover.mutate({ materiaId: v.materiaId, professorId: v.professorId })}
                disabled={remover.isPending}
                className="text-[var(--color-destructive)] hover:underline disabled:opacity-50"
              >
                Remover
              </button>
            </li>
          ))}
        </ul>
      )}

      <form onSubmit={handleSubmit} className="flex flex-wrap items-end gap-2">
        <div className="flex-1 space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Matéria</label>
          <select
            required
            value={materiaId}
            onChange={(e) => setMateriaId(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
          >
            <option value="">Selecione…</option>
            {materias?.map((m) => (
              <option key={m.id} value={m.id}>
                {m.nome}
              </option>
            ))}
          </select>
        </div>
        <div className="flex-1 space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Professor</label>
          <select
            required
            value={professorId}
            onChange={(e) => setProfessorId(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
          >
            <option value="">Selecione…</option>
            {professores?.map((p) => (
              <option key={p.id} value={p.id}>
                {p.nome}
              </option>
            ))}
          </select>
        </div>
        <button
          type="submit"
          disabled={adicionar.isPending}
          className="rounded-md bg-[var(--color-primary)] px-3 py-1.5 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
        >
          Vincular
        </button>
      </form>
      {adicionar.isError && <p className="text-sm text-[var(--color-destructive)]">{adicionar.error.message}</p>}
    </div>
  );
}
