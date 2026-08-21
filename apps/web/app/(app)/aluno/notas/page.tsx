import Link from "next/link";
import { MinhasNotasList } from "@/features/grades/components/MinhasNotasList";

export default function AlunoNotasPage() {
  return (
    <div className="space-y-6 p-8">
      <Link href="/aluno" className="text-sm text-[var(--color-primary)] hover:underline">
        ← Área do Aluno
      </Link>
      <h1 className="text-xl font-semibold">Minhas notas</h1>
      <MinhasNotasList />
    </div>
  );
}
