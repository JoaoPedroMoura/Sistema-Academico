"use client";

import type { InputHTMLAttributes } from "react";
import { inputClass } from "@/shared/components/FormDialog";
import { formatarCep, formatarCpf } from "../mascaras";
import type { DadosPessoais } from "../types";
import { Campo, Secao } from "./Campo";

interface DadosPessoaisFieldsProps {
  value: DadosPessoais;
  onChange: (value: DadosPessoais) => void;
}

/** CPF, RG, nascimento/filiação e endereço — comuns a aluno e professor. */
export function DadosPessoaisFields({ value, onChange }: DadosPessoaisFieldsProps) {
  const campo = (nome: keyof DadosPessoais, props: InputHTMLAttributes<HTMLInputElement> = {}) => (
    <input
      type="text"
      value={value[nome] ?? ""}
      onChange={(e) => onChange({ ...value, [nome]: e.target.value || null })}
      className={inputClass}
      {...props}
    />
  );
  const uf = (nome: "rgUf" | "uf") =>
    campo(nome, {
      maxLength: 2,
      pattern: "[A-Za-z]{2}",
      title: "Sigla com 2 letras (ex.: RJ)",
      onChange: (e) => onChange({ ...value, [nome]: e.target.value.toUpperCase() || null }),
    });

  return (
    <>
      <Secao titulo="Documentos (CPF e RG)">
        <Campo label="CPF" className="col-span-2">
          {campo("cpf", {
            value: formatarCpf(value.cpf ?? ""),
            inputMode: "numeric",
            pattern: "\\d{3}\\.\\d{3}\\.\\d{3}-\\d{2}",
            title: "999.999.999-99",
            onChange: (e) => onChange({ ...value, cpf: formatarCpf(e.target.value) || null }),
          })}
        </Campo>
        <Campo label="RG" className="col-span-2">{campo("rgNumero", { maxLength: 20 })}</Campo>
        <Campo label="Órgão emissor">{campo("rgOrgaoEmissor", { maxLength: 20, placeholder: "DETRAN" })}</Campo>
        <Campo label="UF">{uf("rgUf")}</Campo>
        <Campo label="Data de emissão" className="col-span-2">{campo("rgDataEmissao", { type: "date" })}</Campo>
      </Secao>

      <Secao titulo="Nascimento e filiação">
        <Campo label="Data de nascimento" className="col-span-2">{campo("dataNascimento", { type: "date" })}</Campo>
        <Campo label="Naturalidade" className="col-span-2">{campo("naturalidade", { maxLength: 100, placeholder: "Cidade/UF" })}</Campo>
        <Campo label="Nome da mãe" className="col-span-2">{campo("nomeMae", { maxLength: 200 })}</Campo>
        <Campo label="Nome do pai" className="col-span-2">{campo("nomePai", { maxLength: 200 })}</Campo>
      </Secao>

      <Secao titulo="Endereço">
        <Campo label="CEP">
          {campo("cep", {
            value: formatarCep(value.cep ?? ""),
            inputMode: "numeric",
            pattern: "\\d{5}-\\d{3}",
            title: "99999-999",
            onChange: (e) => onChange({ ...value, cep: formatarCep(e.target.value) || null }),
          })}
        </Campo>
        <Campo label="Logradouro" className="col-span-3">{campo("logradouro", { maxLength: 200 })}</Campo>
        <Campo label="Número">{campo("numero", { maxLength: 20 })}</Campo>
        <Campo label="Complemento" className="col-span-3">{campo("complemento", { maxLength: 100 })}</Campo>
        <Campo label="Bairro" className="col-span-2">{campo("bairro", { maxLength: 100 })}</Campo>
        <Campo label="Cidade">{campo("cidade", { maxLength: 100 })}</Campo>
        <Campo label="UF">{uf("uf")}</Campo>
      </Secao>
    </>
  );
}
