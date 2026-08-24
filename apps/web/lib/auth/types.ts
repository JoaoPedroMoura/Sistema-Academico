export type Role = "Admin" | "Secretaria" | "Professor" | "Aluno";

export interface Session {
  accountId: string;
  name: string;
  email: string;
  tenantSlug: string;
  tenantName: string;
  role: Role;
  /** Senha atual é a temporária gerada na criação da conta — frontend força a troca antes de
   * liberar a área (ARCHITECTURE.md §7.5). */
  precisaTrocarSenha: boolean;
}
