namespace FullForum_WebUi.Services.UI;

/// <summary>
/// Represents a UI error with a title and description.
/// Used for displaying user-friendly error messages.
/// </summary>
public record UiError(string Title, string Detail);

/// <summary>
/// Contract for managing UI status such as loading state,
/// offline mode, and current UI errors.
/// </summary>
public interface IUiStatus
{ 
    bool IsBusy { get; }
    bool IsOffline { get; }
    UiError? Error { get; }
    event Action? Changed;
    void Busy(bool on);
    void Offline(bool on);
    void SetError(UiError? error);
    void Notify();
}