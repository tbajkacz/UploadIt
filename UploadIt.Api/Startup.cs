using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using UploadIt.Data.Db.Account;
using UploadIt.Data.Db.Registration;
using UploadIt.Data.Repositories.Account;
using UploadIt.Data.Repositories.Registration;
using UploadIt.Services.Account;
using UploadIt.Services.Security;
using UploadIt.Services.Storage;
using TokenReader = Microsoft.IdentityModel.Tokens.TokenReader;

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
            services
                .AddScoped<IRegistrationRepository, RegistrationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStorage, Storage>();
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddTransient<ITempTokenValidator, JwtTempTokenValidator>();

            services.AddDbContext<AccountDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("UploadIt"), builder =>
                    {
                        builder.MigrationsAssembly("UploadIt.Data");
                    });
            });

            services.AddDbContext<RegistrationDbContext>(options =>
            {
                options.UseSqlServer(
                    Configuration.GetConnectionString("UploadIt"),
                    builder =>
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

                options.Events = new JwtBearerEvents
                {
                    //checks whether user exists in the db it for example prevents access after account deletion
                    OnTokenValidated = async context =>
                    {
                        var userService = context.HttpContext.RequestServices
                            .GetService<IUserService>();
                        if (!int.TryParse(context.Principal.Identity.Name, out int userId))
                        {
                            context.Fail("Couldn't parse user id");
                        }

                        var user = await userService.GetUserByIdAsync(userId);
                        if (user == null)
                        {
                            context.Fail("Unauthorized");
                        }
                    }
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
