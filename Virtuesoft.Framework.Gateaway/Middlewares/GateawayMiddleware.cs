
    /// <summary>
    /// 
    /// </summary>
    public class GateawayMiddleware : IMiddleware
    {
        /// <summary>
        /// 
        /// </summary>
        protected GateawayDescriptorCollection Gateaways { get; }
        /// <summary>
        /// 
        /// </summary>
        ILogger<GateawayMiddleware> Logger { get; }
        /// <summary>
        /// 
        /// </summary>
        IOptionsMonitor<GateawayOptions> Options { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gateaways"></param>
        /// <param name="logger"></param>
        /// <param name="optionsMonitor"></param>
        public GateawayMiddleware(GateawayDescriptorCollection gateaways, ILogger<GateawayMiddleware> logger, IOptionsMonitor<GateawayOptions> optionsMonitor)
        {
            Gateaways = gateaways;
            Logger = logger;
            Options = optionsMonitor;
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
                descriptor = Gateaways.Where(t => t.Path.Equals(method.ToString(), StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (descriptor == null)
                descriptor = Gateaways.Where(t => queryPath.EndsWith(t.Path, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (descriptor == null)
                descriptor = Gateaways.Where(t => t.Path.Equals("api.doc", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (descriptor == null)
                throw new ArgumentOutOfRangeException($"descriptor not maping rote path:{queryPath},method:{method}");
            return descriptor;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
        {


            var config = Options.CurrentValue;
            if (!config.VerifyIP(httpContext, httpContext.UserIpAddress()))
            {
                var result = config.FormatResult(false, 505, $"IP地址不允许访问:{httpContext.UserIpAddress()}", null);
                await httpContext.WriteAsync(result);
                return;
            }
            var forms = await httpContext.GetPrametersAsync();
            var descriptor = GetDescriptor(httpContext, forms);
            if (descriptor == null)
            {
                var result = config.FormatResult(false, 404, $"接口不存在", null);
                await httpContext.WriteAsync(result);
                return;
            }
            var parametersStatus = await config.VerifyPrameters(httpContext);
            if (!parametersStatus.status)
            {
                var result = config.FormatResult(false, 506, parametersStatus.message, null);
                await httpContext.WriteAsync(result);
                return;
            }

            try
            {
                var invokeResult = await InvokeAsync(httpContext, descriptor);
                object output = invokeResult;
                if (!httpContext.Items.TryGetValue("format", out var format) || format is true)
                {
                    output = config.FormatResult((bool)httpContext.Items["status"], (int)httpContext.Items["code"], httpContext.Items["message"] as string, invokeResult);
                }
                if (output is string outString)
                    await httpContext.WriteAsync(outString);
                else
                    await httpContext.WriteAsync(output);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GateawayMiddleware.InvokeAsync:{ex.Message}");
                var result = config.FormatResult(false, 500, "gateaway invoke error!", null);
                await httpContext.WriteAsync(result);
            }


        }
        /// <summary>
        /// 执行接口的方法
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        protected async Task<object> InvokeAsync(HttpContext httpContext, GateawayDescriptor descriptor)
        {
            try
            {
                httpContext.Items.TryAdd("status", true);
                httpContext.Items.TryAdd("message", string.Empty);
                httpContext.Items.TryAdd("code", 200);
                httpContext.Items.TryAdd("format", true);
                httpContext.Items.TryAdd("translate", true);

                var gateaway = httpContext.RequestServices.GetService(descriptor.Gateaway) as GateawayBase;
                gateaway.Context = httpContext;

                var prameters = await httpContext.CreatePrameters(descriptor.Parameters);

                var result = descriptor.Method.Invoke(gateaway, prameters);

                if (result != null && result is Task)
                {
                    await (result as Task);
                    result = (result as dynamic).Result;
                    return result;
                }
                if (result == null)
                    return "ok";
                if (result is string re)
                    return re;

                if (result.GetType().IsPrimitive)
                    return result.ToString();
                return result;
            }
            catch (ArgumentException ex)
            {
                Logger.LogError(ex, $"GateawayMiddleware.InvokeAsync.ArgumentException:{descriptor.Path},{ex.Message}");
                return "参数错误";
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"GateawayMiddleware.InvokeAsync.Exception:{descriptor.Path},{ex.Message}");
                return "接口执行错误";
            }


        }
    }

