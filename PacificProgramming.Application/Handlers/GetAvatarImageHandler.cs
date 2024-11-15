using MediatR;
using PacificProgramming.Application.Queries;
using PacificProgramming.Application.Services;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Handlers;

public sealed class GetAvatarImageHandler : IRequestHandler<GetAvatarImage, ImageVM> {
    private readonly IAvatarImageService _avatarImageService;

    public GetAvatarImageHandler(IAvatarImageService avatarImageService) {
        _avatarImageService = avatarImageService;
    }

    public async Task<ImageVM> Handle(GetAvatarImage request, CancellationToken cancellationToken) => await _avatarImageService.GetAvatarImage(request.userIdentifier);
}