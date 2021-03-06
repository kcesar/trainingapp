﻿using System.IO;
using IdentityServer4.AccessTokenValidation;
using Kcesar.Training.Website;
using Kcesar.Training.Website.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace esar_training
{
  public class Startup
  {
    private readonly ILogger<Startup> logger;

    public Startup(IHostingEnvironment env, ILogger<Startup> logger)
    {
      logger.LogInformation($"Starting site. Environment: {env.EnvironmentName}");

      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddJsonFile($"appsettings.local.json", optional: true)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
      this.logger = logger;
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton(Configuration);

      services.AddDbContext<TrainingContext>(options => options.UseSqlServer(Configuration["database"], o => o.MigrationsHistoryTable("__Migrations", "trainingapp")));
      services.AddMemoryCache();

      // Add framework services.
      services.AddMvc();

      services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
          options.Authority = Configuration["auth:authority"];
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidAudience = Configuration["auth:authority"].Trim('/') + "/resources"
          };
          options.RequireHttpsMetadata = false;
        });

      services.AddSingleton<RolesService>();

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "frontend/build";
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      var hostingEnvironment = app.ApplicationServices.GetService<IHostingEnvironment>();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseBrowserLink();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      app.UseAuthentication();
      app.UseStaticFiles();
      app.UseSpaStaticFiles();

      app.UseMvc();

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "frontend";

        if (env.IsDevelopment())
        {
          spa.UseReactDevelopmentServer(npmScript: "start");
        }
      });
    }
  }
}
