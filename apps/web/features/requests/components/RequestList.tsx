"use client";

import { useState } from "react";
import { useMarcarEmAnalise, useAprovarSolicitacao, useRejeitarSolicitacao } from "../hooks/useRequests";
import { StatusBadge } from "@/shared/components/StatusBadge";
import type { Solicitacao } from "../types";

const TIPO_LABEL: Record<string, string> = {
  AtestadoMedico: "Atestado médico",
  RevisaoDeNota: "Revisão de nota",
  JustificativaDeFalta: "Justificativa de falta",
  Outro: "Outro",
};

export function RequestList({ solicitacoes }: { solicitacoes: Solicitacao[] }) {
  const [rejeitandoId, setRejeitandoId] = useState<string | null>(null);
  const [motivoRejeicao, setMotivoRejeicao] = useState("");
  const marcarEmAnalise = useMarcarEmAnalise();
  const aprovar = useAprovarSolicitacao();
  const rejeitar = useRejeitarSolicitacao();

  if (solicitacoes.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhuma solicitação.</p>;
  }

  return (
    <ul className="space-y-3">
      {solicitacoes.map((s) => (
        <li key={s.id} className="rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)] p-4">
          <div className="flex items-start justify-between">
            <div>
              <div className="font-medium">{TIPO_LABEL[s.tipo] ?? s.tipo}</div>
              <div className="text-sm text-[var(--color-muted-foreground)]">{s.alunoNome}</div>
            </div>
            <StatusBadge status={s.status} />
          </div>

          <p className="mt-2 text-sm">{s.descricao}</p>

          {s.resposta && (
            <p className="mt-2 rounded bg-[var(--color-muted)] px-3 py-1.5 text-sm">
              <strong>Resposta:</strong> {s.resposta}
            </p>
          )}

          {(s.status === "Aberta" || s.status === "EmAnalise") && (
            <div className="mt-3 flex flex-wrap items-center gap-2">
              {s.status === "Aberta" && (
                <button
                  type="button"
                  onClick={() => marcarEmAnalise.mutate(s.id)}
                  disabled={marcarEmAnalise.isPending}
                  className="rounded-md border border-[var(--color-border)] px-3 py-1.5 text-sm hover:bg-[var(--color-muted)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
                >
                  Marcar em análise
                </button>
              )}
              <button
                type="button"
                onClick={() => aprovar.mutate({ id: s.id })}
                disabled={aprovar.isPending}
                className="rounded-md bg-[var(--color-success)] px-3 py-1.5 text-sm font-medium text-[var(--color-success-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
              >
                Aprovar
              </button>

              {rejeitandoId === s.id ? (
                <div className="flex flex-1 items-center gap-2">
                  <input
                    type="text"
                    placeholder="Motivo da rejeição"
                    value={motivoRejeicao}
                    onChange={(e) => setMotivoRejeicao(e.target.value)}
                    className="flex-1 rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-2 py-1.5 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
                  />
                  <button
                    type="button"
                    onClick={() => {
                      if (!motivoRejeicao.trim()) return;
                      rejeitar.mutate(
                        { id: s.id, resposta: motivoRejeicao },
                        { onSuccess: () => { setRejeitandoId(null); setMotivoRejeicao(""); } },
                      );
                    }}
                    disabled={rejeitar.isPending}
                    className="rounded-md bg-[var(--color-destructive)] px-3 py-1.5 text-sm font-medium text-[var(--color-destructive-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
                  >
                    Confirmar
                  </button>
                </div>
              ) : (
                <button
                  type="button"
                  onClick={() => setRejeitandoId(s.id)}
                  className="rounded-md border border-[var(--color-destructive)] px-3 py-1.5 text-sm text-[var(--color-destructive)] hover:bg-[var(--color-muted)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
                >
                  Rejeitar
                </button>
              )}
            </div>
          )}
        </li>
      ))}
    </ul>
  );
}
