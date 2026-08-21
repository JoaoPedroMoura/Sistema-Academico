"use client";

import { useState, type FormEvent } from "react";
import { useAbrirSolicitacao, useMinhasSolicitacoes } from "../hooks/useRequests";
import type { TipoSolicitacao } from "../types";

const TIPO_LABEL: Record<TipoSolicitacao, string> = {
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

export function MinhasSolicitacoesPanel() {
  const { data: solicitacoes, isLoading } = useMinhasSolicitacoes();
  const abrir = useAbrirSolicitacao();

  const [tipo, setTipo] = useState<TipoSolicitacao>("JustificativaDeFalta");
  const [descricao, setDescricao] = useState("");
  const [mostrarForm, setMostrarForm] = useState(false);

  function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (!descricao.trim()) return;
    abrir.mutate(
      { tipo, descricao },
      {
        onSuccess: () => {
          setDescricao("");
          setMostrarForm(false);
        },
      },
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <h2 className="text-sm font-medium text-[var(--color-muted-foreground)]">Nova solicitação</h2>
        <button
          type="button"
          onClick={() => setMostrarForm((v) => !v)}
          className="rounded-md border border-[var(--color-border)] px-3 py-1.5 text-sm hover:bg-[var(--color-muted)]"
        >
          {mostrarForm ? "Cancelar" : "Abrir solicitação"}
        </button>
      </div>

      {mostrarForm && (
        <form onSubmit={handleSubmit} className="space-y-3 rounded-md border border-[var(--color-border)] p-4">
          <div>
            <label className="mb-1 block text-sm font-medium">Tipo</label>
            <select
              value={tipo}
              onChange={(e) => setTipo(e.target.value as TipoSolicitacao)}
              className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
            >
              {Object.entries(TIPO_LABEL).map(([value, label]) => (
                <option key={value} value={value}>
                  {label}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium">Descrição</label>
            <textarea
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
              rows={3}
              required
              className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-2 py-1.5 text-sm"
            />
          </div>

          <button
            type="submit"
            disabled={abrir.isPending}
            className="rounded-md bg-[var(--color-primary)] px-3 py-1.5 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
          >
            Enviar
          </button>
        </form>
      )}

      <div>
        <h2 className="mb-3 text-sm font-medium text-[var(--color-muted-foreground)]">Minhas solicitações</h2>

        {isLoading ? (
          <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
        ) : !solicitacoes || solicitacoes.length === 0 ? (
          <p className="text-sm text-[var(--color-muted-foreground)]">Nenhuma solicitação aberta ainda.</p>
        ) : (
          <ul className="space-y-3">
            {solicitacoes.map((s) => (
              <li key={s.id} className="rounded-md border border-[var(--color-border)] p-4">
                <div className="flex items-start justify-between">
                  <div className="font-medium">{TIPO_LABEL[s.tipo] ?? s.tipo}</div>
                  <span className={`text-sm font-medium ${STATUS_COLOR[s.status] ?? ""}`}>{s.status}</span>
                </div>
                <p className="mt-2 text-sm">{s.descricao}</p>
                {s.resposta && (
                  <p className="mt-2 rounded bg-[var(--color-muted)] px-3 py-1.5 text-sm">
                    <strong>Resposta:</strong> {s.resposta}
                  </p>
                )}
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
}
