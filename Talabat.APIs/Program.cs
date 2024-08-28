using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Repository.Data;
using Talabat.Repository.Repositories;
using Talabat.APIs.Extensions;
using StackExchange.Redis;
using Talabat.Repository.Identity;
using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Interfaces;
using Talabat.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Talabat.APIs
{
    public class Program
    {

        // Entry Point
        public static async Task Main(string[] args)
        {
            var WebApplicationBuilder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region ConfigureServices

            WebApplicationBuilder.Services.AddControllers(); // Register built-in APIs-Services at the container
            //WebApplicationBuilder.Services.AddControllersWithViews();
            //WebApplicationBuilder.Services.AddRazorPages();
            //WebApplicationBuilder.Services.AddMvc();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            WebApplicationBuilder.Services.AddEndpointsApiExplorer();
            WebApplicationBuilder.Services.AddSwaggerGen();            
            WebApplicationBuilder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("DefaultConnection"));
            });


            WebApplicationBuilder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(WebApplicationBuilder.Configuration.GetConnectionString("IdentityConnection"));
            });



            WebApplicationBuilder.Services.AddSingleton<IConnectionMultiplexer>((servicesProvider) =>
            {
                //var connection = WebApplicationBuilder.Configuration.GetConnectionString("Redis");

                //return ConnectionMultiplexer.Connect(connection
                return ConnectionMultiplexer.Connect(WebApplicationBuilder.Configuration.GetConnectionString("Redis"));
            });


            //WebApplicationBuilder.Services.AddScoped<IBasketRepository, BasketRepository>();
            WebApplicationBuilder.Services.AddScoped(typeof(IBasketRepository),typeof(BasketRepository));

            WebApplicationBuilder.Services.AddApplicationServices();

            WebApplicationBuilder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                //options.Password.RequireDigit = true;
                //options.Password.RequiredUniqueChars = 2;
                //options.Password.RequireNonAlphanumeric = true;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();

            WebApplicationBuilder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                                          .AddJwtBearer(options =>
                                          {
                                              options.TokenValidationParameters = new TokenValidationParameters()
                                              {
                                                  ValidateIssuer = true,
                                                  ValidIssuer = WebApplicationBuilder.Configuration["JWT:ValidIssuer"],
                                                  ValidateAudience = true,
                                                  ValidAudience = WebApplicationBuilder.Configuration["JWT:ValidAudience"],
                                                  ValidateLifetime = true,
                                                  ValidateIssuerSigningKey = true,
                                                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(WebApplicationBuilder.Configuration["JWT:Key"])),
                                          };
                                          });

            WebApplicationBuilder.Services.AddScoped<ITokenService,TokenService>();

            WebApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy",config =>
                {
                    config.AllowAnyHeader();
                    config.AllowAnyMethod();
                    config.WithOrigins(WebApplicationBuilder.Configuration["FrontEndBaseURL"]);
                });
            });


            #endregion

            var app = WebApplicationBuilder.Build();

            using var scope = app.Services.CreateScope();

            var services = scope.ServiceProvider;

            var _context = services.GetRequiredService<StoreDbContext>(); // ask CLR for creating Object from StoreDbContext Explicitly

            var _IdentityDbContext = services.GetRequiredService<AppIdentityDbContext>(); // ask CLR for creating Object from AppIdentityDbContext Explicitly

            var loggerFactory = services.GetRequiredService<ILoggerFactory>();



            try
            {
                await _context.Database.MigrateAsync();  // Update Database (Business)

                // Data Seeding
                await StoreDbContextSeed.SeedAsync(_context);

                
                await _IdentityDbContext.Database.MigrateAsync(); // Update Database (Identity)


                // Data Seeding
                var _userManager = services.GetRequiredService<UserManager<AppUser>>();   // Ask CLR to Create Object From UserManager<AppUser> Explicitly

                await AppIdentityDbContextSeed.SeedUsersAsync(_userManager);

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                
                var logger = loggerFactory.CreateLogger<Program>();

                logger.LogError(ex, "an Error has been occured during Applying the Migration");
            }

            #region Configure

            // Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/error/{0}");


            app.UseHttpsRedirection();

            app.UseStaticFiles();


            app.UseCors("MyPolicy");


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            #endregion


            app.Run();
        }
    }
}
