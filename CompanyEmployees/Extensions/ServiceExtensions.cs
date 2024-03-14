using CompanyEmployees.Formatters;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Repository;
using Service;
using Service.Contracts;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithExposedHeaders("X-Pagination"));
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        { 
            services.Configure<IISOptions>(options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static void ConfigureServiceManager(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opts => opts.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder)
        {
            return builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));
        }

        /*public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter(IServiceCollection services)
        {
            return services.BuildServiceProvider()
                           .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
                           .OfType<NewtonsoftJsonPatchInputFormatter>()
                           .First();
        }*/
    }
}
