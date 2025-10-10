using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Repository.Issue;
using ST10296167_PROG7312_POE.Repository.Feedback;
using ST10296167_PROG7312_POE.Services.Initialization;

namespace ST10296167_PROG7312_POE
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.MaxAge = null; 
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.LoginPath = "/User/Login";
                options.LogoutPath = "/User/Logout";
            });


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<Data.DataStore>();
            builder.Services.AddScoped<Services.Report.IReportService, Services.Report.ReportService>();
            builder.Services.AddScoped<Services.User.IUserService, Services.User.UserService>();
            builder.Services.AddScoped<Repository.User.IUserRepository, Repository.User.UserRepository>();
            builder.Services.AddScoped<IIssueRepository, IssueRepository>();
            builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            builder.Services.AddScoped<DataInitializationService>();

            var app = builder.Build();

            // Initialize database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                
                // Create db if not created
                context.Database.Migrate();
                
                // Call database initializer
                await DbInitializer.Initialize(services);
                
                // Initialize data structures from database
                var dataInitializationService = services.GetRequiredService<DataInitializationService>();
                await dataInitializationService.InitializeDataStructuresAsync();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//