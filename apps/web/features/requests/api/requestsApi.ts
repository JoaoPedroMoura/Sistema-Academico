import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { AbrirSolicitacaoInput, Solicitacao, StatusSolicitacao } from "../types";

export const requestsApi = {
  listar: (auth: HttpClientOptions, status?: StatusSolicitacao) =>
    httpClient.get<Solicitacao[]>(`/api/solicitacoes${status ? `?status=${status}` : ""}`, auth),

  listarMinhas: (auth: HttpClientOptions) => httpClient.get<Solicitacao[]>("/api/solicitacoes/minhas", auth),

  abrir: (auth: HttpClientOptions, input: AbrirSolicitacaoInput) =>
    httpClient.post<Solicitacao>("/api/solicitacoes", input, auth),

  marcarEmAnalise: (auth: HttpClientOptions, id: string) =>
    httpClient.post<Solicitacao>(`/api/solicitacoes/${id}/em-analise`, undefined, auth),

  aprovar: (auth: HttpClientOptions, id: string, resposta?: string) =>
    httpClient.post<Solicitacao>(`/api/solicitacoes/${id}/aprovar`, { resposta }, auth),

  rejeitar: (auth: HttpClientOptions, id: string, resposta: string) =>
    httpClient.post<Solicitacao>(`/api/solicitacoes/${id}/rejeitar`, { resposta }, auth),
};
