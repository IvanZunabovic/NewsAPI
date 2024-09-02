using Application.Authors.Commands.UpdateAuthorInfo;
using Application.Authors.Queries.GetAllAuthors;
using Application.Authors.Queries.GetAuthorById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("authors")]
public sealed class AuthorController : ApiController
{
    public AuthorController(ISender sender) : base(sender)
    {
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthorById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetAuthorByIdQuery(id);

        var response = await Sender.Send(query, cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAuthors(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var response = await Sender.Send(new GetAllAuthorsQuery(pageNumber, pageSize), cancellationToken);
        return response.IsSuccess ? Ok(response.Value) : NotFound(response.Error);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateAuthorInfo([FromBody] UpdateAuthorInfoCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error) /*or NotFound()*/;
    }
}
