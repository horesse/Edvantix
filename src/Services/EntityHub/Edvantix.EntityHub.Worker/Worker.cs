using Edvantix.EntityHub.Worker.Services;

namespace Edvantix.EntityHub.Worker;

public class Worker(IServiceProvider provider, IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var analyzer = new Analyzer(provider);

            await analyzer.AnalyzeAssemblies(stoppingToken);
        }
        catch
        {
            // TODO log or smth
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }
}
