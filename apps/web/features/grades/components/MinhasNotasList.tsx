"use client";

import { useMinhasNotas } from "../hooks/useGrades";
import type { MinhaNota } from "../types";

function agruparPorMateria(notas: MinhaNota[]): Map<string, MinhaNota[]> {
  const grupos = new Map<string, MinhaNota[]>();
  for (const nota of notas) {
    const grupo = grupos.get(nota.materiaNome) ?? [];
    grupo.push(nota);
    grupos.set(nota.materiaNome, grupo);
  }
  return grupos;
}

export function MinhasNotasList() {
  const { data: notas, isLoading } = useMinhasNotas();

  if (isLoading) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>;
  }

  if (!notas || notas.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhuma nota lançada ainda.</p>;
  }

  const porMateria = agruparPorMateria(notas);

  return (
    <div className="space-y-4">
      {[...porMateria.entries()].map(([materia, notasDaMateria]) => (
        <div key={materia} className="rounded-md border border-[var(--color-border)] p-4">
          <div className="mb-2 font-medium">{materia}</div>
          <ul className="space-y-1">
            {notasDaMateria.map((n) => (
              <li key={n.id} className="flex items-center justify-between text-sm">
                <span className="text-[var(--color-muted-foreground)]">{n.tipo}</span>
                <span className="font-medium">{n.valor.toFixed(1)}</span>
              </li>
            ))}
          </ul>
        </div>
      ))}
    </div>
  );
}
