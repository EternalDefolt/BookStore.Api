using Asp.Versioning;
using BookStore.Api.Models;
using BookStore.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[Route("api/books")]
[Route("api/v{version:apiVersion}/books")]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _repository;

    public BooksController(IBookRepository repository) => _repository = repository;

    [HttpGet]
    [HttpHead]
    public ActionResult<IEnumerable<BookV1>> GetAll() =>
        Ok(_repository.GetAll().Select(BookV1.From));

    [HttpGet("{id:int}")]
    [HttpHead("{id:int}")]
    public ActionResult<BookV1> GetById(int id)
    {
        var book = _repository.GetById(id);
        return book is null ? NotFound() : Ok(BookV1.From(book));
    }

    [HttpPost]
    public ActionResult<BookV1> Create(BookV1 request)
    {
        var book = _repository.Add(new Book
        {
            Title = request.Title,
            Author = request.Author,
            Price = request.Price,
            Currency = "RUB",
            Isbn = request.Isbn,
        });

        return Created($"{Request.Path}/{book.Id}", BookV1.From(book));
    }
}
