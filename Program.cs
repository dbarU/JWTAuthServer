using JWTAuthServer.Configuration;
using JWTAuthServer.Models.AuthDbContext;
using JWTAuthServer.Services.Authenticators;
using JWTAuthServer.Services.HashHelper;
using JWTAuthServer.Services.RefreshTokenRepository;
using JWTAuthServer.Services.TokenGenerator;
using JWTAuthServer.Services.TokenValidator;
using JWTAuthServer.Services.UserRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
builder.Configuration.Bind("JwtAuthentication", authenticationConfiguration);

//SecretClient keyVaultClient = new SecretClient(new Uri(authenticationConfiguration.KeyVaultUrl), new DefaultAzureCredential());
// Add services to the container.

string sqlConnectionString = builder.Configuration.GetConnectionString("sqlite");
builder.Services.AddDbContext<AuthenticationDbContext>(options =>
{
    options.UseSqlite(sqlConnectionString);
});
builder.Services.AddControllers();
builder.Services.AddSingleton(authenticationConfiguration);
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<ITokenGenerator, AccessTokenGenerator>();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddSingleton<RefreshTokenValidator>();
builder.Services.AddScoped<IRefreshTokenRepository, EfCoreDbRefreshTokenRepository>();
builder.Services.AddScoped<IUserRepository, EfCoreDbUserRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters()
        {
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.Secret)),
            ValidIssuer = authenticationConfiguration.Issuer,
            ValidAudience = authenticationConfiguration.Audience,
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ClockSkew = TimeSpan.Zero
        };
    });
var app = builder.Build();
using IServiceScope scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>().Database.Migrate();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
