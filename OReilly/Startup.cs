using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OReilly.Configurations;
using OReilly.Data;
using OReilly.IRepository;
using OReilly.Repository;
using OReilly.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OReilly
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Our database service
            services.AddDbContext<DataBaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );

            //We can know how many time a machine is request and avoid cyberattacks
            services.AddMemoryCache();

            //Configure the limit of calls per seconds
            services.ConfigureRateLimiting();
            services.AddHttpContextAccessor();

            ////For cache
            //services.AddResponseCaching();
            services.ConfigureHttpCacheHeaders();

            //Configure our identity service
            services.AddAuthentication();
            services.ConfigureIdentity();//Here method have a this. This means that all services are sending to the method and there are all ones about the database and authentication
            services.ConfigureJWT(Configuration);//Configure out token to authonticate

            //We add the Policy to use our API. If we want that only us can use that, change the builder methods.
            services.AddCors(o => {
                o.AddPolicy("AllowAll", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            //We add the automapper. The autommaper help us to map the database with the DTO. Create a layer between this layers.
            services.AddAutoMapper(typeof(MapperInitializer));

            //Transient mean that every time that it is needed that a new intansce, it will be created.
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //Manager for authentication
            services.AddScoped<IAuthManager, AuthManager>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OReilly", Version = "v1"});
            });

            //Better put it near to the last
            services.AddControllers(config => {
                    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile //Set a global specific cache value
                    {
                        Duration = 120
                    });
                }).AddNewtonsoftJson(op => 
                op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); //The video tutorial has a problem and "resolve" ignoring this. But I havent this problem

            //Configure our version manager
            services.ConfigureVersioning();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }
            //Normally it is goes inside the IsDevelopment 
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OReilly v1"));

            //Use custom error handling
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            //Here, after configurate the Corse, we need to use that.
            app.UseCors("AllowAll");

            //For cache
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();

            //Limit the request per second
            //app.UseIpRateLimiting(); //Not working. Maybe configuration is wrong 

            app.UseRouting();

            app.UseAuthentication();//We need to athenticate previously to authorizate
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
