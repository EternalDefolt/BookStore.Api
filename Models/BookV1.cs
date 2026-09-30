namespace BookStore.Api.Models;

public class BookV1
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Isbn { get; set; }

    public static BookV1 From(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Price = book.Price,
        Isbn = book.Isbn,
    };
}
