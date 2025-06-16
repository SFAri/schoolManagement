using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SchoolManagement.Hub;
using SchoolManagement.Models;


var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers()
    //.AddNewtonsoftJson()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = null;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddDbContext<SchoolContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DevConnection"))
);

builder.Services.AddSignalR();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<SchoolContext>()
    .AddRoles<IdentityRole>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
         policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireStudentRole",
        policy => policy.RequireRole("Student"));
    options.AddPolicy("RequireLecturerRole",
        policy => policy.RequireRole("Lecturer"));
    options.AddPolicy("RequireLecturerOrAdmin",
        policy => policy.RequireRole("Lecturer", "Admin"));
});

// Cấu hình JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

// Adding Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),

        NameClaimType = ClaimTypes.NameIdentifier

    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            // ✅ Nếu là SignalR request tới Hub thì dùng token trong query
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/notification-hub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowAll", builder =>
    //{
    //    builder.AllowAnyOrigin()
    //    .AllowAnyMethod()
    //    .AllowAnyHeader()
    //    .AllowCredentials();
    //});
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder
            .WithOrigins("http://localhost:3000") // <== Chỉ rõ domain của FE
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // <== Quan trọng nếu gửi token
    });
});

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("AutoCloseAcademicYearJob");

    q.AddJob<YearEndCheckJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AutoCloseAcademicYearTrigger")
        .WithCronSchedule("0 0 0 1 6 ?") // mỗi năm vào 01/06 lúc 00:00 (s:m:h:d:m:weekday)
        //.WithCronSchedule("0 0/1 * * * ?")  // Test chạy mỗi 
    );
});

builder.Services.AddQuartzHostedService();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseCors("AllowAll");
app.UseCors("AllowFrontend");

//app.UseCors("AllowSpecific");
app.UseHttpsRedirection();

app.UseRouting();

// Config authentication middleware  
app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();
app.MapHub<NotificationHub>("/notification-hub");

app.Run();
