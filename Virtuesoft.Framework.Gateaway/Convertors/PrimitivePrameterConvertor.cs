
namespace Virtuesoft.Framework.Gateaway.Convertors;


/// <summary>
/// 基元类型转换器
/// </summary>
public class PrimitivePrameterConvertor : IPrameterConvertor
{
    ILogger<PrimitivePrameterConvertor> Logger { get; }
    /// <summary>
    /// 
    /// </summary>
    public PrimitivePrameterConvertor(ILogger<PrimitivePrameterConvertor> logger)
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
        if (taget.IsArray)
        {
            var t = typeof(ArrayList);
            var et = taget.GetElementType();
            ArgumentNullException.ThrowIfNull(et, "Target type is null in PrimitivePrameterConvertor.Convert");
            ArgumentNullException.ThrowIfNull(from, "from object is null in PrimitivePrameterConvertor.Convert");
            var o = Activator.CreateInstance(t);
            var append = t.GetMethod("Add");
            var arry = t.GetMethod("ToArray", new Type[] { typeof(Type) });
            foreach (var item in from as Array)
            {
                var value = convert(et, item, defaultValue);
                append?.Invoke(o, new object[] { value });
            }
            return arry?.Invoke(o, new object[] { et });
        }
        return convert(taget, from, defaultValue);


    }
    object convert(Type taget, object from, object defaultValue = null)
    {
        try
        {
            if (taget.Equals(from.GetType())) return from;
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

