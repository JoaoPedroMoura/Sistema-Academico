import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Imagem Docker de teste (docker-compose) — copia só o runtime mínimo, não o projeto inteiro.
  output: "standalone",
};

export default nextConfig;
