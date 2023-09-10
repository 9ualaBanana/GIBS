using Microsoft.AspNetCore.Authorization;

namespace GIBS;

/// <summary>
/// Allows services to provide class-specific <see cref="Microsoft.AspNetCore.Authorization.AuthorizationPolicy"/>
/// that must be met during imperative authorization.
/// </summary>
/// <remarks>
/// Imperative authorization is performed by explicitly using and calling the corresponding methods of <see cref="IAuthorizationService"/>.
/// Declarative authorization. on the other hand, is performed using <see cref="AuthorizeAttribute"/>.
/// </remarks>
public interface IAuthorizationPolicyProtected
{
    AuthorizationPolicy AuthorizationPolicy { get; }
}
