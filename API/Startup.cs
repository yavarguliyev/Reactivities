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
using AutoMapper;
using Microsoft.OpenApi.Models;
using Application.Activities;
using MediatR;
using Application.Core;

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
    public void ConfigureServices(IServiceCollection services)
    {
      #region 
      // controllers without view
      services.AddControllers()
          .AddJsonOptions(options =>
          {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
          });

      // api cors for allowing methods that coming from different localhosts
      services.AddCors(options =>
      {
        options.AddPolicy(name: ApiCors,
                  builder =>
                  {
                    builder.WithOrigins("http://localhost:3000")
                                              .AllowAnyHeader()
                                              .AllowAnyMethod()
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

      // AutoMapper
      services.AddAutoMapper(typeof(MappingProfiles).Assembly);

      // MediatR
      services.AddMediatR(typeof(List.Handler).Assembly);

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
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
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
        opt.UseSqlite(Configuration.GetConnectionString("DefaultConnection"), m => m.MigrationsAssembly("Persistence"));
      });
      #endregion
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        //   app.UseHsts();
      }

      // app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthorization();
      app.UseCors(ApiCors);
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
      app.UseSwagger();
      app.UseSwaggerUI(s =>
      {
        s.RoutePrefix = "";
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "api/v1");
      });
    }
  }
}
