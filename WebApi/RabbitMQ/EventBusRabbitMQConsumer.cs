using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.DataLayer;
using WebApi.RabbitMQ.Common;
using WebApi.RabbitMQ.Events;

namespace WebApi.RabbitMQ
{
    public class EventBusRabbitMQConsumer
    {
        private readonly IServiceScopeFactory _serviceProvider;
        private readonly IRabbitMQConnection _connection;


        public EventBusRabbitMQConsumer(IRabbitMQConnection connection,
            IServiceScopeFactory serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Consume()
        {
            var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: EventBusConstants.WeatherEventQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += ReceivedEvent;

            channel.BasicConsume(queue: EventBusConstants.WeatherEventQueue, autoAck: true, consumer: consumer);
        }

        private async void ReceivedEvent(object sender, BasicDeliverEventArgs e)
        {
            if (e.RoutingKey == EventBusConstants.WeatherEventQueue)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<RaimunDbContext>();
                    var message = Encoding.UTF8.GetString(e.Body.Span);
                    var checkoutEvent = JsonConvert.DeserializeObject<WeatherEvent>(message);

                    if (checkoutEvent.temp_c >= 14)
                    {
                        await db.Weathers.AddAsync(new Models.Weather() { City = checkoutEvent.City, temp_c = checkoutEvent.temp_c });
                        await db.SaveChangesAsync();
                    }
                   
                }
            }
        }

        public void Disconnect()
        {
            _connection.Dispose();
        }
    }
}
