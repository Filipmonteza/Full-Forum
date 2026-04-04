namespace FullForum;

// DTO for updating a comment's content
public sealed record CommentUpdateDto(
    string CommentContent
);