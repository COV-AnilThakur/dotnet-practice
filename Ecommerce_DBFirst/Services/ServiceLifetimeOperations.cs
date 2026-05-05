namespace Ecommerce_DBFirst.Services
{
    public interface IOperation
    {
        string OperationId { get; }
    }

    public interface ITransientOperation : IOperation;
    public interface IScopedOperation : IOperation;
    public interface ISingletonOperation : IOperation;

    public class OperationService : ITransientOperation, IScopedOperation, ISingletonOperation
    {
        public string OperationId { get; } = Guid.NewGuid().ToString("N")[..8];
    }
}
