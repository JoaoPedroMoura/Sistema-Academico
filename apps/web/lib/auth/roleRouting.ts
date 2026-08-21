import type { Role } from "./types";

const ROLE_HOME: Record<Role, string> = {
  Admin: "/admin",
  Secretaria: "/secretaria",
  Professor: "/professor",
  Aluno: "/aluno",
};

export function roleHomePath(role: Role): string {
  return ROLE_HOME[role];
}
