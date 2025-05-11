using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using UserService.Application.Common;
using UserService.Application.Events;
using UserService.Domain.Entities;
using UserService.Infrastructure.Data;

namespace UserService.Infrastructure.Messaging;

public class UserRegisteredConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public UserRegisteredConsumer(IServiceScopeFactory scopeFactory)
    {
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "UserRegisteredEvent", durable: false, exclusive: false, autoDelete: false);
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        
        consumer.Received += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var userRegistered = JsonSerializer.Deserialize<UserRegisteredEvent>(message);

            Console.WriteLine($"[UserService] Usuario registrado: {userRegistered!.Email}, Rol: {userRegistered.Role}, Fecha: {userRegistered.RegisteredAt}");
            // Acá podrías guardar en base de datos, enviar mail, etc.
            using var scope = _scopeFactory.CreateScope();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            var exists = await db.Users.AnyAsync(u => u.Email == userRegistered.Email);

            if (!exists)
            {
                db.Users.Add(new User
                {
                    Email = userRegistered.Email,
                    Role = userRegistered.Role,
                    RegisteredAt = userRegistered.RegisteredAt
                });

                await db.SaveChangesAsync();

                await emailService.SendAsync(
                    userRegistered.Email,
                    "Bienvenido a FitPro Connect 💪",
                    $"Hola {userRegistered.Email}, tu cuenta fue creada exitosamente el {userRegistered.RegisteredAt}.");

                
                Console.WriteLine($"[UserService] Usuario guardado: {userRegistered.Email}");
            }
            else
            {
                Console.WriteLine($"[UserService] Usuario ya existe: {userRegistered.Email}");
            }
        };

        _channel.BasicConsume(queue: "UserRegisteredEvent", autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
