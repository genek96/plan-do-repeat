using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using PlanDoRepeatWeb.Configurations.DatabaseSettings;
using PlanDoRepeatWeb.Implementations.Middleware;
using PlanDoRepeatWeb.Implementations.Repositories;
using PlanDoRepeatWeb.Implementations.Services;
using React.AspNet;

namespace PlanDoRepeatWeb
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();
            services.AddJsEngineSwitcher(options =>
                options.DefaultEngineName = ChakraCoreJsEngine.EngineName).AddChakraCore();
            
            services.Configure<UsersDatabaseSettings>(Configuration.GetSection(nameof(UsersDatabaseSettings)));
            services.Configure<TimerDatabaseSettings>(Configuration.GetSection(nameof(TimerDatabaseSettings)));

            services.AddSingleton(sp => sp.GetRequiredService<IOptions<UsersDatabaseSettings>>().Value);
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<TimerDatabaseSettings>>().Value);

            services.AddSingleton(typeof(ITimerRepository), typeof(TimerRepository));
            services.AddSingleton(typeof(IUserRepository), typeof(UserRepository));

            services.AddSingleton(typeof(IUsersService), typeof(UsersService));
            services.AddSingleton(typeof(ITimerService), typeof(TimerService));

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseReact(config => { });
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}