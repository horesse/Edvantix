namespace Edvantix.Persona.Infrastructure.Blob;

internal static class Extensions
{
    public static void AddAzureBlobStorage(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        builder.AddAzureBlobContainerClient(
            Components.Azure.Storage.BlobContainer(Services.Persona)
        );
        services.AddScoped<IBlobService, BlobService>();
    }
}
