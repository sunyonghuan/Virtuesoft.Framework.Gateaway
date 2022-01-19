namespace Virtuesoft.Framework.Gateaway.Authentication;
internal class AuthenticationAccount
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public string id { get; set; }
    /// <summary>
    /// 用户名称
    /// </summary>
    public string name { get; set; }
    /// <summary>
    /// 角色id
    /// </summary>
    public string roleid { get; set; }
    /// <summary>
    /// 角色名称
    /// </summary>
    public string rolename { get; set; }
    /// <summary>
    /// 颁发时间
    /// yyyyMMddHHmm
    /// </summary>
    public long time { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();
    /// <summary>
    /// 自定义属性
    /// </summary>
    public IDictionary<string, string> propertys { get; set; } = new Dictionary<string, string>();
    /// <summary>
    /// 生成随机字符串
    /// </summary>
    public string sigleton { get; set; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    /// <summary>
    /// 颁发令牌的机器名称
    /// </summary>
    public string machinename { get; set; } = Environment.MachineName;
}

