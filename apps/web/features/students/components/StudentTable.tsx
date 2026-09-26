"use client";

import { useAtualizarAluno } from "../hooks/useStudents";
import type { Aluno } from "../types";

export function StudentTable({ alunos, onEditar }: { alunos: Aluno[]; onEditar: (aluno: Aluno) => void }) {
  const avancar = useAtualizarAluno();

  if (alunos.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum aluno matriculado.</p>;
  }

  return (
    <div className="overflow-x-auto rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)]">
      <table className="w-full text-sm">
        <thead className="bg-[var(--color-muted)] text-left text-xs uppercase text-[var(--color-muted-foreground)]">
          <tr>
            <th className="px-3 py-2">Nome</th>
            <th className="px-3 py-2">Matrícula</th>
            <th className="px-3 py-2">Email</th>
            <th className="px-3 py-2">Período</th>
            <th className="px-3 py-2" />
          </tr>
        </thead>
        <tbody>
          {alunos.map((a) => (
            <tr key={a.id} className="border-t border-[var(--color-border)]">
              <td className="px-3 py-2 font-medium">{a.nome}</td>
              <td className="px-3 py-2 font-mono tabular-nums text-[var(--color-muted-foreground)]">{a.matricula}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{a.email}</td>
              <td className="px-3 py-2">{a.periodoAtual}º</td>
              <td className="space-x-3 whitespace-nowrap px-3 py-2 text-right">
                <button
                  type="button"
                  onClick={() => onEditar(a)}
                  className="text-[var(--color-primary)] hover:underline focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
                >
                  Editar
                </button>
                {a.periodoAtual < 5 && (
                  <button
                    type="button"
                    onClick={() => avancar.mutate({ ...a, periodoAtual: a.periodoAtual + 1 })}
                    disabled={avancar.isPending}
                    className="text-[var(--color-primary)] hover:underline disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
                  >
                    Avançar período
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
