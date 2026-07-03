using POSApplication.Core.Entities;

namespace POSApplication.Core.Interfaces;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
    void SetCurrentUser(User user);
    void ClearCurrentUser();
}
