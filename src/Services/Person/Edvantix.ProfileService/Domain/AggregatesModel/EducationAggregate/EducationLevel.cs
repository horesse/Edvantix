using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;

public sealed class EducationLevel() : Entity<long>, ISoftDelete
{
    // EF Core требует конструктор без параметров

    internal EducationLevel(string name, string code)
        : this()
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Название уровня образования не может быть пустым.",
                nameof(name)
            );

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Код уровня образования не может быть пустым.",
                nameof(code)
            );

        Name = name;
        Code = code;
    }

    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;

    public bool IsDeleted { get; set; }

    internal void Update(string? name = null, string? code = null)
    {
        if (name != null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(
                    "Название уровня образования не может быть пустым.",
                    nameof(name)
                );
            Name = name;
        }

        if (code != null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException(
                    "Код уровня образования не может быть пустым.",
                    nameof(code)
                );
            Code = code;
        }
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
