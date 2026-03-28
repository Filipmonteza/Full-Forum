using System;
using System.Threading;
using System.Threading.Tasks;
using FullForum_Application.Interfaces;
using FullForum_Application.UseCases.Comments.CreateComment;
using FullForum_Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FullForumTest;

[TestClass]
public class CreateCommentHandlerTests
{
    private Mock<ICommentRepository> _repoMock = null!;
    private CreateCommentHandler _handler = null!;

    [TestInitialize]
    public void Setup()
    {
        _repoMock = new Mock<ICommentRepository>();
        _handler = new CreateCommentHandler(_repoMock.Object);
    }

    [TestMethod]
    public async Task HandleAsync_ShouldFail_WhenThreadDoesNotExist()
    {
        // Arrange
        var threadId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var cmd = new CreateCommentCommand(
            "Hello",
            threadId,
            userId,
            null);

        _repoMock
            .Setup(r => r.ThreadExistsAsync(threadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(cmd);

        // Assert
        Assert.IsFalse(result.Success);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task HandleAsync_ShouldCreateComment_WhenValid()
    {
        // Arrange
        var threadId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var cmd = new CreateCommentCommand(
            "  Hello world  ",
            threadId,
            userId,
            null);

        _repoMock
            .Setup(r => r.ThreadExistsAsync(threadId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repoMock
            .Setup(r => r.UserExistsAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.HandleAsync(cmd);

        // Assert
        Assert.IsTrue(result.Success);

        _repoMock.Verify(
            r => r.AddAsync(
                It.Is<Comment>(c =>
                    c.ThreadId == threadId &&
                    c.ApplicationUserId == userId &&
                    c.CommentContent == "Hello world" &&
                    c.ParentCommentId == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}