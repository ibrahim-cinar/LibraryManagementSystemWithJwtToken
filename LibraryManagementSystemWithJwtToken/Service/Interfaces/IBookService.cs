using LibraryManagementSystemWithJwtToken.Models;


namespace LibraryManagementSystemWithJwtToken.Service.Interfaces
{
    public interface IBookService
    {
        List<Book> GetAllBooks();

        Book getBookByBookTitle(string bookTitle);

        List<Book> GetBooksByAuthorName(string authorName);

        Book addBook(Book book);

    }
}
