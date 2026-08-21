import { RequireRole } from "@/shared/components/RequireRole";

export default function ProfessorLayout({ children }: { children: React.ReactNode }) {
  return <RequireRole role="Professor">{children}</RequireRole>;
}
