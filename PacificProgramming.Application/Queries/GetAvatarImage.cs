using MediatR;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.Application.Queries;

public record GetAvatarImage(string userIdentifier) : IRequest<ImageVM>;