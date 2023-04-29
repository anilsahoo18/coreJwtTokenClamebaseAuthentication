using coreJwtTokenClamebaseAuthentication.Middleware;
using coreJwtTokenClamebaseAuthentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coreJwtTokenClamebaseAuthentication
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
            services.AddDistributedMemoryCache(); //To Store session in Memory, This is default implementation of IDistributedCache    
            services.AddSession(k => k.IdleTimeout = TimeSpan.FromMinutes(1));
            
            services.AddControllers();
            
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = ".Net Core JwtToken Clame Base Authentication", Version = "v1" });
                option.AddSecurityDefinition(name: "Bearer ", securityScheme: new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type= SecuritySchemeType.ApiKey,
                    Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                    In = ParameterLocation.Path,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                             Name = "Bearer",
                             In = ParameterLocation.Path,
                             Reference = new OpenApiReference
                             {
                                  Id = "Bearer",
                                  Type = ReferenceType.SecurityScheme
                             }
                        },
                         new string [] {}
                    }
                });
            }); 



            services.AddAuthentication(k =>
            {
                k.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                k.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(L =>
            {
                var Key = Encoding.UTF8.GetBytes(Configuration["JWT:Key"]);
                L.SaveToken = true;
                L.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Key)
                };
                
                L.SaveToken = true;
               // L.Authority = "https://localhost:44345/swagger/index.html";
                L.RequireHttpsMetadata = false;
                L.Audience = Configuration["JWT:Issuer"];
            });

            services.AddSingleton<ITokenService, TokenService>();
            services.AddTransient<IUserRepository, UserRepository>();

            


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", ".Net Core JwtToken Clame Base Authentication v1"));
            }

            app.UseHttpsRedirection();
           
            app.UseRouting();

            app.UseAuthentication(); // This need to be added	
            app.UseAuthorization();
            app.UseMiddleware<JWTMiddleware>();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
