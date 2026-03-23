namespace FullForum_Application.UseCases.Users;

/// <summary>
/// Command used to request activity data for a specific user.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
public sealed record GetUserActivityCommand(Guid UserId);

    
