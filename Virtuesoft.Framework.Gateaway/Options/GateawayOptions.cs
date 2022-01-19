
    /// <summary>
    /// 配置文件
    /// </summary>
    public class GateawayOptions
    {
        /// <summary>
        /// 不进行参数验证的方法
        /// </summary>
        public string[] NotVerifyForMethods { get; set; } = new string[] { "callback", "turnback" };
        /// <summary>
        /// 系统编码
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="forms"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public virtual bool VerifySign(IDictionary<string, string> forms, HttpContext httpContext)
        {
            return OnVerifySign(forms, httpContext);
        }
        /// <summary>
        /// 验证签名
        /// </summary>
        public Func<IDictionary<string, string>, HttpContext, bool> OnVerifySign = (forms, httpContext) => { return true; };
        /// <summary>
        /// 验证必要参数
        /// </summary>
        public virtual async Task<(bool status, string message)> VerifyPrameters(HttpContext httpContext)
        {
            var froms = await httpContext.GetPrametersAsync();
            var paramters = froms
                .Select(t => new { Key = t.Key, Value = t.Value.ToString() })
                .ToDictionary(t => t.Key, t => t.Value);
            return OnVerifyPrameters(paramters, httpContext);
        }
        /// <summary>
        /// 验证必要参数
        /// </summary>
        public Func<IDictionary<string, string>, HttpContext, (bool status, string message)> OnVerifyPrameters = (forms, httpContext) => { return (true, string.Empty); };
        /// <summary>
        /// 格式化返回值
        /// </summary>
        public virtual string FormatResult(bool s, int c, string m, object d)
        {
            return OnFormatResult(s, c, m, d);
        }

        /// <summary>
        /// 格式化返回值
        /// </summary>
        public Func<bool, int, string, object, string> OnFormatResult = (s, c, m, d) => { return new { s = s, c = c, m = m, d = d }.ToJson(); };
        /// <summary>
        /// 访问开始
        /// </summary>
        public virtual Task BeginRequest(HttpContext httpContext)
        {
            return OnBeginRequest(httpContext);
        }
        /// <summary>
        /// 访问开始
        /// </summary>
        public Func<HttpContext, Task> OnBeginRequest = (httpContext) => { return Task.CompletedTask; };
        /// <summary>
        /// 访问结束
        /// </summary>
        public virtual Task EndReqeust(HttpContext httpContext)
        {
            return OnEndReqeust(httpContext);
        }
        /// <summary>
        /// 访问结束
        /// </summary>
        public Func<HttpContext, Task> OnEndReqeust = (httpContext) => { return Task.CompletedTask; };
        /// <summary>
        /// 验证ip地址是否允许访问接口
        /// </summary>
        public virtual bool VerifyIP(HttpContext httpContext, string ip)
        {
            return OnVerifyIP(httpContext, ip);
        }
        /// <summary>
        /// 验证ip地址是否允许访问接口
        /// </summary>
        public Func<HttpContext, string, bool> OnVerifyIP = (httpContext, ip) => { return true; };
    }

