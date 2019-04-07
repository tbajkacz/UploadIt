using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UploadIt.Data.Db.Account;
using UploadIt.Data.Models.Account;
using UploadIt.Services.Account;
using UploadIt.Services.Storage;

namespace UploadIt.Api
{
    public class Startup
    {
        private readonly string defaultCorsPolicy = "default";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(o =>
            {
                o.AddPolicy(defaultCorsPolicy, builder =>
                {
                    var coreSiteOrigin = Configuration.GetSection("CorsOrigins").GetChildren()
                        .FirstOrDefault()?.GetValue<string>("Core");

                    builder.WithOrigins(coreSiteOrigin)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
            services.AddScoped<IStorage, Storage>();
            services.AddScoped<IUserService, UserService>();

            services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("UploadIt"), builder =>
                    {
                        builder.MigrationsAssembly("UploadIt.Data");
                    });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.ASCII.GetBytes(
                                    Configuration.GetValue<string>(
                                        "AppSettings:Secret"))),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseCors(defaultCorsPolicy);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
