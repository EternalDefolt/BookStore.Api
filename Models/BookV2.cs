namespace BookStore.Api.Models;

public class BookV2
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public Money Price { get; set; } = new();

    public static BookV2 From(Book book) => new()
    {
        Id = book.Id,
        Title = book.Title,
        Author = book.Author,
        Price = new Money { Amount = book.Price, Currency = book.Currency },
    };
}
