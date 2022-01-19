
/// <summary>
/// 
/// </summary>
[Gateaway(Display = "接口文档", Name = "api")]
public class ApiGateaway : GateawayBase
{
    /// <summary>
    /// 
    /// </summary>
    public override string Name => "api";
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [Gateaway(Display = "获取所有的接口信息", Name = "doc")]
    public object doc()
    {
        return (Context.RequestServices.GetService(typeof(GateawayDescriptorCollection)) as GateawayDescriptorCollection)
            .GroupBy(t => t.Gateaway)
            .Select(t => new
            {
                name = t.Key.GetCustomAttribute<GateawayAttribute>()?.Name ?? t.Key.Name.ToLower().TrimEnd("gateaway".ToCharArray()),
                display = t.Key.GetCustomAttribute<GateawayAttribute>()?.Display,
                methods = t.OrderBy(m => m.Name)
                .Select(m => new
                {
                    path = m.Path,
                    name = m.Name,
                    display = m.Display,
                    parameters = m.Parameters.OrderBy(p => p.Position).Select(p => new { name = p.Name, type = p.ParameterType.Name, @default = p.DefaultValue }),
                    results = m.Method.ReturnType.Name
                })

            });
    }
}

