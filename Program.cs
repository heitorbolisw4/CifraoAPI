using System.Security.Claims;
using BackEnd.Data;
using BackEnd.Domain.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using BackEnd.EndPoints;
using BackEnd.Jwt;
using System.Text;
using BackEnd.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BackEnd.Domain.Exceptions;
using Microsoft.OpenApi;
using BackEnd.EndPoints.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionStrings = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionStrings));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var awesomeApiKey = builder.Configuration["AwesomeApi:Key"] ?? throw new InvalidOperationException("AwesomeApi:Key não configurada");

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddHttpClient<IExchangeRateService, AwesomeApiService>( x =>
{
    x.BaseAddress = new Uri("https://economia.awesomeapi.com.br/");
    x.DefaultRequestHeaders.Add("x-api-key", awesomeApiKey);
});
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddExceptionHandler<ExternalApiExceptionHandler>();

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),

        NameClaimType = ClaimTypes.NameIdentifier

    };
});
builder.Services.AddCors( options =>
{
    options.AddPolicy("FrontEnd", policy =>
    {
       policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); 
    });


});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CifraoAPI",
        Version = "V1",
        Description = "BackEnd para aplicacao de tracking financeiro"
    });

    //configuracao JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
       In = ParameterLocation.Header,
       Type = SecuritySchemeType.Http,
       Scheme = "bearer",
       BearerFormat = "JWT"
    });
    options.AddSecurityRequirement( document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEnd");

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();
app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapExpenseEndpoints();
app.MapCategoryEndpoint();
app.MapExchangeEndpoints();
app.Run();
