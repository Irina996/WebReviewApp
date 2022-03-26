using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using WebRecomendationControlApp;
using WebRecomendationControlApp.Data;

var webAppOptions = new WebApplicationOptions()
{
    Args = args,

    EnvironmentName = Environments.Development,

};

var builder = WebApplication.CreateBuilder(webAppOptions);
builder.Configuration.AddJsonFile("appSecrets.json");
var connectionString = builder.Configuration["ConnectionStrings:MyDatabaseAlias"];
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication().AddVkontakte(vkontakteOptions =>
{
    vkontakteOptions.ClientId = builder.Configuration["Authentication:Vkontakte:ClientId"];
    vkontakteOptions.ClientSecret = builder.Configuration["Authentication:Vkontakte:ClientSecret"];
});
builder.Services.AddAuthentication().AddMailRu(mailOptions =>
{
    mailOptions.ClientId = builder.Configuration["Authentication:MailRu:ClientId"];
    mailOptions.ClientSecret = builder.Configuration["Authentication:MailRu:ClientSecret"];
});
builder.Services.AddAuthentication().AddDiscord(discordOptions =>
{
    discordOptions.ClientId = builder.Configuration["Authentication:Discord:ClientId"];
    discordOptions.ClientSecret = builder.Configuration["Authentication:Discord:ClientSecret"];
});

void CheckSameSite(HttpContext httpContext, CookieOptions options)
{
    if (options.SameSite == SameSiteMode.None)
    {
        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
        options.SameSite = SameSiteMode.Unspecified;

    }
}

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.OnAppendCookie = cookieContext =>
        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
    options.OnDeleteCookie = cookieContext =>
        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    })
    .AddViewLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ru")
    };

    options.DefaultRequestCulture = new RequestCulture("ru");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRequestLocalization();

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
