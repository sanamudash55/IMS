using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace inventory_management
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
            services.AddControllersWithViews();
            services.AddSession();
            services.AddDbContext<IMSContext>(options => options.UseSqlite("Data source=IMS.db"));
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options=> {
                // this will route the page to login action when we want to access the page without authentication valid
                options.LoginPath="/login";
                options.AccessDeniedPath="/denied";
                options.Events = new CookieAuthenticationEvents()
                {
                    // check all the event on completion of sining in 
                    OnSigningIn = async context =>
                     {   // accesing the principal 
                        var principal = context.Principal;
                         // checking that it has name identifier of not 
                        if(principal.HasClaim(c=>c.Type == ClaimTypes.NameIdentifier))
                         {   // if the name identifier is test then give him the role of admin 
                         if(principal.Claims.FirstOrDefault(c=>c.Type == ClaimTypes.NameIdentifier).Value == "test")
                            {  // giving the claim of the Admin
                               var claimsIdentity = principal.Identity as ClaimsIdentity;
                                 claimsIdentity.AddClaim(new Claim(ClaimTypes.Role,"Admin"));
                     }
                       }
                        await Task.CompletedTask;
                    },
                    // check all the event on singned in
                    OnSignedIn = async context =>
                    {
                        await Task.CompletedTask;
                    },
                    // check all the event on validateding after login
                    OnValidatePrincipal =async  context =>
                    {
                        await Task.CompletedTask;
                    }
                };
            }
            );

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
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Inventory}/{action=WelcomePage}/{id?}");
            });
        }
    }
}
