
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProjectIssueTracker.Authorization;
using ProjectIssueTracker.Data;
using ProjectIssueTracker.Mappings;
using ProjectIssueTracker.Notifications;
using ProjectIssueTracker.Repositories.Contracts;
using ProjectIssueTracker.Repositories.Repos;
using ProjectIssueTracker.Services;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using System.Text.Json.Serialization;

namespace ProjectIssueTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var connectionString = builder.Configuration.GetConnectionString("ApiConnection");

            builder.Services.AddDbContext<ApiDBContext>(options => options
            //.UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

            //builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddSingleton<IssueHubService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<ICollaboratorService, CollaboratorService>();

            builder.Services.AddScoped<IIssueService, IssueService>();

            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

            builder.Services.AddControllers();
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            //});

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });


            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Secret").Value!)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ProjectOwnerPolicy", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new ProjectOwnershipRequirement());
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://127.0.0.1:4200", "http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()

                );
            });

            builder.Services.AddScoped<IAuthorizationHandler, ProjectOwnershipAuthorization>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseHttpsRedirection();


            app.UseAuthorization();
            app.MapHub<IssueHub>("issue-notifications");


            app.MapControllers();


            app.Run();
        }
    }
}