import { RequireRole } from "@/shared/components/RequireRole";

export default function AlunoLayout({ children }: { children: React.ReactNode }) {
  return <RequireRole role="Aluno">{children}</RequireRole>;
}
