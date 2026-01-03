using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Razor;
using Automation.BotTv.UI.Web;

var builder = WebApplication.CreateBuilder(args);

//ADD MVC
builder.Services.AddMvc(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
})
.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
.AddDataAnnotationsLocalization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization();

//SESSION
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

////ADD AUTENTICAÇÃO
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.SlidingExpiration = true;
        options.LoginPath = "/Login/EfetuarLogin";
        options.LogoutPath = "/Login/Sair";
        options.AccessDeniedPath = "/Login/EfetuarLogin";
        options.Cookie.Name = ".AUTOMATIONBOTTV";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(480);
    });

//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

//ADD DAPPER CONFIG 
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

//ADD CONTAINER DE INJEÇÃO
DependencyContainer.RegisterContainers(builder.Services);

var app = builder.Build();

app.UseRequestLocalization(new RequestLocalizationOptions()
    .AddSupportedCultures(new[] { "pt-BR", "en-US" })
    .AddSupportedUICultures(new[] { "pt-BR", "en-US" }));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Erro", true);
    app.UseStatusCodePages();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();
app.UseCors();
app.UseSession();

//// autenticação sempre após a autorizacao
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");

app.Run();
