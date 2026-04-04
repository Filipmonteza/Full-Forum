namespace FullForum_WebUi.Models.Threads;

public record PagedThreadsModel
(
    List<ThreadListItemModel> Items,
    int TotalCount
);