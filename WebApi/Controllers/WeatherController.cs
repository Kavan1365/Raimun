using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Hangfire;
using WebApi.RabbitMQ.Common;
using WebApi.RabbitMQ.Producer;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [ApiVersion("1")]
    public class WeatherController : BaseController
    {
        private readonly HttpClient _httpClient;
        private readonly EventBusRabbitMQProducer EventBus;
        public WeatherController(HttpClient httpClient, EventBusRabbitMQProducer eventBus)
        {
            _httpClient = httpClient;
            EventBus = eventBus;
        }
        [HttpGet("[action]")]
        public async Task<ApiResult<WeatherViewModel>> Get()
        {


            ///بدست ارودن اطلاعات اب و هوای شهرستان سنندج
            var url = "https://api.weatherapi.com/v1/current.json?key=73af81f98c3a4843b1d153217213006&q=Sanandaj&aqi=no";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            using var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WeatherViewModel>();
                var jobId = BackgroundJob.Schedule(() => MyJob.SendMyJobForEmail("سنندج:" + result.current.temp_c), TimeSpan.FromMinutes(10));

                ////rabbitmq/////
                EventBus.PublishWeatherEvent(EventBusConstants.WeatherEventQueue, new RabbitMQ.Events.WeatherEvent() { City = "Sanandaj", temp_c = result.current.temp_c });


                return new ApiResult<WeatherViewModel>(true, Utilities.ApiResultStatusCode.Success, result);
            }

            return new ApiResult<WeatherViewModel>(true, Utilities.ApiResultStatusCode.LogicError, null, "اطلاعات ناکافی میباشد");
        }

    }


}
