import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { EnviarMaterialInput, Material, MeuMaterial } from "../types";

export const materialsApi = {
  listarPorTurma: (auth: HttpClientOptions, turmaId: string) =>
    httpClient.get<Material[]>(`/api/materiais?turmaId=${turmaId}`, auth),

  enviar: (auth: HttpClientOptions, input: EnviarMaterialInput) =>
    httpClient.post<Material>("/api/materiais", input, auth),

  listarMeus: (auth: HttpClientOptions) => httpClient.get<MeuMaterial[]>("/api/materiais/meus", auth),
};
