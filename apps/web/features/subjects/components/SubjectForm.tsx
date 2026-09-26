"use client";

import { useState, type FormEvent } from "react";
import { FormDialog, inputClass, primaryButtonClass } from "@/shared/components/FormDialog";
import { useAtualizarMateria, useCriarMateria } from "../hooks/useSubjects";
import type { Materia } from "../types";

interface SubjectFormProps {
  /** Matéria em edição; null = criação. Montar só quando o modal deve abrir. */
  materia: Materia | null;
  onClose: () => void;
}

export function SubjectForm({ materia, onClose }: SubjectFormProps) {
  const [nome, setNome] = useState(materia?.nome ?? "");
  const [periodo, setPeriodo] = useState(materia?.periodo ?? 1);
  const [cargaHorariaSemanal, setCargaHorariaSemanal] = useState(materia?.cargaHorariaSemanal ?? 4);
  const criar = useCriarMateria();
  const atualizar = useAtualizarMateria(materia?.id ?? "");
  const mutation = materia ? atualizar : criar;

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    mutation.mutate({ nome, periodo, cargaHorariaSemanal }, { onSuccess: onClose });
  }

  return (
    <FormDialog title={materia ? "Editar matéria" : "Adicionar matéria"} onClose={onClose}>
      <form onSubmit={handleSubmit} className="space-y-3">
        <input type="text" placeholder="Nome" required value={nome} onChange={(e) => setNome(e.target.value)} className={inputClass} />
        <div className="flex gap-3">
          <label className="flex-1 space-y-1">
            <span className="text-xs text-[var(--color-muted-foreground)]">Período</span>
            <input
              type="number"
              min={1}
              max={5}
              required
              value={periodo}
              onChange={(e) => setPeriodo(Number(e.target.value))}
              className={inputClass}
            />
          </label>
          <label className="flex-1 space-y-1">
            <span className="text-xs text-[var(--color-muted-foreground)]">Aulas/semana</span>
            <input
              type="number"
              min={1}
              max={10}
              required
              value={cargaHorariaSemanal}
              onChange={(e) => setCargaHorariaSemanal(Number(e.target.value))}
              className={inputClass}
            />
          </label>
        </div>
        {mutation.isError && <p className="text-sm text-[var(--color-destructive)]">{mutation.error.message}</p>}
        <button type="submit" disabled={mutation.isPending} className={`w-full ${primaryButtonClass}`}>
          {mutation.isPending ? "Salvando…" : materia ? "Salvar" : "Adicionar"}
        </button>
      </form>
    </FormDialog>
  );
}
