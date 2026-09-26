"use client";

import { useEffect, useRef, useState, type FormEvent } from "react";
import { useAtualizarProfessor, useCriarProfessor } from "../hooks/useTeachers";
import type { Professor } from "../types";

const inputClass =
  "w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-3 py-2 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]";

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
  const dialogRef = useRef<HTMLDialogElement>(null);
  const criar = useCriarProfessor();
  const atualizar = useAtualizarProfessor(professor?.id ?? "");
  const mutation = professor ? atualizar : criar;

  useEffect(() => {
    dialogRef.current?.showModal();
  }, []);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    const input = { nome, email, telefone: telefone || null };
    if (professor) {
      atualizar.mutate(input, { onSuccess: onClose });
    } else {
      criar.mutate(input, { onSuccess: (data) => setSenhaGerada(data.senhaTemporaria) });
    }
  }

  return (
    <dialog
      ref={dialogRef}
      onClose={onClose}
      className="m-auto w-full max-w-md rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)] p-4 text-inherit backdrop:bg-black/60"
    >
      <div className="mb-3 flex items-center justify-between">
        <h2 className="text-sm font-medium">{professor ? "Editar professor" : "Adicionar professor"}</h2>
        <button
          type="button"
          aria-label="Fechar"
          onClick={onClose}
          className="rounded-md px-2 text-[var(--color-muted-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
        >
          ✕
        </button>
      </div>
      {senhaGerada ? (
        <div className="space-y-3">
          <div className="rounded-md bg-[var(--color-muted)] p-3 text-sm">
            Conta criada. Senha temporária (compartilhe com o professor):{" "}
            <code className="font-mono font-semibold">{senhaGerada}</code>
          </div>
          <button
            type="button"
            onClick={onClose}
            className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
          >
            Fechar
          </button>
        </div>
      ) : (
        <form onSubmit={handleSubmit} className="space-y-3">
          <input type="text" placeholder="Nome" required value={nome} onChange={(e) => setNome(e.target.value)} className={inputClass} />
          <input type="email" placeholder="Email" required value={email} onChange={(e) => setEmail(e.target.value)} className={inputClass} />
          <input
            type="text"
            placeholder="Telefone (opcional)"
            value={telefone}
            onChange={(e) => setTelefone(e.target.value)}
            className={inputClass}
          />
          {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
          <button
            type="submit"
            disabled={mutation.isPending}
            className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
          >
            {mutation.isPending ? "Salvando…" : professor ? "Salvar" : "Adicionar"}
          </button>
        </form>
      )}
    </dialog>
  );
}
