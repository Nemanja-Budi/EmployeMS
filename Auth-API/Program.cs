using ADMitroSremEmploye;
using ADMitroSremEmploye.Data;
using ADMitroSremEmploye.Mappings;
using ADMitroSremEmploye.Models.Domain;
using ADMitroSremEmploye.Repositories;
using ADMitroSremEmploye.Repositories.AnnualLeave_repository;
using ADMitroSremEmploye.Repositories.Audit_repository;
using ADMitroSremEmploye.Repositories.Bank_repository;
using ADMitroSremEmploye.Repositories.Employe_repository;
using ADMitroSremEmploye.Repositories.Employe_Salary_repository;
using ADMitroSremEmploye.Repositories.Member_repository;
using ADMitroSremEmploye.Repositories.MP.Izvestaj_repository;
using ADMitroSremEmploye.Repositories.MP.Kalkulacija_repository;
using ADMitroSremEmploye.Repositories.MP.Otpremnica_repository;
using ADMitroSremEmploye.Repositories.MP.Racun_repository;
using ADMitroSremEmploye.Repositories.Salary_Service_repository;
using ADMitroSremEmploye.Repositories.State_obligation_repository;
using ADMitroSremEmploye.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<SQLAnnualLeaveRepository>();
builder.Services.AddScoped<IAnnualLeaveRepository, CachedAnnualLeaveRepository>();

builder.Services.AddScoped<SQLAuditRepository>();
builder.Services.AddScoped<IAuditRepository, CachedAuditRepository>();

builder.Services.AddScoped<SQLEmployeRepository>();
builder.Services.AddScoped<IEmployeRepository, CachedEmployeRepository>();

builder.Services.AddScoped<SQLEmployeSalaryRepository>();
builder.Services.AddScoped<IEmployeSalaryRepository, CachedEmployeSalaryRepository>();

builder.Services.AddScoped<SQLMemberRepository>();
builder.Services.AddScoped<IMemberRepository, CachedMemberRepository>();

builder.Services.AddScoped<SQLBankRepository>();
builder.Services.AddScoped<IBankRepository, CachedBankRepository>();

builder.Services.AddScoped<IStateObligationRepository, SQLStateObligationRepository>();
builder.Services.AddScoped<ISalaryServiceRepository, SalaryServiceRepository>();

builder.Services.AddScoped<IKalkulacijaRepository, SQLKalkulacijaRepository>();
builder.Services.AddScoped<IRacunRepository, SQLRacunRepository>();
builder.Services.AddScoped<IOtpremnicaRepository, SQLOtpremnicaRepository>();
builder.Services.AddScoped<IIzvestajRepository, SQLIzvestajRepository>();



builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile<AutoMapperProfiles>();
});

var mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<ContextSeedService>();
builder.Services.AddScoped<SalaryCalculatorService>();
builder.Services.AddScoped<BankSeedService>();

builder.Services.AddIdentityCore<User>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;

        options.SignIn.RequireConfirmedEmail = true;
    }
)
.AddRoles<IdentityRole>()
.AddRoleManager<RoleManager<IdentityRole>>()
.AddEntityFrameworkStores<UserDbContext>()
.AddSignInManager<SignInManager<User>>()
.AddUserManager<UserManager<User>>()
.AddDefaultTokenProviders();

/*builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
*/

//OVO JE KOMENTARISANO ZBOG RESAVANJA CIKLICNIH VEZA KOJE RADE SAD BEZ OVOGA
/*builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

*/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateIssuer = true,
            ValidateAudience = false,
        };
    });

builder.Services.AddCors();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
        .Where(x => x.Value.Errors.Count > 0)
        .SelectMany(x => x.Value.Errors)
        .Select(x => x.ErrorMessage).ToArray();

        var toReturn = new
        {
            Errors = errors
        };

        return new BadRequestObjectResult(toReturn);
    };
});



var app = builder.Build();

app.UseCors(options =>
{
    options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

#region
using var scope = app.Services.CreateScope();
try
{
    var contextSeedService = scope.ServiceProvider.GetService<ContextSeedService>();
    await contextSeedService.InitializeContextAsync();

    var bankSeedService = scope.ServiceProvider.GetService<BankSeedService>();
    await bankSeedService.SeedBankDataAsync();
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
    logger.LogError(ex.Message, "Failed to initialize and seed the database");
}
#endregion

app.Run();
