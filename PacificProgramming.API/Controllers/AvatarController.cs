using MediatR;
using Microsoft.AspNetCore.Mvc;
using PacificProgramming.Application.Queries;
using PacificProgramming.Application.ViewModels;

namespace PacificProgramming.API.Controllers;

[ApiController]
public sealed class AvatarController : ControllerBase {
    private readonly IMediator _mediator;

    public AvatarController(IMediator mediator) {
        _mediator = mediator;
    }

    [HttpGet("Avatar")]
    public async Task<ActionResult<ImageVM>> GetAvatars(string userIdentifier) {
        var request = new GetAvatarImage(userIdentifier);
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}