"use client";

import { useState, type FormEvent } from "react";
import { useCriarProfessor } from "../hooks/useTeachers";

export function TeacherForm() {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [telefone, setTelefone] = useState("");
  const [senhaGerada, setSenhaGerada] = useState<string | null>(null);
  const criar = useCriarProfessor();

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
    <div className="space-y-3 rounded-md border border-[var(--color-border)] p-4">
      <h2 className="text-sm font-medium">Adicionar professor</h2>
      <form onSubmit={handleSubmit} className="space-y-3">
        <input
          type="text"
          placeholder="Nome"
          required
          value={nome}
          onChange={(e) => setNome(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        <input
          type="email"
          placeholder="Email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        <input
          type="text"
          placeholder="Telefone (opcional)"
          value={telefone}
          onChange={(e) => setTelefone(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
        {criar.isError && <p className="text-sm text-[var(--color-destructive)]">{criar.error.message}</p>}
        <button
          type="submit"
          disabled={criar.isPending}
          className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
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
    </div>
  );
}
