using Abp.UI;
using LibraryManagementSystemWithJwtToken.Service.Interfaces;
using LibraryManagementSystemWithJwtToken.Service;
using LibraryManagementSystemWithJwtToken.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using System.Globalization;
using TLife.AdminWeb.Data;
using TLife.AdminWeb.Data.Implementations;
using TLife.AdminWeb.Helpers;
using TLife.AdminWeb.Models;
using TLife.AdminWeb.Settings;
using TLife.AdminWeb.Token;
using LibraryManagementSystemWithJwtToken.Data;
using Microsoft.AspNetCore.Identity;

namespace TLife.AdminWeb
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
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(120);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication("CookieAuthenticationScheme")
            .AddCookie("CookieAuthenticationScheme", options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.LoginPath = "/Account/Login";
            });

            services.AddAuthorization();

            services.AddHttpContextAccessor();
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddViewLocalization(options => options.ResourcesPath = "Resources");



            services.AddDbContext<AppDbContext>(option => option.UseSqlServer(services.Configuration.GetConnectionString("Default")));
            services.AddIdentity<IdentityUser, IdentityRole>(
                options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                }).AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
                {
                    if (context.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            redirectTo = "/Account/Login"
                        }));
                    }
                    else
                    {
                        context.Response.Redirect("/Account/Login");
                    }
                }


            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
