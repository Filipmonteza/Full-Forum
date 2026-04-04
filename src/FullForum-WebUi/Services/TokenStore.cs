namespace FullForum_WebUi.Services;

/// <summary>
/// Simple in-memory implementation of ITokenStore.
/// Stores the JWT access token during the application session.
/// </summary>
public sealed class TokenStore:ITokenStore
{
    private string? _token;
    public string? AccessToken => _token; 
    public event Action? Changed;
    
    /// <summary>
    /// Sets or clears the stored token value.
    /// Passing null removes the token.
    /// </summary>
    public void SetToken(string? token)
    {
        _token = token;
        Changed?.Invoke();
    }
}