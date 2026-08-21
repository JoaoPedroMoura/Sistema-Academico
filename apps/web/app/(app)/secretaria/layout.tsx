import { RequireRole } from "@/shared/components/RequireRole";

export default function SecretariaLayout({ children }: { children: React.ReactNode }) {
  return <RequireRole role="Secretaria">{children}</RequireRole>;
}
