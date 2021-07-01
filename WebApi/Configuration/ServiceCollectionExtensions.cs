using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.DataLayer;
using WebApi.Exceptions;
using WebApi.Utilities;

namespace WebApi.Configuration
{
    public static class ServiceCollectionExtensions
    {
   

        public static void AddMinimalMvc(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter()); 
            }).AddNewtonsoftJson(
           );
        }


    
        public static void AddJwtAuthentication(this IServiceCollection services, SiteSettings jwtSettings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
                var encryptionKey = Encoding.UTF8.GetBytes(jwtSettings.EncryptKey);

                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = jwtSettings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionKey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(JwtBearerEvents));
                        //logger.LogError("Authentication failed.", context.Exception);

                        if (context.Exception != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authentication failed.", HttpStatusCode.Unauthorized, context.Exception, null);

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<RaimunDbContext>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("اطلاعات توکن خالی می باشد.");
                        var roles = claimsIdentity.RoleClaimType;


                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);


                        if (!securityStamp.HasValue())
                            context.Fail("امضای توکن مشکل دارد");

                        //Find user and token from database and perform your custom validation
                        var userId = claimsIdentity.GetUserId<int>();
                        var user = await userRepository.Users.FindAsync(userId);
                        if (user == null)
                            context.Fail("کاربر گرامی اکانت شما مشکل دارد لطفا با مدیر سایت تماس بگیرید.");

                        var getStamp = await userRepository.Users.AnyAsync(c => c.Stamp == Guid.Parse(securityStamp) && c.Id == user.Id, System.Threading.CancellationToken.None);
                        if (!getStamp)
                            context.Fail("امضای توکن مشکل دارد");

                        return;
                    },
                    OnChallenge = context =>
                    {

                        if (context.AuthenticateFailure != null)
                            throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);

                    }
                };
            });
        }


        public static void AddCustomApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true; //default => false;
                options.DefaultApiVersion = new ApiVersion(1, 0); //v1.0 == v1
                options.ReportApiVersions = true;

            });
        }
    }
}