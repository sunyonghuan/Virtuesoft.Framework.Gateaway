# Virtuesoft.Framework.Gateaway
高性能,轻量级,简单的接口框架,升级到.NET6.自主研发,生产环境已经从2018年使用至今,迭代了无数个版本,曾经战绩抗住每日上亿交易额,几十万用户,抗住过高并发(最头疼的问题),早先版本做过开源,但是涉及到其他插件并未解耦,所以不全,现在提出来单独开源.欢迎讨论.
## 功能简介
```
支持 授权认证 token,自带JWT,支持自主加密算法
支持IP 黑白名单
支持与aspnetcore 组合使用,曾经用在winform 和windows service上
可组合 翻译插件 缓存插件 单件登录 自定义日志 等
建议EF和dapper组合使用,曾经在高并发下发现过EF的问题,最后组合DAPPER后提升不少,这是血的教训
```
## 安装包
```PM
Install-Package Virtuesoft.Framework.Gateaway -Version 6.0.1
或者
dotnet add package Virtuesoft.Framework.Gateaway --version 6.0.1
```
## 使用配置
```C#
var builder = WebApplication.CreateBuilder(args);
var sqlConnectionString = builder.Configuration.GetConnectionString("DbContext");
builder
    .Logging
    .AddConsole()
    //配置数据库日志记录(可选项)
    //需要安装日志插件
    //Install-Package Virtuesoft.Framework.Logging.SqlServer -Version 5.0.1
    .AddSqlServer(option => {
        option.ConnectionString= sqlConnectionString;
    })
    .Services
    //配置EF
    .AddDbContextPool<DataContext>(option =>
    {
        option.UseSqlServer(sqlConnectionString);
    })
    //配置接口
    .AddGateaway();


var app = builder.Build();
//使用接口
app.UseGateaway();
app.Run("http://*:5000");
```
## 编写接口 ..省略数据模型
```C#
//Gateaway 属性非必要,默认 小写 并移除gateaway字符
[Gateaway(Name ="user",Display ="用户接口")]
public class UserGateaway:GateawayBase
{
    ILogger<UserGateaway> Logger { get; }
    DataContext Db { get; }
    public UserGateaway(DataContext context,ILogger<UserGateaway> logger)
    {
        Logger=logger;
        Db = context;
    }

    //Gateaway 属性非必要,默认 小写 方法名
    [Gateaway(Name = "add", Display = "添加一个用户")]
    public async Task<object> Add()
    {
        try
        {
            var result= await Db.Accounts.AddAsync(new MemberAccount()
            {
                Name = $"N{new Random((int)DateTime.Now.Ticks).Next(1111, 9999)}",
                Role = Db.Roles.FirstOrDefault()
            });
            await Db.SaveChangesAsync();
            return Success(new {
                result.Entity.ID,
                result.Entity.No,
                result.Entity.Name,
                result.Entity.Phone,
                result.Entity.CreateTime
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Add:{ex.Message}");
            throw;
        }

    }

    //Gateaway 属性非必要,默认 小写 方法名
    [Gateaway(Name="get", Display ="获取最新的用户")]
    public object Get()
    {
        var result = Db.Accounts
            .OrderByDescending(t => t.No)
            .Select(t => new
            {
                t.ID,
                t.Name,
                t.Phone,
                t.Age,
                t.No,
                t.Status,
                Role = new
                {
                    t.Role.ID,
                    t.Role.Name
                }
            })
            .FirstOrDefault();
        return Success(result);
    }

    //参数支持 [post]froms ,[post]json,[get]path;
    //需要注意 需要正确的 Content-Type
    [Gateaway(Name = "detail", Display = "根据ID 查询一个用户")]
    public dynamic Detail(string id) {
        var t = Db.Accounts.Include(t=>t.Role).FirstOrDefault(t=>t.ID==id);
        return new
        {
            t.ID,
            t.Name,
            t.Phone,
            t.Age,
            t.No,
            t.Status,
            Role = new
            {
                t.Role.ID,
                t.Role.Name
            }
        };
    }
}
```
## 路由访问 Http
```HTTP
POST / HTTP/1.1
Host: 10.0.0.151:5000
Content-Type: application/json
User-Agent: App/1.0
{
	"method":"user.add"
}
```

## 路由访问 js
``` JavaScript
var myHeaders = new Headers();
myHeaders.append("Content-Type", "application/json");
myHeaders.append("User-Agent", "App/1.0");

var raw = JSON.stringify({"method":"user.add"});

var requestOptions = {
  method: 'POST',
  headers: myHeaders,
  body: raw,
  redirect: 'follow'
};

fetch("http://10.0.0.151:5000", requestOptions)
  .then(response => response.text())
  .then(result => console.log(result))
  .catch(error => console.log('error', error));
```
## 返回数据格式 (可配置自定义格式)
```json
{
    "s": true,
    "c": 200,
    "m": "ok",
    "d": {
        "ID": "99ead98b666549b590a529f453336e41",
        "No": 7364,
        "Name": "N9189",
        "Phone": "+861642609352",
        "CreateTime": "2022-01-20T00:22:32.5188842+08:00"
    }
}
```
