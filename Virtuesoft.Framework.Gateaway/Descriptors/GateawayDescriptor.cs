
/// <summary>
/// 接口描述
/// </summary>
public class GateawayDescriptor
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 路径
    /// 如 跟路径:/api/images
    /// 本接口路径 /object/delete
    /// 实际路径 : /images/api/object/delete
    /// </summary>
    public string Path { get; }
    /// <summary>
    /// 类型
    /// </summary>
    public Type Gateaway { get; }
    /// <summary>
    /// 方法描述
    /// </summary>
    public string Display { get; }
    /// <summary>
    /// 公共方法
    /// </summary>
    public MethodInfo Method { get; }
    /// <summary>
    /// 参数
    /// </summary>
    public ParameterInfo[] Parameters { get; }
    /// <summary>
    /// 
    /// </summary>
    public IAuthentication AuthenticationAttribute { get; }
    /// <summary>
    /// 实例化一个接口描述文件
    /// </summary>
    /// <param name="gateaway"></param>
    /// <param name="method"></param>
    public GateawayDescriptor(Type gateaway, MethodInfo method) :
        this(gateaway, method, gateaway.GetCustomAttribute<GateawayAttribute>(),
            gateaway.GetCustomAttributes().FirstOrDefault(t => t is IAuthentication) as IAuthentication,
            method.GetCustomAttribute<GateawayAttribute>(),
            method.GetCustomAttributes().FirstOrDefault(t => t is IAuthentication) as IAuthentication)
    {

    }
    /// <summary>
    /// 实例化一个接口描述文件
    /// </summary>
    /// <param name="gateaway"></param>
    /// <param name="method"></param>
    /// <param name="gateawayAttribute"></param>
    /// <param name="gateawayAuthentication"></param>
    /// <param name="methodAttrbute"></param>
    /// <param name="methodAuthentication"></param>
    public GateawayDescriptor(Type gateaway, MethodInfo method,
            GateawayAttribute gateawayAttribute, IAuthentication gateawayAuthentication,
            GateawayAttribute methodAttrbute, IAuthentication methodAuthentication)
    {
        Gateaway = gateaway;
        Method = method;
        Name = methodAttrbute?.Name?.ToLower() ?? Method.Name.ToLower().Trim();
        var gateawayName = gateawayAttribute?.Name?.ToLower() ?? gateaway.Name.ToLower().TrimEnd("gateaway".ToCharArray());
        Display = methodAttrbute?.Display??"";
        Path = new string[] { gateawayName, Name }.Join(".");
        Parameters = Method.GetParameters();
        AuthenticationAttribute = methodAuthentication??gateawayAuthentication;
    }
}

