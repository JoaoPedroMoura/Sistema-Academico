import Link from "next/link";

/**
 * Link "voltar" padrão usado no topo das subpáginas de cada área (ex.: Admin/Professores volta
 * pra "Área do Admin"). Mesmo estilo já usado ad-hoc em `professor/disponibilidade`,
 * `professor/turmas/[id]` e nas subpáginas do Aluno — centralizado aqui pra ficar consistente
 * nas telas que ainda não tinham (admin/*, secretaria/*).
 */
export function BackLink({ href, label }: { href: string; label: string }) {
  return (
    <Link href={href} className="text-sm text-[var(--color-primary)] hover:underline">
      ← {label}
    </Link>
  );
}
