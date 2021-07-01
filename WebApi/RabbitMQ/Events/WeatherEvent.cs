using System;

namespace WebApi.RabbitMQ.Events
{
    public class WeatherEvent
    {
        public decimal temp_c { get; set; }
        public string City { get; set; }
    }
}
