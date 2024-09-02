using Application.Auth.Commands.Register;
using Application.Auth.Commands.Signout;
using Application.Auth.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;
using Presentation.Models;

namespace Presentation.Controllers;

[Route("auth")]
public sealed class AuthController : ApiController
{
    public AuthController(ISender sender) : base(sender)
    {
    }

    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [Route("login")]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken)
    {
        var query = new LoginQuery(model.UserName, model.Password);
        var result = await Sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Route("signout")]
    [HttpGet]
    public async Task<IActionResult> Signout(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new SignoutCommand(), cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest();
    }
}
