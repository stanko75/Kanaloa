namespace Common;

public interface ICommandHandlerAsync<in TCommand>
{
    Task Execute(TCommand command);
}