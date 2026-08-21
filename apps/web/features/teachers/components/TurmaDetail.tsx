"use client";

import { useState } from "react";
import { useMinhasTurmas } from "../hooks/useTeachers";
import { GradesPanel } from "@/features/grades/components/GradesPanel";
import { AttendancePanel } from "@/features/attendance/components/AttendancePanel";
import { MaterialsPanel } from "@/features/materials/components/MaterialsPanel";

const ABAS = ["Notas", "Presença", "Materiais"] as const;
type Aba = (typeof ABAS)[number];

export function TurmaDetail({ turmaId }: { turmaId: string }) {
  const { data: turmas } = useMinhasTurmas();
  const [aba, setAba] = useState<Aba>("Notas");
  const turma = turmas?.find((t) => t.id === turmaId);

  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-xl font-semibold">{turma?.materiaNome ?? "Turma"}</h1>
        {turma && (
          <p className="text-sm text-[var(--color-muted-foreground)]">
            {turma.dia} · {turma.horaInicio}–{turma.horaFim} · {turma.periodoCurricular}º período
          </p>
        )}
      </div>

      <div className="flex gap-2 border-b border-[var(--color-border)]">
        {ABAS.map((a) => (
          <button
            key={a}
            type="button"
            onClick={() => setAba(a)}
            className={`px-3 py-2 text-sm ${
              aba === a
                ? "border-b-2 border-[var(--color-primary)] font-medium text-[var(--color-primary)]"
                : "text-[var(--color-muted-foreground)]"
            }`}
          >
            {a}
          </button>
        ))}
      </div>

      <div className="rounded-md border border-[var(--color-border)] p-4">
        {aba === "Notas" && <GradesPanel turmaId={turmaId} />}
        {aba === "Presença" && <AttendancePanel turmaId={turmaId} />}
        {aba === "Materiais" && <MaterialsPanel turmaId={turmaId} />}
      </div>
    </div>
  );
}
