using WonderfulRabbitsApi.Authorization;
using WonderfulRabbitsApi.DatabaseContext;
using WonderfulRabbitsApi.Services;
using System.Text.Json.Serialization;
using WonderfulRabbitsApi.Helpers;
using WonderfulRabbitsApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WonderfulRabbitsApi
{
    public class Startup
    {
        public IConfigurationRoot _configuration { get; }
        public Startup(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public void Configure(WebApplication app)
        {
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

        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
        {
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
                            "http://localhost:8081"//webb
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
            services.AddSwaggerGen();

            services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRabbitService, RabbitService>();
            services.AddScoped<IImageService, ImageService>();

            // automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddHttpContextAccessor();
        }
    }
}
