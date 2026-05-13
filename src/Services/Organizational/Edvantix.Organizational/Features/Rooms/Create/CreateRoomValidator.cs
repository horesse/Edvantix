namespace Edvantix.Organizational.Features.Rooms.Create;

internal sealed class CreateRoomValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage("Метка кабинета обязательна")
            .MaximumLength(64)
            .WithMessage("Метка кабинета не должна превышать 64 символа");

        RuleFor(x => x.Seats)
            .InclusiveBetween((short)1, (short)200)
            .WithMessage("Вместимость должна быть от 1 до 200 мест");
    }
}
