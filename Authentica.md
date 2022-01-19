# Authentication 授权验证
## Program.cs
```C#
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DbContext");
builder
  .Logging
  .AddConsole()
  .Services
  .AddGateawayAuthentication(option =>
    {
        option.HeaderName = "Security-Auth";//header 名称
        option.Exclude = new string[] { "mine.sign", "account.reg", "set.get" };//排除验证的,method
        option.ExpiredTime = TimeSpan.FromDays(2);//验证过期时间
      //SecurityKey : 用于加密的密钥
      //GenerateAccessToken (Func<string, string>) : accessToken生成,输入明文返回accesstoken
      //ReverseAccessToken  (Func<string, string>) : 反向解析accessToken 为明文
    })
  .AddGateaway();
var app = builder.Build();
app.
    .UseGateawayAuthentication()
    .UseGateaway();
app.Run("http://*:5000");
```
