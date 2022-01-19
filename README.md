# Virtuesoft.Framework.Gateaway
简单的接口框架,升级到.NET6
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

