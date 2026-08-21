import Link from "next/link";
import { MeusMateriaisList } from "@/features/materials/components/MeusMateriaisList";

export default function AlunoMateriaisPage() {
  return (
    <div className="space-y-6 p-8">
      <Link href="/aluno" className="text-sm text-[var(--color-primary)] hover:underline">
        ← Área do Aluno
      </Link>
      <h1 className="text-xl font-semibold">Materiais complementares</h1>
      <MeusMateriaisList />
    </div>
  );
}
