using Edvantix.Chassis.Endpoints;

namespace Edvantix.Schedule.Features.Holidays.Get;

internal sealed class GetHolidaysEndpoint
    : IEndpoint<Ok<IReadOnlyList<HolidayDto>>, (string CountryCode, int Year), ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/holidays",
                async (string countryCode, int year, ISender sender) =>
                    await HandleAsync((countryCode, year), sender)
            )
            .ProducesGet<IReadOnlyList<HolidayDto>>()
            .WithName("GetHolidays")
            .WithTags("Праздники")
            .WithSummary("Получить список праздников")
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<IReadOnlyList<HolidayDto>>> HandleAsync(
        (string CountryCode, int Year) request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(
            new GetHolidaysQuery(request.CountryCode, request.Year),
            cancellationToken
        );

        return TypedResults.Ok(result);
    }
}
