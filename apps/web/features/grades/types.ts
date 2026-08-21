export interface Nota {
  id: string;
  alunoId: string;
  alunoNome: string;
  turmaId: string;
  tipo: string;
  valor: number;
}

export interface LancarNotaInput {
  turmaId: string;
  alunoId: string;
  tipo: string;
  valor: number;
}

export interface MinhaNota {
  id: string;
  turmaId: string;
  materiaNome: string;
  tipo: string;
  valor: number;
}
