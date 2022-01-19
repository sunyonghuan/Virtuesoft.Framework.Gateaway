namespace Virtuesoft.Framework.Gateaway.Authentication;

/// <summary>
/// 授权认证服务
/// </summary>
public interface IGateawayAuthenticationService
{
    /// <summary>
    /// 登录生成accecToken
    /// </summary>
    /// <param name="id">用户ID</param>
    /// <param name="name">用户名称</param>
    /// <param name="roleId">角色ID</param>
    /// <param name="roleName">角色名称</param>
    /// <param name="propertys">其他属性</param>
    /// <returns>accecToken</returns>
    string SignIn(string id, string name = null, string roleId = null, string roleName = null, IDictionary<string, string> propertys = null);
    /// <summary>
    /// 登出
    /// </summary>
    /// <param name="accecToken">登录颁发的 accecToken</param>
    /// <returns></returns>
    string SignOut(string accecToken = null);
    /// <summary>
    /// 刷新当前登录 token
    /// </summary>
    /// <param name="accecToken"></param>
    /// <returns></returns>
    string RefreshToken(string accecToken = null);

    /// <summary>
    /// 获取用户id
    /// </summary>
    /// <returns></returns>
    string Id { get; }
    /// <summary>
    /// 获取其他属性
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    string Get(string key);
    /// <summary>
    /// 是否认证
    /// </summary>
    bool IsAuthented { get; }

    /// <summary>
    /// 当前名称
    /// </summary>
    string Name { get; }
    /// <summary>
    /// 角色ID
    /// </summary>
    string RoleId { get; }
    /// <summary>
    /// 角色名称
    /// </summary>
    string RoleName { get; }
    /// <summary>
    /// 当前机器名称
    /// </summary>
    string MachineName { get; }
    /// <summary>
    /// 登录时间
    /// </summary>
    long? Time { get; }
}

