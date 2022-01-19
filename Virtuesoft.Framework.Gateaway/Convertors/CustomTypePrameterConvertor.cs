namespace Virtuesoft.Framework.Gateaway.Convertors;


/// <summary>
/// 自定义类型转换器
/// </summary>
public class CustomTypePrameterConvertor : IPrameterConvertor
{
    ILogger<CustomTypePrameterConvertor> Logger { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public CustomTypePrameterConvertor(ILogger<CustomTypePrameterConvertor> logger)
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
        if (from is null) return default(T);
        if (TypeDescriptor.GetConverter(typeof(T)).IsValid(from))
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(from);
        if (from is T) return (T)from;
        if (from is string)
            return JsonSerializer.Deserialize<T>(from as string);
        if (from is JsonElement)
            return JsonSerializer.Deserialize<T>(((JsonElement)from).GetRawText());
        if (from is IDictionary<string, object>)
        {
            var result = Activator.CreateInstance<T>();
            var values = from as IDictionary<string, object>;
            var propertys = typeof(T).GetProperties();
            foreach (var m in propertys)
            {
                if (!m.CanWrite) continue;
                m.SetValue(result, Convert(m.PropertyType, values.Where(t => t.Key.Equals(m.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value ?? default));
            }
            return result;
        }
        return default(T);
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
            if (TypeDescriptor.GetConverter(taget).IsValid(from))
                return TypeDescriptor.GetConverter(taget).ConvertFrom(from);
            var valueType = from.GetType();
            if (taget == valueType || taget.IsAssignableFrom(valueType) || valueType.IsAssignableFrom(taget) || taget.IsSubclassOf(valueType))
                return from;
            if (from is string)
                return JsonSerializer.Deserialize(from as string, taget);
            if (from is JsonElement)
                return JsonSerializer.Deserialize(((JsonElement)from).GetRawText(), taget);
            if (from is IDictionary<string, object>)
            {
                var result = Activator.CreateInstance(taget);
                var values = from as IDictionary<string, object>;
                var propertys = taget.GetProperties();
                foreach (var m in propertys)
                {
                    if (!m.CanWrite) continue;
                    m.SetValue(result, Convert(m.PropertyType, values.Where(t => t.Key.Equals(m.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value ?? default));
                }
                return result;
            }
            return defaultValue;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Convert:{ex.Message}");
            return defaultValue;
        }
    }
}

