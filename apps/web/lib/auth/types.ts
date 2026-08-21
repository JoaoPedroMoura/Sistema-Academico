export type Role = "Admin" | "Secretaria" | "Professor" | "Aluno";

export interface Session {
  accountId: string;
  name: string;
  email: string;
  tenantSlug: string;
  tenantName: string;
  role: Role;
}
