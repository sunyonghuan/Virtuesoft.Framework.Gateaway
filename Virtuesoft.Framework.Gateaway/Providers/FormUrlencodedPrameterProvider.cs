
namespace Virtuesoft.Framework.Gateaway.Providers;


/// <summary>
/// application/x-www-form-urlencoded
/// </summary>
[ContentType("application/x-www-form-urlencoded")]
public class FormUrlencodedPrameterProvider : PrameterProviderBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public FormUrlencodedPrameterProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {

    }
    /// <summary>
    /// 获取所有参数
    /// </summary>
    /// <returns></returns>
    public override Task<IDictionary<string, object>> Get()
    {
        var result = new Dictionary<string, object>();
        foreach (var item in HttpContextAccessor.HttpContext?.Request?.Form)
        {
            if (item.Value.Count > 1)
                result.Add(item.Key, item.Value.ToArray());
            else
                result.Add(item.Key, item.Value.ToString());
        }

        return Task.FromResult((IDictionary<string, object>)result);
    }
}

