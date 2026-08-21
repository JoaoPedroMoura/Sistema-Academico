export interface Material {
  id: string;
  turmaId: string;
  titulo: string;
  descricao: string | null;
  arquivoUrl: string;
  arquivoNomeOriginal: string;
  tamanhoBytes: number;
  enviadoEmUtc: string;
}

export interface EnviarMaterialInput {
  turmaId: string;
  titulo: string;
  descricao?: string | null;
  arquivoUrl: string;
  arquivoNomeOriginal: string;
  tamanhoBytes: number;
}

export interface MeuMaterial {
  id: string;
  turmaId: string;
  materiaNome: string;
  titulo: string;
  descricao: string | null;
  arquivoUrl: string;
  arquivoNomeOriginal: string;
  tamanhoBytes: number;
  enviadoEmUtc: string;
}
