using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.ViewModels
{
    public class TokenRequest
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }

        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}
