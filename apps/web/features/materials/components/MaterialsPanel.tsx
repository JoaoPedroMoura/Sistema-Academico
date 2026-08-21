"use client";

import { useState, type FormEvent } from "react";
import { useMateriaisPorTurma, useEnviarMaterial } from "../hooks/useMaterials";

/**
 * O projeto ainda não integra armazenamento de arquivo próprio (S3/Azure Blob — ver
 * ARCHITECTURE.md, trabalho futuro): o professor cola o link de um arquivo já hospedado em
 * outro lugar (Drive, OneDrive etc.), em vez de um upload de verdade.
 */
export function MaterialsPanel({ turmaId }: { turmaId: string }) {
  const { data: materiais } = useMateriaisPorTurma(turmaId);
  const enviar = useEnviarMaterial(turmaId);

  const [titulo, setTitulo] = useState("");
  const [descricao, setDescricao] = useState("");
  const [arquivoUrl, setArquivoUrl] = useState("");
  const [arquivoNomeOriginal, setArquivoNomeOriginal] = useState("");

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    enviar.mutate(
      { turmaId, titulo, descricao: descricao || null, arquivoUrl, arquivoNomeOriginal, tamanhoBytes: 0 },
      {
        onSuccess: () => {
          setTitulo("");
          setDescricao("");
          setArquivoUrl("");
          setArquivoNomeOriginal("");
        },
      },
    );
  }

  return (
    <div className="space-y-4">
      <h3 className="text-sm font-medium">Materiais complementares</h3>

      {materiais && materiais.length > 0 ? (
        <ul className="space-y-1 text-sm">
          {materiais.map((m) => (
            <li key={m.id} className="rounded-md bg-[var(--color-muted)] px-3 py-1.5">
              <a href={m.arquivoUrl} target="_blank" rel="noreferrer" className="font-medium text-[var(--color-primary)] hover:underline">
                {m.titulo}
              </a>
              {m.descricao && <p className="text-[var(--color-muted-foreground)]">{m.descricao}</p>}
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-sm text-[var(--color-muted-foreground)]">Nenhum material enviado ainda.</p>
      )}

      <form onSubmit={handleSubmit} className="space-y-2">
        <input
          type="text"
          placeholder="Título"
          required
          value={titulo}
          onChange={(e) => setTitulo(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        <input
          type="text"
          placeholder="Descrição (opcional)"
          value={descricao}
          onChange={(e) => setDescricao(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        <input
          type="url"
          placeholder="Link do arquivo (Drive, OneDrive…)"
          required
          value={arquivoUrl}
          onChange={(e) => setArquivoUrl(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        <input
          type="text"
          placeholder="Nome do arquivo (ex.: aula1.pdf)"
          required
          value={arquivoNomeOriginal}
          onChange={(e) => setArquivoNomeOriginal(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        {enviar.isError && <p className="text-sm text-[var(--color-destructive)]">{enviar.error.message}</p>}
        <button
          type="submit"
          disabled={enviar.isPending}
          className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
        >
          {enviar.isPending ? "Enviando…" : "Enviar material"}
        </button>
      </form>
    </div>
  );
}
