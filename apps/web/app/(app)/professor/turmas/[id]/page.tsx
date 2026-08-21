import Link from "next/link";
import { TurmaDetail } from "@/features/teachers/components/TurmaDetail";

export default async function TurmaPage({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;

  return (
    <div className="space-y-6 p-8">
      <Link href="/professor" className="text-sm text-[var(--color-primary)] hover:underline">
        ← Minhas turmas
      </Link>
      <TurmaDetail turmaId={id} />
    </div>
  );
}
