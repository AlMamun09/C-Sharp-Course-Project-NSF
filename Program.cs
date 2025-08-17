using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NeighborhoodServiceFinder.Data;
using NeighborhoodServiceFinder.Services;
using NeighborhoodServiceFinder.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Get the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add the DbContext for our SQLite database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Add the ASP.NET Core Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<DbInitializer>();
builder.Services.AddSingleton<FirestoreService>();
// This line reads the "Cloudinary" section from your User Secretsand loads it into the CloudinarySettings class.
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
// This line registers our CloudinaryService so we can use it in our controllers.
builder.Services.AddTransient<CloudinaryService>();

var app = builder.Build();

// This section finds the DbInitializer service and runs the seeder
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbInitializer = services.GetRequiredService<DbInitializer>();
    await dbInitializer.SeedAsync();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
