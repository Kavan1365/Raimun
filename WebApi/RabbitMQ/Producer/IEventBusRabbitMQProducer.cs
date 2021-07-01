using WebApi.RabbitMQ.Events;

namespace WebApi.RabbitMQ.Producer
{
    public interface IEventBusRabbitMQProducer
    {
        void PublishBasketCheckout(string queueName, WeatherEvent publishModel);
    }
}
