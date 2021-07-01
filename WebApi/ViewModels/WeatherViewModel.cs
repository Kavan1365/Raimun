using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class WeatherViewModel
    {
        public Current current { get; set; }
    }
    public class Current 
    {

        public decimal temp_c { get; set; }
    }
}
