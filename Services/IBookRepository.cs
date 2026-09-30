using BookStore.Api.Models;

namespace BookStore.Api.Services;

public interface IBookRepository
{
    IReadOnlyList<Book> GetAll();
    Book? GetById(int id);
    Book Add(Book book);
}
