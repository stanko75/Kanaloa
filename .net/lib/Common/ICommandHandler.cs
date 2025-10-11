namespace Common;

public interface ICommandHandler<in TCommand>
{
    void Execute(TCommand command);
}