import { httpClient, type HttpClientOptions } from "@/shared/api/httpClient";
import type { DocumentoAnexo, TipoDocumento } from "../types";

export const documentsApi = {
  listar: (auth: HttpClientOptions, pessoaId: string) =>
    httpClient.get<DocumentoAnexo[]>(`/api/documentos?pessoaId=${pessoaId}`, auth),

  enviar: (auth: HttpClientOptions, pessoaId: string, tipo: TipoDocumento, arquivo: File) => {
    const form = new FormData();
    form.append("pessoaId", pessoaId);
    form.append("tipo", tipo);
    form.append("arquivo", arquivo);
    return httpClient.upload<DocumentoAnexo>("/api/documentos", form, auth);
  },

  baixar: (auth: HttpClientOptions, id: string) => httpClient.blob(`/api/documentos/${id}/arquivo`, auth),

  remover: (auth: HttpClientOptions, id: string) => httpClient.delete<void>(`/api/documentos/${id}`, auth),
};
