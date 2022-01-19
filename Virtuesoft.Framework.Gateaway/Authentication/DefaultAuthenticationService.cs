namespace Virtuesoft.Framework.Gateaway.Authentication;
/// <summary>
/// 
/// </summary>
public class DefaultAuthenticationService : IGateawayAuthenticationService
{
    /// <summary>
    /// 
    /// </summary>
    ILogger<DefaultAuthenticationService> Logger { get; }
    /// <summary>
    /// 
    /// </summary>
    IOptionsMonitor<GateawayAuthenticationOption> Options { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    /// <param name="logger"></param>
    public DefaultAuthenticationService(IOptionsMonitor<GateawayAuthenticationOption> option, ILogger<DefaultAuthenticationService> logger)
    {
        Options = option; Logger = logger;
    }
    /// <summary>
    /// 
    /// </summary>
    public string Id => Account?.id??"";
    /// <summary>
    /// 
    /// </summary>
    public bool IsAuthented => Account != null;
    /// <summary>
    /// 
    /// </summary>
    public string Name => Account?.name??"";
    /// <summary>
    /// 
    /// </summary>
    public string RoleId => Account?.roleid ?? "";
    /// <summary>
    /// 
    /// </summary>
    public string RoleName => Account?.rolename ?? "";
    /// <summary>
    /// 
    /// </summary>
    public string MachineName => Account?.machinename ?? "";
    /// <summary>
    /// 
    /// </summary>
    public long? Time => Account?.time;

    private AuthenticationAccount Account { get; set; }
    internal void AuthenteIn(AuthenticationAccount _AuthAccount)
    {
        Account = _AuthAccount;
    }
    /// <summary>
    /// 获取一个属性
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Get(string key)
    {
        string value = string.Empty;
        if (key.Equals("name"))
            return Account?.name ?? "";
        if (key.Equals("roleid"))
            return Account?.roleid ?? "";
        if (key.Equals("rolename"))
            return Account?.rolename ?? "";
        if (key.Equals("time"))
            return Account?.time.ToString() ?? "";
        if (key.Equals("machinename"))
            return Account?.machinename ?? "";
        var b = Account?.propertys.TryGetValue(key, out value);
        return value ?? "";
    }
    /// <summary>
    /// 刷新token
    /// </summary>
    /// <param name="accecToken"></param>
    /// <returns></returns>
    public string RefreshToken(string accecToken = null)
    {
        return accecToken;
    }
    /// <summary>
    /// 登录并生成accessToken
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="roleId"></param>
    /// <param name="roleName"></param>
    /// <param name="propertys"></param>
    /// <returns></returns>
    public string SignIn(string id, string name = null, string roleId = null, string roleName = null, IDictionary<string, string> propertys = null)
    {
        var account = new AuthenticationAccount()
        {
            id = id,
            name = name,
            propertys = propertys,
            roleid = roleId,
            rolename = roleName
        };
        var accecToken= Options.CurrentValue.GenerateAccessToken(account.ToJson());
        Options.CurrentValue.OnSignIn?.Invoke(accecToken);
        return accecToken;
    }
    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="accecToken"></param>
    /// <returns></returns>
    public string SignOut(string accecToken = null)
    {
        Options.CurrentValue.OnSignOut?.Invoke(accecToken);
        return accecToken;
    }
}

