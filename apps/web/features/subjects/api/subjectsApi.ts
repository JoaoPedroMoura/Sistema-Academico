import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { AtualizarMateriaInput, CriarMateriaInput, Materia, Vinculo } from "../types";

export const subjectsApi = {
  listar: (auth: HttpClientOptions, pesquisa?: string) =>
    httpClient.get<Materia[]>(`/api/materias${pesquisa ? `?pesquisa=${encodeURIComponent(pesquisa)}` : ""}`, auth),

  criar: (auth: HttpClientOptions, input: CriarMateriaInput) => httpClient.post<Materia>("/api/materias", input, auth),

  atualizar: (auth: HttpClientOptions, id: string, input: AtualizarMateriaInput) =>
    httpClient.put<Materia>(`/api/materias/${id}`, input, auth),

  excluir: (auth: HttpClientOptions, id: string) => httpClient.delete<void>(`/api/materias/${id}`, auth),

  listarVinculos: (auth: HttpClientOptions) => httpClient.get<Vinculo[]>("/api/vinculos", auth),

  adicionarVinculo: (auth: HttpClientOptions, materiaId: string, professorId: string) =>
    httpClient.post<Vinculo>("/api/vinculos", { materiaId, professorId }, auth),

  removerVinculo: (auth: HttpClientOptions, materiaId: string, professorId: string) =>
    httpClient.delete<void>(`/api/vinculos?materiaId=${materiaId}&professorId=${professorId}`, auth),
};
