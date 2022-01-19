
namespace Virtuesoft.Framework.Gateaway.Providers;

/// <summary>
/// multipart/form-data
/// </summary>
[ContentType("multipart/form-data")]
public class FormDataPrameterProvider : PrameterProviderBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public FormDataPrameterProvider(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {

    }
    /// <summary>
    /// 获取所有参数
    /// </summary>
    /// <returns></returns>
    public override Task<IDictionary<string, object>> Get()
    {
        var result = new Dictionary<string, object>();
        foreach (var item in HttpContextAccessor?.HttpContext?.Request?.Form?.Files)
        {
            if (result.ContainsKey(item.Name))
            {
                var value = result[item.Name];
                if (value is Array ar)
                {
                    var o = new ArraySegment<IFormFile>((IFormFile[])ar);
                    o.Append(item);
                    result[item.Name] = o.Array;
                    continue;
                }
                var arr = new ArraySegment<IFormFile>(new IFormFile[] { (IFormFile)value, item });
                result[item.Name] = arr.Array;
                continue;
            }
            result.Add(item.Name, item);
        }

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

