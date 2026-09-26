"use client";

import { useEffect, useRef, type ReactNode } from "react";

export const inputClass =
  "w-full rounded-md border border-[var(--color-border-strong)] bg-[var(--color-surface)] px-3 py-2 text-sm outline-none focus:border-[var(--color-ring)] focus:shadow-[var(--focus-ring)]";

export const primaryButtonClass =
  "rounded-md bg-[var(--color-primary)] px-4 py-2 text-sm font-medium text-[var(--color-primary-foreground)] disabled:opacity-50 focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none";

interface FormDialogProps {
  title: string;
  onClose: () => void;
  children: ReactNode;
  /** Largura máxima (classe Tailwind). */
  maxWidth?: string;
}

/** Modal nativo (<dialog>) que abre ao montar — renderize só quando deve estar aberto. */
export function FormDialog({ title, onClose, children, maxWidth = "max-w-md" }: FormDialogProps) {
  const dialogRef = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    dialogRef.current?.showModal();
  }, []);

  return (
    <dialog
      ref={dialogRef}
      onClose={onClose}
      className={`m-auto w-full ${maxWidth} rounded-[var(--radius-lg)] border border-[var(--color-border)] bg-[var(--color-surface)] p-4 text-inherit backdrop:bg-black/60`}
    >
      <div className="mb-3 flex items-center justify-between">
        <h2 className="text-sm font-medium">{title}</h2>
        <button
          type="button"
          aria-label="Fechar"
          onClick={onClose}
          className="rounded-md px-2 text-[var(--color-muted-foreground)] focus-visible:shadow-[var(--focus-ring)] focus-visible:outline-none"
        >
          ✕
        </button>
      </div>
      {children}
    </dialog>
  );
}

/** Tela pós-criação mostrando a senha temporária gerada para a nova conta. */
export function SenhaTemporaria({
  senha,
  destinatario,
  onClose,
  children,
}: {
  senha: string;
  destinatario: string;
  onClose: () => void;
  children?: ReactNode;
}) {
  return (
    <div className="space-y-3">
      <div className="rounded-md bg-[var(--color-muted)] p-3 text-sm">
        Conta criada. Senha temporária (compartilhe com o {destinatario}):{" "}
        <code className="font-mono font-semibold">{senha}</code>
      </div>
      {children}
      <button type="button" onClick={onClose} className={`w-full ${primaryButtonClass}`}>
        Fechar
      </button>
    </div>
  );
}
