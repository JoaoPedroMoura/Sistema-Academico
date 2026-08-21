import { LoginForm } from "@/lib/auth/LoginForm";

export default function LoginPage() {
  return (
    <main className="flex flex-1 items-center justify-center p-8">
      <div className="w-full max-w-sm rounded-lg border border-[var(--color-border)] p-8">
        <h1 className="mb-6 text-center text-lg font-semibold">Sistema Acadêmico Faeterj</h1>
        <LoginForm />
      </div>
    </main>
  );
}
