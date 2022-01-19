namespace Virtuesoft.Framework.Gateaway.Convertors;
/// <summary>
/// 参数转换接口
/// </summary>
public interface IPrameterConvertor
{
    /// <summary>
    /// 转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from"></param>
    /// <returns></returns>
    public T Convert<T>(object from);
    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="taget"></param>
    /// <param name="from"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public object Convert(Type taget, object from, object defaultValue = null);
}

