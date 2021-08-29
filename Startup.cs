using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using SimpleRestApiQuestions;
using SimpreRestApiQuestions.Service;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SimpleRestApiQuestions.Service;

namespace WebApplication2
{
    public class Startup
    {
        private const string _MyCorsPolicy = "RestPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IQuestionService, QuestionServicesMySql>();

            //jwt
            services.AddAuthentication(d => //Agregamos autentificación de JWT, le decimos al .Net que coja la autentificación con JWT
            {
                d.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                d.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(d =>
            {
                d.RequireHttpsMetadata = false;
                d.SaveToken = true;
                d.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SK"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            
            //Swagger Gen
            services.AddSwaggerGen(c =>
            {
                //Basic swagger doc
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0.0",
                    Title = $"Questions API v1",
                    Description = "API To get and create questions, created only to practice.",
                    Contact = new OpenApiContact
                    {
                        Name = "Ramon Martinez (back-end dev)",
                        Email = "ramon.martinez.nieto@gmail.com",
                        Url = new Uri(@"https://www.ramonmartineznieto.com")
                    },
                });
                c.UseAllOfToExtendReferenceSchemas();

                //Get XML file and include it
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                //Add custom operation filter (for the padlock and etc..) 
                c.OperationFilter<AuthOperationFilter>();

                //Add Scheme and configure box 
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = "bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: _MyCorsPolicy, builder =>
                {
                    builder.WithOrigins("https://localhost:44377", "https://quiz-questions-front.herokuapp.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Questions API V1");
                c.ShowExtensions();
                c.ShowCommonExtensions();
                c.DocExpansion(DocExpansion.List);

            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors(_MyCorsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
