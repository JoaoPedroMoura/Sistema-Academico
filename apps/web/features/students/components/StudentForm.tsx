"use client";

import { useState, type FormEvent } from "react";
import { useMatricularAluno } from "../hooks/useStudents";

export function StudentForm() {
  const [nome, setNome] = useState("");
  const [email, setEmail] = useState("");
  const [matricula, setMatricula] = useState("");
  const [periodoAtual, setPeriodoAtual] = useState(1);
  const [senhaGerada, setSenhaGerada] = useState<string | null>(null);
  const matricular = useMatricularAluno();

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    matricular.mutate(
      { nome, email, matricula, periodoAtual },
      {
        onSuccess: (data) => {
          setSenhaGerada(data.senhaTemporaria);
          setNome("");
          setEmail("");
          setMatricula("");
        },
      },
    );
  }

  return (
    <div className="space-y-3 rounded-md border border-[var(--color-border)] p-4">
      <h2 className="text-sm font-medium">Matricular aluno</h2>
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
        <div className="flex gap-3">
          <input
            type="text"
            placeholder="Matrícula"
            required
            value={matricula}
            onChange={(e) => setMatricula(e.target.value)}
            className="flex-1 rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
          />
          <input
            type="number"
            min={1}
            max={5}
            required
            value={periodoAtual}
            onChange={(e) => setPeriodoAtual(Number(e.target.value))}
            className="w-24 rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
          />
        </div>
        {matricular.isError && <p className="text-sm text-[var(--color-destructive)]">{matricular.error.message}</p>}
        <button
          type="submit"
          disabled={matricular.isPending}
          className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
        >
          {matricular.isPending ? "Salvando…" : "Matricular"}
        </button>
      </form>
      {senhaGerada && (
        <div className="rounded-md bg-[var(--color-muted)] p-3 text-sm">
          Conta criada. Senha temporária (compartilhe com o aluno):{" "}
          <code className="font-mono font-semibold">{senhaGerada}</code>
        </div>
      )}
    </div>
  );
}
