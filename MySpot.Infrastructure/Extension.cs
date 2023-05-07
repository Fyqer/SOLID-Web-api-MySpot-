using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySpot.Application.Abstractions;
using MySpot.Application.Services;
using MySpot.Core.Abstractions;
using MySpot.Infrastructure.DAL;
using MySpot.Infrastructure.Exceptions;
using MySpot.Infrastructure.Logging;
using MySpot.Infrastructure.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("MySpot.Tests.Unit")]
namespace MySpot.Infrastructure
{
    public static class Extension
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.Configure<AppOptions>(configuration.GetRequiredSection("app"));
            services.AddSingleton<ExceptionMiddleware>();
            services.AddPostgres(configuration);
            services.AddSingleton<IClock, Clock>();
            services.AddCustomLogging();
            var applicationAssembly = typeof(AppOptions).Assembly;
            services.Scan(s => s.FromAssemblies(applicationAssembly)
             .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
             .AsImplementedInterfaces()
             .WithScopedLifetime());


            return services;
        }






        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.MapControllers();

            return app;
        }


        public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : class, new()
        {
            var options = new T();
            var section = configuration.GetRequiredSection(sectionName);
            section.Bind(options);

            return options;
        }
    }
}

