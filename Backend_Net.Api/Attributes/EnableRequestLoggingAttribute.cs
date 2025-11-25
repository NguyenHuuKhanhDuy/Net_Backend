namespace Backend_Net.Api.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class EnableRequestLoggingAttribute : Attribute
{
}