using System.Text.RegularExpressions;

namespace SalesAPI.Specifications
{
    /// <summary>
    /// Especificação para validação do formato de e-mail
    /// </summary>
    public class EmailSpecification : ISpecification<string>
    {
        public string ErrorMessage { get; private set; } = string.Empty;

        public bool IsSatisfiedBy(string email)
        {
            // Valida apenas o formato, não se é obrigatório ou não
            var regex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");

            if (!regex.IsMatch(email))
            {
                ErrorMessage = "E-mail em formato inválido.";
                return false;
            }

            return true;
        }
    }
}