using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Persistence;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Application.Activities;
using MediatR;
using FluentValidation.AspNetCore;
using API.Middleware;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Security;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;
using Infrastructure.Photos;
using API.SignalR;
using Application.Profiles;

namespace API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    private readonly string ApiCors = "_apiCors";

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureDevelopmentServices(IServiceCollection services)
    {
      services.AddDbContext<DataDbContext>(opt =>
      {
        opt.UseLazyLoadingProxies();
        opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
      });

      ConfigureServices(services);
    }

    public void ConfigureProductionServices(IServiceCollection services)
    {
      services.AddDbContext<DataDbContext>(opt =>
      {
        opt.UseLazyLoadingProxies();
        opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
      });

      ConfigureServices(services);
    }

    public void ConfigureServices(IServiceCollection services)
    {

      #region 
      // controllers without view
      services.AddControllers(opt =>
      {
        var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opt.Filters.Add(new AuthorizeFilter(policy));
      })
          .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Create>())
          .AddJsonOptions(options =>
          {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
          });

      // api cors for allowing methods that coming from different localhosts
      services.AddCors(opt =>
            {
              opt.AddPolicy(name: ApiCors, policy =>
              {
                policy
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .WithExposedHeaders("WWW-Authenticate")
                      .WithOrigins("http://localhost:3000")
                      .AllowCredentials();
              });
            });

      // api versioning
      services.AddApiVersioning(v =>
      {
        // shows that api accept any versions
        v.ReportApiVersions = true;

        // if there is available version show the default version
        v.AssumeDefaultVersionWhenUnspecified = true;

        // default version 1.0
        v.DefaultApiVersion = new ApiVersion(1, 0);
      });

      // MediatR
      services.AddMediatR(typeof(List.Handler).Assembly);

      // Auto Mapper
      services.AddAutoMapper(typeof(List.Handler));

      // SignalR
      services.AddSignalR();

      // swagger documentation for api
      services.AddSwaggerGen(options =>
            {
              options.SwaggerDoc("v1", new OpenApiInfo
              {
                Version = "v1",
                Title = "Api"
              });
              options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
              {
                Description = "JWT Authorization header using the Bearer scheme (Example: Authorization: 'Bearer {token}')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
              });
              options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                    new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

      // routing to lowercase
      services.AddRouting(options => options.LowercaseUrls = true);

      services.AddDbContext<DataDbContext>(opt =>
      {
        opt.UseLazyLoadingProxies();
        opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"), m => m.MigrationsAssembly("Persistence"));
      });

      var builder = services.AddDefaultIdentity<AppUser>();
      var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
      identityBuilder.AddEntityFrameworkStores<DataDbContext>();
      identityBuilder.AddSignInManager<SignInManager<AppUser>>();

      services.AddAuthorization(opt =>
            {
              opt.AddPolicy("IsActivityHost", policy =>
              {
                policy.Requirements.Add(new IsHostRequirement());
              });
            });
      services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenKey"]));
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(opt =>
              {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = key,
                  ValidateAudience = false,
                  ValidateIssuer = false,
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.Zero
                };
                opt.Events = new JwtBearerEvents
                {
                  OnMessageReceived = context =>
                  {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                    {
                      context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                  }
                };
              });

      services.AddScoped<IJwtGenerator, JwtGenerator>();
      services.AddScoped<IUserAccessor, UserAccessor>();
      services.AddScoped<IPhotoAccessor, PhotoAccessor>();
      services.AddScoped<IProfileReader, ProfileReader>();
      services.Configure<CloudinarySettings>(Configuration.GetSection("Cloudinary"));
      #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseMiddleware<ErrorHandlingMiddleware>();

      if (env.IsDevelopment())
      {
        // app.UseDeveloperExceptionPage();
      }
      else
      {
        // app.UseHsts();
      }

      // app.UseHttpsRedirection();
      // app.UseXContentTypeOptions();
      // app.UseReferrerPolicy(opt => opt.NoReferrer());
      // app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
      // app.UseXfo(opt => opt.Deny());
      // app.UseCsp(opt => opt
      //              .BlockAllMixedContent()
      //              .StyleSources(s => s.Self()
      //                  .CustomSources("https://fonts.googleapis.com", "sha256-F4GpCPyRepgP5znjMD8sc7PEjzet5Eef4r09dEGPpTs="))
      //              .FontSources(s => s.Self().CustomSources("https://fonts.gstatic.com", "data:"))
      //              .FormActions(s => s.Self())
      //              .FrameAncestors(s => s.Self())
      //              .ImageSources(s => s.Self().CustomSources("https://res.cloudinary.com", "blob:", "data:"))
      //              .ScriptSources(s => s.Self().CustomSources("sha256-5As4+3YpY62+l38PsxCEkjB1R4YtyktBtRScTJ3fyLU="))
      //          );

      app.UseDefaultFiles();
      app.UseStaticFiles();

      app.UseAuthentication();
      app.UseRouting();
      app.UseAuthorization();

      app.UseCors(ApiCors);

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
        endpoints.MapHub<ChatHub>("/chat");

        endpoints.MapControllerRoute(
                    name: "spa-fallback",
                    pattern: "api/v1/{controller=fallback}/{action=index}");
      });

      app.UseSwagger();
      app.UseSwaggerUI(s =>
      {
        s.RoutePrefix = "";
        s.DocumentTitle = "Swagger Documentation";
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "api/v1");
      });
    }
  }
}
