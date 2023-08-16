using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Services;
using System.Text.Json.Serialization;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.Helpers.MapperProfiles;
using AutoMapper;

namespace WonderfulRabbitsApi
{
    public class Startup
    {
        public IConfigurationRoot _configuration { get; }
        public Startup(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        //this runs at every request
        public void Configure(WebApplication app)
        {
            //the order of things is important so be careful when adding things

            app.UseRouting();

            // global cors policy
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();

            app.UseHttpsRedirection();

            app.MapControllers();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

        //this runs when application starts
        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
        {
            //the order of things is important so be careful when adding things
            services.AddDbContext<RabbitDbContext>(
                options => options
                    .UseSqlServer(_configuration.GetConnectionString("DefaultConnection"))
                    .UseLazyLoadingProxies()
                    );

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("https://localhost:7009",//api
                            "https://localhost:4001"//webb
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            //so it doesn't cycle forever
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions
                            .ReferenceHandler = ReferenceHandler.IgnoreCycles);
            ;
            services.AddSwaggerGen();//acces via https://localhost:4000/swagger

            // automapper
            services.AddAutoMapper(typeof(UserMapperProfile), typeof(RabbitMapperProfile));

            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IUserService, UserService>();
            // services.AddScoped<IRabbitService, RabbitService>();
            // services.AddScoped<IPhotoService, PhotoService>();

            //so httpcontext can be accessed in the services
            services.AddHttpContextAccessor();
        }
    }
}
