using FullForum_WebUiWebUi.Services;
using FullForum_WebUiWebUi.Services.UI;

namespace FullForum_WebUi.Services.UI;

/// <summary>
/// Shared UI status service.
/// Tracks loading state, offline status, and current UI errors.
/// </summary>
public class UiStatus : IUiStatus
{
    public bool IsBusy { get; private set; }
    public bool IsOffline { get; private set; }
    public UiError? Error { get; private set; }
    public event Action? Changed;
    
    /// <summary>
    /// Sets the busy/loading state of the UI.
    /// </summary>
    public void Busy(bool on)
    {
        IsBusy = on;
        Changed?.Invoke();
    }

    /// <summary>
    /// Sets whether the application is offline.
    /// </summary>
    public void Offline(bool on)
    {
        IsOffline = on;
        Changed?.Invoke();
    }

    /// <summary>
    /// Sets or clears the current UI error.
    /// </summary>
    public void SetError(UiError? error)
    {
        Error = error;
        Changed?.Invoke();
    }

    /// <summary>
    /// Manually notifies listeners that the UI state has changed.
    /// </summary>
    public void Notify() => Changed?.Invoke();
    
}