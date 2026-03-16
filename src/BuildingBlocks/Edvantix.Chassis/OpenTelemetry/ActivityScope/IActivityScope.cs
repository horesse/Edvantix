using System.Diagnostics;

namespace Edvantix.Chassis.OpenTelemetry.ActivityScope;

public interface IActivityScope
{
    Activity? Start(string name)
    {
        return Start(name, new());
    }

    Activity? Start(string name, StartActivityOptions options);

    Task Run(
        string name,
        Func<Activity?, CancellationToken, Task> run,
        CancellationToken cancellationToken
    )
    {
        return Run(name, run, new(), cancellationToken);
    }

    Task Run(
        string name,
        Func<Activity?, CancellationToken, Task> run,
        StartActivityOptions options,
        CancellationToken cancellationToken
    );

    Task<TResult> Run<TResult>(
        string name,
        Func<Activity?, CancellationToken, Task<TResult>> run,
        CancellationToken cancellationToken
    )
    {
        return Run(name, run, new(), cancellationToken);
    }

    Task<TResult> Run<TResult>(
        string name,
        Func<Activity?, CancellationToken, Task<TResult>> run,
        StartActivityOptions options,
        CancellationToken cancellationToken
    );
}
