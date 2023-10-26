﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CloneHabr.Dto;
using CloneHabrService.Models.Options;
using CloneHabr.Dto.Requests;
using CloneHabrService.Models.Validators;
using FluentValidation;
using System.Text;
using CloneHabr.Data;
using NLog.Web;
using CloneHabrService.Services;
using CloneHabrService.Services.Impl;

namespace CloneHabrService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            #region Configure Options

            builder.Services.Configure<LoggerOptions>(options =>
            {
                builder.Configuration.GetSection("Settings:DatabaseOptions:ConnectionString").Bind(options);
            });

            #endregion

            #region Configure EF DBContext Service (Database)

            builder.Services.AddDbContext<ClonehabrDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["Settings:DatabaseOptions:ConnectionString"]);

            });

            #endregion

            #region Configure Logging Services

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.All | HttpLoggingFields.RequestQuery;
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;
                logging.RequestHeaders.Add("Authorization");
                logging.RequestHeaders.Add("X-Real-IP");
                logging.RequestHeaders.Add("X-Forwarded-For");
            });

            builder.Host.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

            }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });

            #endregion

            #region Configure Services

            builder.Services.AddSingleton<IAuthenticateService, AuthenticateService>();
            builder.Services.AddSingleton<IArticleService, ArticleService>();

            #endregion

            #region Configure repository

            //builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            #endregion

            #region Configure Authenticate 

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new
                TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(AuthenticateService.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            #endregion

            #region Configure FluentValidator

            builder.Services.AddScoped<IValidator<AuthenticationRequest>, AuthenticationRequestValidator>();
            builder.Services.AddScoped<IValidator<RegistrationRequest>, RegistrationRequestValidator>();
            builder.Services.AddScoped<IValidator<CreationArticleRequest>, CreationArticleRequestValidator>();
            //builder.Services.AddScoped<IValidator<AccountDto>, AccountDtoValidator>();
            builder.Services.AddScoped<IValidator<ArticleDto>, ArticleDtoValidator>();

            #endregion


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Проект для SCRUM", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme(Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseWebAssemblyDebugging(); // for Blazor
            }
            #region for Blazor
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.MapFallbackToFile("index.html");

            #endregion

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            app.UseHttpLogging();

            app.MapControllers();

            app.Run();
        }
    }
}