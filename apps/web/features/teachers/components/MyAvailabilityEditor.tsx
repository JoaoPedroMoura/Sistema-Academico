"use client";

import { useState, type FormEvent } from "react";
import { useMeuPerfil, useAdicionarMinhaDisponibilidade, useRemoverMinhaDisponibilidade } from "../hooks/useTeachers";
import { DIAS_SEMANA } from "../types";

/** Tela self-service do professor (ANALISE-TCC.md §6 — evolução em relação ao TCC original). */
export function MyAvailabilityEditor() {
  const { data: professor } = useMeuPerfil();
  const [dia, setDia] = useState<string>(DIAS_SEMANA[0]);
  const [horaInicio, setHoraInicio] = useState("07:00");
  const [horaFim, setHoraFim] = useState("07:50");
  const adicionar = useAdicionarMinhaDisponibilidade();
  const remover = useRemoverMinhaDisponibilidade();

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    adicionar.mutate({ dia, horaInicio, horaFim });
  }

  if (!professor) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>;
  }

  return (
    <div className="space-y-3 rounded-md border border-[var(--color-border)] p-4">
      <h2 className="text-sm font-medium">Minha disponibilidade</h2>

      {professor.disponibilidades.length === 0 ? (
        <p className="text-sm text-[var(--color-muted-foreground)]">Nenhuma disponibilidade cadastrada.</p>
      ) : (
        <ul className="space-y-1 text-sm">
          {professor.disponibilidades.map((d) => (
            <li key={d.id} className="flex items-center justify-between rounded-md bg-[var(--color-muted)] px-3 py-1.5">
              <span>
                {d.dia} · {d.horaInicio} – {d.horaFim}
              </span>
              <button
                type="button"
                onClick={() => remover.mutate(d.id)}
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
        <div className="space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Dia</label>
          <select
            value={dia}
            onChange={(e) => setDia(e.target.value)}
            className="rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
          >
            {DIAS_SEMANA.map((d) => (
              <option key={d} value={d}>
                {d}
              </option>
            ))}
          </select>
        </div>
        <div className="space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Início</label>
          <input
            type="time"
            value={horaInicio}
            onChange={(e) => setHoraInicio(e.target.value)}
            className="rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
          />
        </div>
        <div className="space-y-1">
          <label className="text-xs text-[var(--color-muted-foreground)]">Fim</label>
          <input
            type="time"
            value={horaFim}
            onChange={(e) => setHoraFim(e.target.value)}
            className="rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
          />
        </div>
        <button
          type="submit"
          disabled={adicionar.isPending}
          className="rounded-md bg-[var(--color-primary)] px-3 py-1.5 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
        >
          Adicionar
        </button>
      </form>
      {adicionar.isError && <p className="text-sm text-[var(--color-destructive)]">{adicionar.error.message}</p>}
    </div>
  );
}
