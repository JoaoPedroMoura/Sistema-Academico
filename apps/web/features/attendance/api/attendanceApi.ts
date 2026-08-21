import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { MinhaPresenca, Presenca, RegistroPresencaInput } from "../types";

export const attendanceApi = {
  listar: (auth: HttpClientOptions, turmaId: string, data: string) =>
    httpClient.get<Presenca[]>(`/api/presencas?turmaId=${turmaId}&data=${data}`, auth),

  registrar: (auth: HttpClientOptions, turmaId: string, dataAula: string, registros: RegistroPresencaInput[]) =>
    httpClient.post<Presenca[]>("/api/presencas", { turmaId, dataAula, registros }, auth),

  listarMinhas: (auth: HttpClientOptions) => httpClient.get<MinhaPresenca[]>("/api/presencas/minhas", auth),
};
