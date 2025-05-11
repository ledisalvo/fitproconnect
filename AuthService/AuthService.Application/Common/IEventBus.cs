namespace AuthService.Application.Common;

public interface IEventBus
{
    void Publish<T>(T @event) where T : class;
}
