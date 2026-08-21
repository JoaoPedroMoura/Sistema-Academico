"use client";

import type { Grade } from "../types";

const DIAS = ["Segunda", "Terca", "Quarta", "Quinta", "Sexta", "Sabado"];

export function GradeView({ grade }: { grade: Grade }) {
  const horarios = [...new Set(grade.turmas.map((t) => t.horaInicio))].sort();

  return (
    <div className="space-y-3">
      <div className="flex items-center gap-3 text-sm text-[var(--color-muted-foreground)]">
        <span>Status: <strong className="text-[var(--color-foreground)]">{grade.status}</strong></span>
        {grade.custoSolucao !== null && <span>Custo: {grade.custoSolucao}</span>}
        <span>{grade.turmas.length} aula(s)</span>
      </div>

      <div className="overflow-x-auto rounded-md border border-[var(--color-border)]">
        <table className="w-full text-xs">
          <thead className="bg-[var(--color-muted)] text-[var(--color-muted-foreground)]">
            <tr>
              <th className="px-2 py-2 text-left">Horário</th>
              {DIAS.map((dia) => (
                <th key={dia} className="px-2 py-2 text-left">{dia}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {horarios.map((hora) => (
              <tr key={hora} className="border-t border-[var(--color-border)] align-top">
                <td className="whitespace-nowrap px-2 py-2 font-medium">{hora}</td>
                {DIAS.map((dia) => {
                  const turmas = grade.turmas.filter((t) => t.dia === dia && t.horaInicio === hora);
                  return (
                    <td key={dia} className="px-2 py-2">
                      {turmas.map((t) => (
                        <div key={t.id} className="mb-1 rounded bg-[var(--color-muted)] px-2 py-1">
                          <div className="font-medium">{t.materiaNome}</div>
                          <div className="text-[var(--color-muted-foreground)]">
                            {t.professorNome} · {t.periodoCurricular}º período
                          </div>
                        </div>
                      ))}
                    </td>
                  );
                })}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
