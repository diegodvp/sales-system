namespace SalesAPI.Specifications
{
    /// <summary>
    /// Padrão Specification para validações de regras de negócio
    /// </summary>
    public interface ISpecification<T>
    {
        bool IsSatisfiedBy(T entity);
        string ErrorMessage { get; }
    }
}