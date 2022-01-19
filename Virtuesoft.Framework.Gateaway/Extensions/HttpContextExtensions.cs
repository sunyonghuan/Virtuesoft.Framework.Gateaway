namespace Virtuesoft.Framework.Gateaway.Extensions;
/// <summary>
/// HttpContext 扩展
/// </summary>
public static class HttpContextExtensions
{
    /// <summary>
    /// 获取当前的认证服务
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static IGateawayAuthenticationService Authentication(this HttpContext httpContext)
        => httpContext.RequestServices<IGateawayAuthenticationService>();
    /// <summary>
    /// 获取一个服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static T RequestServices<T>(this HttpContext httpContext)
         => httpContext.RequestServices.GetService<T>();
    /// <summary>
    /// 获取用户IP地址
    /// 首先获取 X-Forwarded-For 
    /// 无数据再返回 RemoteIPAddress
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static string UserIpAddress(this HttpContext httpContext)
        => httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? httpContext.RemoteIPAddress().ToString();
    /// <summary>
    /// 远程连接的IP地址
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static IPAddress RemoteIPAddress(this HttpContext httpContext)
        => httpContext.Connection.RemoteIpAddress;
    /// <summary>
    /// 写入输出数据
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="message">输出消息</param>
    /// <param name="contentType">数据 Content-Type</param>
    /// <param name="encoding">数据编码</param>
    /// <returns></returns>
    public static Task WriteAsync(this HttpContext httpContext, string message, string contentType = "application/json; charset=utf-8", Encoding encoding = null)
    {
        if (message.IsNullOrEmpty()) throw new ArgumentNullException($"message is null");
        var response = httpContext.Response;
        response.Headers.TryAdd("Content-Type", contentType);
        //var buffer = (encoding ?? Encoding.UTF8).GetBytes(message);
        return response.WriteAsync(message, (encoding ?? Encoding.UTF8), httpContext.RequestAborted);
        //.Body.WriteAsync(buffer, 0, buffer.Length);
    }
    /// <summary>
    /// 写入输出数据
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="message">输出消息</param>
    /// <param name="contentType">数据 Content-Type</param>
    /// <param name="encoding">数据编码</param>
    /// <returns></returns>
    public static Task WriteAsync<T>(this HttpContext httpContext, T message, string contentType = "application/json; charset=utf-8", Encoding encoding = null) => httpContext.WriteAsync(message.ToJson(), contentType, encoding);
    /// <summary>
    /// 获取当前的 ContentType
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static string GetContentType(this HttpContext httpContext)
    {
        var conctentType = httpContext.Request.ContentType;
        if (conctentType.IsNotNullOrEmpty() && conctentType.Contains(";"))
        {
            try
            {
                var contentTypes = httpContext.Request.ContentType
                .Split(';')
                .Select(t => t.Trim().ToLower())
                .ToArray();
                if (contentTypes.Any())
                    conctentType = contentTypes[0];
            }
            catch (Exception ex)
            {
                httpContext.RequestServices<ILogger<HttpContext>>().LogError(ex, $"GetContentType:{ex.Message}");
            }

        }
        return conctentType;
    }
    /// <summary>
    /// 获取当前的 Encoding
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static Encoding GetEncoding(this HttpContext httpContext)
    {
        Encoding encoding = Encoding.UTF8;
        try
        {
            var contentTypes = httpContext.Request.ContentType
                .Split(';')
                .Select(t => t.Trim().ToLower())
                .ToArray();
            if (contentTypes.Any(t => t.StartsWith("charset")))
                encoding = Encoding.GetEncoding(contentTypes.First(t => t.StartsWith("charset")).Split('=')[1]);
        }
        catch (Exception ex)
        {
            httpContext.RequestServices<ILogger<HttpContext>>().LogError(ex, $"GetEncoding:{ex.Message}");
        }
        return encoding;
    }
    /// <summary>
    /// 获取所有参数
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static async Task<IDictionary<string, object>> GetPrametersAsync(this HttpContext context)
    {
        if (context.Items.TryGetValue("forms", out var forms)) return forms as IDictionary<string, object>;
        var pt = context.RequestServices.GetRequiredService<PrameterProviderCollection>();
        Type providerType;
        if (string.Equals(context.Request.Method, "get", StringComparison.CurrentCultureIgnoreCase))
        {
            providerType = typeof(QueryStringPrameterProvider);
        }
        else
        {
            var providers = pt.Where(t => t.Key == context.GetContentType());
            if (!providers.Any())
            {
                context.RequestServices<ILogger<HttpContext>>().LogError(new KeyNotFoundException($"找不到[{context.GetContentType()}]类型的参数解析驱动"), "HttpContext.GetPrametersAsync");
                context.Items.TryAdd("forms", new Dictionary<string, object>());
                return (IDictionary<string, object>)context.Items["forms"];
            }
            providerType = providers.First().Value;
        }
        var result = await (context.RequestServices.GetRequiredService(providerType) as IPrameterProvider).Get();
        context.Items.TryAdd("forms", result);
        return result;
    }
    /// <summary>
    /// 创建参数对象
    /// </summary>
    /// <param name="context"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static async Task<object[]> CreatePrameters(this HttpContext context, ParameterInfo[] parameters)
    {

        try
        {
            var froms = await context.GetPrametersAsync();
            var args = new List<object>();
            foreach (var parameter in parameters.OrderBy(t => t.Position))
            {
                var o = context.CreatePrameters(parameter, froms);
                args.Add(o);
            }
            return args.ToArray();
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"创建参数失败:{ex.Message}", ex);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="parameter"></param>
    /// <param name="froms"></param>
    /// <returns></returns>
    static object CreatePrameters(this HttpContext context, ParameterInfo parameter, IDictionary<string, object> froms)
    {
        object result = parameter.DefaultValue;
        object pob = parameter.DefaultValue;
        if (froms.Where(t => t.Key.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase)).Any())
            pob = froms.Where(t => t.Key.Equals(parameter.Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Value;
        else
            pob = froms;
        try
        {
            if (parameter.ParameterType.IsPrimitive || parameter.ParameterType.IsValueType || (parameter.ParameterType.HasElementType && parameter.ParameterType.GetElementType().IsPrimitive))
                return context.RequestServices<PrimitivePrameterConvertor>()
                    .Convert(parameter.ParameterType, pob, parameter.DefaultValue);
            if (parameter.ParameterType.IsEnum)
                return context.RequestServices<EnumPrameterConvertor>()
                    .Convert(parameter.ParameterType, pob, parameter.DefaultValue);
            if (parameter.ParameterType.IsClass || parameter.ParameterType.IsInterface || parameter.ParameterType.IsAbstract)
                return context.RequestServices<CustomTypePrameterConvertor>()
                     .Convert(parameter.ParameterType, pob, parameter.DefaultValue);
        }
        catch (Exception ex)
        {
            context.RequestServices<ILogger<IPrameterConvertor>>().LogError(ex.Message, $"CreatePrameters.Private:{parameter.Name}");
        }

        return result;
    }

    /// <summary>
    /// 获取当前的登录信息
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public static IGateawayAuthenticationService Auth(this HttpContext httpContext)
    {
        return httpContext.RequestServices<IGateawayAuthenticationService>();
    }
}

