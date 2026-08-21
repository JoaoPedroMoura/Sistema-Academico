import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { Aluno, AlunoMatriculado, MatricularAlunoInput, MeuPerfilAluno } from "../types";

export const studentsApi = {
  listar: (auth: HttpClientOptions, pesquisa?: string) =>
    httpClient.get<Aluno[]>(`/api/alunos${pesquisa ? `?pesquisa=${encodeURIComponent(pesquisa)}` : ""}`, auth),

  matricular: (auth: HttpClientOptions, input: MatricularAlunoInput) =>
    httpClient.post<AlunoMatriculado>("/api/alunos", input, auth),

  avancarPeriodo: (auth: HttpClientOptions, id: string, novoPeriodo: number) =>
    httpClient.put<Aluno>(`/api/alunos/${id}`, { novoPeriodo }, auth),

  meuPerfil: (auth: HttpClientOptions) => httpClient.get<MeuPerfilAluno>("/api/alunos/me", auth),
};
