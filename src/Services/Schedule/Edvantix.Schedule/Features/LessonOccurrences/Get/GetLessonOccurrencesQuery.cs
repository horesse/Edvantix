using Edvantix.Chassis.CQRS;
using Edvantix.Chassis.Mapper;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Edvantix.Schedule.Features.LessonOccurrences.Specifications;

namespace Edvantix.Schedule.Features.LessonOccurrences.Get;

public sealed record GetLessonOccurrencesQuery(Guid GroupId, DateOnly From, DateOnly To)
    : IQuery<IReadOnlyList<LessonOccurrenceDto>>;

internal sealed class GetLessonOccurrencesQueryHandler(
    ILessonOccurrenceRepository repository,
    IMapper<LessonOccurrence, LessonOccurrenceDto> mapper
) : IQueryHandler<GetLessonOccurrencesQuery, IReadOnlyList<LessonOccurrenceDto>>
{
    public async ValueTask<IReadOnlyList<LessonOccurrenceDto>> Handle(
        GetLessonOccurrencesQuery query,
        CancellationToken cancellationToken
    )
    {
        var spec = new LessonOccurrencesByGroupIdAndDateRangeSpec(
            query.GroupId,
            query.From,
            query.To
        );
        var occurrences = await repository.ListAsync(spec, cancellationToken);

        return occurrences.Select(mapper.Map).ToList();
    }
}
