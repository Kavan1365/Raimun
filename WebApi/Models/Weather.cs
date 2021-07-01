using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Models
{
    public class Weather
    {
        public int Id { get; set; }
        public string City { get; set; }
        public decimal temp_c { get; set; }
    }
}
