"use client";

import { useState } from "react";
import { useMarcarEmAnalise, useAprovarSolicitacao, useRejeitarSolicitacao } from "../hooks/useRequests";
import type { Solicitacao } from "../types";

const TIPO_LABEL: Record<string, string> = {
  AtestadoMedico: "Atestado médico",
  RevisaoDeNota: "Revisão de nota",
  JustificativaDeFalta: "Justificativa de falta",
  Outro: "Outro",
};

const STATUS_COLOR: Record<string, string> = {
  Aberta: "text-[var(--color-warning)]",
  EmAnalise: "text-[var(--color-primary)]",
  Aprovada: "text-[var(--color-success)]",
  Rejeitada: "text-[var(--color-destructive)]",
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
        <li key={s.id} className="rounded-md border border-[var(--color-border)] p-4">
          <div className="flex items-start justify-between">
            <div>
              <div className="font-medium">{TIPO_LABEL[s.tipo] ?? s.tipo}</div>
              <div className="text-sm text-[var(--color-muted-foreground)]">{s.alunoNome}</div>
            </div>
            <span className={`text-sm font-medium ${STATUS_COLOR[s.status] ?? ""}`}>{s.status}</span>
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
                  className="rounded-md border border-[var(--color-border)] px-3 py-1.5 text-sm hover:bg-[var(--color-muted)] disabled:opacity-50"
                >
                  Marcar em análise
                </button>
              )}
              <button
                type="button"
                onClick={() => aprovar.mutate({ id: s.id })}
                disabled={aprovar.isPending}
                className="rounded-md bg-[var(--color-success)] px-3 py-1.5 text-sm font-medium text-white disabled:opacity-50"
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
                    className="flex-1 rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
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
                    className="rounded-md bg-[var(--color-destructive)] px-3 py-1.5 text-sm font-medium text-white disabled:opacity-50"
                  >
                    Confirmar
                  </button>
                </div>
              ) : (
                <button
                  type="button"
                  onClick={() => setRejeitandoId(s.id)}
                  className="rounded-md border border-[var(--color-destructive)] px-3 py-1.5 text-sm text-[var(--color-destructive)] hover:bg-[var(--color-muted)]"
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
