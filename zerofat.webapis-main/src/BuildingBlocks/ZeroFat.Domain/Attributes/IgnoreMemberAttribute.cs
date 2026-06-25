using System.Reflection;

namespace ZeroFat.Domain.Attributes;

/// <summary>
/// Ignore Field Or Property In Value Object In Equal Operator or HashCode
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class IgnoreMemberAttribute : Attribute
{

}
