using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Discussly.Data;
using Discussly.Areas.Identity.Data;
namespace Discussly
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DiscusslyContextConnection") ?? throw new InvalidOperationException("Connection string 'DiscusslyContextConnection' not found.");

            builder.Services.AddDbContext<DiscusslyContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<DiscusslyUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DiscusslyContext>();

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
