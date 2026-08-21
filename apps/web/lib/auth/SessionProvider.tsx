"use client";

import { createContext, useContext, useMemo, useState, type ReactNode } from "react";
import type { Session } from "./types";

interface SessionContextValue {
  session: Session | null;
  accessToken: string | null;
  setSession: (session: Session | null, accessToken: string | null) => void;
}

const SessionContext = createContext<SessionContextValue | undefined>(undefined);

/**
 * Contexto de sessão do lado do client. O login (Fase 6) grava o refresh token em cookie
 * httpOnly (setado pela API) e o access token de curta duração aqui em memória — nunca em
 * localStorage, para reduzir superfície de XSS.
 */
export function SessionProvider({ children }: { children: ReactNode }) {
  const [session, setSessionState] = useState<Session | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);

  const value = useMemo<SessionContextValue>(
    () => ({
      session,
      accessToken,
      setSession: (nextSession, nextAccessToken) => {
        setSessionState(nextSession);
        setAccessToken(nextAccessToken);
      },
    }),
    [session, accessToken],
  );

  return <SessionContext.Provider value={value}>{children}</SessionContext.Provider>;
}

export function useSession(): SessionContextValue {
  const context = useContext(SessionContext);
  if (!context) {
    throw new Error("useSession deve ser usado dentro de <SessionProvider>");
  }
  return context;
}
