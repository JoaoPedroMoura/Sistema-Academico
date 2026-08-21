import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { LancarNotaInput, MinhaNota, Nota } from "../types";

export const gradesApi = {
  listarPorTurma: (auth: HttpClientOptions, turmaId: string) =>
    httpClient.get<Nota[]>(`/api/notas?turmaId=${turmaId}`, auth),

  lancar: (auth: HttpClientOptions, input: LancarNotaInput) => httpClient.post<Nota>("/api/notas", input, auth),

  revisar: (auth: HttpClientOptions, id: string, novoValor: number) =>
    httpClient.put<Nota>(`/api/notas/${id}`, { novoValor }, auth),

  listarMinhas: (auth: HttpClientOptions) => httpClient.get<MinhaNota[]>("/api/notas/minhas", auth),
};
