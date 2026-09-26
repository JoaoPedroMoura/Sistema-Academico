"use client";

import type { Professor } from "../types";

interface TeacherTableProps {
  professores: Professor[];
  onEditar: (professor: Professor) => void;
  onExcluir: (id: string) => void;
  excluindo: boolean;
}

export function TeacherTable({ professores, onEditar, onExcluir, excluindo }: TeacherTableProps) {
  if (professores.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum professor cadastrado.</p>;
  }

  return (
    <div className="overflow-x-auto rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)]">
      <table className="w-full text-sm">
        <thead className="bg-[var(--color-muted)] text-left text-xs uppercase text-[var(--color-muted-foreground)]">
          <tr>
            <th className="px-3 py-2">Nome</th>
            <th className="px-3 py-2">Matrícula</th>
            <th className="px-3 py-2">Email</th>
            <th className="px-3 py-2">Disponibilidades</th>
            <th className="px-3 py-2" />
          </tr>
        </thead>
        <tbody>
          {professores.map((p) => (
            <tr key={p.id} className="border-t border-[var(--color-border)]">
              <td className="px-3 py-2 font-medium">{p.nome}</td>
              <td className="px-3 py-2 font-mono tabular-nums text-[var(--color-muted-foreground)]">{p.matricula}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{p.email}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{p.disponibilidades.length}</td>
              <td className="space-x-3 whitespace-nowrap px-3 py-2 text-right">
                <button
                  type="button"
                  onClick={() => onEditar(p)}
                  className="text-[var(--color-primary)] hover:underline focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
                >
                  Editar
                </button>
                <button
                  type="button"
                  onClick={() => onExcluir(p.id)}
                  disabled={excluindo}
                  className="text-[var(--color-destructive)] hover:underline disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
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
