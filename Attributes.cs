#if NETSTANDARD
namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
public sealed class OverloadResolutionPriorityAttribute(int priority) : Attribute;
#endif
