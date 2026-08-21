export type StatusSolicitacao = "Aberta" | "EmAnalise" | "Aprovada" | "Rejeitada";
export type TipoSolicitacao = "AtestadoMedico" | "RevisaoDeNota" | "JustificativaDeFalta" | "Outro";

export interface Solicitacao {
  id: string;
  alunoId: string;
  alunoNome: string;
  tipo: TipoSolicitacao;
  descricao: string;
  anexoUrl: string | null;
  status: StatusSolicitacao;
  resposta: string | null;
  abertaEmUtc: string;
  respondidaEmUtc: string | null;
}

export interface AbrirSolicitacaoInput {
  tipo: TipoSolicitacao;
  descricao: string;
  anexoUrl?: string | null;
}
