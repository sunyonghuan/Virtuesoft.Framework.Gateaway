namespace Virtuesoft.Framework.Gateaway.Attributes;
/// <summary>
/// 授权认证
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AuthenticationAttribute : Attribute, IAuthentication
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public string[] Roles { get; set; }
}

