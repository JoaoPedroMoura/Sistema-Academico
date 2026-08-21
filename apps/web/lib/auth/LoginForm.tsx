"use client";

import { useState, type FormEvent } from "react";
import { useLogin } from "./useLogin";
import type { TenantOption } from "./authApi";

/**
 * Formulário de login, incluindo o fluxo de seleção de unidade quando a conta tem acesso a mais
 * de um tenant (ARCHITECTURE.md §3.2) — primeiro submit sem tenantSlug, API responde com a lista
 * de opções, usuário escolhe uma e o form resubmete já com o slug.
 */
export function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [tenantOptions, setTenantOptions] = useState<TenantOption[] | null>(null);
  const login = useLogin();

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    login.mutate(
      { email, password },
      {
        onSuccess: (data) => {
          setTenantOptions(data.requiresTenantSelection ? data.tenantOptions : null);
        },
      },
    );
  }

  function handleEscolherTenant(slug: string) {
    login.mutate({ email, password, tenantSlug: slug });
  }

  if (tenantOptions) {
    return (
      <div className="w-full max-w-sm space-y-4">
        <h2 className="text-base font-medium">Escolha a unidade</h2>
        <div className="space-y-2">
          {tenantOptions.map((option) => (
            <button
              key={option.slug}
              type="button"
              onClick={() => handleEscolherTenant(option.slug)}
              disabled={login.isPending}
              className="w-full rounded-md border border-[var(--color-border)] px-4 py-3 text-left text-sm hover:bg-[var(--color-muted)] disabled:opacity-50"
            >
              <div className="font-medium">{option.nome}</div>
              <div className="text-[var(--color-muted-foreground)]">{option.role}</div>
            </button>
          ))}
        </div>
        {login.isError && <p className="text-sm text-[var(--color-destructive)]">{login.error.message}</p>}
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit} className="w-full max-w-sm space-y-4">
      <div className="space-y-1">
        <label htmlFor="email" className="text-sm font-medium">
          Email
        </label>
        <input
          id="email"
          type="email"
          required
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
      </div>
      <div className="space-y-1">
        <label htmlFor="password" className="text-sm font-medium">
          Senha
        </label>
        <input
          id="password"
          type="password"
          required
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="w-full rounded-md border border-[var(--color-border)] bg-[var(--color-background)] px-3 py-2 text-sm outline-none focus:border-[var(--color-primary)]"
        />
      </div>
      {login.isError && <p className="text-sm text-[var(--color-destructive)]">{login.error.message}</p>}
      <button
        type="submit"
        disabled={login.isPending}
        className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50"
      >
        {login.isPending ? "Entrando…" : "Entrar"}
      </button>
    </form>
  );
}
