namespace Virtuesoft.Framework.Gateaway;
/// <summary>
/// 
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 注册接口中间件
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseGateaway(this IApplicationBuilder app)
    => app.UseMiddleware<GateawayMiddleware>();

    /// <summary>
    /// 添加接口授权中间件
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseGateawayAuthentication(this IApplicationBuilder app)
    => app.UseMiddleware<GateawayAuthenticationMiddleware>();
}

