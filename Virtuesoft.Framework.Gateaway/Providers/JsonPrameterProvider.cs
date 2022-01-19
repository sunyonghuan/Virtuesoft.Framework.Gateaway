

namespace Virtuesoft.Framework.Gateaway.Providers;

/// <summary>
/// application/json
/// </summary>
[ContentType("application/json")]
public class JsonPrameterProvider : PrameterProviderBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public JsonPrameterProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {

    }
    /// <summary>
    /// 获取所有参数
    /// </summary>
    /// <returns></returns>
    public override async Task<IDictionary<string, object>> Get()
    {

        var result = await JsonSerializer.DeserializeAsync<IDictionary<string, object>>(HttpContextAccessor?.HttpContext?.Request?.Body, new JsonSerializerOptions()
        {
            //IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true
        });
        return result??new Dictionary<string, object>();
    }
}

