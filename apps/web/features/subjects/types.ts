export interface Materia {
  id: string;
  nome: string;
  periodo: number;
  cargaHorariaSemanal: number;
  ativa: boolean;
}

export interface CriarMateriaInput {
  nome: string;
  periodo: number;
  cargaHorariaSemanal: number;
}

export type AtualizarMateriaInput = CriarMateriaInput;

export interface Vinculo {
  id: string;
  materiaId: string;
  materiaNome: string;
  professorId: string;
  professorNome: string;
}
