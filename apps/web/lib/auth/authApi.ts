import { httpClient } from "@/shared/api/httpClient";
import type { Role } from "./types";

export interface LoginPayload {
  email: string;
  password: string;
  tenantSlug?: string;
}

export interface TenantOption {
  slug: string;
  nome: string;
  role: Role;
}

export interface LoginResponse {
  requiresTenantSelection: boolean;
  tenantOptions: TenantOption[];
  accessToken?: string;
  accessTokenExpiresAtUtc?: string;
  accountId?: string;
  nome?: string;
  email?: string;
  tenantSlug?: string;
  tenantNome?: string;
  role?: Role;
}

export interface MeResponse {
  accountId: string;
  nome: string;
  email: string;
  tenantSlug: string;
  tenantNome: string;
  role: Role;
}

/**
 * Chama as rotas BFF do próprio Next.js (`app/api/auth/*`), não a API .NET diretamente — é isso
 * que faz o cookie httpOnly viver na origem do frontend (necessário para o proxy.ts funcionar,
 * ver `app/api/auth/_lib.ts`). `credentials: "include"` aqui é redundante (mesma origem já manda
 * cookies por padrão) mas inofensivo, deixado por clareza.
 */
async function authFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(path, {
    ...init,
    headers: { "Content-Type": "application/json", ...init?.headers },
    credentials: "include",
  });

  if (!response.ok) {
    const body = await response.json().catch(() => undefined);
    throw new Error(body?.message ?? response.statusText);
  }
  if (response.status === 204) {
    return undefined as T;
  }
  return (await response.json()) as T;
}

export const authApi = {
  login: (payload: LoginPayload) =>
    authFetch<LoginResponse>("/api/auth/login", { method: "POST", body: JSON.stringify(payload) }),

  refresh: () => authFetch<LoginResponse>("/api/auth/refresh", { method: "POST" }),

  logout: () => authFetch<void>("/api/auth/logout", { method: "POST" }),

  // Este vai direto na API .NET (Bearer token, sem cookie) — não passa pelo BFF.
  me: (accessToken: string, tenantSlug: string) =>
    httpClient.get<MeResponse>("/api/me", { accessToken, tenantSlug }),
};
