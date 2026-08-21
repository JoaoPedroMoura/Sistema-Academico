"use client";

import type { Materia } from "../types";

interface SubjectTableProps {
  materias: Materia[];
  onExcluir: (id: string) => void;
  excluindo: boolean;
}

export function SubjectTable({ materias, onExcluir, excluindo }: SubjectTableProps) {
  if (materias.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhuma matéria cadastrada.</p>;
  }

  return (
    <div className="overflow-x-auto rounded-md border border-[var(--color-border)]">
      <table className="w-full text-sm">
        <thead className="bg-[var(--color-muted)] text-left text-xs uppercase text-[var(--color-muted-foreground)]">
          <tr>
            <th className="px-3 py-2">Nome</th>
            <th className="px-3 py-2">Período</th>
            <th className="px-3 py-2">Carga/semana</th>
            <th className="px-3 py-2" />
          </tr>
        </thead>
        <tbody>
          {materias.map((m) => (
            <tr key={m.id} className="border-t border-[var(--color-border)]">
              <td className="px-3 py-2 font-medium">{m.nome}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{m.periodo}º</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{m.cargaHorariaSemanal} aulas</td>
              <td className="px-3 py-2 text-right">
                <button
                  type="button"
                  onClick={() => onExcluir(m.id)}
                  disabled={excluindo}
                  className="text-[var(--color-destructive)] hover:underline disabled:opacity-50"
                >
                  Excluir
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
