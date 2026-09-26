"use client";

import { useRef, useState, type FormEvent } from "react";
import { useCriarProfessor } from "../hooks/useTeachers";

export function TeacherForm() {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [telefone, setTelefone] = useState("");
  const [senhaGerada, setSenhaGerada] = useState<string | null>(null);
  const criar = useCriarProfessor();
  const dialogRef = useRef<HTMLDialogElement>(null);

  function fechar() {
    setSenhaGerada(null);
    criar.reset();
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    criar.mutate(
      { nome, email, telefone: telefone || null },
      {
        onSuccess: (data) => {
          setSenhaGerada(data.senhaTemporaria);
          setNome("");
          setEmail("");
          setTelefone("");
        },
      },
    );
  }

  return (
    <>
      <button
        type="button"
        onClick={() => dialogRef.current?.showModal()}
        className="rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
      >
        Adicionar professor
      </button>
      <dialog
        ref={dialogRef}
        onClose={fechar}
        className="m-auto w-full max-w-md space-y-3 rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)] p-4 text-inherit backdrop:bg-black/60"
      >
      <div className="flex items-center justify-between">
        <h2 className="text-sm font-medium">Adicionar professor</h2>
        <button
          type="button"
          aria-label="Fechar"
          onClick={() => dialogRef.current?.close()}
          className="rounded-md px-2 text-[var(--color-muted-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
        >
          ✕
        </button>
      </div>
      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="text"
          placeholder="Nome"
          required
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-3 py-2 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
        />
        <input
          type="email"
          placeholder="Email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-3 py-2 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
        />
        <input
          type="text"
          placeholder="Telefone (opcional)"
          value={telefone}
          onChange={(e) => setTelefone(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-3 py-2 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
        />
        {criar.isError && <p className="text-sm text-[var(--color-destructive)]">{criar.error.message}</p>}
        <button
          type="submit"
          disabled={criar.isPending}
          className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
        >
          {criar.isPending ? "Salvando…" : "Adicionar"}
        </button>
      </form>
      {senhaGerada && (
        <div className="rounded-md bg-[var(--color-muted)] p-3 text-sm">
          Conta criada. Senha temporária (compartilhe com o professor):{" "}
          <code className="font-mono font-semibold">{senhaGerada}</code>
        </div>
      )}
      </dialog>
    </>
  );
}
