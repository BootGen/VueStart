using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VueStart.Services;
using Microsoft.AspNetCore.Http;
using VueStart.Authorization;
using System.Threading.Channels;

namespace VueStart
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
            services.AddControllers();
            services.AddMemoryCache();
            services.AddHostedService<StatisticsService>();
            services.AddSingleton(Channel.CreateUnbounded<EventData>());
            services.AddScoped<GenerationService>();
            services.AddScoped<UserService>();
            services.AddDbContext<ApplicationDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.Use((context, next) =>
            {
                context.Request.EnableBuffering();
                return next();
            });
            app.UseExceptionHandler(builder => {
                builder.Run(async context => {
                        using var service = new ErrorHandlerService(Configuration);
                        var handler = context.Features.Get<IExceptionHandlerFeature>();
                        var exception = handler?.Error;
                        context.Request.Body.Seek(0, SeekOrigin.Begin);
                        if (exception != null) {
                            service.OnException(exception, await new StreamReader(context.Request.Body).ReadToEndAsync());
                        }
                    });
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<BasicAuthMiddleware>();
            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            if (env.IsDevelopment())
            {
                app.UseSpa(spa =>
                {
                    spa.UseProxyToSpaDevelopmentServer($"http://localhost:8080");
                });
            }
        }
    }
}
