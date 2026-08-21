import Link from "next/link";
import { MinhasSolicitacoesPanel } from "@/features/requests/components/MinhasSolicitacoesPanel";

export default function AlunoSolicitacoesPage() {
  return (
    <div className="space-y-6 p-8">
      <Link href="/aluno" className="text-sm text-[var(--color-primary)] hover:underline">
        ← Área do Aluno
      </Link>
      <h1 className="text-xl font-semibold">Solicitações</h1>
      <MinhasSolicitacoesPanel />
    </div>
  );
}
