using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Interfaces;

public interface IAvatarImageStrategy {
    Task<ImageVM> GetAvatarImage(string userIdentifier);
}