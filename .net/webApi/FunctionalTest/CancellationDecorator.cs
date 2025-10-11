using FunctionalTest.Log;

namespace FunctionalTest;

public class CancellationDecorator<TCommand>(ICommandHandler<TCommand> decoratedHandler, ILogger logger)
    : ICommandHandler<TCommand>
{
    public CancellationTokenSource? CancellationTokenSource
    {
        get => decoratedHandler.CancellationTokenSource;
        set => decoratedHandler.CancellationTokenSource = value;
    }

    public async Task Execute(TCommand command)
    {
        try
        {
            CancellationTokenSource ??= new CancellationTokenSource();
            await decoratedHandler.Execute(command);
        }
        catch (OperationCanceledException)
        {
            logger.Log("Canceled.");
        }
        catch (Exception ex)
        {
            logger.Log(ex);
        }
    }

    public void CancelOperation()
    {
        CancellationTokenSource?.Cancel();
        CancellationTokenSource?.Dispose();
        CancellationTokenSource = null;
    }
}