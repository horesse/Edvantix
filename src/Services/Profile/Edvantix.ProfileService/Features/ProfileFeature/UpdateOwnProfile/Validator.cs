using Edvantix.ProfileService.Features.ProfileFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.ProfileFeature.UpdateOwnProfile;

/// <summary>
/// Валидатор для контакта пользователя
/// </summary>
public sealed class UpdateUserContactModelValidator : AbstractValidator<UpdateUserContactModel>
{
    public UpdateUserContactModelValidator()
    {
        RuleFor(x => x.Type).IsInEnum().WithMessage("Некорректный тип контакта");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Значение контакта обязательно для заполнения")
            .MaximumLength(255)
            .WithMessage("Значение контакта не должно превышать 255 символов");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Описание контакта не должно превышать 500 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

/// <summary>
/// Валидатор для истории трудоустройства
/// </summary>
public sealed class UpdateEmploymentHistoryModelValidator
    : AbstractValidator<UpdateEmploymentHistoryModel>
{
    public UpdateEmploymentHistoryModelValidator()
    {
        RuleFor(x => x.Workplace)
            .NotEmpty()
            .WithMessage("Название компании обязательно для заполнения")
            .MaximumLength(255)
            .WithMessage("Название компании не должно превышать 255 символов");

        RuleFor(x => x.Position)
            .NotEmpty()
            .WithMessage("Должность обязательна для заполнения")
            .MaximumLength(255)
            .WithMessage("Должность не должна превышать 255 символов");

        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Дата начала работы обязательна");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("Дата окончания не может быть раньше даты начала")
            .When(x => x.EndDate.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Описание не должно превышать 1000 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
    }
}

/// <summary>
/// Валидатор для образования
/// </summary>
public sealed class UpdateEducationModelValidator : AbstractValidator<UpdateEducationModel>
{
    public UpdateEducationModelValidator()
    {
        RuleFor(x => x.Institution)
            .NotEmpty()
            .WithMessage("Название учебного заведения обязательно для заполнения")
            .MaximumLength(255)
            .WithMessage("Название учебного заведения не должно превышать 255 символов");

        RuleFor(x => x.DateStart).NotEmpty().WithMessage("Дата начала обучения обязательна");

        RuleFor(x => x.DateEnd)
            .GreaterThan(x => x.DateStart)
            .WithMessage("Дата окончания не может быть раньше даты начала")
            .When(x => x.DateEnd.HasValue);

        RuleFor(x => x.EducationLevelId)
            .GreaterThan(0)
            .WithMessage("Уровень образования должен быть указан");

        RuleFor(x => x.Specialty)
            .MaximumLength(255)
            .WithMessage("Специальность не должна превышать 255 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.Specialty));
    }
}

/// <summary>
/// Валидатор для модели обновления профиля
/// </summary>
public sealed class UpdateProfileModelValidator : AbstractValidator<UpdateProfileModel>
{
    public UpdateProfileModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Имя обязательно для заполнения")
            .MaximumLength(100)
            .WithMessage("Имя не должно превышать 100 символов");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Фамилия обязательна для заполнения")
            .MaximumLength(100)
            .WithMessage("Фамилия не должна превышать 100 символов");

        RuleFor(x => x.MiddleName)
            .MaximumLength(100)
            .WithMessage("Отчество не должно превышать 100 символов")
            .When(x => !string.IsNullOrWhiteSpace(x.MiddleName));

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage("Дата рождения обязательна для заполнения")
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Дата рождения не может быть в будущем");

        RuleFor(x => x.Gender).IsInEnum().WithMessage("Некорректное значение пола");

        RuleForEach(x => x.Contacts)
            .SetValidator(new UpdateUserContactModelValidator())
            .When(x => x.Contacts is not null);

        RuleForEach(x => x.EmploymentHistories)
            .SetValidator(new UpdateEmploymentHistoryModelValidator())
            .When(x => x.EmploymentHistories is not null);

        RuleForEach(x => x.Educations)
            .SetValidator(new UpdateEducationModelValidator())
            .When(x => x.Educations is not null);
    }
}

/// <summary>
/// Валидатор для команды обновления собственного профиля
/// </summary>
public sealed class UpdateOwnProfileCommandValidator : AbstractValidator<UpdateOwnProfileCommand>
{
    public UpdateOwnProfileCommandValidator()
    {
        RuleFor(x => x.Profile).NotNull().SetValidator(new UpdateProfileModelValidator());
    }
}
