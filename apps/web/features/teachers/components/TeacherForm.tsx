"use client";

import { useState, type FormEvent } from "react";
import { FormDialog, SenhaTemporaria, inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useEmailDisponivel } from "@/shared/hooks/useEmailDisponivel";
import { useAtualizarProfessor, useCriarProfessor } from "../hooks/useTeachers";
import type { Professor } from "../types";

interface TeacherFormProps {
  /** Professor em edição; null = criação. Montar só quando o modal deve abrir. */
  professor: Professor | null;
  onClose: () => void;
}

export function TeacherForm({ professor, onClose }: TeacherFormProps) {
  const [nome, setNome] = useState(professor?.nome ?? "");
  const [email, setEmail] = useState(professor?.email ?? "");
  const [telefone, setTelefone] = useState(professor?.telefone ?? "");
  const [senhaGerada, setSenhaGerada] = useState<string | null>(null);
  const criar = useCriarProfessor();
  const atualizar = useAtualizarProfessor(professor?.id ?? "");
  const mutation = professor ? atualizar : criar;
  const emailDisponivel = useEmailDisponivel(email, !professor);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    if (professor) {
      atualizar.mutate({ nome, telefone: telefone || null }, { onSuccess: onClose });
    } else {
      criar.mutate({ nome, email, telefone: telefone || null }, { onSuccess: (data) => setSenhaGerada(data.senhaTemporaria) });
    }
  }

  return (
    <FormDialog title={professor ? "Editar professor" : "Adicionar professor"} onClose={onClose}>
      {senhaGerada ? (
        <SenhaTemporaria senha={senhaGerada} destinatario="professor" onClose={onClose} />
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
            disabled={Boolean(professor)}
            title={professor ? "O email é o login do professor e não pode ser alterado." : undefined}
            className={`${inputClass} disabled:opacity-60`}
          />
          {emailDisponivel === false && (
            <p className="-mt-1 text-xs text-[var(--color-destructive)]">Já existe uma conta com este email.</p>
          )}
          <input
            type="text"
            placeholder="Telefone (opcional)"
            value={telefone}
            onChange={(e) => setTelefone(e.target.value)}
            className={inputClass}
          />
          {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
          <button type="submit" disabled={mutation.isPending || emailDisponivel === false} className={`w-full ${primaryButtonClass}`}>
            {mutation.isPending ? "Salvando…" : professor ? "Salvar" : "Adicionar"}
          </button>
        </form>
      )}
    </FormDialog>
  );
}
