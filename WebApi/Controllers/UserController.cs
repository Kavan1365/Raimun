using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Configuration;
using WebApi.Configuration.JWT;
using WebApi.DataLayer;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Utilities;
using WebApi.ViewModels;

namespace WebApi.Controllers
{
    [ApiVersion("1")]
    public class UserController : BaseController
    {
        private readonly ILogger<UserController> Logger;
        private readonly RaimunDbContext RaimunDbContext;
        private readonly IJwtService JwtService;
        public UserController(ILogger<UserController> logger, IJwtService jwtService, RaimunDbContext raimunDbContext)
        {
            JwtService = jwtService;
            Logger = logger;
               RaimunDbContext = raimunDbContext;
        }


        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ApiResult> Create(UserViewModel model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
                return BadRequest("تمامی فیلدها الزامیست");

            if (RaimunDbContext.Users.Any(x => x.UserName == model.UserName))
                return BadRequest("نام کاربری مورد نظر در سیستم موجود است");

            var user = new User()
            {
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
            };
            await RaimunDbContext.Users.AddAsync(user, cancellationToken);
           await RaimunDbContext.SaveChangesAsync(cancellationToken);

            return new ApiResult(true, ApiResultStatusCode.Success, "عملیات باموفقیت انجام شد.");
        }



        [HttpPost("[action]")]
        [AllowAnonymous]
        public virtual async Task<ActionResult> Token([FromForm] TokenRequest tokenRequest, CancellationToken cancellationToken)
        {
            var user = await RaimunDbContext.Users.Where(z=>z.UserName==tokenRequest.username &&z.Password==tokenRequest.password).SingleOrDefaultAsync(cancellationToken);
            if (user == null)
                throw new BadRequestException("نام کاربری یا رمز عبور اشتباه میباشد.");
                Logger.LogWarning("متد Token فراخوانی شد توسط");
            user.Stamp = Guid.NewGuid();
           await RaimunDbContext.SaveChangesAsync(cancellationToken);

            var jwt = await JwtService.GenerateAsync(user, user.Stamp);

            return new JsonResult(jwt);
        }

    }
}
