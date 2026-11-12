using DevameetCSharp.Hubs;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using DevameetCSharp.Repository.Impl;
using DevameetCSharp.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DevameetContext>(option => option.UseSqlServer(connectString));

builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IMeetRepository, MeetRepositoryImpl>();
builder.Services.AddScoped<IRoomRepository, RoomRepositoryImpl>();
builder.Services.AddScoped<IMeetObjectsRepository, MeetObjectsRepositoryImpl>();
builder.Services.AddSingleton<TokenService>();

builder.Services.AddSignalR();

// Origens do front em Cors:Origins (o SignalR exige origem explícita com credenciais).
var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermissions", policy =>
    policy.AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(origins)
    .AllowCredentials()
    );
});

// Falha na subida, e não na primeira requisição, se a chave não estiver configurada.
var secretkey = TokenService.ReadKey(builder.Configuration);
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(authentication =>
{
    authentication.SaveToken = true;
    authentication.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretkey),
        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Swagger UI em /swagger e referência Scalar em /scalar.
app.UseSwagger();
app.UseSwaggerUI();
app.MapScalarApiReference(options => options
    .WithTitle("Devameet API")
    .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json"));

app.UseCors("ClientPermissions");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<RoomHub>("roomHub");

app.Run();
