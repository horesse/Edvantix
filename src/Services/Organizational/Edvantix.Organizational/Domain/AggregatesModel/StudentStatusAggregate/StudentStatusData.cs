namespace Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

[ExcludeFromCodeCoverage]
internal sealed class StudentStatusData : List<StudentStatus>
{
    public StudentStatusData(Guid organizationId)
    {
        AddRange([
            CreateStudentStatus(organizationId, "Активный", "ACTIVE", StudentStatusTone.Active, 0),
            CreateStudentStatus(
                organizationId,
                "В академе",
                "ON_LEAVE",
                StudentStatusTone.Warning,
                1
            ),
            CreateStudentStatus(
                organizationId,
                "Выпускник",
                "GRADUATE",
                StudentStatusTone.Neutral,
                2
            ),
            CreateStudentStatus(
                organizationId,
                "Отчислен",
                "EXPELLED",
                StudentStatusTone.Inactive,
                3
            ),
        ]);
    }

    private static StudentStatus CreateStudentStatus(
        Guid organizationId,
        string name,
        string code,
        StudentStatusTone tone,
        int order
    )
    {
        return new StudentStatus(organizationId, name, code, tone, isSystem: true, order: order);
    }
}
