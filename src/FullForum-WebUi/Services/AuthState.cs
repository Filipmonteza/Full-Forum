namespace FullForum_WebUi.Services.Exceptions;

/// <summary>
/// Holds the current authentication state for the UI.
/// Keeps track of whether the user is logged in and basic user information.
/// Notifies components when the auth state changes.
/// </summary>
public class AuthState
{
    public bool IsAuthenticated { get; private set; } 
    public Guid? UserId { get; private set; }
    public string? Role { get; private set; }
    public string? PreferredName { get; private set; }
    public event Action? OnChanged;
   
    /// <summary>
    /// Signs in the user and updates the authentication state.
    /// Validates that required values are provided.
    /// </summary>
    public void SignIn(Guid userId, string role, string preferredName)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId must be non-empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role must be non-empty", nameof(role));
        if (string.IsNullOrWhiteSpace(preferredName))
            throw new ArgumentException("Name must be non-empty", nameof(preferredName));
        IsAuthenticated = true;
        UserId = userId;
        Role = role;
        PreferredName = preferredName.Trim();
        OnChanged?.Invoke();
    }
    
    /// <summary>
    /// Updates the user's preferred display name.
    /// Notifies the UI after the update.
    /// </summary>
    public void UpdatePreferredName(string preferredName)
    {
        if (string.IsNullOrWhiteSpace(preferredName))
            throw new ArgumentException("Name must be non-empty", nameof(preferredName));
        PreferredName = preferredName.Trim();
        OnChanged?.Invoke();
    }
    
    /// <summary>
    /// Signs out the current user and clears the authentication state.
    /// Triggers a UI update through the OnChanged event.
    /// </summary>
    public void SignOut()
    {
        IsAuthenticated = false;
        UserId = null;
        Role = null;
        PreferredName = null;
        OnChanged?.Invoke();
    }
}