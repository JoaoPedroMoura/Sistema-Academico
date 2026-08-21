"use client";

import { useMeusMateriais } from "../hooks/useMaterials";

function formatarTamanho(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(0)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}

export function MeusMateriaisList() {
  const { data: materiais, isLoading } = useMeusMateriais();

  if (isLoading) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Carregando…</p>;
  }

  if (!materiais || materiais.length === 0) {
    return <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum material disponível ainda.</p>;
  }

  return (
    <ul className="space-y-3">
      {materiais.map((m) => (
        <li key={m.id} className="rounded-md border border-[var(--color-border)] p-4">
          <div className="flex items-start justify-between">
            <div>
              <div className="font-medium">{m.titulo}</div>
              <div className="text-sm text-[var(--color-muted-foreground)]">{m.materiaNome}</div>
            </div>
            <span className="text-xs text-[var(--color-muted-foreground)]">{formatarTamanho(m.tamanhoBytes)}</span>
          </div>

          {m.descricao && <p className="mt-2 text-sm">{m.descricao}</p>}

          <a
            href={m.arquivoUrl}
            target="_blank"
            rel="noreferrer"
            className="mt-3 inline-block text-sm text-[var(--color-primary)] hover:underline"
          >
            Baixar {m.arquivoNomeOriginal}
          </a>
        </li>
      ))}
    </ul>
  );
}
