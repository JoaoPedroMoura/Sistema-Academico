import type { DadosPessoais, FormacaoProfessor } from "@/features/documents/types";

export interface Disponibilidade {
  id: string;
  dia: string;
  horaInicio: string;
  horaFim: string;
}

export interface Professor {
  id: string;
  nome: string;
  email: string;
  matricula: string;
  telefone: string | null;
  ativo: boolean;
  disponibilidades: Disponibilidade[];
  dadosPessoais: DadosPessoais;
  formacao: FormacaoProfessor;
}

export interface ProfessorCriado {
  professor: Professor;
  senhaTemporaria: string;
}

export interface CriarProfessorInput {
  nome: string;
  email: string;
  telefone?: string | null;
  dadosPessoais: DadosPessoais;
  formacao: FormacaoProfessor;
}

export interface AtualizarProfessorInput {
  nome: string;
  telefone?: string | null;
  dadosPessoais: DadosPessoais;
  formacao: FormacaoProfessor;
}

export interface AdicionarDisponibilidadeInput {
  dia: string;
  horaInicio: string;
  horaFim: string;
}

export const DIAS_SEMANA = ["Segunda", "Terca", "Quarta", "Quinta", "Sexta", "Sabado"] as const;

export interface MinhaTurma {
  id: string;
  materiaId: string;
  materiaNome: string;
  dia: string;
  horaInicio: string;
  horaFim: string;
  periodoCurricular: number;
}

export interface AlunoResumo {
  id: string;
  nome: string;
  matricula: string;
}
