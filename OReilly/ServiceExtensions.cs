using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OReilly.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(q=>q.User.RequireUniqueEmail=true);//Here we said with parameter are necesary to secure identify. In our example we only need the Email

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);//We said wich class of identity and the role. The role comes from services
            builder.AddEntityFrameworkStores<DataBaseContext>().AddDefaultTokenProviders();//Said to tell wich database need interact with to identify
        }
    }
}
