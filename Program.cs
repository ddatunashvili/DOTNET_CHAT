using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChatApp.Data;
using ChatApp.Hubs;
using ChatApp.Models;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1. Configure services BEFORE builder.Build()
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // 🔥 Remove password complexity
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;

    // 🔥 Remove username restrictions
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

    options.User.AllowedUserNameCharacters = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();


// ✅ 2. Build app AFTER services
var app = builder.Build();


// ✅ 3. Optional commands
if (args.Contains("reset-db"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureDeleted();
    db.Database.Migrate();
    Console.WriteLine("✅ Database has been reset.");
    return;
}

if (args.Contains("clear-chat-safe"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var total = db.Messages.Count();
    db.Messages.RemoveRange(db.Messages);
    db.SaveChanges();
    Console.WriteLine($"✅ Deleted {total} messages.");
    return;
}


// ✅ 4. Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


if (args.Contains("clear-chat"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var total = db.Messages.Count();
    db.Messages.RemoveRange(db.Messages);
    db.SaveChanges();

    Console.WriteLine($"✅ Cleared {total} messages.");
    return;
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapHub<ChatHub>("/chathub");

app.Run();
