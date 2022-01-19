namespace Virtuesoft.Framework.Gateaway.Convertors;


/// <summary>
/// 枚举转换器
/// </summary>
public class EnumPrameterConvertor : IPrameterConvertor
{
    ILogger<EnumPrameterConvertor> Logger { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public EnumPrameterConvertor(ILogger<EnumPrameterConvertor> logger)
    {
        Logger = logger;
    }
    /// <summary>
    /// 转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from"></param>
    /// <returns></returns>
    public T Convert<T>(object from)
    {
        if (TypeDescriptor.GetConverter(typeof(int)).IsValid(from))
            return (T)TypeDescriptor.GetConverter(typeof(int)).ConvertFrom(from);
        if (!TypeDescriptor.GetConverter(typeof(T)).IsValid(from))
            return default(T);
        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(from);
    }
    /// <summary>
    /// 转换
    /// </summary>
    /// <param name="taget"></param>
    /// <param name="from"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public object Convert(Type taget, object from, object defaultValue = null)
    {
        if (from is null) return defaultValue;
        try
        {
            if (TypeDescriptor.GetConverter(typeof(int)).IsValid(from))
                return TypeDescriptor.GetConverter(typeof(int)).ConvertFrom(from);
            if (!TypeDescriptor.GetConverter(taget).IsValid(from))
                return defaultValue;
            return TypeDescriptor.GetConverter(taget).ConvertFrom(from);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Convert:{ex.Message}");
            return defaultValue;
        }

    }
}

