export interface Turma {
  id: string;
  materiaId: string;
  materiaNome: string;
  professorId: string;
  professorNome: string;
  dia: string;
  horaInicio: string;
  horaFim: string;
  periodoCurricular: number;
}

export interface Grade {
  id: string;
  status: string;
  geradoEmUtc: string;
  custoSolucao: number | null;
  turmas: Turma[];
}

export interface GerarGradeResult {
  grade: Grade;
  completa: boolean;
  materiasNaoAlocadas: string[];
}
