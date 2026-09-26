"use client";

import { Fragment } from "react";
import { DIAS_SEMANA, type AdicionarDisponibilidadeInput, type Disponibilidade } from "../types";

const DIA_LABEL: Record<string, string> = { Terca: "Terça", Sabado: "Sábado" };

// ponytail: tempos de aula fixos no front (espelham PeriodosAula do tenant); trocar pelo catálogo quando houver endpoint.
const TEMPOS = [
  ["07:00", "07:45"],
  ["07:45", "08:30"],
  ["08:30", "09:15"],
  ["09:30", "10:15"],
  ["10:15", "11:00"],
  ["11:00", "11:45"],
  ["11:45", "12:30"],
  ["18:00", "18:45"],
  ["18:45", "19:30"],
  ["19:30", "20:15"],
  ["20:30", "21:15"],
  ["21:15", "22:00"],
  ["22:00", "22:45"],
].map(([horaInicio, horaFim]) => ({ horaInicio, horaFim }));
const TURNOS: Record<string, string> = { "07:00": "Manhã", "18:00": "Noite" };

interface AvailabilityGridProps {
  disponibilidades: Disponibilidade[];
  onAdicionar: (input: AdicionarDisponibilidadeInput) => void;
  onRemover: (disponibilidadeId: string) => void;
  pendente: boolean;
}

/** Grade semanal clicável: cada célula liga/desliga a disponibilidade daquele tempo de aula. */
export function AvailabilityGrid({ disponibilidades, onAdicionar, onRemover, pendente }: AvailabilityGridProps) {
  // Faixas mais largas (ex. 07:00–12:00) marcam todas as células que contêm; clicar remove a faixa inteira.
  const cobrindo = (dia: string, horaInicio: string, horaFim: string) =>
    disponibilidades.find((d) => d.dia === dia && d.horaInicio <= horaInicio && d.horaFim >= horaFim);

  return (
    <div className="overflow-x-auto rounded-[var(--radius-lg)] border border-[var(--color-border)]">
      <table className="w-full text-xs">
        <thead className="bg-[var(--color-muted)] text-[var(--color-muted-foreground)]">
          <tr>
            <th className="px-2 py-2 text-left font-medium">Horário</th>
            {DIAS_SEMANA.map((dia) => (
              <th key={dia} className="px-2 py-2 text-center font-medium">
                {DIA_LABEL[dia] ?? dia}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {TEMPOS.map(({ horaInicio, horaFim }) => (
            <Fragment key={horaInicio}>
              {TURNOS[horaInicio] && (
                <tr className="border-t border-[var(--color-border)] bg-[var(--color-muted)]">
                  <th colSpan={DIAS_SEMANA.length + 1} className="px-2 py-1.5 text-left font-semibold">
                    {TURNOS[horaInicio]}
                  </th>
                </tr>
              )}
              <tr className="border-t border-[var(--color-border)]">
                <td className="whitespace-nowrap px-2 py-1 font-mono tabular-nums text-[var(--color-muted-foreground)]">
                  {horaInicio}–{horaFim}
                </td>
                {DIAS_SEMANA.map((dia) => {
                  const disp = cobrindo(dia, horaInicio, horaFim);
                  return (
                    <td key={dia} className="p-0.5">
                      <button
                        type="button"
                        aria-pressed={Boolean(disp)}
                        aria-label={`${DIA_LABEL[dia] ?? dia} ${horaInicio}–${horaFim}`}
                        title={
                          disp
                            ? `Disponível ${disp.horaInicio}–${disp.horaFim} (clique para remover)`
                            : "Clique para marcar"
                        }
                        disabled={pendente}
                        onClick={() => (disp ? onRemover(disp.id) : onAdicionar({ dia, horaInicio, horaFim }))}
                        className={`h-7 w-full rounded disabled:cursor-wait focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none ${
                          disp
                            ? "bg-[var(--color-primary)] hover:opacity-80"
                            : "bg-[var(--color-muted)] hover:bg-[var(--color-border)]"
                        }`}
                      />
                    </td>
                  );
                })}
              </tr>
            </Fragment>
          ))}
        </tbody>
      </table>
    </div>
  );
}
