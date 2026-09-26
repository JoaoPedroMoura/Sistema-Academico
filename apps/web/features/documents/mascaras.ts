const digitos = (valor: string, max: number) => valor.replace(/\D/g, "").slice(0, max);

/** (99) 9999-9999 ou (99) 99999-9999, conforme a quantidade de dígitos. */
export function formatarTelefone(valor: string) {
  const d = digitos(valor, 11);
  if (d.length <= 2) return d.length ? `(${d}` : "";
  const meio = d.length === 11 ? 7 : 6;
  return `(${d.slice(0, 2)}) ${d.slice(2, meio)}${d.length > meio ? `-${d.slice(meio)}` : ""}`;
}

/** 999.999.999-99 */
export function formatarCpf(valor: string) {
  const d = digitos(valor, 11);
  return d
    .replace(/^(\d{3})(\d)/, "$1.$2")
    .replace(/^(\d{3})\.(\d{3})(\d)/, "$1.$2.$3")
    .replace(/\.(\d{3})(\d{1,2})$/, ".$1-$2");
}

/** 99999-999 */
export function formatarCep(valor: string) {
  return digitos(valor, 8).replace(/^(\d{5})(\d)/, "$1-$2");
}
