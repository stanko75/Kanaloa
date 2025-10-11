namespace FunctionalTest;

public interface ICommandHandler<in TCommand>
{
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    Task Execute(TCommand command);
}