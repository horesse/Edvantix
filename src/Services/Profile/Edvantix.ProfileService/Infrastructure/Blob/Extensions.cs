using Edvantix.Constants.Aspire;

namespace Edvantix.ProfileService.Infrastructure.Blob;

internal static class Extensions
{
    public static void AddAzureBlobStorage(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        builder.AddAzureBlobContainerClient(
            Components.Azure.Storage.BlobContainer(Services.Profile)
        );
        services.AddScoped<IBlobService, BlobService>();
    }
}
