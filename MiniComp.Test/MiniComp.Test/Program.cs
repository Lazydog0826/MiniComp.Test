using MiniComp.ApiLog;
using MiniComp.Autofac;
using MiniComp.Cache;
using MiniComp.Core;
using MiniComp.Core.App;
using MiniComp.Core.App.CoreException;
using MiniComp.Core.App.JsonConverter;
using MiniComp.Core.Extension;
using MiniComp.SnowFlake;
using MiniComp.SqlSugar.DynamicExpression;
using SqlSugar;
using Setup = MiniComp.ApiLog.Setup;

await HostApp.StartWebAppAsync(
    args,
    async builder =>
    {
        builder
            .Services.AddControllers(opt =>
            {
                opt.Filters.Add<RecordRequestFilter>();
                opt.Filters.Add<CoreActionFilter>();
            })
            .AddControllersAsServices()
            .ConfigureApiBehaviorOptions(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            })
            .AddNewtonsoftJson(opt =>
            {
                NewtonsoftJsonConfiguration.Configure.Invoke(opt.SerializerSettings);
            });
        builder
            .Services.AddApiLog()
            .Configure<RecordLogEvent>(opt =>
            {
                opt.Event += Setup.AnsiConsoleLogger;
                opt.Event += Setup.WriteLogFile;
            });
        builder.Services.AddCacheService();
        builder.Host.UseAutofac();
        builder.Services.AddComparisonType();
        builder.Services.AddScoped<ISqlSugarClient>(_ => new SqlSugarClient(
            new ConnectionConfig
            {
                DbType = DbType.MySql,
                ConfigId = 0,
                ConnectionString = "",
                IsAutoCloseConnection = true,
            }
        ));
        builder.Services.AutoAddDependency(ObjectExtension.GetProjectAllType());
        SnowFlakeConfiguration.SetOption(6, 6, new DateTime(2025, 1, 1, 0, 0, 0), 60000, 70000);
        builder.Services.AddHostedService<SnowFlakeHostService>();
        builder.Services.AddCors("*", "*", "*");
        await Task.CompletedTask;
    },
    async app =>
    {
        app.UseCors();
        app.Use(
            async (context, next) =>
            {
                var apiLogService = context.RequestServices.GetRequiredService<IApiLogService>();
                try
                {
                    await next.Invoke();
                }
                catch (CustomException ce)
                {
                    var res = ce.GetWebApiResponse();
                    apiLogService.SetExceptionResponseResult(ce, res, false, res.Code);
                }
                catch (Exception ex)
                {
                    var res = WebApiResponse.Error("系统内部异常");
                    apiLogService.SetExceptionResponseResult(ex, res, true, res.Code);
                }
                finally
                {
                    await apiLogService.SaveApiLogAsync();
                }
            }
        );
        var staticFilesPath = Path.Join(HostApp.AppRootPath, "wwwroot");
        if (Directory.Exists(staticFilesPath))
        {
            app.UseStaticFiles();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await Task.CompletedTask;
    }
);
