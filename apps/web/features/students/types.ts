export interface Aluno {
  id: string;
  nome: string;
  email: string;
  matricula: string;
  periodoAtual: number;
  ativo: boolean;
}

export interface MatricularAlunoInput {
  nome: string;
  email: string;
  matricula: string;
  periodoAtual: number;
}

export interface AlunoMatriculado {
  aluno: Aluno;
  senhaTemporaria: string;
}

export type MeuPerfilAluno = Aluno;
