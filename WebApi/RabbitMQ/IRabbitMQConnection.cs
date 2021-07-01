using RabbitMQ.Client;
using System;

namespace WebApi.RabbitMQ
{
    public interface IRabbitMQConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
