namespace SalesAPI.Specifications
{
    /// <summary>
    /// Especificação para validação de CNPJ
    /// </summary>
    public class CnpjSpecification : ISpecification<string>
    {
        public string ErrorMessage { get; private set; } = string.Empty;

        public bool IsSatisfiedBy(string cnpj)
        {
            // 1. Validar se foi informado
            if (string.IsNullOrWhiteSpace(cnpj))
            {
                ErrorMessage = "CNPJ é obrigatório.";
                return false;
            }

            // 2. Limpar caracteres não numéricos
            var cnpjLimpo = new string(cnpj.Where(char.IsDigit).ToArray());

            // 3. Validar quantidade de dígitos
            if (cnpjLimpo.Length != 14)
            {
                ErrorMessage = "CNPJ deve conter 14 dígitos.";
                return false;
            }

            // 4. Validar se não são todos dígitos iguais
            if (cnpjLimpo.Distinct().Count() == 1)
            {
                ErrorMessage = "CNPJ inválido.";
                return false;
            }

            // 5. Validar dígitos verificadores
            if (!ValidarDigitosVerificadores(cnpjLimpo))
            {
                ErrorMessage = "CNPJ inválido. Dígito verificador não confere.";
                return false;
            }

            return true;
        }

        private bool ValidarDigitosVerificadores(string cnpj)
        {
            var multiplicadores1 = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicadores2 = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var digitos = cnpj.Substring(0, 12);
            var digito1 = CalcularDigito(digitos, multiplicadores1);
            var digito2 = CalcularDigito(digitos + digito1, multiplicadores2);

            return cnpj.EndsWith(digito1.ToString() + digito2.ToString());
        }

        private int CalcularDigito(string valor, int[] multiplicadores)
        {
            var soma = 0;
            for (int i = 0; i < multiplicadores.Length; i++)
            {
                soma += int.Parse(valor[i].ToString()) * multiplicadores[i];
            }

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        /// <summary>
        /// Remove caracteres não numéricos do CNPJ
        /// </summary>
        public static string Limpar(string cnpj)
        {
            return new string(cnpj.Where(char.IsDigit).ToArray());
        }
    }
}