"use client";

import { useEffect, useRef, useState, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { useLogin } from "./useLogin";
import { authApi, type TenantOption } from "./authApi";
import { useSession } from "./SessionProvider";
import { roleHomePath } from "./roleRouting";

/**
 * Formulário de login, incluindo o fluxo de seleção de unidade quando a conta tem acesso a mais
 * de um tenant (ARCHITECTURE.md §3.2) — primeiro submit sem tenantSlug, API responde com a lista
 * de opções, usuário escolhe uma e o form resubmete já com o slug.
 */
export function LoginForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [senhaVisivel, setSenhaVisivel] = useState(false);
  const [tenantOptions, setTenantOptions] = useState<TenantOption[] | null>(null);
  const login = useLogin();
  const { session, setSession } = useSession();
  const router = useRouter();
  const checagemFeita = useRef(false);

  // Já logado (sessão em memória ou refresh token válido no cookie)? Vai direto pra home do papel.
  // O ref evita refresh duplo no StrictMode — o refresh token é rotacionado a cada uso.
  useEffect(() => {
    if (session) {
      router.replace(session.precisaTrocarSenha ? "/trocar-senha" : roleHomePath(session.role));
      return;
    }
    if (checagemFeita.current) {
      return;
    }
    checagemFeita.current = true;

    authApi
      .refresh()
      .then((data) => {
        if (!data.accessToken || !data.role || !data.tenantSlug || !data.accountId || !data.email || !data.nome || !data.tenantNome) {
          return;
        }
        setSession(
          {
            accountId: data.accountId,
            name: data.nome,
            email: data.email,
            tenantSlug: data.tenantSlug,
            tenantName: data.tenantNome,
            role: data.role,
            precisaTrocarSenha: data.precisaTrocarSenha ?? false,
          },
          data.accessToken,
        );
      })
      .catch(() => {}); // sem sessão válida: fica no login
  }, [session, setSession, router]);

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
              className="w-full rounded-md border border-[var(--color-border)] px-4 py-3 text-left text-sm hover:bg-[var(--color-muted)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
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
          className="w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-3 py-2 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
        />
      </div>
      <div className="space-y-1">
        <label htmlFor="password" className="text-sm font-medium">
          Senha
        </label>
        <div className="relative">
          <input
            id="password"
            type={senhaVisivel ? "text" : "password"}
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] py-2 pr-10 pl-3 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]"
          />
          <button
            type="button"
            onClick={() => setSenhaVisivel((v) => !v)}
            aria-label={senhaVisivel ? "Ocultar senha" : "Mostrar senha"}
            aria-pressed={senhaVisivel}
            className="absolute inset-y-0 right-0 flex items-center rounded-md px-3 text-[var(--color-muted-foreground)] hover:text-[var(--color-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
          >
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden>
              <path d="M2 12s3.5-7 10-7 10 7 10 7-3.5 7-10 7S2 12 2 12z" />
              <circle cx="12" cy="12" r="3" />
              {senhaVisivel && <path d="M3 3l18 18" />}
            </svg>
          </button>
        </div>
      </div>
      {login.isError && <p className="text-sm text-[var(--color-destructive)]">{login.error.message}</p>}
      <button
        type="submit"
        disabled={login.isPending}
        className="w-full rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
      >
        {login.isPending ? "Entrando…" : "Entrar"}
      </button>
    </form>
  );
}
