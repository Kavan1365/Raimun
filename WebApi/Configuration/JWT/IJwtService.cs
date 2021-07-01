using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Models;

namespace WebApi.Configuration.JWT
{
    public interface IJwtService
    {
        Task<AccessToken> GenerateAsync(User user, Guid stamp);
    }
}
