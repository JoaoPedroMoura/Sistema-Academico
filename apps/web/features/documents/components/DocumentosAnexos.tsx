"use client";

import { useState } from "react";
import { inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useBaixarDocumento, useDocumentosAnexos, useEnviarDocumento, useRemoverDocumento } from "../hooks/useDocuments";
import type { TipoDocumento } from "../types";

const TIPO_DOCUMENTO_LABEL: Record<TipoDocumento, string> = {
  Cpf: "CPF",
  Rg: "RG",
  CertidaoNascimento: "Certidão de nascimento",
  ComprovanteResidencia: "Comprovante de residência",
  HistoricoEnsinoMedio: "Histórico do ensino médio",
  TituloEleitor: "Título de eleitor",
  Reservista: "Certificado de reservista",
  Diploma: "Diploma",
  Outro: "Outro",
};

const TAMANHO_MAXIMO = 5 * 1024 * 1024;

interface DocumentosAnexosProps {
  pessoaId: string;
  /** Tipos oferecidos no upload (variam entre aluno e professor). */
  tipos: TipoDocumento[];
}

/** Lista, envia (PDF/JPG/PNG até 5 MB), abre e remove os arquivos digitalizados de uma pessoa. */
export function DocumentosAnexos({ pessoaId, tipos }: DocumentosAnexosProps) {
  const { data: documentos, isLoading } = useDocumentosAnexos(pessoaId);
  const enviar = useEnviarDocumento(pessoaId);
  const remover = useRemoverDocumento(pessoaId);
  const baixar = useBaixarDocumento();
  const [tipo, setTipo] = useState<TipoDocumento>(tipos[0]);
  const [arquivo, setArquivo] = useState<File | null>(null);
  const [erroArquivo, setErroArquivo] = useState<string | null>(null);

  // Não é <form>: este bloco fica dentro do modal, às vezes perto de outro formulário.
  function handleEnviar() {
    if (!arquivo) return;
    enviar.mutate({ tipo, arquivo }, { onSuccess: () => setArquivo(null) });
  }

  const erro = erroArquivo ?? enviar.error?.message ?? remover.error?.message;

  return (
    <div className="space-y-3">
      <h3 className="text-sm font-medium">Arquivos dos documentos</h3>

      {isLoading ? (
        <p className="text-xs text-[var(--color-muted-foreground)]">Carregando…</p>
      ) : documentos?.length ? (
        <ul className="divide-y divide-[var(--color-border)] rounded-md border border-[var(--color-border)] text-sm">
          {documentos.map((d) => (
            <li key={d.id} className="flex items-center gap-3 px-3 py-2">
              <span className="w-40 shrink-0 text-xs text-[var(--color-muted-foreground)]">{TIPO_DOCUMENTO_LABEL[d.tipo]}</span>
              <button
                type="button"
                onClick={() => baixar(d.id)}
                className="min-w-0 flex-1 truncate text-left text-[var(--color-primary)] hover:underline"
                title="Abrir"
              >
                {d.nomeArquivo}
              </button>
              <span className="text-xs tabular-nums text-[var(--color-muted-foreground)]">
                {Math.ceil(d.tamanhoBytes / 1024)} KB
              </span>
              <button
                type="button"
                onClick={() => remover.mutate(d.id)}
                disabled={remover.isPending}
                className="text-xs text-[var(--color-destructive)] hover:underline disabled:opacity-50"
              >
                Remover
              </button>
            </li>
          ))}
        </ul>
      ) : (
        <p className="text-xs text-[var(--color-muted-foreground)]">Nenhum arquivo enviado.</p>
      )}

      <div className="flex flex-wrap items-center gap-2">
        <select
          value={tipo}
          onChange={(e) => setTipo(e.target.value as TipoDocumento)}
          aria-label="Tipo do documento"
          className={`${inputClass} w-auto`}
        >
          {tipos.map((t) => (
            <option key={t} value={t}>
              {TIPO_DOCUMENTO_LABEL[t]}
            </option>
          ))}
        </select>
        <input
          // key: limpa o input depois de um envio bem-sucedido.
          key={documentos?.length ?? 0}
          type="file"
          accept="application/pdf,image/jpeg,image/png"
          aria-label="Arquivo"
          onChange={(e) => {
            const f = e.target.files?.[0] ?? null;
            const grande = f !== null && f.size > TAMANHO_MAXIMO;
            setErroArquivo(grande ? "Arquivo maior que 5 MB." : null);
            setArquivo(grande ? null : f);
          }}
          className="min-w-0 flex-1 text-xs"
        />
        <button type="button" onClick={handleEnviar} disabled={!arquivo || enviar.isPending} className={primaryButtonClass}>
          {enviar.isPending ? "Enviando…" : "Enviar"}
        </button>
      </div>
      {erro && <p className="text-xs text-[var(--color-destructive)]">{erro}</p>}
    </div>
  );
}
