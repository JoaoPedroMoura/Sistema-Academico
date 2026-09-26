"use client";

import { useState, type FormEvent } from "react";
import { Campo, Secao } from "@/features/documents/components/Campo";
import { DadosPessoaisFields } from "@/features/documents/components/DadosPessoaisFields";
import { DocumentosAnexos } from "@/features/documents/components/DocumentosAnexos";
import { formatarTelefone } from "@/features/documents/mascaras";
import { DADOS_PESSOAIS_VAZIO, type FormacaoProfessor, type TipoDocumento, type Titulacao } from "@/features/documents/types";
import { FormDialog, SenhaTemporaria, inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useEmailDisponivel } from "@/shared/hooks/useEmailDisponivel";
import { useAtualizarProfessor, useCriarProfessor } from "../hooks/useTeachers";
import type { Professor } from "../types";
import { TeacherAvailabilityEditor } from "./TeacherAvailabilityEditor";

const TITULACOES: { value: Titulacao; label: string }[] = [
  { value: "Graduacao", label: "Graduação" },
  { value: "Especializacao", label: "Especialização" },
  { value: "Mestrado", label: "Mestrado" },
  { value: "Doutorado", label: "Doutorado" },
];

const TIPOS_ANEXO: TipoDocumento[] = ["Cpf", "Rg", "ComprovanteResidencia", "Diploma", "Outro"];

interface TeacherFormProps {
  /** Professor em edição; null = criação. Montar só quando o modal deve abrir. */
  professor: Professor | null;
  onClose: () => void;
}

export function TeacherForm({ professor, onClose }: TeacherFormProps) {
  const [nome, setNome] = useState(professor?.nome ?? "");
  const [email, setEmail] = useState(professor?.email ?? "");
  const [telefone, setTelefone] = useState(professor?.telefone ?? "");
  const [dadosPessoais, setDadosPessoais] = useState(professor?.dadosPessoais ?? DADOS_PESSOAIS_VAZIO);
  const [formacao, setFormacao] = useState<FormacaoProfessor>(professor?.formacao ?? { titulacao: null, lattesUrl: null });
  const [criado, setCriado] = useState<{ id: string; senha: string } | null>(null);
  const criar = useCriarProfessor();
  const atualizar = useAtualizarProfessor(professor?.id ?? "");
  const mutation = professor ? atualizar : criar;
  const emailDisponivel = useEmailDisponivel(email, !professor);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const dados = { nome, telefone: telefone || null, dadosPessoais, formacao };
    if (professor) {
      atualizar.mutate(dados, { onSuccess: onClose });
    } else {
      criar.mutate(
        { ...dados, email },
        { onSuccess: (data) => setCriado({ id: data.professor.id, senha: data.senhaTemporaria }) },
      );
    }
  }

  return (
    <FormDialog title={professor ? "Editar professor" : "Adicionar professor"} onClose={onClose} maxWidth="max-w-2xl">
      {criado ? (
        <SenhaTemporaria senha={criado.senha} destinatario="professor" onClose={onClose}>
          <TeacherAvailabilityEditor professorId={criado.id} />
          <div className="border-t border-[var(--color-border)] pt-4">
            <DocumentosAnexos pessoaId={criado.id} tipos={TIPOS_ANEXO} />
          </div>
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
              disabled={Boolean(professor)}
              title={professor ? "O email é o login do professor e não pode ser alterado." : undefined}
              className={`${inputClass} disabled:opacity-60`}
            />
            {emailDisponivel === false && (
              <p className="-mt-1 text-xs text-[var(--color-destructive)]">Já existe uma conta com este email.</p>
            )}
            <div className="flex gap-3">
              {professor && (
                <input
                  type="text"
                  value={professor.matricula}
                  disabled
                  aria-label="Matrícula"
                  title="A matrícula é gerada automaticamente e não pode ser alterada."
                  className={`${inputClass} w-40 font-mono tabular-nums disabled:opacity-60`}
                />
              )}
              <input
                type="tel"
                inputMode="numeric"
                placeholder="Telefone (opcional)"
                value={telefone}
                onChange={(e) => setTelefone(formatarTelefone(e.target.value))}
                pattern="\(\d{2}\) \d{4,5}-\d{4}"
                title="(99) 99999-9999"
                className={inputClass}
              />
            </div>

            <DadosPessoaisFields value={dadosPessoais} onChange={setDadosPessoais} />

            <Secao titulo="Formação">
              <Campo label="Maior titulação" className="col-span-2">
                <select
                  value={formacao.titulacao ?? ""}
                  onChange={(e) => setFormacao({ ...formacao, titulacao: (e.target.value || null) as Titulacao | null })}
                  className={inputClass}
                >
                  <option value="">—</option>
                  {TITULACOES.map((t) => (
                    <option key={t.value} value={t.value}>
                      {t.label}
                    </option>
                  ))}
                </select>
              </Campo>
              <Campo label="Currículo Lattes" className="col-span-2">
                <input
                  type="url"
                  placeholder="http://lattes.cnpq.br/…"
                  maxLength={300}
                  value={formacao.lattesUrl ?? ""}
                  onChange={(e) => setFormacao({ ...formacao, lattesUrl: e.target.value || null })}
                  className={inputClass}
                />
              </Campo>
            </Secao>

            {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
            <button type="submit" disabled={mutation.isPending || emailDisponivel === false} className={`w-full ${primaryButtonClass}`}>
              {mutation.isPending ? "Salvando…" : professor ? "Salvar" : "Adicionar"}
            </button>
          </form>
          {professor && (
            <>
              <div className="mt-4 border-t border-[var(--color-border)] pt-4">
                <DocumentosAnexos pessoaId={professor.id} tipos={TIPOS_ANEXO} />
              </div>
              <div className="mt-4 border-t border-[var(--color-border)] pt-4">
                <TeacherAvailabilityEditor professorId={professor.id} />
              </div>
            </>
          )}
        </>
      )}
    </FormDialog>
  );
}
