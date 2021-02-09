using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IntegrationLab
{
    public class Startup
    {
        // Methods
        public void ConfigureServices(IServiceCollection services)
        {
            #region Contracts

            if (services == null) throw new ArgumentException(nameof(services));

            #endregion

            // Mvc
            services.AddMvc();

            // RazorViewEngine
            services.Configure<RazorViewEngineOptions>(options =>
            {
                // ViewLocationFormats
                {
                    // Area
                    options.AreaViewLocationFormats.Add("/Views/{2}/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/{2}/Shared/{0}.cshtml");
                }
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region Contracts

            if (app == null) throw new ArgumentException(nameof(app));
            if (env == null) throw new ArgumentException(nameof(env));

            #endregion

            // EntryDirectory
            var entryDirectory = AppContext.BaseDirectory;
            if (Directory.Exists(entryDirectory) == false) throw new InvalidOperationException($"{nameof(entryDirectory)}=null");

            // Development
            if (env.IsDevelopment() == true)
            {
                app.UseDeveloperExceptionPage();
            }

            // StaticFiles
            {
                // ModuleAssembly
                var moduleAssembly = Assembly.LoadFile(Path.Combine(entryDirectory, $"IntegrationLab.Module.dll"));
                if (moduleAssembly == null) throw new InvalidOperationException($"{nameof(moduleAssembly)}=null");

                // Use
                app.UseStaticFiles(new StaticFileOptions()
                {
                    FileProvider = new CompositeFileProvider
                    (
                       env.WebRootFileProvider,
                       new ModuleWebAssetsFileProvider(moduleAssembly)
                    )
                });
            }

            // Routing
            app.UseRouting();
            {

            }

            // Endpoints
            app.UseEndpoints(endpoints =>
            {
                // Default
                endpoints.MapControllerRoute
                (
                    name: "Default",
                    pattern: "{controller=Home}/{action=Index}"
                );

                // Area
                endpoints.MapControllerRoute
                (
                    name: "Area",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}"
                );
            });
        }
    }
}
