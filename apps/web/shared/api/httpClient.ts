/**
 * Cliente HTTP base. Toda chamada à API (Fase 6+) passa por aqui — nenhuma feature deve chamar
 * `fetch` diretamente. Isolado para centralizar: base URL, header de tenant, token de acesso,
 * e tratamento uniforme de erro.
 */

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5100";

export class ApiError extends Error {
  constructor(
    public readonly status: number,
    message: string,
    public readonly details?: unknown,
  ) {
    super(message);
    this.name = "ApiError";
  }
}

export interface HttpClientOptions {
  accessToken?: string | null;
  tenantSlug?: string | null;
}

async function request<TResponse>(
  path: string,
  init: RequestInit,
  options: HttpClientOptions = {},
): Promise<TResponse> {
  const headers = new Headers(init.headers);
  headers.set("Content-Type", "application/json");

  if (options.accessToken) {
    headers.set("Authorization", `Bearer ${options.accessToken}`);
  }
  // Resolução de tenant em dev, via header — ver ARCHITECTURE.md §3.3.
  if (options.tenantSlug) {
    headers.set("X-Tenant-Slug", options.tenantSlug);
  }

  // "include": necessário para o cookie httpOnly de refresh token ir/voltar entre front e API
  // (origens diferentes mesmo em dev — ver ARCHITECTURE.md §5.2 e Program.cs da Api, CORS
  // AllowCredentials).
  const response = await fetch(`${API_BASE_URL}${path}`, { ...init, headers, credentials: "include" });

  if (!response.ok) {
    const body = await response.json().catch(() => undefined);
    throw new ApiError(response.status, body?.message ?? response.statusText, body);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return (await response.json()) as TResponse;
}

export const httpClient = {
  get: <T>(path: string, options?: HttpClientOptions) =>
    request<T>(path, { method: "GET" }, options),
  post: <T>(path: string, body?: unknown, options?: HttpClientOptions) =>
    request<T>(path, { method: "POST", body: body ? JSON.stringify(body) : undefined }, options),
  put: <T>(path: string, body?: unknown, options?: HttpClientOptions) =>
    request<T>(path, { method: "PUT", body: body ? JSON.stringify(body) : undefined }, options),
  patch: <T>(path: string, body?: unknown, options?: HttpClientOptions) =>
    request<T>(path, { method: "PATCH", body: body ? JSON.stringify(body) : undefined }, options),
  delete: <T>(path: string, options?: HttpClientOptions) =>
    request<T>(path, { method: "DELETE" }, options),
};
