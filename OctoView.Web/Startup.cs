using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestApplicationReact.Data;
using TestApplicationReact.Models;
using TestApplicationReact.Services;

namespace TestApplication
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


		    services.AddDbContext<ApplicationDbContext>(options =>
			    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

		    services.AddIdentity<ApplicationUser, IdentityRole>()
			    .AddEntityFrameworkStores<ApplicationDbContext>()
			    .AddDefaultTokenProviders();

		    // Add application services.
		    services.AddTransient<IEmailSender, EmailSender>();
		    services.AddMvc();
	    }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
	        try
	        {
		        using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
			        .CreateScope())
		        {
			        var ctx = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
	
                    if(ctx.Database.GetPendingMigrations().Any()){
                        ctx.Database.Migrate();
                    } 
		        }
	        }
	        catch (Exception)
	        {
		        //Fail lord.
	        }
			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

	        app.UseAuthentication();
			app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
