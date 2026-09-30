using Asp.Versioning;
using BookStore.Api.Models;
using BookStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[ApiVersion("2.0")]
[Route("api/books")]
[Route("api/v{version:apiVersion}/books")]
public class BooksV2Controller : ControllerBase
{
    private readonly IBookRepository _repository;

    public BooksV2Controller(IBookRepository repository) => _repository = repository;

    [HttpGet]
    [HttpHead]
    public ActionResult<PagedResult<BookV2>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var books = _repository.GetAll();
        var items = books
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(BookV2.From)
            .ToList();

        return Ok(new PagedResult<BookV2>
        {
            Items = items,
            Total = books.Count,
            Page = page,
            PageSize = pageSize,
        });
    }

    [HttpGet("{id:int}")]
    [HttpHead("{id:int}")]
    public ActionResult<BookV2> GetById(int id)
    {
        var book = _repository.GetById(id);
        return book is null ? NotFound() : Ok(BookV2.From(book));
    }

    [HttpPost]
    public ActionResult<BookV2> Create(BookV2 request)
    {
        var book = _repository.Add(new Book
        {
            Title = request.Title,
            Author = request.Author,
            Price = request.Price.Amount,
            Currency = request.Price.Currency,
            Isbn = null,
        });

        return Created($"{Request.Path}/{book.Id}", BookV2.From(book));
    }
}
