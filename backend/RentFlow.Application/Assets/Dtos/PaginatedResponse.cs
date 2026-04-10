namespace RentFlow.Application.Assets.Dtos;

public sealed record PaginatedResponse<T>(
    IReadOnlyList<T> Data,
    int Total,
    int Page,
    int Limit,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
);