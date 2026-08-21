"use client";

import type { Professor } from "../types";

interface TeacherTableProps {
  professores: Professor[];
  selecionadoId: string | null;
  onSelecionar: (id: string) => void;
  onExcluir: (id: string) => void;
  excluindo: boolean;
}

export function TeacherTable({ professores, selecionadoId, onSelecionar, onExcluir, excluindo }: TeacherTableProps) {
  if (professores.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum professor cadastrado.</p>;
  }

  return (
    <div className="overflow-x-auto rounded-md border border-[var(--color-border)]">
      <table className="w-full text-sm">
        <thead className="bg-[var(--color-muted)] text-left text-xs uppercase text-[var(--color-muted-foreground)]">
          <tr>
            <th className="px-3 py-2">Nome</th>
            <th className="px-3 py-2">Email</th>
            <th className="px-3 py-2">Disponibilidades</th>
            <th className="px-3 py-2" />
          </tr>
        </thead>
        <tbody>
          {professores.map((p) => (
            <tr
              key={p.id}
              onClick={() => onSelecionar(p.id)}
              className={`cursor-pointer border-t border-[var(--color-border)] hover:bg-[var(--color-muted)] ${
                selecionadoId === p.id ? "bg-[var(--color-muted)]" : ""
              }`}
            >
              <td className="px-3 py-2 font-medium">{p.nome}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{p.email}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{p.disponibilidades.length}</td>
              <td className="px-3 py-2 text-right">
                <button
                  type="button"
                  onClick={(e) => {
                    e.stopPropagation();
                    onExcluir(p.id);
                  }}
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
