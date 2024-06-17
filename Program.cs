using Ma3ak.Helpers;
using Ma3ak.Models;
using Ma3ak.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;


using Ma3ak.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Configuration;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext< ApplicationDbContext>(options=>
options.UseSqlServer(connectionString));
////////builder.Services.AddHttpClient<IDistanceService, DistanceService>();
////////builder.Services.AddScoped<IDistanceService, DistanceService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors();


builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
//builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Tokens.ProviderMap["Default"] = new TokenProviderDescriptor(
        typeof(IUserTwoFactorTokenProvider<User>));
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.SaveToken = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {  
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime=true,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience= builder.Configuration["JWT:Audience"],
            IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))

        };
    });


builder.Services.AddSwaggerGen(options=> 
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {

        Version = "v1",
        Title = "Ma3ak",
        Description = "Application maintenance services"
        
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name="Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme= "Bearer",
        BearerFormat = "JWT",
        In= ParameterLocation.Header,
        Description="Enter your Jwt"

    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {

        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type= ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Name = "Bearer",
                 In= ParameterLocation.Header,
            },
            new List<string> ()


        }
    });


});

///////////builder.Services.AddScoped<SignInManager<User>, CustomSignInManager<User>>();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<AiChatBotService>();
//builder.Services.AddScoped<MaintenanceRequestService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
/////
app.UseStaticFiles(); // Enable serving static files from wwwroot
app.UseRouting();


app.UseCors(c=>c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
