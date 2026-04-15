using System.Linq.Expressions;
using Edvantix.Chassis.Specification.Expressions;

namespace Edvantix.Chassis.Specification.Builders;

public static partial class SpecificationBuilderExtensions
{
    extension<T>(ISpecificationBuilder<T> builder)
        where T : class
    {
        public ISpecificationBuilder<T> Search(
            Expression<Func<T, string?>> keySelector,
            string pattern,
            int group = 1
        )
        {
            var expr = new SearchExpression<T>(keySelector, pattern, group);
            builder.Specification.Add(expr);
            return builder;
        }
    }
}
