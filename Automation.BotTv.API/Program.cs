using Automation.BotTv.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.Text;
using Automation.BotTv.API.Endpoints;
using Automation.BotTv.Data;
using Automation.BotTv.Repository;

var builder = WebApplication.CreateBuilder(args);

var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT options não configurado corretamente.");
builder.Services.AddSingleton(jwtOptions);


Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// Adiciona serviços ao container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{

    //Obtendo o diretório e depois o nome do arquivo .xml de comentários
    var applicationBasePath = PlatformServices.Default.Application.ApplicationBasePath;
    var applicationName = PlatformServices.Default.Application.ApplicationName;
    var xmlDocumentPath = Path.Combine(applicationBasePath, $"{applicationName}.xml");

    //Caso exista arquivo então adiciona-lo
    if (File.Exists(xmlDocumentPath))
    {
        options.IncludeXmlComments(xmlDocumentPath);
    }


    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
            "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
            "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configuração de autenticação JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var signingKeyBytes = Encoding.UTF8.GetBytes(jwtOptions.SigningKey);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://www.Automation.coop.br/Servicos/ApiAutomationBotTv", // FIXO como no código funcional
            ValidAudience = "https://www.Automation.coop.br/Servicos/ApiAutomationBotTv", // FIXO como no código funcional
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            ClockSkew = TimeSpan.Zero // Remove tolerância de tempo extra para expiração do token
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<AcessaDados>();
builder.Services.AddScoped<PainelREP>();
builder.Services.AddScoped<AcessoREP>();
builder.Services.AddScoped<PainelCampoREP>();
builder.Services.AddScoped<CampoREP>();
builder.Services.AddScoped<PainelMaquinaREP>();


var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();

app.MapGroup("/Campos")
    .RequireAuthorization()
    .MapCampos();
app.MapGroup("/Acessos")
    .RequireAuthorization()
    .MapAcessos();
app.MapGroup("/Maquinas")
     .RequireAuthorization()
    .MapMaquinas();
app.MapGroup("/Paineis")
    .RequireAuthorization()
    .MapPaineis();

app.Run();