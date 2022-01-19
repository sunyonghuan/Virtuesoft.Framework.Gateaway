
namespace Virtuesoft.Framework.Gateaway.Authentication;
/// <summary>
/// 
/// </summary>
public class GateawayAuthenticationOption
{
    /// <summary>
    /// 
    /// </summary>
    public GateawayAuthenticationOption()
    {
        GenerateAccessToken = (source) => source.AESEncrypt(SecurityKey.MD5Encrypt());
        ReverseAccessToken = (source) => source.AESDecrypt(SecurityKey.MD5Encrypt());
        OnSignOut = (accecToken) => { };
        OnSignIn = (accecToken) => { };
    }
    /// <summary>
    /// 排除的路径
    /// 支持 path,method
    /// method优先制
    /// </summary>
    public IEnumerable<string> Exclude { get; set; } = new string[] { };

    /// <summary>
    /// 用于加密的密码,不修改使用默认
    /// </summary>
    public string SecurityKey { get; set; } = "Virtuesoft.Authentication.Security.Aeskey";
    /// <summary>
    /// 过期时间
    /// 默认2天
    /// </summary>
    public TimeSpan ExpiredTime { get; set; } = TimeSpan.FromDays(2);
    /// <summary>
    /// 需要传递或者获取的header名称
    /// </summary>
    public string HeaderName { get; set; } = "Security.AccecToken";
    /// <summary>
    /// 返回格式
    /// </summary>
    public Func<bool, int, string, object> FailedFormat { get; set; } = (s, c, m) => new
    {
        s,
        c,
        m
    };
    /// <summary>
    /// accessToken生成
    /// 输入明文
    /// 返回accesstoken
    /// </summary>
    public Func<string, string> GenerateAccessToken { get; set; }
    /// <summary>
    /// 反向解析accessToken 为明文
    /// </summary>
    public Func<string, string> ReverseAccessToken { get; set; }
    /// <summary>
    /// 退出登录事件
    /// </summary>
    public Action<string> OnSignOut { get; set; }

    /// <summary>
    /// 退出登录事件
    /// </summary>
    public Action<string> OnSignIn { get; set; }
}

