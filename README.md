# Virtuesoft.Framework.Gateaway
高性能,轻量级,简单的接口框架,升级到.NET6
## 简介
```
自2018年来一直在使用与更新迭代,一切为了简单,为了简单的一切.
只是为了做个短小精干的接口服务,去除臃肿.该框架在生产环境中表现良好,
扩展性强,性能优越,特别是其足够的简单!
```
```
用于接口快速开发,扩展签名验证,IP限制,格式化参数,访问记录等等.
给外包部门使用广受好评,曾经为客户撑起上亿/日的交易成绩,
顶住了大几十万的访问并发.
```
## 安装使用
```
Install-Package Virtuesoft.Framework.Gateaway -Version 6.0.1
```
## Program.cs 简单使用
```C#
using Virtuesoft.Framework.Gateaway;

var builder = WebApplication.CreateBuilder(args);
var sqlConnectionString = builder.Configuration.GetConnectionString("DbContext");
builder
    .Logging
    .AddConsole()
    .Services
    //配置EF
    .AddDbContextPool<DataContext>(option =>
    {
        option.UseSqlServer(sqlConnectionString);
    })
    .AddGateaway();
var app = builder.Build();
//使用接口
app.UseGateaway();
app.Run("http://*:5000");
```
## 编写接口
```C#
public class User:GateawayBase
{
    ILogger<User> Logger { get; }
    DataContext Db { get; }
    public User(DataContext context,ILogger<User> logger)
    {
        Logger=logger;
        Db = context;
    }
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
     public dynamic Detail(string id){
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
## 访问
```
默认文档地址
GET:http://localhost:5000/api/doc
POST: JSON {"method":"api.doc"}

POST / HTTP/1.1
Host: localhost:5000
Content-Type: application/json
User-Agent: App/1.0

{
	"method":"user.detail",
    "id":1000
}

GET / HTTP/1.1 http://localhost:5000/user/detail?id=10000

```
## 数据返回
```
//弱类型返回
public object Get(){
    //自定义其他重载
    return Success();//默认执行成功,不返回任何数据
    return Success(new {name="孙悟空"});//带数据返回
    return Error();
    return Error("账户名密码错误");
}
//强类型返回
public Account Get(){
    return new Account(){Name="孙悟空"};
}
```
### 默认格式
```json
{
    "s": true,//执行状态
    "c": 200, //执行代码 授权失败:401,其他代码可以自定义
    "m": "ok",//执行消息
    //返回数据
    "d": {
        "ID": "99ead98b666549b590a529f453336e41",
        "No": 7364,
        "Name": "N9189",
        "Phone": "+861642609352",
        "CreateTime": "2022-01-20T00:22:32.5188842+08:00"
    }
}
```
## 路由与参数
```C#
路由参数:均小写,
参数支持:json,from,path
方法支持:POST,GET

//默认路由 user.*
public class User:GateawayBase
{
    //默认路由 user.get
    public object Get(string id){
        ...
    }
}
//自定义路由
[Gateaway(Name ="account",Display ="用户接口")]
public class User:GateawayBase{
    //完整路由: account.detail.get
    [Gateaway(Name ="detail.get",Display ="获取用户")]
    public object Get(string id){}
}

```
