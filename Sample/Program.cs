using System.Text;

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
    .AddGateaway(option => {
        //���ñ���
        option.Encoding = Encoding.UTF8;
        //��֤���� args:IDictionary<string, string>
        //���� true��֤�ɹ�,����false ��֤ʧ��,����msg
        option.OnVerifyPrameters = (args, context) => (true,"ok");
        //��֤ǩ��, �Զ���ǩ���㷨,����MD5 ����true:�ɹ�,false:ʧ��
        option.OnVerifySign = (args, context) => true;
        //��֤ip ,����IP�����Ƕ��IP�Զ��ŷָ�,��Ҫ�Լ�����,�����Լ�ʹ��context��ȡIP 
        option.OnVerifyIP = (context, ip) => true;
        //���ʿ�ʼ�¼�
        option.OnBeginRequest =async context =>await Task.CompletedTask;
        //���ʽ����¼�
        option.OnEndReqeust = async context => await Task.CompletedTask;
        //��ʽ�����ز���
        //status(bool):ִ��״̬
        //code(int):״̬����
        //message(string):ִ�н����Ϣ,�������������Ϊ������Ϣ
        //data(object):��������
        //��Ҫ��װ  Virtuesoft.Framework.JsonExtensions ��չ
        option.OnFormatResult = (status, code, message, data) => new
            {
                s = status, c = code, m = message, d = data
            }.ToJson();

    });


var app = builder.Build();
//ʹ�ýӿ�
app.UseGateaway();
app.Run("http://*:5000");
