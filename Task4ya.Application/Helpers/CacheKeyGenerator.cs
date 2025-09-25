namespace Task4ya.Application.Helpers;

public static class CacheKeyGenerator
{
    internal const string BoardsPrefix = "boards";
    internal const string TaskitemsPrefix = "taskitems";
    internal const string UsersPrefix = "users";

    public static string GetBoardsKey(int page, int pageSize, string? sortBy = null, bool sortDescending = false, string? searchTerm = null)
        => $"{BoardsPrefix}:page:{page}:size:{pageSize}:sort:{sortBy}:{sortDescending}:search:{searchTerm}";

    public static string GetBoardByIdKey(int id)
        => $"{BoardsPrefix}:id:{id}";

    public static string GetTaskItemsKey(int page, int pageSize, string? sortBy = null, string? sortOrder = null)
        => $"{TaskitemsPrefix}:page:{page}:size:{pageSize}:sort:{sortBy}:{sortOrder}";

    public static string GetTaskItemByIdKey(int id)
        => $"{TaskitemsPrefix}:id:{id}";

    public static string GetTaskItemsByBoardKey(int boardId, int page, int pageSize)
        => $"{TaskitemsPrefix}:board:{boardId}:page:{page}:size:{pageSize}";

    public static string GetUsersKey(int page, int pageSize)
        => $"{UsersPrefix}:page:{page}:size:{pageSize}";

    public static string GetUserByIdKey(int id)
        => $"{UsersPrefix}:id:{id}";
}
