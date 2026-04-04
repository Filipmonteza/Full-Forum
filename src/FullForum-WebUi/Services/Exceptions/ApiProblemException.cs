namespace FullForum_WebUi.Services.Exceptions;

/// <summary>
/// Exception used for API errors with optional HTTP status code.
/// </summary>
public class ApiProblemException : Exception
{
    public int Status { get; set; }
    public ApiProblemException(string? message) : base(message) { }
}