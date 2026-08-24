import { httpClient, ApiError, type HttpClientOptions } from "@/shared/api/httpClient";
import type { GerarGradeResult, Grade } from "../types";

export const scheduleApi = {
  gerar: (auth: HttpClientOptions) => httpClient.post<GerarGradeResult>("/api/grades/gerar", undefined, auth),

  obterAtiva: async (auth: HttpClientOptions): Promise<Grade | null> => {
    try {
      return await httpClient.get<Grade>("/api/grades/ativa", auth);
    } catch (error) {
      if (error instanceof ApiError && error.status === 404) {
        return null;
      }
      throw error;
    }
  },

  excluir: (auth: HttpClientOptions, id: string) => httpClient.delete<void>(`/api/grades/${id}`, auth),
};
