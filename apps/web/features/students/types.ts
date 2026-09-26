import type { DadosPessoais, DocumentosAluno } from "@/features/documents/types";

export interface Aluno {
  id: string;
  nome: string;
  email: string;
  matricula: string;
  periodoAtual: number;
  ativo: boolean;
  dadosPessoais: DadosPessoais;
  documentos: DocumentosAluno;
}

export interface MatricularAlunoInput {
  nome: string;
  email: string;
  periodoAtual: number;
  dadosPessoais: DadosPessoais;
  documentos: DocumentosAluno;
}

export interface AtualizarAlunoInput {
  nome: string;
  periodoAtual: number;
  dadosPessoais: DadosPessoais;
  documentos: DocumentosAluno;
}

export interface AlunoMatriculado {
  aluno: Aluno;
  senhaTemporaria: string;
}

export type MeuPerfilAluno = Aluno;
