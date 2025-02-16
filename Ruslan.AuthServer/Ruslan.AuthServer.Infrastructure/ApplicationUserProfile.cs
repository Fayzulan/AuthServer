using Ruslan.AuthServer.Domain.Base;

namespace Ruslan.AuthServer.Infrastructure;

/// <summary>
/// Represent person with login information (ApplicationUser)
/// </summary>
public class ApplicationUserProfile : Auditable
{
    /// <summary>
    /// Account
    /// </summary>
    public virtual ApplicationUser? ApplicationUser { get; set; }

    /// <summary>
    /// Application permission for policy-based authorization
    /// </summary>
    public List<AppPermission>? Permissions { get; set; }
}