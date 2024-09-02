using Application.News.Commands.CreateNews;
using Application.News.Commands.DeleteNews;
using Application.News.Commands.UpdateNews;
using Application.News.Queries.GetAllNews;
using Application.News.Queries.GetNewsByAuthor;
using Application.News.Queries.GetNewsById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Abstractions;

namespace Presentation.Controllers;

[Route("news")]
public sealed class NewsController : ApiController
{
    public NewsController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateNews([FromBody] CreateNewsCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetNewsByIdQuery(id);

        var result = await Sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNews(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetAllNewsQuery(pageNumber, pageSize);

        var result = await Sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpGet("get-by-author/{authorId}")]
    public async Task<IActionResult> GetNewsByAuthor(Guid authorId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = new GetNewsByAuthorQuery(authorId, pageNumber, pageSize);

        var result = await Sender.Send(query, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateNews([FromBody] UpdateNewsCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : NotFound(result.Error);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNews(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteNewsCommand(id);

        var result = await Sender.Send(command, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest();
    }
}
