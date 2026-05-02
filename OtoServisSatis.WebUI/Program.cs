using OtoServisSatis.Data;
using OtoServisSatis.Service.Abstract;
using OtoServisSatis.Service.Concrete;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace OtoServisSatis.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSession();

            builder.Services.AddDbContext<DatabaseContext>();
            builder.Services.AddTransient(typeof(IService<>), typeof(Service<>));
            builder.Services.AddTransient<ICarService, CarService>();
            builder.Services.AddTransient<IUserService, UserService>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(x =>
                {
                    x.LoginPath = "/Account/Login";
                    x.AccessDeniedPath = "/AccessDenied";
                    x.LogoutPath = "/Account/Logout";
                    x.Cookie.Name = "Admin";
                    x.Cookie.MaxAge = TimeSpan.FromDays(7);
                    x.Cookie.IsEssential = true;

                    // Dynamic yönlendirme
                    x.Events.OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/Admin"))
                        {
                            context.Response.Redirect("/Admin/Users/AccessDenied");
                        }
                        else
                        {
                            context.Response.Redirect("/AccessDenied");
                        }
                        return Task.CompletedTask;
                    };
                });

            builder.Services.AddAuthorization(x =>
            {
                //x.AddPolicy("AdminPolicy", policy => policy.RequireClaim("Role","Admin"));
                //x.AddPolicy("UserPolicy", policy => policy.RequireClaim("Role","User"));

                x.AddPolicy("AdminPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
                x.AddPolicy("UserPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
                x.AddPolicy("CustomerPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User", "Customer"));
                x.AddPolicy("ServisPersoneliPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "ServisPersoneli"));
                x.AddPolicy("SatisTemsilcisiPolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "SatisTemsilcisi"));
                // Admin sayfası tüm rolleri kabul etsin
                x.AddPolicy("AdminPagePolicy", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User", "ServisPersoneli", "SatisTemsilcisi", "Customer"));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
            name: "admin",
            pattern: "{area:exists}/{controller=Main}/{action=Index}/{id?}"
            );

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
