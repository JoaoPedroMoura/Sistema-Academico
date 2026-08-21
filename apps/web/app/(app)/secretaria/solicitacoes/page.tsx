"use client";

import { useState } from "react";
import { useSolicitacoes } from "@/features/requests/hooks/useRequests";
import { RequestList } from "@/features/requests/components/RequestList";
import type { StatusSolicitacao } from "@/features/requests/types";

const FILTROS: { label: string; value: StatusSolicitacao | undefined }[] = [
  { label: "Todas", value: undefined },
  { label: "Abertas", value: "Aberta" },
  { label: "Em análise", value: "EmAnalise" },
  { label: "Aprovadas", value: "Aprovada" },
  { label: "Rejeitadas", value: "Rejeitada" },
];

export default function SolicitacoesPage() {
  const [filtro, setFiltro] = useState<StatusSolicitacao | undefined>(undefined);
  const { data: solicitacoes, isLoading } = useSolicitacoes(filtro);

  return (
    <div className="space-y-6 p-8">
      <h1 className="text-xl font-semibold">Solicitações</h1>

      <div className="flex gap-2">
        {FILTROS.map((f) => (
          <button
            key={f.label}
            type="button"
            onClick={() => setFiltro(f.value)}
            className={`rounded-md border px-3 py-1.5 text-sm ${
              filtro === f.value
                ? "border-[var(--color-primary)] bg-[var(--color-primary)] text-[var(--color-primary-foreground)]"
                : "border-[var(--color-border)] hover:bg-[var(--color-muted)]"
            }`}
          >
            {f.label}
          </button>
        ))}
      </div>

      {isLoading ? (
        <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>
      ) : (
        <RequestList solicitacoes={solicitacoes ?? []} />
      )}
    </div>
  );
}
