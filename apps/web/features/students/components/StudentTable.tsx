"use client";

import { useAvancarPeriodo } from "../hooks/useStudents";
import type { Aluno } from "../types";

export function StudentTable({ alunos }: { alunos: Aluno[] }) {
  const avancar = useAvancarPeriodo();

  if (alunos.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum aluno matriculado.</p>;
  }

  return (
    <div className="overflow-x-auto rounded-md border border-[var(--color-border)]">
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
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{a.matricula}</td>
              <td className="px-3 py-2 text-[var(--color-muted-foreground)]">{a.email}</td>
              <td className="px-3 py-2">{a.periodoAtual}º</td>
              <td className="px-3 py-2 text-right">
                {a.periodoAtual < 5 && (
                  <button
                    type="button"
                    onClick={() => avancar.mutate({ id: a.id, novoPeriodo: a.periodoAtual + 1 })}
                    disabled={avancar.isPending}
                    className="text-[var(--color-primary)] hover:underline disabled:opacity-50"
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
