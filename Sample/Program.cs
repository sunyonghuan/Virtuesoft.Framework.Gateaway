var builder = WebApplication.CreateBuilder(args);
var sqlConnectionString = builder.Configuration.GetConnectionString("DbContext");
builder
    .Logging
    .AddConsole()
    //�������ݿ���־��¼
    .AddSqlServer(option => {
        option.ConnectionString= sqlConnectionString;
    })
    .Services
    //����EF
    .AddDbContextPool<DataContext>(option =>
    {
        option.UseSqlServer(sqlConnectionString);
    })
    //���ýӿ�
    .AddGateaway();


var app = builder.Build();
//ʹ�ýӿ�
app.UseGateaway();
app.Run("http://*:5000");
