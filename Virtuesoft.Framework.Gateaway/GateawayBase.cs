
/// <summary>
/// 接口基类
/// </summary>
public abstract class GateawayBase : IDisposable
{
    /// <summary>
    /// 当前上下文
    /// </summary>
    public HttpContext Context { get; set; }
    /// <summary>
    /// 当前接口名称
    /// </summary>
    public virtual string Name { get { return this.GetType().Name; } }
    /// <summary>
    /// 返回对象
    /// </summary>
    /// <param name="s">执行状态</param>
    /// <param name="c">执行代码</param>
    /// <param name="m">执行消息</param>
    /// <param name="d">数据</param>
    /// <param name="isFormat">是否格式化</param>
    /// <param name="isTranslate">是否翻译</param>
    /// <returns></returns>
    protected object Result(bool s, int c, string m, object d, bool isFormat = true, bool isTranslate = true)
    {
        Context.Items["status"] = s;
        Context.Items["code"] = c;
        Context.Items["message"] = m;
        Context.Items["format"] = isFormat;
        Context.Items["translate"] = isTranslate;
        return d;
    }
    /// <summary>
    /// 执行成功
    /// </summary>
    /// <param name="d">数据</param>
    /// <param name="m">消息</param>
    /// <param name="isFormat">是否格式化</param>
    /// <param name="isTranslate">是否翻译</param>
    /// <returns></returns>
    protected object Success(object d = null, string m = "ok", bool isFormat = true, bool isTranslate = true)
        => Result(true, 200, m, d, isFormat, isTranslate);
    /// <summary>
    /// 执行错误
    /// </summary>
    /// <param name="m">消息</param>
    /// <param name="c">代码</param>
    /// <param name="isFormat">是否格式化</param>
    /// <param name="isTranslate">是否翻译</param>
    /// <returns></returns>
    protected object Error(string m = "error", int c = 500, bool isFormat = true, bool isTranslate = true)
       => Result(false, c, m, new object(), isFormat, isTranslate);

    /// <summary>
    /// 当前登录的用户
    /// </summary>
    public IGateawayAuthenticationService User { get { return Context.Authentication(); } }

    /// <summary>
    /// 当前登录的用户
    /// </summary>
    [Obsolete("请使用 User ")] 
    public IGateawayAuthenticationService Auth => User;
    /// <summary>
    /// 是否经过验证
    /// </summary>
    public bool IsAuthenticated { get { return Context.User.Identity?.IsAuthenticated??false; } }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Dispose(bool disposing)
    {
        //GC.Collect();
    }
    /// <summary>
    /// 
    /// </summary>
    public void Dispose() => Dispose(true);
}

