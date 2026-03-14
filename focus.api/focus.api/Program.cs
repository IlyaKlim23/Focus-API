using System.Text;
using Focus.Application.Interfaces;
using Focus.Application.Services;
using Focus.Domain.Interfaces;
using Focus.Infrastructure.Auth;
using Focus.Infrastructure.ML;
using Focus.Infrastructure.Nlp;
using Focus.Infrastructure.Repositories;
using Focus.Infrastructure.Schedule;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using Focus.Database;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFocusDatabase(builder.Configuration.GetConnectionString("DefaultConnection")!);

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<IDailyNoteRepository, EfDailyNoteRepository>();
builder.Services.AddScoped<IProductivityPredictor, StubProductivityPredictor>();
builder.Services.AddScoped<IScheduleOptimizer, GreedyScheduleOptimizer>();
builder.Services.AddScoped<INlpAnalyzer, StubNlpAnalyzer>();

builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IDailyNoteService, DailyNoteService>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Section));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.Section).Get<JwtSettings>() ?? new JwtSettings();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo { Title = "Focus API", Version = "v1" });
    var xmlPath = Path.Combine(AppContext.BaseDirectory, "focus.api.xml");
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Введите JWT токен. Получите через POST /api/v1/auth/login или /api/v1/auth/register"
    });
    c.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        { new Microsoft.OpenApi.OpenApiSecuritySchemeReference("bearer", document), [] }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FocusDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Focus API v1"));

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
