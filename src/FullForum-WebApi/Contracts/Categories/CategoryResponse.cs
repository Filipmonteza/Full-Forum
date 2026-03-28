namespace FullForum_WebApi.Contracts.Categories;


/// Api response representing a category
public sealed record CategoryResponse(
    Guid Id,
    string CategoryName,
    string CategoryDescription
    );

    
