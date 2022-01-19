namespace Virtuesoft.Framework.Gateaway.Attributes;
/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class AllowAnyoneAttribute : Attribute, IAuthentication
{

}

