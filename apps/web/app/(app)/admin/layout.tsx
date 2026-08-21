import { RequireRole } from "@/shared/components/RequireRole";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  return <RequireRole role="Admin">{children}</RequireRole>;
}
