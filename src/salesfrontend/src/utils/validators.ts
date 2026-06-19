// Validações (duplica backend para feedback imediato)

export function isValidCNPJ(cnpj: string): boolean {
  const cleaned = cnpj.replace(/\D/g, '');
  
  if (cleaned.length !== 14) return false;
  if (/^(\d)\1{13}$/.test(cleaned)) return false;
  
  // Validação dos dígitos verificadores
  const multiplicadores1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
  const multiplicadores2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
  
  let soma = 0;
  for (let i = 0; i < 12; i++) {
    soma += parseInt(cleaned[i]) * multiplicadores1[i];
  }
  
  let resto = soma % 11;
  const digito1 = resto < 2 ? 0 : 11 - resto;
  
  soma = 0;
  for (let i = 0; i < 13; i++) {
    soma += parseInt(cleaned[i]) * multiplicadores2[i];
  }
  
  resto = soma % 11;
  const digito2 = resto < 2 ? 0 : 11 - resto;
  
  return (
    parseInt(cleaned[12]) === digito1 && 
    parseInt(cleaned[13]) === digito2
  );
}