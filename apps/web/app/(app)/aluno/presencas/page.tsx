import Link from "next/link";
import { MinhasPresencasList } from "@/features/attendance/components/MinhasPresencasList";

export default function AlunoPresencasPage() {
  return (
    <div className="space-y-6 p-8">
      <Link href="/aluno" className="text-sm text-[var(--color-primary)] hover:underline">
        ← Área do Aluno
      </Link>
      <h1 className="text-xl font-semibold">Minha presença</h1>
      <MinhasPresencasList />
    </div>
  );
}
