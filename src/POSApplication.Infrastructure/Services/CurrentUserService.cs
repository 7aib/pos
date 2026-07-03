using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;

namespace POSApplication.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private User? _currentUser;

    public int? UserId => _currentUser?.UserID;
    public string? Username => _currentUser?.Username;
    public string? Role => _currentUser?.Role.ToString();
    public bool IsAuthenticated => _currentUser != null;

    public void SetCurrentUser(User user)
    {
        _currentUser = user;
    }

    public void ClearCurrentUser()
    {
        _currentUser = null;
    }
}
