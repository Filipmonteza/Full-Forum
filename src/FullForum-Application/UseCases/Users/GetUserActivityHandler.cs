using FullForum_Application.DTOs;
using FullForum_Application.Interfaces;

namespace FullForum_Application.UseCases.Users;

/// <summary>
/// Handles the use case for retrieving a user's forum activity,
/// including created threads and posted comments.
/// </summary>
public sealed class GetUserActivityHandler
{
    private readonly IThreadRepository _threadRepository;
    private readonly ICommentRepository _commentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserActivityHandler"/> class.
    /// </summary>
    /// <param name="threadRepository">Provides access to thread data.</param>
    /// <param name="commentRepository">Provides access to comment data.</param>
    public GetUserActivityHandler(
        IThreadRepository threadRepository,
        ICommentRepository commentRepository)
    {
        _threadRepository = threadRepository;
        _commentRepository = commentRepository;
    }

    /// <summary>
    /// Retrieves the activity for a specific user, including threads and comments.
    /// </summary>
    /// <param name="command">The request containing the user ID.</param>
    /// <param name="ct">A cancellation token for the asynchronous operation.</param>
    /// <returns>A <see cref="UserActivityDto"/> containing the user's threads and comments.</returns>
    public async Task<UserActivityDto> HandleAsync(GetUserActivityCommand command, CancellationToken ct = default)
    {
        var threads = await _threadRepository.GetByUserIdAsync(command.UserId, ct);
        var comments = await _commentRepository.GetByUserIdAsync(command.UserId, ct);

        var threadDtos = threads
            .Select(t => new UserThreadDto(
                t.Id,
                t.ThreadTitle,
                t.CategoryId,
                t.CreatedAt,
                t.UpdatedAt,
                t.Comments.Count(c => !c.IsDeleted)
            ))
            .ToList();

        var commentDtos = comments
            .Select(c => new UserCommentDto(
                c.Id,
                c.CommentContent,
                c.Thread.ThreadTitle,
                c.ThreadId,
                c.CreatedAt,
                c.UpdatedAt,
                c.IsDeleted
            ))
            .ToList();

        return new UserActivityDto(threadDtos, commentDtos);
    }
}