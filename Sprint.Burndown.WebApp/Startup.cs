using System;
using System.Linq;
using System.Text;

using AutoMapper;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Sprint.Burndown.WebApp.Abstractions;
using Sprint.Burndown.WebApp.Const;
using Sprint.Burndown.WebApp.Core;
using Sprint.Burndown.WebApp.Extensions;
using Sprint.Burndown.WebApp.ExternalServices;
using Sprint.Burndown.WebApp.Hubs;
using Sprint.Burndown.WebApp.JiraModels;
using Sprint.Burndown.WebApp.Models;
using Sprint.Burndown.WebApp.Services;
using Sprint.Burndown.WebApp.ViewModels;

namespace Sprint.Burndown.WebApp
{
    public class Startup
    {
        private static readonly NLog.ILogger Log = LoggerType.System;

        public IConfigurationRoot Configuration { get; }

        internal static IServiceProvider ServiceProvider { get; private set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<JiraOptions>(Configuration);

            services.AddSignalR(); // NB Should be called before AddMvc() due SPA routing issues .
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Burn-Down Chart API",
                    Description = "Burn-Down Chart internal API"
                });
            });

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("77135F4BEC914BB3A76320FBD9D8B856")),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true, //validate the expiration and not before values in the token
                    ClockSkew = TimeSpan.FromMinutes(5), //5 minute tolerance for the expiration date
                };
            });

            RegisterDependencies(services);
            Log.Info("Services configured succefully");
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime applicationLifetime, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            RegisterMappings(app);

            ConfigureLifetimeEvents(applicationLifetime);

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseSignalR(routes =>
            {
                routes.MapHub<LoadingNotificationHub>("/notification");
            });

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Burn-Down Chart internal API");
            });

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                // For working browser Refresh Page button
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });

            Log.Info("Application configured succefully");
        }

        private void ConfigureLifetimeEvents(IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStarted.Register(() => Log.Info("Application has started"));
            applicationLifetime.ApplicationStopped.Register(() => Log.Info("Application has stopped"));
        }

        private void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICredentialsStorage, CredentialsStorage>();

            services.AddScoped<IRepository, Repository>();
            services.AddScoped<IJiraFacade, JiraFacade>();
            services.AddScoped<IJiraService, JiraService>();
            services.AddScoped<ICacheStorage, RedisStorage>();
            services.AddScoped<IBoardService, BoardService>();
            services.AddScoped<ISprintService, SprintService>();
            services.AddScoped<IIssueService, IssueService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IProductionScheduleService, ProductionScheduleService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<IChartService, ChartService>();
            services.AddScoped<IGlobalContext, GlobalContext>();

            ServiceProvider = services.BuildServiceProvider();
        }

        private void RegisterMappings(IApplicationBuilder app)
        {
            Mapper.Initialize(
                cfg =>
                {
                    cfg.CreateMap<BoardModel, BoardViewModel>();

                    cfg.CreateMap<SprintModel, SprintViewModel>()
                        .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.Additions.IsFavorite))
                        .ForMember(dest => dest.ActualStartDate, opt => opt.MapFrom(src => src.Additions.DevelopBeginDate))
                        .ForMember(dest => dest.ActualEndDate, opt => opt.MapFrom(src => src.Additions.DevelopEndDate))
                        .ForMember(dest => dest.ChartPreview, opt => opt.MapFrom(src => src.ChartPreview.ImageBody))
                        .ForMember(dest => dest.PercentPassed, opt => opt.Ignore());

                    cfg.CreateMap<IssueModel, IssueViewModel>()
                        .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.Fields.Parent.Id))
                        .ForMember(dest => dest.ParentKey, opt => opt.MapFrom(src => src.Fields.Parent.Key))
                        .ForMember(
                            dest => dest.FixVersionId,
                            opt => opt.MapFrom(src => src.Fields.FixVersions.Select(p => p.Id).FirstOrDefault()))
                        .ForMember(
                            dest => dest.FixVersionName,
                            opt => opt.MapFrom(src => src.Fields.FixVersions.Select(p => p.Name).FirstOrDefault()))
                        .ForMember(dest => dest.PriorityId, opt => opt.MapFrom(src => src.Fields.Priority.Id))
                        .ForMember(dest => dest.PriorityName, opt => opt.MapFrom(src => src.Fields.Priority.Name))
                        .ForMember(dest => dest.PriorityIconUrl, opt => opt.MapFrom(src => src.Fields.Priority.IconUrl))
                        .ForMember(dest => dest.Labels, opt => opt.MapFrom(src => src.Fields.Labels.ToArray()))
                        .ForMember(dest => dest.AssigneeId, opt => opt.MapFrom(src => src.Fields.Assignee.Id))
                        .ForMember(dest => dest.AssigneeName, opt => opt.MapFrom(src => src.Fields.Assignee.Name ?? src.Fields.Assignee.DisplayName))
                        .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Fields.Status.Id))
                        .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Fields.Status.Name))
                        .ForMember(dest => dest.StatusIconUrl, opt => opt.MapFrom(src => src.Fields.Status.IconUrl))
                        .ForMember(dest => dest.Components, opt => opt.MapFrom(src => src.Fields.Status.Id))
                        .ForMember(dest => dest.ReporterId, opt => opt.MapFrom(src => src.Fields.Reporter.Id))
                        .ForMember(dest => dest.ReporterName, opt => opt.MapFrom(src => src.Fields.Reporter.Name ?? src.Fields.Reporter.DisplayName))
                        .ForMember(
                            dest => dest.ReporterDisplayName,
                            opt => opt.MapFrom(src => src.Fields.Reporter.DisplayName))
                        .ForMember(dest => dest.IssueTypeId, opt => opt.MapFrom(src => src.Fields.IssueType.Id))
                        .ForMember(dest => dest.IssueTypeName, opt => opt.MapFrom(src => src.Fields.IssueType.Name))
                        .ForMember(
                            dest => dest.IssueTypeIconUrl,
                            opt => opt.MapFrom(src => src.Fields.IssueType.IconUrl))
                        .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Fields.Created))
                        .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Fields.Updated))
                        .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Fields.Description))
                        .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Fields.Summary))
                        .ForMember(dest => dest.FixVersions, opt => opt.MapFrom(src => src.Fields.FixVersions != null ? src.Fields.FixVersions.Select(p => p.Name).ToArray() : new string[0]))
                        
                        .ForMember(dest => dest.TimeSpent, opt => opt.MapFrom(src => src.Fields.TimeTracking.TimeSpent))
                        .ForMember(dest => dest.DevelopmentEstimateSeconds, opt => opt.MapFrom(src => src.Fields.TimeTracking.OriginalEstimateSeconds))
                        .ForMember(dest => dest.TestingEstimateSeconds, opt => opt.MapFrom(src => src.Fields.ActualTestTimeInDays * EstimatesExtension.SECONDS_IN_WORKDAY))
                        .ForMember(dest => dest.TimeSpentSeconds, opt => opt.MapFrom(src => src.Fields.TimeTracking.TimeSpentSeconds))

                        .ForMember(dest => dest.IncludedInSprintDate, opt => opt.MapFrom(src => src.Additions.IncludedInSprint.Created))
                        .ForMember(dest => dest.TechnicalAnalysisCompletedDate, opt => opt.MapFrom(src => src.Additions.TechnicalAnalysisCompleted.Created))
                        .ForMember(dest => dest.DevelopmentCompletedDate, opt => opt.MapFrom(src => src.Additions.DevelopmentCompleted.Created))
                        .ForMember(dest => dest.CodeReviewCompletedDate, opt => opt.MapFrom(src => src.Additions.СodeReviewCompleted.Created))
                        .ForMember(dest => dest.TestingCompletedDate, opt => opt.MapFrom(src => src.Additions.TestingCompleted.Created))
                        
                        .ForMember(dest => dest.IsChildrenEstimates, opt => opt.Ignore())
                        .ForMember(dest => dest.Group, opt => opt.Ignore());

                    cfg.CreateMap<IssueModel, IssueChangesModel>()
                        .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Fields.Created))
                        .ForMember(dest => dest.Updated, opt => opt.MapFrom(src => src.Fields.Updated));

                    cfg.CreateMap<IssueHistoryModel, IssueTransitionModel>()
                        .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))
                        .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Items.Where(p => p.FieldId == "status" && p.FieldType == "jira").Select(p => p.ToValue).FirstOrDefault()));

                    cfg.CreateMap<UserPreferences, UserPreferencesViewModel>();
                });

            Mapper.AssertConfigurationIsValid();
        }
    }
}
