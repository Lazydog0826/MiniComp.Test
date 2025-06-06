using MiniComp.ApiLog;
using MiniComp.Autofac;
using MiniComp.Cache;
using MiniComp.Core.App;
using MiniComp.Core.App.CoreException;
using MiniComp.Core.Extension;
using MiniComp.SnowFlake;
using MiniComp.SqlSugar.DynamicExpression;
using SqlSugar;
using Setup = MiniComp.ApiLog.Setup;

var builder = WebApplication.CreateBuilder(args);
HostApp.Configuration = builder.Configuration;
HostApp.AppRootPath = AppDomain.CurrentDomain.BaseDirectory;
HostApp.HostEnvironment = builder.Environment;
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<RecordRequestFilter>();
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
builder.Services.AddControllers();
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

var app = builder.Build();
HostApp.RootServiceProvider = app.Services;
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
app.MapControllers();
app.Run();
