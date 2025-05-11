using AuthService.Application.Events;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace AuthService.Infrastructure.Messaging;

public class RabbitMqEventPublisher
{
    private readonly RabbitMQ.Client.IModel _channel;

    public RabbitMqEventPublisher()
    {
        var factory = new ConnectionFactory { HostName = "rabbitmq" }; // nombre del contenedor en docker
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _channel.QueueDeclare(queue: "user-registered", durable: false, exclusive: false, autoDelete: false);
    }

    public void PublishUserRegistered(UserRegisteredEvent @event)
    {
        var json = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: "", routingKey: "user-registered", basicProperties: null, body: body);
        Console.WriteLine($"[x] Evento publicado para {@event.Email}");
    }
}
