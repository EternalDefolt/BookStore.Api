using BookStore.Api.Models;

namespace BookStore.Api.Services;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books =
    [
        new() { Id = 1, Title = "Мастер и Маргарита", Author = "Михаил Булгаков", Price = 890, Currency = "RUB", Isbn = "978-5-389-01686-3" },
        new() { Id = 2, Title = "Преступление и наказание", Author = "Фёдор Достоевский", Price = 720, Currency = "RUB", Isbn = "978-5-04-116345-3" },
        new() { Id = 3, Title = "Война и мир", Author = "Лев Толстой", Price = 1490, Currency = "RUB", Isbn = "978-5-17-090326-5" },
        new() { Id = 4, Title = "Евгений Онегин", Author = "Александр Пушкин", Price = 460, Currency = "RUB", Isbn = "978-5-699-92580-6" },
        new() { Id = 5, Title = "Мёртвые души", Author = "Николай Гоголь", Price = 610, Currency = "RUB", Isbn = "978-5-04-119117-5" },
    ];

    public IReadOnlyList<Book> GetAll() => _books;

    public Book? GetById(int id) => _books.FirstOrDefault(b => b.Id == id);

    public Book Add(Book book)
    {
        book.Id = _books.Count == 0 ? 1 : _books.Max(b => b.Id) + 1;
        _books.Add(book);
        return book;
    }
}
