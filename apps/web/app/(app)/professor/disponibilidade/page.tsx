"use client";

import Link from "next/link";
import { MyAvailabilityEditor } from "@/features/teachers/components/MyAvailabilityEditor";

export default function DisponibilidadePage() {
  return (
    <div className="space-y-6 p-8">
      <div className="flex items-center gap-4">
        <Link href="/professor" className="text-sm text-[var(--color-primary)] hover:underline">
          ← Minhas turmas
        </Link>
      </div>
      <h1 className="text-xl font-semibold">Minha disponibilidade</h1>
      <MyAvailabilityEditor />
    </div>
  );
}
