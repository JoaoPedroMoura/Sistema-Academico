"use client";

import { useState, type FormEvent } from "react";
import { FormDialog, SenhaTemporaria, inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useEmailDisponivel } from "@/shared/hooks/useEmailDisponivel";
import { useAtualizarProfessor, useCriarProfessor } from "../hooks/useTeachers";
import type { Professor } from "../types";
import { TeacherAvailabilityEditor } from "./TeacherAvailabilityEditor";

/** Máscara BR: (99) 9999-9999 ou (99) 99999-9999, conforme a quantidade de dígitos. */
function formatarTelefone(valor: string) {
  const d = valor.replace(/\D/g, "").slice(0, 11);
  if (d.length <= 2) return d.length ? `(${d}` : "";
  const meio = d.length === 11 ? 7 : 6;
  return `(${d.slice(0, 2)}) ${d.slice(2, meio)}${d.length > meio ? `-${d.slice(meio)}` : ""}`;
}

interface TeacherFormProps {
  /** Professor em edição; null = criação. Montar só quando o modal deve abrir. */
  professor: Professor | null;
  onClose: () => void;
}

export function TeacherForm({ professor, onClose }: TeacherFormProps) {
  const [nome, setNome] = useState(professor?.nome ?? "");
  const [email, setEmail] = useState(professor?.email ?? "");
  const [telefone, setTelefone] = useState(professor?.telefone ?? "");
  const [criado, setCriado] = useState<{ id: string; senha: string } | null>(null);
  const criar = useCriarProfessor();
  const atualizar = useAtualizarProfessor(professor?.id ?? "");
  const mutation = professor ? atualizar : criar;
  const emailDisponivel = useEmailDisponivel(email, !professor);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (professor) {
      atualizar.mutate({ nome, telefone: telefone || null }, { onSuccess: onClose });
    } else {
      criar.mutate({ nome, email, telefone: telefone || null }, { onSuccess: (data) => setCriado({ id: data.professor.id, senha: data.senhaTemporaria }) });
    }
  }

  return (
    <FormDialog title={professor ? "Editar professor" : "Adicionar professor"} onClose={onClose} maxWidth="max-w-2xl">
      {criado ? (
        <SenhaTemporaria senha={criado.senha} destinatario="professor" onClose={onClose}>
          <TeacherAvailabilityEditor professorId={criado.id} />
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
            {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
            <button type="submit" disabled={mutation.isPending || emailDisponivel === false} className={`w-full ${primaryButtonClass}`}>
              {mutation.isPending ? "Salvando…" : professor ? "Salvar" : "Adicionar"}
            </button>
          </form>
          {professor && (
            <div className="mt-4 border-t border-[var(--color-border)] pt-4">
              <TeacherAvailabilityEditor professorId={professor.id} />
            </div>
          )}
        </>
      )}
    </FormDialog>
  );
}
