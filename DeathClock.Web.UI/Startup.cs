using DeathClock.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeathClock.Web.UI
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = builder.Build();
            var connectionString = config.GetConnectionString("DeathClockDatabase");

            services.AddDbContext<DeathClockContext>(options => options.UseSqlServer(connectionString));
            services.AddSingleton<DataContext>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var dataPathList = Configuration.GetValue<string>("DeathClockData")?.Split(",").Where(p => !string.IsNullOrEmpty(p)).ToArray();
            var persistence = app.ApplicationServices.GetService<DeathClockContext>();
            var dataContext = app.ApplicationServices.GetService<DataContext>();

            if (dataPathList?.Any() != true)
            {
                throw new ArgumentException("A DeathClockData must be supplied.");
            }
            else
            {
                List<TmdbPerson> deathClockDatas = new List<TmdbPerson>();
                foreach (var path in dataPathList)
                {
                    var persons = persistence.TmdbPersons.ToArray();
                    deathClockDatas.AddRange(persons);

                }
                dataContext.Persons = deathClockDatas.ToArray();
            }
        }
    }
}
