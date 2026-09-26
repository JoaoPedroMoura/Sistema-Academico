/** Espelha Domain.Common.DadosPessoais — tudo opcional; CPF/CEP chegam só com dígitos. */
export interface DadosPessoais {
  cpf: string | null;
  rgNumero: string | null;
  rgOrgaoEmissor: string | null;
  rgUf: string | null;
  rgDataEmissao: string | null;
  dataNascimento: string | null;
  naturalidade: string | null;
  nomeMae: string | null;
  nomePai: string | null;
  cep: string | null;
  logradouro: string | null;
  numero: string | null;
  complemento: string | null;
  bairro: string | null;
  cidade: string | null;
  uf: string | null;
}

export interface DocumentosAluno {
  ensinoMedioInstituicao: string | null;
  ensinoMedioAnoConclusao: number | null;
  tituloEleitor: string | null;
  certificadoReservista: string | null;
}

export type Titulacao = "Graduacao" | "Especializacao" | "Mestrado" | "Doutorado";

export interface FormacaoProfessor {
  titulacao: Titulacao | null;
  lattesUrl: string | null;
}

export type TipoDocumento =
  | "Cpf"
  | "Rg"
  | "CertidaoNascimento"
  | "ComprovanteResidencia"
  | "HistoricoEnsinoMedio"
  | "TituloEleitor"
  | "Reservista"
  | "Diploma"
  | "Outro";

export interface DocumentoAnexo {
  id: string;
  tipo: TipoDocumento;
  nomeArquivo: string;
  contentType: string;
  tamanhoBytes: number;
  enviadoEmUtc: string;
}

export const DADOS_PESSOAIS_VAZIO: DadosPessoais = {
  cpf: null,
  rgNumero: null,
  rgOrgaoEmissor: null,
  rgUf: null,
  rgDataEmissao: null,
  dataNascimento: null,
  naturalidade: null,
  nomeMae: null,
  nomePai: null,
  cep: null,
  logradouro: null,
  numero: null,
  complemento: null,
  bairro: null,
  cidade: null,
  uf: null,
};
