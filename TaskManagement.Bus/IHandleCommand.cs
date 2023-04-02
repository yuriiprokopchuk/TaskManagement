namespace TaskManagement.Bus.Infrastructure
{
    public interface IHandleCommand<TCommand>
    {
        void Handle(TCommand command);
    }
}
