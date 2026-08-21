import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type {
  AdicionarDisponibilidadeInput,
  AlunoResumo,
  AtualizarProfessorInput,
  CriarProfessorInput,
  MinhaTurma,
  Professor,
  ProfessorCriado,
} from "../types";

export const teachersApi = {
  listar: (auth: HttpClientOptions, pesquisa?: string) =>
    httpClient.get<Professor[]>(`/api/professores${pesquisa ? `?pesquisa=${encodeURIComponent(pesquisa)}` : ""}`, auth),

  obter: (auth: HttpClientOptions, id: string) => httpClient.get<Professor>(`/api/professores/${id}`, auth),

  criar: (auth: HttpClientOptions, input: CriarProfessorInput) =>
    httpClient.post<ProfessorCriado>("/api/professores", input, auth),

  atualizar: (auth: HttpClientOptions, id: string, input: AtualizarProfessorInput) =>
    httpClient.put<Professor>(`/api/professores/${id}`, input, auth),

  excluir: (auth: HttpClientOptions, id: string) => httpClient.delete<void>(`/api/professores/${id}`, auth),

  adicionarDisponibilidade: (auth: HttpClientOptions, id: string, input: AdicionarDisponibilidadeInput) =>
    httpClient.post<Professor>(`/api/professores/${id}/disponibilidades`, input, auth),

  removerDisponibilidade: (auth: HttpClientOptions, id: string, disponibilidadeId: string) =>
    httpClient.delete<Professor>(`/api/professores/${id}/disponibilidades/${disponibilidadeId}`, auth),

  // Self-service do professor autenticado (área Professor).
  meuPerfil: (auth: HttpClientOptions) => httpClient.get<Professor>("/api/professores/me", auth),

  minhasTurmas: (auth: HttpClientOptions) => httpClient.get<MinhaTurma[]>("/api/professores/me/turmas", auth),

  alunosDaTurma: (auth: HttpClientOptions, turmaId: string) =>
    httpClient.get<AlunoResumo[]>(`/api/professores/me/turmas/${turmaId}/alunos`, auth),

  adicionarMinhaDisponibilidade: (auth: HttpClientOptions, input: AdicionarDisponibilidadeInput) =>
    httpClient.post<Professor>("/api/professores/me/disponibilidades", input, auth),

  removerMinhaDisponibilidade: (auth: HttpClientOptions, disponibilidadeId: string) =>
    httpClient.delete<Professor>(`/api/professores/me/disponibilidades/${disponibilidadeId}`, auth),
};
