"use client";

import { useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useSession } from "./SessionProvider";
import { useTrocarSenha } from "./useTrocarSenha";
import { roleHomePath } from "./roleRouting";
import { LogoutButton } from "@/shared/components/LogoutButton";

const TAMANHO_MINIMO = 8;

export function TrocarSenhaForm() {
  const { session } = useSession();
  const router = useRouter();
  const trocarSenha = useTrocarSenha();

  const [senhaAtual, setSenhaAtual] = useState("");
  const [novaSenha, setNovaSenha] = useState("");
  const [confirmacao, setConfirmacao] = useState("");
  const [erroLocal, setErroLocal] = useState<string | null>(null);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErroLocal(null);

    if (novaSenha.length < TAMANHO_MINIMO) {
      setErroLocal(`A nova senha precisa ter pelo menos ${TAMANHO_MINIMO} caracteres.`);
      return;
    }
    if (novaSenha !== confirmacao) {
      setErroLocal("A confirmação não bate com a nova senha.");
      return;
    }

    trocarSenha.mutate(
      { senhaAtual, novaSenha },
      {
        onSuccess: () => {
          if (session) {
            router.push(roleHomePath(session.role));
          }
        },
      },
    );
  }

  const erro = erroLocal ?? (trocarSenha.isError ? trocarSenha.error.message : null);

  return (
    <div className="w-full max-w-sm space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-lg font-semibold">Troque sua senha</h1>
        <LogoutButton />
      </div>
      <p className="text-sm text-[var(--color-muted-foreground)]">
        Você está usando uma senha temporária. Defina uma senha própria para continuar.
      </p>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div className="space-y-1">
          <label htmlFor="senhaAtual" className="text-sm font-medium">
            Senha temporária
          </label>
          <input
            id="senhaAtual"
            type="password"
            required
            value={senhaAtual}
            onChange={(e) => setSenhaAtual(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
          />
        </div>
        <div className="space-y-1">
          <label htmlFor="novaSenha" className="text-sm font-medium">
            Nova senha
          </label>
          <input
            id="novaSenha"
            type="password"
            required
            minLength={TAMANHO_MINIMO}
            value={novaSenha}
            onChange={(e) => setNovaSenha(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
          />
        </div>
        <div className="space-y-1">
          <label htmlFor="confirmacao" className="text-sm font-medium">
            Confirmar nova senha
          </label>
          <input
            id="confirmacao"
            type="password"
            required
            value={confirmacao}
            onChange={(e) => setConfirmacao(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
          />
        </div>

        {erro && <p className="text-sm text-[var(--color-destructive)]">{erro}</p>}

        <button
          type="submit"
          disabled={trocarSenha.isPending}
          className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
        >
          {trocarSenha.isPending ? "Salvando…" : "Trocar senha"}
        </button>
      </form>
    </div>
  );
}
