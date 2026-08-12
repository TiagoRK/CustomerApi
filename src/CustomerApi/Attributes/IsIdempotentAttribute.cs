namespace CustomerApi.Web.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IsIdempotentAttribute(string headerName = IsIdempotentAttribute.DefaultHeaderName, int expirationInMinutes = 30) : Attribute
{
  public const string DefaultHeaderName = "Idempotency-Key";

  public string HeaderName { get; } = headerName;
  public int ExpirationInMinutes { get; } = expirationInMinutes;
}
