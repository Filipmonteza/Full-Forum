using FullForum_Application.Interfaces;
using FullForum_Domain.Entities;

namespace FullForum_Application.UseCases.Comments.CreateComment;

    /// <summary>
    /// Handles CreateCommentCommand and return CreateCommentResult
    /// </summary>
    public sealed class CreateCommentHandler
    {
        private readonly ICommentRepository _repo;
        public CreateCommentHandler(ICommentRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Validates business rules and persist new comment to repository
        /// </summary>
        public async Task<CreateCommentResult> HandleAsync(
            CreateCommentCommand cmd,
            CancellationToken ct = default)
        {            
            // Business rules
            if (!await _repo.ThreadExistsAsync(cmd.ThreadId, ct))
                return CreateCommentResult.Fail($"ThreadId '{cmd.ThreadId}' does not exist.");

            if (!await _repo.UserExistsAsync(cmd.ApplicationUserId, ct))
                return CreateCommentResult.Fail($"ApplicationUserId '{cmd.ApplicationUserId}' does not exist.");

            // ParentComment must exist and belong to the same Thread
            if (cmd.ParentCommentId is not null)
            {
                var parent = await _repo.GetParentCommentAsync(cmd.ParentCommentId.Value, ct);

                if (parent is null)
                    return CreateCommentResult.Fail($"ParentCommentId '{cmd.ParentCommentId}' does not exist.");

                if (parent.ThreadId != cmd.ThreadId)
                    return CreateCommentResult.Fail("ParentCommentId must belong to the same ThreadId.");
            }

            // Create Domain entity
            var comment = new Comment
            {
                CommentContent = cmd.CommentContent.Trim(),
                ThreadId = cmd.ThreadId,
                ApplicationUserId = cmd.ApplicationUserId,
                ParentCommentId = cmd.ParentCommentId
            };

            try
            {
                comment.Validate();
            }
            catch (ArgumentException exception)
            {
                return CreateCommentResult.Fail(exception.Message);
            }

            // Persist via repository
            await _repo.AddAsync(comment, ct);

            // Return created comment
            return CreateCommentResult.Ok(comment);
        }
    }