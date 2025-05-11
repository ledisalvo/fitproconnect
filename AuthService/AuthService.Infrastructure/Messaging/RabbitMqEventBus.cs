using AuthService.Application.Common;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AuthService.Infrastructure.Messaging;

public class RabbitMqEventBus : IEventBus
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqEventBus()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Publish<T>(T @event) where T : class
    {
        var queueName = typeof(T).Name; // ej: UserRegisteredEvent
        _channel.QueueDeclare(queue: "UserRegisteredEvent", durable: false, exclusive: false, autoDelete: false);
        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

        Console.WriteLine($"[x] Publicado evento {typeof(T).Name} → {json}");
    }
}
