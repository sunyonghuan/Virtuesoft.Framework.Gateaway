namespace Virtuesoft.Framework.Gateaway.Providers;
/// <summary>
/// 
/// </summary>
[ContentType("application/query-string")]
public class QueryStringPrameterProvider : PrameterProviderBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public QueryStringPrameterProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Task<IDictionary<string, object>> Get()
    {
        var result = new Dictionary<string, object>();
        foreach (var item in HttpContextAccessor.HttpContext?.Request?.Query)
        {
            if (item.Value.Count > 1)
                result.Add(item.Key, item.Value.ToArray());
            else
                result.Add(item.Key, item.Value.ToString());
        }
        return Task.FromResult((IDictionary<string, object>)result);
    }
}

