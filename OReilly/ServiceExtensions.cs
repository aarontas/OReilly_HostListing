using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OReilly.Data;
using OReilly.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OReilly
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);//Here we said with parameter are necesary to secure identify. In our example we only need the Email

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);//We said wich class of identity and the role. The role comes from services
            builder.AddEntityFrameworkStores<DataBaseContext>().AddDefaultTokenProviders();//Said to tell wich database need interact with to identify
        }

        //Configure the JSON for security that have the token to use our API
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");

            //Our secret key. Previously we add our key in the system with de cmd console. Video 04:00 "Implement JWT Authentication". Its just a enviaroment variable
            var key = Environment.GetEnvironmentVariable("KEY");

            //This give us the authentication for the application and by default this is the Jwt authentication (1st line)
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(O =>
                {
                    O.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, //We hace the Issuer in the appsettings.json. This is the application that use the token
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true, //Is the value on top (our key from enviaroment)
                        ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))//We encode out key (enviaroment). Our encoding is nothing, but could be harder
                    };
                });
        }

        //Create a custom exception. We have in the middleware pipeline and not a service for this reason is a applicationBuilder and not a servicecollection
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError; //In all our exceptions use the 500 status error code 
                    context.Response.ContentType = "application/json"; //Type of response
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Something went wrong in the {contextFeature.Error}");//Use the message that we allways used in the log catch

                        //We generate the log and after that, we generate a error model with a status code and a message
                        await context.Response.WriteAsync(new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error. Please Try again later"
                        }.ToString());
                    }
                });
            });
        }

        //Service for API Version
        public static void ConfigureVersioning(this IServiceCollection servies)
        {
            servies.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0); //This our version by default
                opt.ApiVersionReader = new HeaderApiVersionReader("api-version");// we can use in the header verison and is not necesary to use query or expand the path
            });
        }

        //This give a lot of information in the header when get any information using the catche
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(//Now the cache configuration is global and all httpresponse have this header. Is not necesary specific in everyone
                (expirationOpt) =>
                {
                    expirationOpt.MaxAge = 120;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validationOpt) =>
                {
                    validationOpt.MustRevalidate = true;
                }
                );
        }
    }
}
