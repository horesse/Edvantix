namespace Edvantix.Constants.Other;

public enum CrudAction
{
    // Commands
    Create,
    CreateRange,
    Delete,
    DeleteRange,
    Update,
    UpdateRange,
    Validate,

    // Queries
    GetAll,
    GetByExpression,
    GetById,
    GetCount,
    IsExist,
    FetchPagedData,
}

/// <summary>
/// Flags для выборочной регистрации CRUD операций
/// </summary>
[Flags]
public enum CrudActions
{
    None = 0,

    // Commands
    Create = 1 << 0, // 1
    Update = 1 << 1, // 2
    Delete = 1 << 2, // 4
    DeleteRange = 1 << 3, // 8

    // Queries
    GetById = 1 << 4, // 16
    GetByExpression = 1 << 5, // 32
    GetCount = 1 << 6, // 64
    IsExist = 1 << 7, // 128
    FetchPagedData = 1 << 8, // 256

    // Группы операций
    AllCommands = Create | Update | Delete | DeleteRange,
    AllQueries = GetById | GetByExpression | GetCount | IsExist | FetchPagedData,
    All = AllCommands | AllQueries,

    // Часто используемые комбинации
    ReadOnly = AllQueries,
    WriteOnly = AllCommands,
    Basic = Create | Update | Delete | GetById | FetchPagedData,
}
