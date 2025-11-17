namespace Domain;

public record PagedResult<T>(ICollection<T> Items, int? NextPointer, bool HasNextPage);
