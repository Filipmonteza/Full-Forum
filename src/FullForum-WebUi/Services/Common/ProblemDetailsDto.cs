namespace FullForum_WebUi.Services.Common;

/// <summary>
/// Represents a simplified ProblemDetails response from the API.
/// </summary>
public class ProblemDetailsDto
{ 
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public int? Status { get; set; }
} 