"use client";

import { useState, type FormEvent } from "react";
import { FormDialog, SenhaTemporaria, inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useEmailDisponivel } from "@/shared/hooks/useEmailDisponivel";
import { useAtualizarAluno, useMatricularAluno } from "../hooks/useStudents";
import type { Aluno } from "../types";

interface StudentFormProps {
  /** Aluno em edição; null = matrícula. Montar só quando o modal deve abrir. */
  aluno: Aluno | null;
  onClose: () => void;
}

export function StudentForm({ aluno, onClose }: StudentFormProps) {
  const [nome, setNome] = useState(aluno?.nome ?? "");
  const [email, setEmail] = useState(aluno?.email ?? "");
  const [periodoAtual, setPeriodoAtual] = useState(aluno?.periodoAtual ?? 1);
  const [senhaGerada, setSenhaGerada] = useState<string | null>(null);
  const matricular = useMatricularAluno();
  const atualizar = useAtualizarAluno();
  const mutation = aluno ? atualizar : matricular;
  const emailDisponivel = useEmailDisponivel(email, !aluno);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (aluno) {
      atualizar.mutate({ id: aluno.id, nome, periodoAtual }, { onSuccess: onClose });
    } else {
      matricular.mutate(
        { nome, email, periodoAtual },
        { onSuccess: (data) => setSenhaGerada(data.senhaTemporaria) },
      );
    }
  }

  return (
    <FormDialog title={aluno ? "Editar aluno" : "Matricular aluno"} onClose={onClose}>
      {senhaGerada ? (
        <SenhaTemporaria senha={senhaGerada} destinatario="aluno" onClose={onClose} />
      ) : (
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
          {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
          <button type="submit" disabled={mutation.isPending || emailDisponivel === false} className={`w-full ${primaryButtonClass}`}>
            {mutation.isPending ? "Salvando…" : aluno ? "Salvar" : "Matricular"}
          </button>
        </form>
      )}
    </FormDialog>
  );
}
