using Autofac;
using Autofac.Annotation;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.View;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;
using Repository;
using System;
using ant.mgr.core.Filter;
using Infrastructure.Web;
using Microsoft.Extensions.Hosting;

namespace ant.mgr.core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            //autofac打标签模式 文档：https://github.com/yuzd/Autofac.Annotation
            builder.RegisterModule(new AutofacAnnotationModule(
                    this.GetType().Assembly,
                    typeof(BaseRepository<>).Assembly,
                    typeof(HttpContext).Assembly)
                .SetAllowCircularDependencies(true)
                .SetDefaultAutofacScopeToInstancePerLifetimeScope());

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy("Any", r =>
            {
                r.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddControllersWithViews(op => op.Filters.Add<GlobalExceptionFilter>());
            services.AddRazorPages().AddNewtonsoftJson(options =>
                options.SerializerSettings.ContractResolver =
                    new DefaultContractResolver()); 

            services.AddScoped<IViewRenderService, ViewRenderService>();

            services.AddHttpContextAccessor();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logging)
        {
            #region AntORM
            //文档：https://github.com/yuzd/AntData.ORM
            app.UseAntData();
            #endregion

            #region AutoMapperConfig
            var autoMapperConfig = new Mapping.AutoMapper();
            autoMapperConfig.ExecuteByAssemblyName("DbModel", "ServicesModel");
            #endregion

            app.UseStaticFiles();

            app.UseStatusCodePagesWithReExecute("/admin/error/{0}");

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapAreaControllerRoute(
                    name: "Admin", "Admin",
                    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            
            });

            Infrastructure.Web.HttpContext.ServiceProvider = this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
        }
    }
}
