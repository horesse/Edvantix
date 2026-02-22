namespace Edvantix.Blog.Features.PostFeature.Features.CreatePost;

/// <summary>
/// Валидатор команды создания поста.
/// </summary>
internal sealed class CreatePostValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Заголовок поста обязателен.")
            .MaximumLength(500)
            .WithMessage("Заголовок не должен превышать 500 символов.");

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug поста обязателен.")
            .MaximumLength(255)
            .WithMessage("Slug не должен превышать 255 символов.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug может содержать только строчные буквы, цифры и дефисы.");

        RuleFor(x => x.Content).NotEmpty().WithMessage("Содержимое поста обязательно.");

        RuleFor(x => x.Summary)
            .MaximumLength(1000)
            .When(x => x.Summary is not null)
            .WithMessage("Краткое описание не должно превышать 1000 символов.");

        RuleFor(x => x.CoverImageUrl)
            .MaximumLength(1000)
            .When(x => x.CoverImageUrl is not null)
            .WithMessage("URL обложки не должен превышать 1000 символов.");
    }
}
