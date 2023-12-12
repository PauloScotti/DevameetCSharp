using System.Text;
using DevameetCSharp;
using DevameetCSharp.Hubs;
using DevameetCSharp.Models;
using DevameetCSharp.Repository;
using DevameetCSharp.Repository.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DevameetContext>(option => option.UseSqlServer(connectString));

builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IMeetRepository, MeetRepositoryImpl>();
builder.Services.AddScoped<IRoomRepository, RoomRepositoryImpl>();
builder.Services.AddScoped<IMeetObjectsRepository, MeetObjectsRepositoryImpl>();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermissions", policy =>
    policy.AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:3000")
    .AllowCredentials()
    );
});

var jwtsettings = builder.Configuration.GetRequiredSection("JWT").Get<JWTKey>();

var secretkey = Encoding.ASCII.GetBytes(jwtsettings.SecretKey);
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(authentication =>
{
    authentication.RequireHttpsMetadata = false;
    authentication.SaveToken = true;
    authentication.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretkey),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ClientPermissions");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RoomHub>("roomHub");

app.Run();
