namespace FullForum_WebUi.Services;

/// <summary>
/// Contract for storing and retrieving the authentication token.
/// Used by services that need access to the current JWT token.
/// </summary>
public interface ITokenStore
{ 
    string? AccessToken { get; }
    void SetToken(string? token);
    event Action? Changed;
}