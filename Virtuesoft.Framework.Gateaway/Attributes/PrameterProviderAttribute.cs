namespace Virtuesoft.Framework.Gateaway.Attributes;
/// <summary>
/// ContentType 参数转换属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ContentTypeAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="contentType"></param>
    public ContentTypeAttribute(string contentType)
    {
        ContentType = contentType;
    }
    /// <summary>
    /// application/json
    /// </summary>
    public string ContentType { get; set; } = "application/json";
}

