using NationsAPI.Database;
//using NationsAPI.Jobs;
using NationsAPI.Models;
using NationsAPI.Repositories;
using NationsAPI.Services;
using NationsAPI.Services.Messages;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Quartz;
using System.Text;
using NationsAPI.Utils;

namespace NationsAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Enable CORS
            services.AddCors(c =>
                {
                    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                });

            //JSON Initializer
            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft
                .Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver =
                new DefaultContractResolver());

            //Connection String
            services.AddDbContext<NationsDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("NationsAppCon")));

            //Repositories
            services
                .AddTransient<IMessageRepository, MessageRepository>()
                .AddTransient<IPlayerRepository, PlayerRepository>();

            //Services
            services
                .AddTransient<MessageService>()
                .AddTransient<PlayerService>();

            //Utils
            services
                .AddTransient<ISecurityUtils, SecurityUtils>()
                .AddTransient<IAuthUtils, AuthUtils>();

            //Security
            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = Configuration.GetValue<string>("Security:ValidIssuer"),
                        ValidAudience = Configuration.GetValue<string>("Security:ValidAudience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Security:AuthKey")))
                    };
                });

            services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            });
            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
