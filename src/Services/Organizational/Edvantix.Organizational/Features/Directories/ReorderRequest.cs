namespace Edvantix.Organizational.Features.Directories;

/// <summary>Тело запроса на изменение порядка записей справочника.</summary>
/// <param name="OrderedIds">Полный список активных идентификаторов в желаемом порядке (0..n-1).</param>
public sealed record ReorderRequest(IReadOnlyList<Guid> OrderedIds);
