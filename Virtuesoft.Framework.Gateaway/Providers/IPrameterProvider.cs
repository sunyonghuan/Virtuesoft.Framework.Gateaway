namespace Virtuesoft.Framework.Gateaway.Providers;
/// <summary>
/// 参数解析
/// </summary>
public interface IPrameterProvider
{
    /// <summary>
    /// 获取所有参数
    /// </summary>
    /// <returns></returns>
    public Task<IDictionary<string, object>> Get();
}

