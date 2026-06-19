namespace SalesAPI.Models
{
    public class Cliente
    {
        public int CodCliente { get; set; }
        public string CNPJ { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}