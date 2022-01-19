namespace Virtuesoft.Framework.Gateaway.Authentication;


/// <summary>
/// 
/// </summary>
public class GateawayAuthenticationMiddleware : IMiddleware
{
    /// <summary>
    /// 
    /// </summary>
    ILogger<GateawayAuthenticationMiddleware> Logger { get; }
    /// <summary>
    /// 
    /// </summary>
    IOptionsMonitor<GateawayAuthenticationOption> Options { get; }
    /// <summary>
    /// 
    /// </summary>
    IGateawayAuthenticationService Service { get; }
    /// <summary>
    /// 
    /// </summary>
    protected GateawayDescriptorCollection Gateaways { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    /// <param name="service"></param>
    /// <param name="gateaways"></param>
    /// <param name="logger"></param>
    public GateawayAuthenticationMiddleware(IOptionsMonitor<GateawayAuthenticationOption> option, IGateawayAuthenticationService service, GateawayDescriptorCollection gateaways, ILogger<GateawayAuthenticationMiddleware> logger)
    {
        Options = option; Logger = logger; Service = service; Gateaways = gateaways;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="forms"></param>
    /// <returns></returns>
    protected GateawayDescriptor GetDescriptor(HttpContext context, IDictionary<string, object> forms)
    {
        string path = string.Empty, queryPath = context.Request.Path.ToString().Split("/").Where(t => !t.IsNullOrEmpty()).Join(".");
        GateawayDescriptor descriptor = null;
        if (forms.TryGetValue("method", out var method))
        {
            var m = method.ToString();
            descriptor = Gateaways.Where(t => t.Path.Equals(m, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        if (descriptor == null)
            descriptor = Gateaways.Where(t => queryPath.EndsWith(t.Path, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        if (descriptor == null)
            descriptor = Gateaways.Where(t => t.Path.Equals("api.doc", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        if (descriptor == null)
            throw new ArgumentOutOfRangeException($"descriptor not maping rote");
        return descriptor;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var forms = await context.GetPrametersAsync();
        if (!forms.TryGetValue("method", out var path))
            path = context.Request.Path.Value.Split("/").Where(t => !t.IsNullOrEmpty()).Join("/");
        var config = Options.CurrentValue;
        #region 无需授权验证
        if (config.Exclude.Any(t => t.Equals(path as string, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }
        var descriptor = GetDescriptor(context, forms);
        if (descriptor.AuthenticationAttribute is not null && descriptor.AuthenticationAttribute is AllowAnyoneAttribute)
        {
            await next(context);
            return;
        }
        #endregion
        if (config.HeaderName.IsNullOrEmpty())
        {
            await context.WriteAsync(config.FailedFormat(false, 401, "未授权"));
            return;
        }
        var ishead = context.Request.Headers.TryGetValue(config.HeaderName, out var authHeaders);
        if (!ishead || !authHeaders.Any())
        {
            await context.WriteAsync(config.FailedFormat(false, 401, "未授权"));
            return;
        }
        var accecToken = authHeaders.First();
        try
        {
            var account = config.ReverseAccessToken(accecToken).ToObject<AuthenticationAccount>();
            if (account == null || !account.id.IsNotNullOrEmpty())
            {
                await context.WriteAsync(config.FailedFormat(false, 401, "非法token"));
                return;
            }
            if (DateTimeOffset.Now.Subtract(DateTimeOffset.FromUnixTimeSeconds(account.time)).TotalSeconds > config.ExpiredTime.TotalSeconds)
            {
                await context.WriteAsync(config.FailedFormat(false, 401, "授权超时"));
                return;
            }
            if (descriptor.AuthenticationAttribute is AuthenticationAttribute authention && authention.Roles.Any())
            {
                if (account.roleid.IsNullOrEmpty() || !authention.Roles.Any(t => t.Equals(account.roleid, StringComparison.OrdinalIgnoreCase)))
                {
                    await context.WriteAsync(config.FailedFormat(false, 402, "无权操作"));
                }
            }
                (Service as DefaultAuthenticationService).AuthenteIn(account);
            await next(context);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "授权验证");
            await context.WriteAsync(config.FailedFormat(false, 401, "accecToken 授权失败"));
        }


    }
}

