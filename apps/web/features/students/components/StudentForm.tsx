"use client";

import { useState, type FormEvent } from "react";
import { Campo, Secao } from "@/features/documents/components/Campo";
import { DadosPessoaisFields } from "@/features/documents/components/DadosPessoaisFields";
import { DocumentosAnexos } from "@/features/documents/components/DocumentosAnexos";
import { DADOS_PESSOAIS_VAZIO, type DocumentosAluno, type TipoDocumento } from "@/features/documents/types";
import { FormDialog, SenhaTemporaria, inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useEmailDisponivel } from "@/shared/hooks/useEmailDisponivel";
import { useAtualizarAluno, useMatricularAluno } from "../hooks/useStudents";
import type { Aluno } from "../types";

const DOCUMENTOS_VAZIO: DocumentosAluno = {
  ensinoMedioInstituicao: null,
  ensinoMedioAnoConclusao: null,
  tituloEleitor: null,
  certificadoReservista: null,
};

const TIPOS_ANEXO: TipoDocumento[] = [
  "Cpf",
  "Rg",
  "CertidaoNascimento",
  "ComprovanteResidencia",
  "HistoricoEnsinoMedio",
  "TituloEleitor",
  "Reservista",
  "Outro",
];

interface StudentFormProps {
  /** Aluno em edição; null = matrícula. Montar só quando o modal deve abrir. */
  aluno: Aluno | null;
  onClose: () => void;
}

export function StudentForm({ aluno, onClose }: StudentFormProps) {
  const [nome, setNome] = useState(aluno?.nome ?? "");
  const [email, setEmail] = useState(aluno?.email ?? "");
  const [periodoAtual, setPeriodoAtual] = useState(aluno?.periodoAtual ?? 1);
  const [dadosPessoais, setDadosPessoais] = useState(aluno?.dadosPessoais ?? DADOS_PESSOAIS_VAZIO);
  const [documentos, setDocumentos] = useState(aluno?.documentos ?? DOCUMENTOS_VAZIO);
  const [criado, setCriado] = useState<{ id: string; senha: string } | null>(null);
  const matricular = useMatricularAluno();
  const atualizar = useAtualizarAluno();
  const mutation = aluno ? atualizar : matricular;
  const emailDisponivel = useEmailDisponivel(email, !aluno);

  const documento = (nome: "ensinoMedioInstituicao" | "tituloEleitor" | "certificadoReservista", maxLength: number) => (
    <input
      type="text"
      maxLength={maxLength}
      value={documentos[nome] ?? ""}
      onChange={(e) => setDocumentos({ ...documentos, [nome]: e.target.value || null })}
      className={inputClass}
    />
  );

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (aluno) {
      atualizar.mutate({ id: aluno.id, nome, periodoAtual, dadosPessoais, documentos }, { onSuccess: onClose });
    } else {
      matricular.mutate(
        { nome, email, periodoAtual, dadosPessoais, documentos },
        { onSuccess: (data) => setCriado({ id: data.aluno.id, senha: data.senhaTemporaria }) },
      );
    }
  }

  return (
    <FormDialog title={aluno ? "Editar aluno" : "Matricular aluno"} onClose={onClose} maxWidth="max-w-2xl">
      {criado ? (
        <SenhaTemporaria senha={criado.senha} destinatario="aluno" onClose={onClose}>
          <DocumentosAnexos pessoaId={criado.id} tipos={TIPOS_ANEXO} />
        </SenhaTemporaria>
      ) : (
        <>
          <form onSubmit={handleSubmit} className="space-y-3">
            <input type="text" placeholder="Nome" required value={nome} onChange={(e) => setNome(e.target.value)} className={inputClass} />
            <input
              type="email"
              placeholder="Email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              // Email é o login da conta: nunca editável depois de criado.
              disabled={Boolean(aluno)}
              title={aluno ? "O email é o login do aluno e não pode ser alterado." : undefined}
              className={`${inputClass} disabled:opacity-60`}
            />
            {emailDisponivel === false && (
              <p className="-mt-1 text-xs text-[var(--color-destructive)]">Já existe uma conta com este email.</p>
            )}
            <div className="flex items-end gap-3">
              {aluno && (
                <label className="flex-1 space-y-1">
                  <span className="text-xs text-[var(--color-muted-foreground)]">Matrícula</span>
                  {/* Gerada na criação (ano + semestre + aleatório) e nunca muda. */}
                  <input
                    type="text"
                    value={aluno.matricula}
                    disabled
                    title="A matrícula é gerada automaticamente e não pode ser alterada."
                    className={`${inputClass} font-mono tabular-nums disabled:opacity-60`}
                  />
                </label>
              )}
              <label className="w-24 space-y-1">
                <span className="text-xs text-[var(--color-muted-foreground)]">Período</span>
                <input
                  type="number"
                  min={1}
                  max={5}
                  required
                  value={periodoAtual}
                  onChange={(e) => setPeriodoAtual(Number(e.target.value))}
                  className={inputClass}
                />
              </label>
            </div>
            {!aluno && (
              <p className="text-xs text-[var(--color-muted-foreground)]">A matrícula é gerada automaticamente.</p>
            )}

            <DadosPessoaisFields value={dadosPessoais} onChange={setDadosPessoais} />

            <Secao titulo="Escolaridade, título e reservista">
              <Campo label="Escola do ensino médio" className="col-span-3">{documento("ensinoMedioInstituicao", 200)}</Campo>
              <Campo label="Ano de conclusão">
                <input
                  type="number"
                  min={1900}
                  max={new Date().getFullYear()}
                  value={documentos.ensinoMedioAnoConclusao ?? ""}
                  onChange={(e) =>
                    setDocumentos({ ...documentos, ensinoMedioAnoConclusao: e.target.value ? Number(e.target.value) : null })
                  }
                  className={inputClass}
                />
              </Campo>
              <Campo label="Título de eleitor" className="col-span-2">{documento("tituloEleitor", 20)}</Campo>
              <Campo label="Certificado de reservista" className="col-span-2">{documento("certificadoReservista", 30)}</Campo>
            </Secao>

            {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
            <button type="submit" disabled={mutation.isPending || emailDisponivel === false} className={`w-full ${primaryButtonClass}`}>
              {mutation.isPending ? "Salvando…" : aluno ? "Salvar" : "Matricular"}
            </button>
          </form>
          {aluno && (
            <div className="mt-4 border-t border-[var(--color-border)] pt-4">
              <DocumentosAnexos pessoaId={aluno.id} tipos={TIPOS_ANEXO} />
            </div>
          )}
        </>
      )}
    </FormDialog>
  );
}
