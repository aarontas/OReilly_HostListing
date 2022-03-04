using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OReilly.Data;
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
    }
}
