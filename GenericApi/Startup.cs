
using GenericApi.Config;
using MedRoom.ServerFramework.WebConfigHelpers.XSRF;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using ModelContext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Utils.Authorization;
using Utils.Contracts;
using Utils.Helpers;
using Utils.Interfaces;

namespace GenericApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConnectionStringsConfiguration>(Configuration.GetSection("ConnectionStrings"));

#if DEBUG
            services.AddSwaggerDocument(config =>
            {
                config.UseControllerSummaryAsTagDescription = true;
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Med API";
                };
            });

            services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandlerDeveloper>();

#endif

            IMvcCoreBuilder builder = services.AddMvcCore().AddApiExplorer();

            builder.AddFormatterMappings();
            builder.AddCacheTagHelper();

            builder.AddNewtonsoftJson(options =>
            {
                JsonSerializer js = new JsonSerializer
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                options.SerializerSettings.Converters.Add(new StringTrimConverter(js));
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddRouting();

            services.AddCors();

            services.ConfigureAntiforgery();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            });

            SetServices(services);

           
        }

        private void SetServices(IServiceCollection services)
        {
            ServiceProvider sc = services.BuildServiceProvider();

            IOptions<ConnectionStringsConfiguration> databaseConfiguration = sc.GetService<IOptions<ConnectionStringsConfiguration>>();
            services.AddEntityFrameworkNpgsql().AddDbContextPool<WebContext>(options =>
            {
                options.UseNpgsql(databaseConfiguration == null ? "" : databaseConfiguration.Value.Web);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.UseLazyLoadingProxies(true);
                options.UseInternalServiceProvider(sc);
            });

            services.AddOData();

            new InjectionServices().getServices(services);

            var config = new MapperConfig().Register();

            services.AddSingleton(config);


            var mp = new MapsterMapper.Mapper(config);
            services.AddSingleton<MapsterMapper.IMapper>(mp);

            services.AddMvcCore(options =>
            {
                foreach (ODataOutputFormatter outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (ODataInputFormatter inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(x => x.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
               
                app.UseHsts();
            }

            app.UseForwardedHeaders();

            ConfigureCORS(app);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapODataRoute("api", "api", GetEdmModel());
                endpoints.Count().Filter().OrderBy().Expand().Select().MaxTop(200);
                endpoints.EnableDependencyInjection();
                endpoints.MapControllerRoute("default", "{controller}/{action}/{id?}");
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        private static IEdmModel GetEdmModel()
        {
            Microsoft.AspNet.OData.Builder.ODataConventionModelBuilder builder = new Microsoft.AspNet.OData.Builder.ODataConventionModelBuilder
            {
                Namespace = "WebApiOData",
                ContainerName = "DefaultContainer"
            };


            List<Assembly> assemblies = new List<Assembly>();

            assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName != null && (
                a.FullName.StartsWith("ModelContext"))).ToList();

            assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName != null && (
                a.FullName.StartsWith("Service.DTO"))).ToList());

            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<Type> entityTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Any(x => x == typeof(IEntityModel)));

                foreach (Type type in entityTypes)
                {
                    builder.AddEntityType(type);
                }
            }

            return builder.GetEdmModel();
        }

        private void ConfigureCORS(IApplicationBuilder app)
        {
            app.UseCors(builder =>
            {
                builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials()
                        .WithExposedHeaders("Content-Disposition");
            });
        }
    }
}
