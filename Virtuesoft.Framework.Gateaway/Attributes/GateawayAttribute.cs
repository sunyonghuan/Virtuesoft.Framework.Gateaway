namespace Virtuesoft.Framework.Gateaway.Attributes;
/// <summary>
/// 接口属性
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class GateawayAttribute : Attribute
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = null;

    /// <summary>
    /// 描述
    /// </summary>
    public string Display { get; set; } = "";
    /// <summary>
    /// 是否允许外部调用
    /// </summary>
    public bool CallEnable { get; set; } = true;

    /// <summary>
    /// 是否显示到文档
    /// </summary>
    public bool ShowEnable { get; set; } = true;
}
/// <summary>
/// 方法的显示属性
/// 可以将方法不公开给文档
/// 也可以定义该方法不允许外包调用
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
[Obsolete("请使用最新的 GateawayAttribute 定义")]
public class MethodDisplayAttribute : GateawayAttribute
{

}
/// <summary>
/// 接口显示属性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
[Obsolete("请使用最新的 GateawayAttribute 定义")]
public class GateawayDisplayAttribute : GateawayAttribute
{

}

