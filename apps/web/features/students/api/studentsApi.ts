import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { Aluno, AlunoMatriculado, AtualizarAlunoInput, MatricularAlunoInput, MeuPerfilAluno } from "../types";

export const studentsApi = {
  listar: (auth: HttpClientOptions, pesquisa?: string) =>
    httpClient.get<Aluno[]>(`/api/alunos${pesquisa ? `?pesquisa=${encodeURIComponent(pesquisa)}` : ""}`, auth),

  matricular: (auth: HttpClientOptions, input: MatricularAlunoInput) =>
    httpClient.post<AlunoMatriculado>("/api/alunos", input, auth),

  atualizar: (auth: HttpClientOptions, id: string, input: AtualizarAlunoInput) =>
    httpClient.put<Aluno>(`/api/alunos/${id}`, input, auth),

  meuPerfil: (auth: HttpClientOptions) => httpClient.get<MeuPerfilAluno>("/api/alunos/me", auth),
};
