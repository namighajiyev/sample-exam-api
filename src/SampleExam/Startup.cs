using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MediatR;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using SampleExam.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using SampleExam.Infrastructure.Errors;
using SampleExam.Infrastructure.Filters;
using SampleExam.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using SampleExam.Common;

//[assembly: ApiConventionType(typeof(DefaultApiConventions))]
[assembly: ApiConventionType(typeof(AppApiConventions))]

namespace SampleExam
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
            services.AddMediatR(typeof(Startup).Assembly);

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    In = "header",
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = "apiKey"
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }}
                });

                options.SwaggerDoc("v1", new Info() { Title = "SampleExam API", Version = "v1" });
                options.CustomSchemaIds(y => y.FullName);
                options.DocInclusionPredicate((version, apiDescription) => true);
                //use custom convention set GroupName as tag labels
                options.TagActionsBy(y => new List<string>()
                {
                    y.GroupName
                });
            });

            services.AddCors();

            services
                    .AddMvc(options =>
                    {
                        options.Conventions.Add(new AppControllerModelConvention());
                        options.Filters.Add(typeof(ValidatorActionFilter));
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(opt =>
                    {
                        opt.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    })
                    .AddFluentValidation(cfg =>
                    {
                        cfg.RegisterValidatorsFromAssemblyContaining<Startup>();
                    });

            services.AddTransient<IValidatorInterceptor, ApiValidatorInterceptor>();

            services.AddAutoMapper(GetType().Assembly);

            var connectionString = Configuration.GetValue<string>(Constants.CONN_STRING_KEY);
            services.AddDbContext<SampleExamContext>(opt => opt.UseNpgsql(connectionString));

            var jwtSecret = Configuration.GetValue<string>(Constants.API_JWT_KEY);
            services.AddJwt(jwtSecret);
            services.Configure<PasswordHasherOptions>(options => options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3);
            services.AddTransient<IPasswordHasher<Domain.User>, PasswordHasher<Domain.User>>();
            services.AddTransient<IApiJwtTokenGenerator, ApiJwtTokenGenerator>();
            services.AddTransient<IApiTokenRefreshTokenGenrator, ApiTokenRefreshTokenGenrator>();
            services.AddTransient<ICurrentUserAccessor, CurrentUserAccessor>();
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
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(ExceptionHandler.HandleException);
            });
            app.UseHttpsRedirection();

            app.UseMvc();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/swagger/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "SampleExam API");
            });
        }
    }
}
