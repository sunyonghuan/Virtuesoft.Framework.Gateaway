using System.Text;

var builder = WebApplication.CreateBuilder(args);
var sqlConnectionString = builder.Configuration.GetConnectionString("DbContext");
builder
    .Logging
    .AddConsole()
    //配置数据库日志记录
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
    .AddGateaway(option => {
        //设置编码
        option.Encoding = Encoding.UTF8;
        //验证参数 args:IDictionary<string, string>
        //返回 true验证成功,返回false 验证失败,返回msg
        option.OnVerifyPrameters = (args, context) => (true,"ok");
        //验证签名, 自定义签名算法,比如MD5 返回true:成功,false:失败
        option.OnVerifySign = (args, context) => true;
        //验证ip ,这里IP可能是多个IP以逗号分割,需要自己处理,或者自己使用context获取IP 
        option.OnVerifyIP = (context, ip) => true;
        //访问开始事件
        option.OnBeginRequest =async context =>await Task.CompletedTask;
        //访问结束事件
        option.OnEndReqeust = async context => await Task.CompletedTask;
        //格式化返回参数
        //status(bool):执行状态
        //code(int):状态代码
        //message(string):执行结果消息,如果发生错误则为错误消息
        //data(object):返回数据
        //需要安装  Virtuesoft.Framework.JsonExtensions 扩展
        option.OnFormatResult = (status, code, message, data) => new
            {
                s = status, c = code, m = message, d = data
            }.ToJson();

    });


var app = builder.Build();
//使用接口
app.UseGateaway();
app.Run("http://*:5000");
