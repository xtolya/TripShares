using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TripShare.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TripShare.Services;
using TripShare.Abstract;
using TripShare.Implementations;
using Microsoft.AspNetCore.NodeServices;

namespace TripShare
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
            services.AddDbContext<ApplicationUserDbContext>(options => options.UseSqlServer("Server=tcp:tripuseridentity.database.windows.net,1433;Initial Catalog=ApplicationUserDb;Persist Security Info=False;User ID=tolya;Password=Hd3s1589;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
            services.AddDbContext<TripsDbContext>(options => options.UseSqlServer("Server=tcp:tripsdb.database.windows.net,1433;Initial Catalog=TripsDb;Persist Security Info=False;User ID=tolya;Password=Hd3s1589;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationUserDbContext>()
                .AddDefaultTokenProviders();
            services.AddNodeServices();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ITripRepository, EFTripRepository>();
            services.AddTransient<IBlockchainRepository, NeoBlockchainRepository>();
            services.AddTransient<IRefundRepository, EFRefundRepository>();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
