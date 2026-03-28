namespace FullForum_WebApi.Contracts.Threads.ThreadResponse;

// Api Response wrapping pginated list of threads, incuding total count
public record PagedThreadsResponse(
    List<ThreadListItemResponse> Items,
    int TotalCount
    );

    
