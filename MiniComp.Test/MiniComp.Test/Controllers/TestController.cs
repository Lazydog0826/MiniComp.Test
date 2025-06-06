using Microsoft.AspNetCore.Mvc;
using MiniComp.HttpRequest;
using MiniComp.SqlSugar.DynamicExpression;
using Yitter.IdGenerator;

namespace MiniComp.Test.Controllers;

[ApiController]
public class TestController : ControllerBase
{
    [HttpPost("Test")]
    public IActionResult Test([FromBody] DynamicExpressionSearchRequest request)
    {
        // var sql = _db.Queryable<TestModel>()
        //     .Where(request.Search)
        //     .OrderBy(request.Sort)
        //     .ToSqlString();
        // return Ok(sql);
        throw new NotImplementedException();
    }

    [HttpPost("Test2")]
    public IActionResult Test2([FromBody] DynamicExpressionSearchRequest request)
    {
        // var sql = _db.Queryable<TestModel>()
        //     .LeftJoin<TestModel2>((t1, t2) => t1.TestModel2Id == t2.Id)
        //     .Where(request.Search)
        //     .OrderBy(request.Sort)
        //     .ToSqlString();
        // return Ok(sql);
        throw new NotImplementedException();
    }

    [HttpGet("Test3")]
    public async Task<IActionResult> Test3()
    {
        // await _cacheService.AddCacheAsync(
        //     "test",
        //     new TestModel
        //     {
        //         Id = 123,
        //         Name = "董雅涵",
        //         IsActive = false,
        //         TestModel2Id = 0,
        //     },
        //     TimeSpan.FromMinutes(5)
        // );
        //
        // var a = await _cacheService.GetCacheAsync<TestModel>("test");
        // var c = await _cacheService.IsExistAsync("test");
        // await _cacheService.KeyExpireAsync("test", TimeSpan.FromMinutes(6));
        // await _cacheService.DeleteCacheAsync("test");
        //
        // await _cacheService.LockAsync(
        //     "test2",
        //     async () =>
        //     {
        //         await Task.CompletedTask;
        //     }
        // );
        //
        // var data = await _cacheService.GetOrCreateCacheAsync(
        //     "test3",
        //     async () =>
        //     {
        //         await Task.CompletedTask;
        //         return new List<int> { 1, 2, 3 };
        //     },
        //     TimeSpan.FromMinutes(6)
        // );
        await Task.CompletedTask;
        return Ok(new { code = 200 });
    }

    [HttpGet("Test4")]
    public IActionResult Test4()
    {
        return Ok(YitIdHelper.NextId());
    }

    [HttpGet("Test5")]
    public IActionResult Test5()
    {
        throw new Exception("测试");
    }

    [HttpGet("Test6")]
    public async Task<IActionResult> Test6()
    {
        var res = await HttpRequest.HttpRequest.RequestAsync<object>(
            new HttpRequestModel
            {
                UriBuilder = new UriBuilder("https://sz.tdzh.cc/api/auth/login"),
                HttpMethod = HttpMethod.Post,
                HttpContent = JsonContent.Create(
                    new
                    {
                        account = "superadmin",
                        password = "123456",
                        code = "",
                        codeId = 0,
                    }
                ),
            }
        );
        return Ok(res);
    }
}
