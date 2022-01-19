namespace Virtuesoft.Framework.Gateaway.Providers;
/// <summary>
/// 
/// </summary>
public abstract class PrameterProviderBase : IPrameterProvider
{
    /// <summary>
    /// 
    /// </summary>
    protected IHttpContextAccessor HttpContextAccessor { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContextAccessor"></param>
    public PrameterProviderBase(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }
    /// <summary>
    /// 获取所有参数
    /// </summary>
    /// <returns></returns>
    public abstract Task<IDictionary<string, object>> Get();
}

