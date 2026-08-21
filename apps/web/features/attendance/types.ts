export interface Presenca {
  id: string;
  alunoId: string;
  alunoNome: string;
  turmaId: string;
  dataAula: string;
  presente: boolean;
  justificativa: string | null;
}

export interface RegistroPresencaInput {
  alunoId: string;
  presente: boolean;
}

export interface MinhaPresenca {
  id: string;
  turmaId: string;
  materiaNome: string;
  dataAula: string;
  presente: boolean;
  justificativa: string | null;
}
