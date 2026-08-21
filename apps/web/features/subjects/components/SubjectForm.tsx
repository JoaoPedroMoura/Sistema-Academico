"use client";

import { useState, type FormEvent } from "react";
import { useCriarMateria } from "../hooks/useSubjects";

export function SubjectForm() {
  const [nome, setNome] = useState("");
  const [periodo, setPeriodo] = useState(1);
  const [cargaHorariaSemanal, setCargaHorariaSemanal] = useState(4);
  const criar = useCriarMateria();

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    criar.mutate(
      { nome, periodo, cargaHorariaSemanal },
      { onSuccess: () => setNome("") },
    );
  }

  return (
    <div className="space-y-3 rounded-md border border-[var(--color-border)] p-4">
      <h2 className="text-sm font-medium">Adicionar matéria</h2>
      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="text"
          placeholder="Nome"
          required
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        <div className="flex gap-3">
          <div className="flex-1 space-y-1">
            <label className="text-xs text-[var(--color-muted-foreground)]">Período</label>
            <input
              type="number"
              min={1}
              max={5}
              required
              value={periodo}
              onChange={(e) => setPeriodo(Number(e.target.value))}
              className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
            />
          </div>
          <div className="flex-1 space-y-1">
            <label className="text-xs text-[var(--color-muted-foreground)]">Aulas/semana</label>
            <input
              type="number"
              min={1}
              max={10}
              required
              value={cargaHorariaSemanal}
              onChange={(e) => setCargaHorariaSemanal(Number(e.target.value))}
              className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
            />
          </div>
        </div>
        {criar.isError && <p className="text-sm text-[var(--color-destructive)]">{criar.error.message}</p>}
        <button
          type="submit"
          disabled={criar.isPending}
          className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
        >
          {criar.isPending ? "Salvando…" : "Adicionar"}
        </button>
      </form>
    </div>
  );
}
