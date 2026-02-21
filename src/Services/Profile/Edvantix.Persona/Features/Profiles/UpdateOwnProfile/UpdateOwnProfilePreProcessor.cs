using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

public sealed class UpdateOwnProfilePreProcessor(IBlobService blobService)
    : IRequestPreProcessor<UpdateOwnProfileCommand>
{
    public async Task Process(UpdateOwnProfileCommand request, CancellationToken cancellationToken)
    {
        if (request.Profile.Avatar is not null)
        {
            var url = await blobService.UploadFileAsync(request.Profile.Avatar, cancellationToken);
            request.Profile.AvatarUrl = url;
        }
    }
}
