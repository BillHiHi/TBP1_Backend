using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TBP_Backend.Data;
using TBP_Backend.Models;
using TBP_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

// =====================
// DbContext
// =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// =====================
// Identity
// =====================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// =====================
// JWT Authentication
// =====================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;

    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                 // kiểm tra Issuer
        ValidateAudience = false,              // không cần Audience
        ValidateLifetime = true,               // bắt buộc còn hạn
        ValidateIssuerSigningKey = true,       // kiểm tra Key

        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// =====================
// Authorization
// =====================
builder.Services.AddAuthorization();

// =====================
// Swagger + JWT
// =====================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TBP API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// =====================
// DI Services
// =====================
builder.Services.AddScoped<TProductService>();
builder.Services.AddScoped<TCategoryService>();
builder.Services.AddScoped<TCartService>();
builder.Services.AddScoped<TAdminService>();

builder.Services.AddControllers();

var app = builder.Build();

// =====================
// Seed Admin
// =====================
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // 1. Tạo role Admin nếu chưa có
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    var adminEmail = "admin@tbp.com";

    // 2. Tìm admin theo email
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    // 3. Nếu chưa tồn tại → tạo mới
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,

            FirstName = "System",
            LastName = "Admin",
            PhoneNumber = "0000000000"
        };

        var result = await userManager.CreateAsync(adminUser, "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            throw new Exception(
                "Không tạo được admin: " +
                string.Join(", ", result.Errors.Select(e => e.Description))
            );
        }
    }
}


// =====================
// Middleware (THỨ TỰ QUAN TRỌNG)
// =====================
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication(); // 🔐 BẮT BUỘC
app.UseAuthorization();

app.MapControllers();
app.Run();
