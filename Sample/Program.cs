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
    .AddGateaway();


var app = builder.Build();
//使用接口
app.UseGateaway();
app.Run("http://*:5000");
