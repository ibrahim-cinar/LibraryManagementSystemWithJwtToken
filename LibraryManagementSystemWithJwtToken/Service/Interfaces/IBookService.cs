using LibraryManagementSystemWithJwtToken.Models;
using LibraryManagementSystemWithJwtToken.Pagination;


namespace LibraryManagementSystemWithJwtToken.Service.Interfaces
{
    public interface IBookService
    {
        PagedResult<Book> GetAllBooks(int page, int pageSize);

        Book getBookByBookTitle(string bookTitle);

        List<Book> GetBooksByAuthorName(string authorName);

        Task AddBookAsync(Book book);

    }
}
